using System;
using UnityEngine;
using Physics_Engine.Physics.Properties;
using Physics_Engine.Helpers;
using System.Diagnostics;

namespace Physics_Engine.Physics
{
    public sealed class PhysicsPolygonDef : PhysicsShape
    {
        private Vector2[] m_Vertices;
        public int VertexCount { get; private set; }
        public Vector2[] Vertices { get { return m_Vertices; } }
        public Vector2 GetVertex(int p_Index) { return m_Vertices[p_Index];}


        public PhysicsPolygonDef(PhysicsPolygonDef other)
        {
            Trigger = other.Trigger;
            Shape = other.Shape;
            Bounciness = other.Bounciness;
            Mass = other.Mass;
            //Shape specific
            VertexCount = other.VertexCount;
            m_Vertices = other.m_Vertices;
        }
        public PhysicsPolygonDef(Vector2[] p_Vertices, bool isStatic = false, bool isTrigger = false, int p_Material = Properties.Material.SOLID)
        {
            //Debug.Assert(p_Vertices.Length > 2, "Polygon must have at least 3 or more vertices!");
            Trigger = isTrigger;
            Shape = SHAPE.POLYGON;
            Material = p_Material;
            //Water MUST be static (makes no sense to simulate rigid body of water)
            if (Material == Physics.Properties.Material.FLUID)
                isStatic = true;

            Density = (p_Material == Properties.Material.SOLID ? Properties.Density.WOOD : Properties.Density.WATER);      //??????
            Bounciness = 0.0f;
            VertexCount = p_Vertices.GetLength(0);
            m_Vertices = p_Vertices;

            //Calculate area/volume
            ComputeVolume();
            //Calculate Axis-aligned bounding box (to be implemented)
            ComputeAABB();
            //Calculate center of mass
            ComputeCenterOfMass();
            if (isStatic == false)
            {

                SetInertia(1f);
                SetMass(5f);

                //ComputeMass();
                //Calculate moment of inertia
                //ComputeInertia();                   ???????
            }
            else
            {
                SetInertia(0);
                SetMass(0);
            }
        }

        //Forced members
        public override void Rotate(float angle, Vector2 origin = new Vector2())
        {
            UnityEngine.Debug.Log("a " +Angle + " " + angle);
            Angle += angle;
            if (Angle > 2 * (float)Math.PI)
                Angle = 0;
            else if (Angle < 0)
                Angle = 2 * (float)Math.PI;

            float tempX;
            float tempY;
            for (int i = 0; i < VertexCount; i++)
            {
                //RenderManager.Instance.DrawLine(origin, origin+m_Vertices[i]);
                //m_Vertices[i] += origin;
                tempX = m_Vertices[i].x;
//                tempY = m_Vertices[i].y;


                m_Vertices[i].x = (float)(Math.Cos(-angle) * tempX - Math.Sin(-angle) * m_Vertices[i].y);
                m_Vertices[i].y = (float)(Math.Sin(-angle) * tempX + Math.Cos(-angle) * m_Vertices[i].y);
                
                //m_Vertices[i] -= origin;
            }
        }
        protected override void ComputeAABB()
        {
            float minX = 0;
            float minY = 0;
            float maxX = 0;
            float maxY = 0;
            for (int i = 0; i < VertexCount; i++)
            {
                if (m_Vertices[i].x < minX)
                    minX = m_Vertices[i].x;
                if (m_Vertices[i].y < minY)
                    minY = m_Vertices[i].y;
                if (m_Vertices[i].x > maxX)
                    maxX = m_Vertices[i].x;
                if (m_Vertices[i].y > maxY)
                    maxY = m_Vertices[i].y;
            }
            SetAABB(minX + maxX, minY + maxY);
        }

        public void SetMass(float mass)
        {
            Mass = mass;
        }

        public void SetInertia(float inertia)
        {
            Inertia = inertia;
        }

        protected override void ComputeMass()
        {
            Mass = Volume * Density * 0.0000002f; //TO BE CORRECTED (?)
            if (Mass > Limits.MAX_MASS)
                Mass = Limits.MAX_MASS;
            else if (Mass < Limits.MIN_MASS)
                Mass = Limits.MIN_MASS;
        }
        protected override void ComputeCenterOfMass()
        {
            Vector2 centroid = peMath.PolygonCentroid(Vertices, Volume);

            //Adjust all vertices to new center-of-mass
            for (int i = 0; i < VertexCount; i++)
                m_Vertices[i] -= centroid;
        }
        protected override void ComputeInertia()
        {

            Inertia = 1;
            Inertia = peMath.PolygonInertia(Vertices, Mass);
            if (Inertia > Limits.MAX_INERTIA)
                Inertia = Limits.MAX_INERTIA;
            else if (Inertia < Limits.MIN_INERTIA)
                Inertia = Limits.MIN_INERTIA;                      //Here is the point that the inertia is set!  ?????
        }
        protected override void ComputeVolume()
        {
            //Area will always be positive
            Volume = peMath.PolygonVolume(Vertices);
        }
    }
}