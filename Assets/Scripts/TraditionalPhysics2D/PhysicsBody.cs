using System;
using UnityEngine;

using Physics_Engine.Physics.Properties;

using Physics_Engine.Helpers;

namespace Physics_Engine.Physics
{
    public enum FORCE_TYPE
    {
        IMPULSE,
        FORCE
    };
    public class PhysicsBody
    {
        private static int m_ObjectCount = 0;
        private bool m_ShowDebug = true;

        //SHAPES
        public PhysicsShape ShapeDef = null;
        public PhysicsCircleDef CircleDef = null;
        public PhysicsPolygonDef PolygonDef = null;
        //SHAPES

        private int m_ID;
        private Vector2 m_Force = new Vector2();
        private Vector2 m_Acceleration = new Vector2();
        private Vector2 m_Velocity = new Vector2();
        private float   m_AngularVelocity = 0;
        private Vector2 m_Position = new Vector2();
        private Vector2 m_LastPosition = new Vector2();
        private float   m_InvMass = -1;
        private float   m_InvInertia = -1;

        public PhysicsBody(Vector2 p_Pos, PhysicsShape p_ShapeDef)
        {
            //Used to avoid type-checking during update
            ShapeDef = p_ShapeDef;
            Position = p_Pos;
            Bounciness = p_ShapeDef.Bounciness;
            //Inverses will be calculated in property
            Mass = p_ShapeDef.Mass;
            Debug.Log("mass ---- " + Mass);
            Inertia = p_ShapeDef.Inertia;

            m_ID = m_ObjectCount++;
            //Typecast only once here for efficiency (therefor no need to typecast every frame during collision-checks)
            switch (p_ShapeDef.Shape)
            {
                case SHAPE.CIRCLE:
                    CircleDef = (PhysicsCircleDef)p_ShapeDef;
                    break;
                case SHAPE.POLYGON:
                    PolygonDef = (PhysicsPolygonDef)p_ShapeDef;
                    break;
            }
            //Add to PhysicsManager to simulate in the world
            PhysicsManager.Instance.AddBody(this);
        }
        public virtual void Update()
        {
            LastPosition = Position;
            float deltatime = Time.deltaTime;
            Acceleration = Force * InvMass;
            Velocity += Acceleration * deltatime;
            //Damping
            Velocity *= 0.99f;
            AngularVelocity *= 0.995f;
            //Update position
            Position += (Velocity * deltatime);
            //Update angle
            ShapeDef.Rotate(AngularVelocity * deltatime); //??????
            //PolygonDef.Rotate(AngularVelocity * deltatime); 

            //Center of mass
            if(IsStatic() == false)
//                RenderManager.Instance.DrawCircle(Position, 6);     ////????

#region Debug
            /*if (m_ShowDebug == true)
            {
                //TEMP
                //RenderManager.Instance.DrawString("Mass: " + Mass, Position);
                //RenderManager.Instance.DrawString("Force: " + Force.Length().ToString(), Position);
                //RenderManager.Instance.DrawString("Angle: " + Angle.ToString(), Position);
                //RenderManager.Instance.DrawString("Mass: " + Mass.ToString(), Position + new Vector2(0, 10));
                //RenderManager.Instance.DrawString("Bounciness: " + Bounciness.ToString(), Position + new Vector2(0, 20));
                //RenderManager.Instance.DrawString("InvInertia: " + InvInertia.ToString());
                //RenderManager.Instance.DrawString("InvMass: " + InvMass.ToString());
                //RenderManager.Instance.DrawString(GetID().ToString(), Position);
                //RenderManager.Instance.DrawString("Angular Velocity: " + AngularVelocity.ToString());
                //RenderManager.Instance.DrawString(Position.ToString(), new Vector2(Position.X, Position.Y));
                //RenderManager.Instance.Draw("Force" + Force.ToString());
                //RenderManager.Instance.DrawLine(Position, Position + Force);
                //if (m_Position.X > GameManager.Instance.ScreenWidth - Radius && Velocity.X > 0)
                //{
                //    m_Position.X = GameManager.Instance.ScreenWidth - Radius;
                //    AddForce(new Vector2((-Velocity.X * Bounciness) - m_Velocity.X, 0), FORCE_TYPE.IMPULSE);
                //}
                //if (m_Position.X < Radius && Velocity.X < 0)
                //{
                //    m_Position.X = Radius;
                //    AddForce(new Vector2((-Velocity.X * Bounciness) - m_Velocity.X, 0), FORCE_TYPE.IMPULSE);
                //}
                //if (m_Position.Y > GameManager.Instance.ScreenHeight - Radius && Velocity.Y > 0)
                //{
                //    m_Position.Y = GameManager.Instance.ScreenHeight - Radius;
                //    AddForce(new Vector2(0, (-Velocity.Y * Bounciness) - m_Velocity.Y), FORCE_TYPE.IMPULSE);
                //}
                //if (m_Position.Y < Radius && Velocity.Y < 0)
                //{
                //    m_Position.Y = Radius;
                //    AddForce(new Vector2(0, (-Velocity.Y * Bounciness) - m_Velocity.Y), FORCE_TYPE.IMPULSE);
                //}

                //if (m_ID == 0)
                //{
                //    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                //        AddForce(-Velocity, FORCE_TYPE.IMPULSE);
                //    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                //        AddForce(new Vector2(500, 0));
                //    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                //        AddForce(new Vector2(-500, 0));
                //    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                //        AddForce(new Vector2(0, -500));
                //    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                //        AddForce(new Vector2(0, 500));
                //    if (Keyboard.GetState().IsKeyDown(Keys.M))
                //    {
                //        if (Keyboard.GetState().IsKeyDown(Keys.Add))
                //            Mass = Mass < 5.0f ? Mass + 0.01f : 5.0f;
                //        else if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                //            Mass = Mass > 0.5f ? Mass - 0.01f : 0.5f;
                //    }
                //    if (Keyboard.GetState().IsKeyDown(Keys.B))
                //    {
                //        if (Keyboard.GetState().IsKeyDown(Keys.Add))
                //            Bounciness = Bounciness < 1.0f ? Bounciness + 0.01f : 1.0f;
                //        else if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                //            Bounciness = Bounciness > 0.01f ? Bounciness - 0.01f : 0;
                //    }
                //}
                //TEMP
                if (GetID() == m_ObjectCount - 1)
                {
                    if (InputManager.Instance.KeyIsPressed(Keys.Right))
                        AddTorque(-Inertia * 0.1f);
                    if (InputManager.Instance.KeyIsPressed(Keys.Left))
                        AddTorque(Inertia * 0.1f);
                    if (InputManager.Instance.KeyIsPressed(Keys.Up))
                        AddForce(new Vector2(0, -50 * Mass), Position, FORCE_TYPE.IMPULSE);
                    if (InputManager.Instance.KeyIsPressed(Keys.Down))
                        AddForce(new Vector2(0, 50 * Mass), Position, FORCE_TYPE.IMPULSE);
                    if (InputManager.Instance.KeyIsPressed(Keys.Space))
                        AngularVelocity = 0;
                    if (InputManager.Instance.KeyIsPressed(Keys.D1))
                        Mass += 0.01f;
                    if (InputManager.Instance.KeyIsPressed(Keys.D2) && Mass > 0)
                        Mass -= 0.01f;
                }
                //TEMP
            }*/
#endregion
            //Remove old force
            AddForce(-Force, Position);
        }
        public virtual int GetID() { return m_ID; }
        public bool IsStatic() { return ShapeDef.Mass == 0; /*Should have m_IsStatic variable, fix later*/}
        public void AddForce(Vector2 p_Force, Vector2 p_Positition, FORCE_TYPE p_ForceType = FORCE_TYPE.FORCE, bool applyRotation = true) 
        {
            switch (p_ForceType)
            {
                case FORCE_TYPE.FORCE:
                    //All forces are seen as vectors in pixel-units and converted to "metric" representation (See PhysicsProperties) <------ NOT IMPLEMENTED YET
                    Force += p_Force/* * Units.PIXELS_PER_METER_INVERSE*/;
                    //If force really small, set to zero
                    //Force = (Force.Length() < Limits.MIN_FORCE) ? Vector2.Zero : Force;
                    break;
                case FORCE_TYPE.IMPULSE:
                    Velocity += p_Force * InvMass;
                    break;
            }
            //Early out
            if (p_Positition == Position)
                return;
            
            if(applyRotation)
            {
                //calculate torque
                //Radius-vector from center of mass to position force being applied
                Vector2 radiusVector = p_Positition - Position;
                Vector2 radiusVectorNormal = peMath.RightPerp(radiusVector);
                radiusVectorNormal.Normalize();

                float torque = Vector2.Dot(p_Force, radiusVectorNormal) * radiusVector.magnitude;
                //Apply torque generated by the force acting on the body
                AddTorque(torque);
            }
        }
        public void AddTorque(float p_Torque)
        {
            AngularVelocity += p_Torque * InvInertia;
            //RenderManager.Instance.DrawString("Torque: " + p_Torque.ToString());
        }
        public Vector2 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }
        public Vector2 LastPosition
        {
            get { return m_LastPosition; }
            private set { m_LastPosition = value; }
        }
        public Vector2 Force 
        {  
            get { return m_Force; } 
            private set { m_Force = value; }
        }
        public Vector2 Velocity
        {
            get { return m_Velocity; }
            private set { 
                m_Velocity = value;
                //Limit (Add if necessary, inefficient!)
                //if(m_Velocity.Length() > Limits.MAX_LINEAR_VELOCITY)
            }
        }
        public float AngularVelocity
        {
            get { return m_AngularVelocity; }
            private set { 
                m_AngularVelocity = value;
                //Limit to avoid insanity
                if (m_AngularVelocity > Limits.MAX_ANGULAR_VELOCITY)
                    m_AngularVelocity = Limits.MAX_ANGULAR_VELOCITY;
                else if (Math.Abs(m_AngularVelocity) < Limits.MIN_ANGULAR_VELOCITY)
                    m_AngularVelocity = 0;                                               //?????? 
            }
        }
        public Vector2 Acceleration
        {
            get { return m_Acceleration; }
            private set { m_Acceleration = value; }
        }
        public int Shape { 
            get { return ShapeDef.Shape; } 
        }
        public int Material
        {
            get { return ShapeDef.Material; }
        }
        public float Angle
        {
            get { return ShapeDef.Angle; }
            //set { ShapeDef.Angle = value; }//Must make sure body rotates to new angle   <---- TO FIX?
        }
        public float Bounciness { 
            get { return ShapeDef.Bounciness; }
            set { ShapeDef.Bounciness = value; } 
        }
        public float Mass { 
            get { return ShapeDef.Mass; } 
            private set 
            { 
                ShapeDef.Mass = value;
                if (ShapeDef.Mass > 0)
                    m_InvMass = 1 / ShapeDef.Mass;
                else
                    m_InvMass = 0;
            }
        }
        public float InvMass { 
            get { return m_InvMass; }
        }
        public float Inertia
        {
            get { return ShapeDef.Inertia; }
            private set 
            { 
                ShapeDef.Inertia = value;
                if (ShapeDef.Inertia > 0)
                    m_InvInertia = 1 / ShapeDef.Inertia;
                else
                    m_InvInertia = 0;
            }
        }
        public float InvInertia
        {
            get { return m_InvInertia; }
        }
    }
}