using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

public class PlayerMoveSystem : ComponentSystem
{
    public struct Data
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Rotation> Heading;
        public ComponentDataArray<PlayerInput> Input;
        public ComponentDataArray<Parent> Parent;
    }

    [Inject] private Data m_Data;

    protected override void OnUpdate()
    {
        var settings = Bootstrap.Settings;

        float dt = Time.deltaTime;
        for (int index = 0; index < m_Data.Length; ++index)
        {
            var position = m_Data.Position[index].Value;
            var rotation = m_Data.Heading[index].Value;

            var playerInput = m_Data.Input[index];

            Parent p = m_Data.
            position += dt * playerInput.Move * settings.playerMovementSpeed;
            rotation.value.z += dt * playerInput.Look * settings.playerRotationSpeed;

            m_Data.Position[index] = new Position { Value = position };
            m_Data.Heading[index] = new Rotation { Value = rotation };
            m_Data.Input[index] = playerInput;
        }
    }
}
