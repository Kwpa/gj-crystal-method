using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using System;


public class EnterExitVehicleSystem : JobComponentSystem
{
    private struct EnterExitVehicleJob : IJobParallelFor
    {
        [ReadOnly] public EntityArray entityArray;
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        Int16 moveDirection;

        public void Execute(int index)
        {
            Entity e = entityArray[index];
            if (moveDirection > 0)
            {
                EntityCommandBuffer.AddComponent(index, e, new EnterVehicle());
            }
            else if (moveDirection < 0)
            {
                EntityCommandBuffer.AddComponent(index, e, new ExitVehicle());
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return base.OnUpdate(inputDeps);
    }
}

