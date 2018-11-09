using System;
using UnityEngine;

using Physics_Engine.Physics.Properties;

namespace Physics_Engine.Physics
{
    public sealed class PhysicsCircleDef : PhysicsShape
    {
        public PhysicsCircleDef(PhysicsCircleDef other)
        {
            Shape = other.Shape;
            Bounciness = other.Bounciness;
            Mass = other.Mass;
            Inertia = other.Inertia;
            //Shape specific
            Radius = other.Radius;
        }
        public PhysicsCircleDef(float p_Radius, bool isStatic = false, int p_Material = Properties.Material.SOLID)
        {
            Bounciness = 0.7f;
            Material = p_Material;
            Density = (p_Material == Properties.Material.SOLID ? Properties.Density.PLASTIC : Properties.Density.WATER);
            Radius = p_Radius;
            Shape = SHAPE.CIRCLE;
            ComputeVolume();
            ComputeAABB();
            if (isStatic == false)
            {
                ComputeMass();
                //Calculate moment of inertia
                ComputeInertia();
            }
            else
            {
                Mass = 0;
                Inertia = 0;
            }
        }
        public float Radius { get; private set; }

        //Forced members
        public override void Rotate(float angle, Vector2 originTranslation = new Vector2())
        {
            Angle += angle;
            if (Angle > 2 * (float)Math.PI)
                Angle = 0;
            else if (Angle < 0)
                Angle = 2 * (float)Math.PI;
        }
        protected override void ComputeAABB()
        {
            SetAABB(Radius, Radius);
        }
        protected override void ComputeMass()
        {
            Mass = Volume * Density * 0.0000002f;
            if (Mass > Limits.MAX_MASS)
                Mass = Limits.MAX_MASS;
            else if (Mass < Limits.MIN_MASS)
                Mass = Limits.MIN_MASS;
        }
        protected override void ComputeCenterOfMass()
        {

        }
        protected override void ComputeInertia()
        {
            //Circle inertia = 1/2 * (m * r^2)
            Inertia = 0.5f * Mass * (Radius * Radius);
        }
        protected override void ComputeVolume()
        {
            //π * Radius^2
            Volume = (float)Math.PI * (Radius * Radius);
        }
    }
}