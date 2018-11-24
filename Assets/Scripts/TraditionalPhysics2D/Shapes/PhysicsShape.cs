using UnityEngine;

namespace Physics_Engine.Physics
{
    public abstract class PhysicsShape
    {
        public struct AABB
        {
            public Vector2 Center { get; set; }
            public float HalfWidth { get; set; }
            public float HalfHeight { get; set; }
        }
        private AABB m_AAABB;
        public int Shape { get; protected set; }
        public bool Trigger { get; protected set; }
        public int Material { get; protected set; }
        public int Density { get; set; }
        public float Angle { get; protected set; }
        public float Volume { get; protected set; }
        public float Bounciness { get; set; }
        public float Mass { get; set; }
        public float Inertia { get; set; }

        protected void SetAABB(float p_HalfWidth, float p_HalfHeight) 
        {
            m_AAABB.Center = new Vector2();
            m_AAABB.HalfWidth = p_HalfWidth;
            m_AAABB.HalfHeight = p_HalfHeight;
        }
        public AABB GetAABB() { return m_AAABB; }
        public abstract void Rotate(float angle, Vector2 originTranslation = new Vector2());
        protected abstract void ComputeAABB();
        protected abstract void ComputeVolume();
        protected abstract void ComputeMass();
        protected abstract void ComputeCenterOfMass();
        protected abstract void ComputeInertia();
    }
}