using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

public class PlayerMoveSystem : ComponentSystem
{
    public struct PlayerData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Rotation> Heading;
        public ComponentDataArray<PlayerInput> Input;
        public ComponentDataArray<Scale> Scale;
    }

    public struct AIPlayerData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Rotation> Heading;
        public ComponentDataArray<AIControlInput> Input;
        public ComponentDataArray<Scale> Scale;
        public SubtractiveComponent<PreventTransform> PreventTransforms;
    }

    [Inject] private PlayerData p_Data;
    [Inject] private AIPlayerData ai_Data;

    protected override void OnUpdate()
    {
        var settings = Bootstrap.Settings;
        float dt = Time.deltaTime;

        for (int index = 0; index < p_Data.Length; ++index)
        {
            var position = p_Data.Position[index].Value;
            var rotation = p_Data.Heading[index].Value;
            var scale = p_Data.Scale[index].Value;

            var playerInput = p_Data.Input[index];

            //rotation
            float3 e = UpdateRotation(rotation, playerInput.Look, dt, settings.playerRotationSpeed);
            rotation = quaternion.EulerXYZ(e);

            //movement
            position = UpdatePosition(position, e.z, playerInput.Move, dt, settings.playerMovementSpeed);

            p_Data.Position[index] = new Position { Value = position };
            p_Data.Heading[index] = new Rotation { Value = rotation };
            p_Data.Input[index] = playerInput;
        }

        //ai
        for (int index = 0; index < ai_Data.Length; ++index)
        {
            var position = ai_Data.Position[index].Value;
            var rotation = ai_Data.Heading[index].Value;
            var scale = ai_Data.Scale[index].Value;
            var AIPlayerInput = ai_Data.Input[index];

            //rotation
            float3 e = UpdateRotation(rotation, AIPlayerInput.Look, dt, settings.AIPlayerRotationSpeed);
            rotation = quaternion.EulerXYZ(e);    

            //movement
            position = UpdatePosition(position, e.z, AIPlayerInput.Move, dt, settings.AIPlayerMovementSpeed);

            ai_Data.Position[index] = new Position { Value = position };
            ai_Data.Heading[index] = new Rotation { Value = rotation };
            ai_Data.Input[index] = AIPlayerInput;
        }
    }

    float3 UpdateRotation(quaternion currentRotation, float rotationAmount, float dt, float rotationSpeed)
    {
        float4 rot = currentRotation.value;
        Quaternion q = new Quaternion(rot.x, rot.y, rot.z, rot.w);
        float x = q.eulerAngles.x; float y = q.eulerAngles.y; float z = q.eulerAngles.z;
        float change = rotationAmount * dt * rotationSpeed;
        x *= Mathf.PI / 180;
        y *= Mathf.PI / 180;
        z *= Mathf.PI / 180;
        change *= Mathf.PI / 180;
        z -= change;

        return new float3(x, y, z);
    }

    float3 UpdatePosition(float3 currentPosition, float currentZRotation, float3 movementInput, float dt, float movementSpeed)
    {
        float3 v = movementInput;
        v.x = v.y * -math.sin(currentZRotation);
        v.y = v.y * math.cos(currentZRotation);

        currentPosition += dt * v * movementSpeed;
        return currentPosition;
    }
}
