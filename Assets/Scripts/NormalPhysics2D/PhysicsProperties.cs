namespace Physics_Engine.Physics.Properties
{
    public static class SHAPE
    {
        public const int UNSPECIFIED = 0;
        public const int CIRCLE = 1;
        public const int POLYGON = 2;
        public const int NR_SHAPES = 3;
    }
    public static class Material
    {
        public const int UNSPECIFIED = 0;
        public const int SOLID = 1;
        public const int FLUID = 2;
        public const int NR_MATERIALS = 3;
    }
    public static class Density
    {
        public const int UNSPECIFIED = 0;
        public const int AIR = 2;
        public const int WOOD = 500;
        public const int OIL = 900;
        public const int WATER = 1000;
        public const int RUBBER = 1200;
        public const int PLASTIC = 1300;
        public const int STEEL = 8000;
    }
    public static class Friction//Not implemented yet
    {
        public const float ICE = 0.1f;
    }
    public static class Limits //Should be in metric-units eventually
    {
        public const float MAX_FORCE = 10000.0f;
        public const float MIN_FORCE = 5.0f;
        public const float MAX_LINEAR_VELOCITY = 500.0f;
        public const float MIN_LINEAR_VELOCITY = 5.0f;
        public const float MAX_ANGULAR_VELOCITY = 18.84f;
        public const float MIN_ANGULAR_VELOCITY = 0.001f;
        public const float MAX_MASS = 100.0f;
        public const float MIN_MASS = 0.001f;
        public const int MAX_INERTIA = 900000000;
        public const int MIN_INERTIA = 0;
    }
    public static class Units
    {
        public const int PIXELS_PER_METER = 32;
        public const float PIXELS_PER_METER_INVERSE = (float)1 / (float)PIXELS_PER_METER;
    }
}