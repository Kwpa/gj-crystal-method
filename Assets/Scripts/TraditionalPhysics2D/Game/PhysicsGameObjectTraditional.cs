using UnityEngine;
using Physics_Engine.Physics;
using Physics_Engine.Physics.Properties;

namespace Physics_Engine
{
    public sealed class PhysicsGameObjectTraditional
    {
        private PhysicsBody m_Body;
        private Texture2D m_Texture;
        private Color m_Color;

        public Vector2 Position {   get { return m_Body.Position; }
                                    private set { m_Body.Position = value; } }
        public PhysicsBody Body {   get { return m_Body; } }

        public PhysicsGameObjectTraditional(Vector2 p_Pos, Texture2D p_Texture, PhysicsShape p_ShapeDef) 
        {
            m_Texture = p_Texture;
            m_Body = new PhysicsBody(p_Pos, p_ShapeDef);
            //switch (p_ShapeDef.Material)
            //{
            //    case Material.SOLID:
            //        m_Color = Color.SandyBrown;
            //        break;
            //    case Material.FLUID:
            //        m_Color = Color.CornflowerBlue;
            //        break;
            //}
        }
        public void Update() {

            if (Body.Shape == SHAPE.CIRCLE)
            { 
                UnityEngine.Debug.Log("p:" + Position + " ," + Body.CircleDef.Radius);
                UnityEngine.Debug.DrawLine(Position, Position + new Vector2(Body.CircleDef.Radius, Body.CircleDef.Radius), m_Color);
            }
            else
            {
                UnityEngine.Debug.Log("p:" + Position + " ," +  Body.PolygonDef.GetVertex(0) + " " + Body.PolygonDef.GetVertex(2));  
                for (int i = 0; i < Body.PolygonDef.VertexCount; i++)
                    UnityEngine.Debug.DrawLine(Position + Body.PolygonDef.GetVertex(i), Position + Body.PolygonDef.GetVertex((i + 1 == Body.PolygonDef.VertexCount ? 0 : i + 1)), m_Color);
            }
        }
    }
}