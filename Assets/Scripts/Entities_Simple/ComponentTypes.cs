using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct EnterVehicle : IComponentData { }

public struct ExitVehicle : IComponentData { }

public struct PlayerInput : IComponentData
{
    public float3 Move;
    public float Look;
    public float3 Scale;
}

public struct AIControlInput : IComponentData
{
    public float3 Move;
    public float Look;
    public float3 Scale;
}

public struct CameraZoom : IComponentData
{
    public float Value;
}

public struct Health : IComponentData
{
    public float Value;
}

public struct ForceBasedMovement : IComponentData
{
    public float MovementSpeed;
    public float RotationSpeed;
    public float TurningCircleForce;
}

public struct TransformBasedMovement : IComponentData
{
    public float MovementSpeed;
    public float RotationSpeed;
}

public struct TimePeriod : IComponentData
{
    public float Value;
}

public struct Depth : IComponentData
{
    public int Value;
}

public struct Size : IComponentData
{
    public int Value;
}

public struct ApplyGravity : IComponentData
{
}

public struct Physics2DEntity : IComponentData
{
    private int ID;
    private float2 Force;
    private float2 Acceleration;
    private float2 Velocity;
    private float AngularVelocity;
    private float2 Position;
    private float2 LastPosition;
    private float InvMass;
    private float InvInertia;
}

[InternalBufferCapacity(8)]
public struct VerticesBuffer : IBufferElementData
{
    public float2 vertPosition;
}

public struct Vertices : IComponentData
{

}

public struct VehicleType : IComponentData
{
    public int Value;
}

public struct Physics2DPolygonShape : IComponentData
{
    public float2 AABBCentre;
    public float AABBHalfWidth;
    public float AABBHalfHeight;

    public float Angle;
    public float Bounciness;
    public float Mass;
    public float Inertia;
    //public int VertexCount;
    //private float2 m_Vertices; //should be an array but can't have arrays
    //public float Volume;
    //public int Material;
    //public int Density;
}

public struct Physics2DAwake : IComponentData
{
}

public struct Physics2DTrigger : IComponentData
{
}

public struct Physics2DCollider : IComponentData
{
}

public struct PreventTransform : IComponentData
{
}

public struct ShapeSetupIncomplete : IComponentData
{
}

