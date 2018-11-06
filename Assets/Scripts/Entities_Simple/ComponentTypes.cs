using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
