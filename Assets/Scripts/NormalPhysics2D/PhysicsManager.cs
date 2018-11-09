using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

using Physics_Engine.Physics.Properties;
using Physics_Engine.Helpers;

namespace Physics_Engine.Physics
{
    class PhysicsManager
    {
        private struct Projection
        {
            public Projection(float min, float max)
            {
                Min = min;
                Max = max;
            }
            public float Min;
            public float Max;
            public bool Overlap(Projection other, out float amount)
            {
                if (Max >= other.Min && other.Max >= Min)
                {
                    if (Max < other.Max)
                        amount = Math.Abs(Max - other.Min);
                    else
                        amount = Math.Abs(other.Max - Min);
                    return true;
                }
                else
                {
                    amount = -1;
                    return false;
                }
            }
        }
        private Projection Project(Vector2 pos, Vector2[] vertices, Vector2 axis)
        {
            float min = Vector2.Dot(pos + vertices[0], axis);
            float max = min;

            for (int i = 1; i < vertices.Length; i++)
            {
                // NOTE: the axis must be normalized to get accurate projections
                float p = Vector2.Dot(pos + vertices[i], axis);
                if (p < min)
                    min = p;
                else if (p > max)
                    max = p;
            }
            return new Projection(min, max);
        }
        public struct CollisionInfo
        {
            public Vector2 CollisionNormal;
            public Vector2 Overlapping;
            public List<Vector2> CollisionPoints;
            public List<Vector2> IntersectionPoints;
        }
        private delegate bool IsCollidingDelegate(PhysicsBody body1, PhysicsBody body2, out CollisionInfo collisionInfo);
        private delegate void CollisionResolveDelegate(PhysicsBody body1, PhysicsBody body2, CollisionInfo collisionInfo);
        public delegate void CollisionInfoDelegate(CollisionInfo collisionInfo, PhysicsBody body1, PhysicsBody body2);
        private CollisionInfoDelegate m_CollisionListeners = null;
        private IsCollidingDelegate[,] m_CollisionFuncPtrs;
        private CollisionResolveDelegate[] m_ResolveFuncPtrs;
        private List<PhysicsBody> m_Bodies;
        private static Vector2 m_Gravity = new Vector2(0, 20.0f);
        private static Vector2 m_GravityNormal = peMath.LeftPerp(m_Gravity);//Normalized in constructor
        private int m_Iterations = 3;
        private bool m_ShowDebug = true;

        //TEMP
        PhysicsBody bodySelected = null;
        Vector2 lastMousePos;
        //InputManager m_InputMgr = InputManager.Instance;
        //TEMP

        public void RegisterListener(CollisionInfoDelegate p_Listener)
        {
            m_CollisionListeners += p_Listener;
        }
        public void UnregisterListener(CollisionInfoDelegate p_Listener)
        {
            m_CollisionListeners -= p_Listener;
        }
        
        public virtual void Update(){
            CollisionInfo collisionInfo;
#region TEMP
            //RenderManager.Instance.DrawString("Bodies: " + m_Bodies.Count.ToString());
            //Move selected body
            Vector2 mousePos = Input.mousePosition;
            if (bodySelected != null)
            {
                //Stop velocity if at mouse-pointer to avoid spring-effect
                if(Math.Abs(mousePos.x - bodySelected.Position.x) < 10
                   && Math.Abs(mousePos.y - bodySelected.Position.y) < 10)
                    bodySelected.AddForce(-bodySelected.Velocity * bodySelected.Mass * 0.5f, bodySelected.Position, FORCE_TYPE.IMPULSE);
                bodySelected.AddForce(((Vector2)Input.mousePosition - (bodySelected.Position))*0.5f, bodySelected.Position, FORCE_TYPE.IMPULSE);
                
                //RenderManager.Instance.DrawLine(bodySelected.Position, Input.mousePosition); ???????
            }
#endregion
                foreach (PhysicsBody body1 in m_Bodies)
                {
                    if (body1.IsStatic() == false)
                    {
                        foreach (PhysicsBody body2 in m_Bodies)
                        {
                            if (body1.GetID() != body2.GetID() && IsColliding(body1, body2, out collisionInfo))
                            {
                                //if (m_CollisionListeners != null)
                                //m_CollisionListeners(collisionInfo, body1, body2);

                                //Call correct collision-solver
                                m_ResolveFuncPtrs[body2.Material].Invoke(body1, body2, collisionInfo);
                            }
                        }
                        #region TEMP
                        //If mouse pressed and no body selected, find new
                        if (bodySelected == null && Input.GetMouseButtonDown(0) && Math.Abs(mousePos.x - body1.Position.x) < 10
                                                && Math.Abs(mousePos.y - body1.Position.y) < 10)
                        {
                            bodySelected = body1;
                            bodySelected.AddForce(-bodySelected.Velocity * bodySelected.Mass, bodySelected.Position, FORCE_TYPE.IMPULSE);
                        }
                        //Mousebutton released
                        if (Input.GetMouseButtonUp(0))
                            bodySelected = null;
                        //if mousebutton pressed and body selected, don't apply gravity
                        //if (bodySelected != null &&  body1.GetID() != bodySelected.GetID() || bodySelected == null)
                        #endregion
                        body1.AddForce((m_Gravity) * body1.Mass, body1.Position, FORCE_TYPE.IMPULSE);
                        body1.Update();
                    }
                }
            lastMousePos = mousePos;
        }
        public void AddBody(PhysicsBody p_Body) 
        { 
            m_Bodies.Add(p_Body); 
        }
        public void Remove(PhysicsBody p_Body) 
        {
            //TODO: Make more efficient (O(1) possible with IDs?)
            m_Bodies.Remove(p_Body);
        }
        public void RemoveLast()
        {
            if(m_Bodies.Count > 0)
                m_Bodies.RemoveAt(m_Bodies.Count - 1);
        }
        public void Clear() 
        { 
            m_Bodies.Clear();
        }

        //Collision
        private bool IsColliding(PhysicsBody body1, PhysicsBody body2, out CollisionInfo collisionInfo)
        {
            return m_CollisionFuncPtrs[body1.Shape, body2.Shape].Invoke(body1, body2, out collisionInfo);
        }
        private bool IsCollidingCircleCircle(PhysicsBody body1, PhysicsBody body2, out CollisionInfo collisionInfo)
        {
            collisionInfo.CollisionPoints = new List<Vector2>();
            collisionInfo.IntersectionPoints = new List<Vector2>();

            //Vector from body2 to body1
            Vector2 distance = body1.Position - body2.Position;
            float widths = body1.CircleDef.Radius + body2.CircleDef.Radius;
            //See if distance is greater than the sum of their radius (square to avoid sqrt)
            if (distance.magnitude*distance.magnitude < widths * widths)
            {
                //In case the circles are exactly inside of each other (special-fix)
                if (distance.x == 0 && distance.y == 0)
                {
                    Vector2 specialDistance = body1.LastPosition - body2.Position;
                    if (specialDistance.x == 0 && specialDistance.y == 0)
                        specialDistance.x = 1;
                    else
                        specialDistance.Normalize();
                    collisionInfo.Overlapping = specialDistance * widths;
                    collisionInfo.CollisionNormal = specialDistance;
                    collisionInfo.CollisionPoints = new List<Vector2>();
                }
                else
                {
                    float overlappingAmount = widths - distance.magnitude;
                    //Make directionVector
                    distance.Normalize();
                    //How much they are overlapping
                    collisionInfo.Overlapping = distance * overlappingAmount;
                    //Normal-vector of collision line
                    collisionInfo.CollisionNormal = distance;
                    //Collision-point (no need to consider overlappingAmount since body1 will move back that dist)
                    collisionInfo.CollisionPoints.Add(body2.Position + (collisionInfo.CollisionNormal * body2.CircleDef.Radius));
                }
                return true;
            }
            else
            {
                collisionInfo.CollisionNormal = new Vector2();
                collisionInfo.Overlapping = new Vector2();
                return false;
            }
        }
        private bool IsCollidingConvexConvex(PhysicsBody body1, PhysicsBody body2, out CollisionInfo collisionInfo)
        {
            //VERY INEFFICIENT ATM! Mainly because of the extra calculations (LOOPS!) to find intersection- and collision-points.
            collisionInfo.CollisionNormal = new Vector2();
            collisionInfo.Overlapping = new Vector2();
            collisionInfo.CollisionPoints = new List<Vector2>();
            collisionInfo.IntersectionPoints = new List<Vector2>();

            List<Vector2> verticesInside = new List<Vector2>();

            Vector2 edge;
            Vector2 edgeNormal;
            
            Vector2 shortestOverlapAxis = body1.PolygonDef.GetVertex(0);

            Projection p1;
            Projection p2;
            float shortestOverlapAmount = int.MaxValue;
            float overlapAmount = 0;

            //For polygon 1, find all edge-normals and check if other polygons vertices is overlapping
            for (int i = 0; i < body1.PolygonDef.VertexCount; i++)
            {
                edge = body1.PolygonDef.GetVertex((i + 1 == body1.PolygonDef.VertexCount ? 0 : i + 1)) - body1.PolygonDef.GetVertex(i);
                // NOTE: Edges must be represented in clockwise order for normals to be correct! (should point OUT of polygon)
                edgeNormal = peMath.RightPerp(edge);
                edgeNormal.Normalize();

                p1 = Project(body1.Position, body1.PolygonDef.Vertices, edgeNormal);
                p2 = Project(body2.Position, body2.PolygonDef.Vertices, edgeNormal);
                if (!p1.Overlap(p2, out overlapAmount))
                    return false;
                else if (overlapAmount < shortestOverlapAmount)
                {
                    shortestOverlapAxis = edgeNormal;
                    shortestOverlapAmount = overlapAmount;
                }

                //Find any line-line-intersections
                for (int j = 0; j < body2.PolygonDef.VertexCount; j++)
                {
                    Vector2 point1 = (body2.PolygonDef.GetVertex(j) + body2.Position);
                    Vector2 point2 = (body2.PolygonDef.GetVertex((j + 1 == body2.PolygonDef.VertexCount ? 0 : j + 1)) + body2.Position);
                    Vector2 point3 = (body1.PolygonDef.GetVertex(i) + body1.Position);
                    Vector2 point4 = (body1.PolygonDef.GetVertex((i + 1 == body1.PolygonDef.VertexCount ? 0 : i + 1)) + body1.Position);
                    Vector2 intersectionPoint;
                    if (peMath.LineLineIntersection(point1, point2, point3, point4, out intersectionPoint))
                        collisionInfo.IntersectionPoints.Add(intersectionPoint);
                }

                //Check if any body2-vertex is inside body1-polygon
                if (i == 0)
                {
                    for (int j = 0; j < body2.PolygonDef.VertexCount; j++)
                    {
                        Vector2 vertex = (body2.PolygonDef.GetVertex(j) + body2.Position) - (body1.PolygonDef.GetVertex(i) + body1.Position);
                        float dot = Vector2.Dot(vertex, edgeNormal);
                        if ( dot < 0)
                            verticesInside.Add(body2.PolygonDef.GetVertex(j) + body2.Position);
                    }
                }
                else
                {
                    //Every vertex of body2 that is not inside any other edge is not inside body1, remove
                    for (int j = 0; j < verticesInside.Count; j++)
                    {
                        Vector2 vertex = verticesInside[j] - (body1.PolygonDef.GetVertex(i) + body1.Position);
                        float dot = Vector2.Dot(vertex, edgeNormal);
                        if (dot > 0)
                        {
                            verticesInside.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            //Do same for other polygon
            for (int i = 0; i < body2.PolygonDef.VertexCount; i++)
            {
                edge = body2.PolygonDef.GetVertex((i + 1 == body2.PolygonDef.VertexCount ? 0 : i + 1)) - body2.PolygonDef.GetVertex(i);
                // NOTE: Edges must be represented in clockwise order for normals to be correct! (should point OUT of polygon)
                edgeNormal = peMath.RightPerp(edge);
                edgeNormal.Normalize();

                //Just swop
                p2 = Project(body1.Position, body1.PolygonDef.Vertices, edgeNormal);
                p1 = Project(body2.Position, body2.PolygonDef.Vertices, edgeNormal);
                if (!p1.Overlap(p2, out overlapAmount))
                    return false;
                else if (overlapAmount < shortestOverlapAmount)
                {
                    shortestOverlapAxis = edgeNormal;
                    shortestOverlapAmount = overlapAmount;
                }

                //Check if any body1-vertex is inside body2-polygon
                if (i == 0)
                {
                    for (int j = 0; j < body1.PolygonDef.VertexCount; j++)
                    {
                        Vector2 vertex = (body1.PolygonDef.GetVertex(j) + body1.Position) - (body2.PolygonDef.GetVertex(i) + body2.Position);
                        float dot = Vector2.Dot(vertex, edgeNormal);
                        if (dot < 0)
                            verticesInside.Add(body1.PolygonDef.GetVertex(j) + body1.Position);
                    }
                }
                else
                {
                    //Every vertex of body1 that is not inside any other edge is not inside body2, remove
                    for (int j = 0; j < verticesInside.Count; j++)
                    {
                        Vector2 vertex = verticesInside[j] - (body2.PolygonDef.GetVertex(i) + body2.Position);
                        float dot = Vector2.Dot(vertex, edgeNormal);
                        if (dot > 0)
                        {
                            verticesInside.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }

            collisionInfo.CollisionNormal = shortestOverlapAxis;
            collisionInfo.Overlapping = shortestOverlapAxis * shortestOverlapAmount;
            collisionInfo.CollisionPoints.AddRange(verticesInside);
            
            return true;
        }
        private bool IsCollidingConvexCircle(PhysicsBody polygon, PhysicsBody circle, out CollisionInfo collisionInfo)
        {
            collisionInfo.CollisionPoints = new List<Vector2>();
            collisionInfo.IntersectionPoints = new List<Vector2>();

            //Used in case inside polygon
            Vector2 closestProjection = new Vector2(Screen.width, Screen.height);
            bool insidePolygon = true;
            for (int i=0; i<polygon.PolygonDef.VertexCount; i++)
            {
                //Vector from current edge to circle-center
                Vector2 vecToCircle = circle.Position - (polygon.Position + polygon.PolygonDef.GetVertex(i));
                Vector2 currentEdge = polygon.PolygonDef.GetVertex(i+1 == polygon.PolygonDef.VertexCount ? 0 : i+1) - polygon.PolygonDef.GetVertex(i);
                //Length of current edge (squared for efficiency)
                float currentEdgeLengthSquared = currentEdge.magnitude * currentEdge.magnitude;
                currentEdge.Normalize();
                //Project circle-vector on current edge
                float circleToEdgeProj = Vector2.Dot(vecToCircle, currentEdge);
                //If above edge
                if (circleToEdgeProj > 0)
                {
                    //If projection is inside edge
                    if ((circleToEdgeProj * circleToEdgeProj) < currentEdgeLengthSquared)
                    {
                        //Projectionvector on edge
                        Vector2 vecCircleProjEdge = currentEdge * circleToEdgeProj;
                        //Get the vector projected on the edge by circle
                        Vector2 projectionToCircle = vecToCircle - vecCircleProjEdge;
                        float projectionToCircleLengthSquared = projectionToCircle.magnitude* projectionToCircle.magnitude;
                        if (projectionToCircleLengthSquared < closestProjection.magnitude * closestProjection.magnitude)
                        closestProjection = projectionToCircle;
                        //Is the distance less than circle-radius?
                        if (projectionToCircleLengthSquared < circle.CircleDef.Radius * circle.CircleDef.Radius)
                        {
                            collisionInfo.CollisionNormal = -projectionToCircle;
                            collisionInfo.CollisionNormal.Normalize();
                            collisionInfo.CollisionPoints.Add(circle.Position + (collisionInfo.CollisionNormal * circle.CircleDef.Radius));
                            collisionInfo.Overlapping = collisionInfo.CollisionNormal * (circle.CircleDef.Radius - projectionToCircle.magnitude);
                            return true;
                        }
                    }
                }
                //If not above any line (projection is negative), check distance to closest vertex is less than radius! (Squared for efficiency)
                else if (vecToCircle.magnitude * vecToCircle.magnitude < circle.CircleDef.Radius * circle.CircleDef.Radius)
                {                    
                    collisionInfo.CollisionNormal = -vecToCircle;
                    collisionInfo.CollisionNormal.Normalize();
                    //Collision-point will be the vertex on the polygon in this case
                    collisionInfo.CollisionPoints.Add(polygon.PolygonDef.GetVertex(i) + polygon.Position);
                    collisionInfo.Overlapping = collisionInfo.CollisionNormal * (circle.CircleDef.Radius - vecToCircle.magnitude);
                    return true;
                }
                //The edge-normal (used to see if inside polygon)
                Vector2 edgeNormal = peMath.RightPerp(currentEdge);
                //If projection is negative, the circle can't be inside the polygon
                if(Vector2.Dot(vecToCircle, edgeNormal) > 0)
                    insidePolygon = false;
                //In case last vertex, polygon is inside
                else if (i == polygon.PolygonDef.VertexCount-1)
                {
                    //Use the way out (to edge)
                    collisionInfo.CollisionNormal = vecToCircle;
                    collisionInfo.CollisionNormal.Normalize();
                    collisionInfo.Overlapping = vecToCircle * (vecToCircle.magnitude + circle.CircleDef.Radius);
                    //Just use first vertex as "closest"
                    collisionInfo.CollisionPoints.Add(polygon.PolygonDef.GetVertex(0) + polygon.Position);
                    //If whole circle inside polygon (no edge-normal were positive)
                    return insidePolygon;
                }
            }
            collisionInfo.CollisionNormal = new Vector2();
            collisionInfo.Overlapping = new Vector2();
            return false;
        }
        private bool IsCollidingCircleConvex(PhysicsBody body1, PhysicsBody body2, out CollisionInfo collisionInfo)
        {
            //Dummy, redirect to IsCollidingConvexCircle-function.
            return IsCollidingConvexCircle(body2, body1, out collisionInfo);
        }

        //Resolution for "solid against solid"-collision
        private void ResolveSolid(PhysicsBody body1, PhysicsBody body2, CollisionInfo collisionInfo)
        {
            //Make sure normal is in right direction (avoids a lot of special-code in IsColliding-functions)
            if (Vector2.Dot(body1.Position - body2.Position, collisionInfo.CollisionNormal) < 0)
            {
                collisionInfo.CollisionNormal = -collisionInfo.CollisionNormal;
                collisionInfo.Overlapping = -collisionInfo.Overlapping;
            }
            //Translate minimum distance so not overlapping
            body1.Position += collisionInfo.Overlapping;

            float j = 0;
            Vector2 collisionPointRelativeVelocity1to2 = Vector2.right;
            //Not 100% accurate if lands on flat surface because collision-points are calculated one at a time, but works okey.
            foreach (Vector2 collisionPoint in collisionInfo.CollisionPoints)
            {
                if (m_ShowDebug == true)


                { 
                    //    RenderManager.Instance.DrawCircle(collisionPoint, 6, Color.Red);    ????
                }



                //Calculate angular velocities (multiple steps)

                //First get distance-vector from each body-center to collision-point


                Vector2 collisionPointRadius1 = collisionPoint - body1.Position;
                Vector2 collisionPointRadius2 = collisionPoint - body2.Position;

                //RenderManager.Instance.DrawLine(collisionPoint, collisionPoint + collisionInfo.CollisionNormal * 50, Color.Blue);
                //RenderManager.Instance.DrawLine(body2.Position, body2.Position + collisionPointRadius2, Color.BlueViolet);

                //Calculate vector perpendicular to the vector from mass-center to collision-point
                Vector2 collisionPointRadius1Perp = peMath.RightPerp(collisionPointRadius1);
                Vector2 collisionPointRadius2Perp = peMath.RightPerp(collisionPointRadius2);

                //RenderManager.Instance.DrawLine(body1.Position + collisionPointRadius1, body1.Position + collisionPointRadius1 + collisionPointRadius1Perp, Color.Red);
                //RenderManager.Instance.DrawLine(body2.Position + collisionPointRadius2, body2.Position + collisionPointRadius2 + collisionPointRadius2Perp, Color.MediumVioletRed);

                //Velocity at collisionpoint: Vp = ω * r⊥ + Va (r⊥ = perp vector to the vector from bodyCenter to collisionpoint)
                Vector2 collisionPointVelocity1 = (body1.AngularVelocity * collisionPointRadius1Perp) + body1.Velocity;
                Vector2 collisionPointVelocity2 = (body2.AngularVelocity * collisionPointRadius2Perp) + body2.Velocity;

                //TEMP
                //RenderManager.Instance.DrawLine(body1.Position + collisionPointRadius1, body1.Position + collisionPointRadius1 + collisionPointVelocity1, Color.Green);
                //RenderManager.Instance.DrawLine(body2.Position + collisionPointRadius2, body2.Position + collisionPointRadius2 + collisionPointVelocity2, Color.Red);
                //TEMP

                //Relative velocity of body1s with respect to body2s collision-point velocity
                collisionPointRelativeVelocity1to2 = collisionPointVelocity1 - collisionPointVelocity2;


                //Calculate new impulse (NOTE: Hairy as a dog, don't try to read or your brain might melt!)
                j = Vector2.Dot(-(1 + (body1.Bounciness + body2.Bounciness) * 0.5f) * collisionPointRelativeVelocity1to2, collisionInfo.CollisionNormal)
                            / (Vector2.Dot(collisionInfo.CollisionNormal, (collisionInfo.CollisionNormal * (body1.InvMass + body2.InvMass)))
                            + (float)(Math.Pow(Vector2.Dot(collisionPointRadius1Perp, collisionInfo.CollisionNormal), 2) * body1.InvInertia + Math.Pow(Vector2.Dot(collisionPointRadius2Perp, collisionInfo.CollisionNormal), 2) * body2.InvInertia));

                //"Temp"-fix for j (make sure sign is correct)
                if (Vector2.Dot(j * collisionInfo.CollisionNormal, collisionInfo.CollisionNormal) < 0)
                    j = -j;

                //Apply force (torque manually calculated by the body itself)
                body1.AddForce(j * collisionInfo.CollisionNormal, collisionPoint, FORCE_TYPE.IMPULSE);
                body2.AddForce(-j * collisionInfo.CollisionNormal, collisionPoint, FORCE_TYPE.IMPULSE);
            }
        }
        private void ResolveFluid(PhysicsBody body1, PhysicsBody body2, CollisionInfo collisionInfo)
        {
            collisionInfo.CollisionPoints.AddRange(collisionInfo.IntersectionPoints);

            //Not exactly sure why this works (made while brain was in zombie-mode), but seems to do the job so far! :)
            collisionInfo.CollisionPoints = peMath.FindConvexHull(collisionInfo.CollisionPoints);

            //TEMP
            //if (m_ShowDebug)
            //    foreach (Vector2 collisionPoint in collisionInfo.CollisionPoints)
            //        RenderManager.Instance.DrawCircle(collisionPoint, 6, Color.Red);       ??????
            //TEMP

            //TODO: Fix so moment of inertia is not calculated here (not needed and is therefor very inefficient)
            PhysicsPolygonDef waterPoly = new PhysicsPolygonDef(collisionInfo.CollisionPoints.ToArray(), false, Properties.Density.WATER);

            Vector2 centroid = peMath.PolygonCentroid(collisionInfo.CollisionPoints.ToArray(), waterPoly.Volume);

            //TEMP
            //if (m_ShowDebug)
            //{
            //    for (int i = 0; i < waterPoly.VertexCount; i++)
            //        RenderManager.Instance.DrawLine(centroid + waterPoly.GetVertex(i), centroid + waterPoly.GetVertex((i + 1 == waterPoly.VertexCount ? 0 : i + 1)), 4, Color.CornflowerBlue);
            //    RenderManager.Instance.DrawCircle(centroid, 6, Color.CornflowerBlue);
            //}                                                                             ???????
            //TEMP

            body1.AddForce(waterPoly.Volume * -body1.Velocity * 0.00001f, centroid);
            //Apply water-resistance (must happen before buoyancy is applied)
            body1.AddForce(waterPoly.Volume * -body1.Velocity * 0.0005f, body1.Position);
            //Apply buoyancy
            body1.AddForce(-waterPoly.Mass * (m_Gravity), centroid, FORCE_TYPE.IMPULSE);
            body1.AddTorque(waterPoly.Volume * -body1.AngularVelocity * 0.03f);

            //TEMP
            //if (m_InputMgr.KeyIsPressed(Keys.D3))
            //{
            //    RenderManager.Instance.DrawString("Weight: " + (body1.Mass * (m_Gravity * m_Gravity)).Length().ToString(), body1.Position - Vector2.UnitY * 16);
            //    RenderManager.Instance.DrawString("Buoyancy: " + (waterPoly.Mass * (m_Gravity * m_Gravity)).Length().ToString(), centroid);
            //}                                                                                 ??????
            //TEMP
        }

        //Singleton
        private static volatile PhysicsManager m_Instance;
        //private static object m_SyncRoot = new Object();                  ??????
        private PhysicsManager()
        {
            m_Bodies = new List<PhysicsBody>();
            //Instantiate matrix of function pointers to different collision-functions
            m_CollisionFuncPtrs = new IsCollidingDelegate[SHAPE.NR_SHAPES, SHAPE.NR_SHAPES];
            //If two circles collide, use "CircleCircle" function
            m_CollisionFuncPtrs[SHAPE.CIRCLE, SHAPE.CIRCLE] = new IsCollidingDelegate(IsCollidingCircleCircle);
            //If two convex shapes collide, use "ConvexConvex" function
            m_CollisionFuncPtrs[SHAPE.POLYGON, SHAPE.POLYGON] = new IsCollidingDelegate(IsCollidingConvexConvex);
            //If two convex and circle shapes collide, use "ConvexCircle" function
            m_CollisionFuncPtrs[SHAPE.POLYGON, SHAPE.CIRCLE] = new IsCollidingDelegate(IsCollidingConvexCircle);
            //If two circle and convex shapes collide, use "CircleConvex" function (Redirects to ConvexCircle)
            m_CollisionFuncPtrs[SHAPE.CIRCLE, SHAPE.POLYGON] = new IsCollidingDelegate(IsCollidingCircleConvex);

            m_ResolveFuncPtrs = new CollisionResolveDelegate[Properties.Material.NR_MATERIALS];
            //If something is colliding with water (water never checks with collision for others, always static)
            //m_ResolveFuncPtrs[Material.SOLID] = new CollisionResolveDelegate(ResolveSolid);                        ??????
            ////If something is colliding with water (water never checks with collision for others, always static)
            //m_ResolveFuncPtrs[Material.FLUID] = new CollisionResolveDelegate(ResolveFluid);                       ?????

            m_GravityNormal.Normalize();
        }
        public static PhysicsManager Instance
        {
            get
            {
                if (m_Instance == null)//Make sure not null == not instantiated
                    //lock (m_SyncRoot)//For thread-safety
                        if (m_Instance == null)
                        {//Check again to avoid earlier instantiation by other thread
                            m_Instance = new PhysicsManager();
                        }
                return m_Instance;
            }
        }
    }
}