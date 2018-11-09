using System;
using System.Collections.Generic;
using UnityEngine;

namespace Physics_Engine.Helpers
{
    public static class peMath
    {
        public static double Angle(Vector2 v1, Vector2 v2)
        {
            //Make sure vectors are normalized
            v1.Normalize();
            v2.Normalize();
            double angle = System.Math.Atan2(v2.y - v1.y, v2.x - v1.x);
            //To avoid insignificant angles
            if (angle < 0.0001)
                return 0;
            else
                return angle;
        }
        public static Vector2 LeftPerp(Vector2 v)
        {
            return new Vector2(-v.y, v.x);
        }
        public static Vector2 RightPerp(Vector2 v)
        {
            return new Vector2(v.y, -v.x);
        }
        public static bool LineLineIntersection(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.zero;
            float denom = ((end1.x - start1.x) * (end2.y - start2.y)) - ((end1.y - start1.y) * (end2.x - start2.x));

            //  AB & CD are parallel 
            if (denom == 0)
                return false;

            float numer = ((start1.y - start2.y) * (end2.x - start2.x)) - ((start1.x - start2.x) * (end2.y - start2.y));

            float r = numer / denom;

            float numer2 = ((start1.y - start2.y) * (end1.x - start1.x)) - ((start1.x - start2.x) * (end1.y - start1.y));

            float s = numer2 / denom;

            if ((r < 0 || r > 1) || (s < 0 || s > 1))
                return false;

            // Find intersection point
            intersectionPoint = new Vector2();
            intersectionPoint.x = start1.x + (r * (end1.x - start1.x));
            intersectionPoint.y = start1.y + (r * (end1.y - start1.y));

            return true;
        }
        public static List<Vector2> FindConvexHull(List<Vector2> p_Vertices)
        {
            //Only work when all points together can form a convex polygon (only "halfway" implemented variant of Grahams Scan)
            if (p_Vertices.Count == 0)
                return p_Vertices;

            List<Vector2> convexHull = new List<Vector2>();
            Vector2 pivot = p_Vertices[0];
            //Find pivot-vertex
            foreach (Vector2 vertex in p_Vertices)
            {
                if (vertex.y < pivot.y)
                    pivot = vertex;
                else if (vertex.y == pivot.y)
                    if (vertex.x > pivot.x)
                        pivot = vertex;
            }

            //Should use better sort, but I'm to lazy atm... (:

            //Just sort all vertices by the vector pivot -> vertex depending on the angle to 
            //the horizontal plane (Will also always be sorted in clockwise order)
            p_Vertices.Sort(delegate(Vector2 v1, Vector2 v2) { return peMath.Angle(Vector2.right, v1 - pivot).CompareTo(peMath.Angle(Vector2.right, v2 - pivot)); });   

            //MAYBE breaks down here?

            return p_Vertices;
        }
        public static Vector2 PolygonCentroid(Vector2[] p_Vertices, float p_Volume)
        {
            Vector2 centroid = new Vector2();
            for (int i = 0; i < p_Vertices.Length; i++)
            {
                Vector2 vertex1 = p_Vertices[i];
                Vector2 vertex2 = p_Vertices[i + 1 < p_Vertices.Length ? i + 1 : 0];
                centroid.x += (vertex1.x + vertex2.x) * (vertex1.x * vertex2.y - vertex2.x * vertex1.y);
                centroid.y += (vertex1.y + vertex2.y) * (vertex1.x * vertex2.y - vertex2.x * vertex1.y);
            }
            centroid.x *= 1 / (6 * p_Volume);
            centroid.y *= 1 / (6 * p_Volume);
            return centroid;
        }
        public static float PolygonInertia(Vector2[] p_Vertices, float p_Mass)
        {
            //Polygon inertia (Credit to chipmunk physics engine!)
            float sum1 = 0.0f;
            float sum2 = 0.0f;
            for (int i = 0; i < p_Vertices.Length; i++)
            {
                Vector2 v1 = p_Vertices[i];
                Vector2 v2 = p_Vertices[(i + 1) % p_Vertices.Length];
                v2.Normalize();

                float a = Vector2.Dot(v2, v1);
                float b = Vector2.Dot(v1, v1) + Vector2.Dot(v1, v2) + Vector2.Dot(v2, v2);

                sum1 += a * b;
                sum2 += a;
            }
            return Math.Abs((p_Mass * sum1) / (6.0f * sum2));
        }
        public static float PolygonVolume(Vector2[] p_Vertices)
        {
            float area = 0;
            int j = p_Vertices.Length - 1;

            for (int i = 0; i < p_Vertices.Length; i++)
            {
                area += (p_Vertices[j].x + p_Vertices[i].x) * (p_Vertices[j].y - p_Vertices[i].y);
                j = i;
            }
            //If polygon represented in clockwise order (as it should be) area will be negative, 
            //take abs(area) to get positive area
            return Math.Abs(area * 0.5f);
        }
    }
}
