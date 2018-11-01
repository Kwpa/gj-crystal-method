using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct PlayerInput : IComponentData {

    public float3 Move;
}

public struct Health : IComponentData
{
    public float Value;
}

public struct MoveSpeed : IComponentData
{
    public float speed;
}
