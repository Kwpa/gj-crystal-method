using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class ECSPhysics2D : JobComponentSystem{

    public class ApplyGravityBarrier : BarrierSystem { }

    struct Data
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<Position> Positions;
        public ComponentDataArray<Physics2DEntity> Physics2D;
    }

    [Inject] Data _data;
    [Inject] ApplyGravityBarrier _gravBarrier;



    struct ApplyGravityJob : IJobParallelFor
    {
        [ReadOnly] public EntityArray EntityArray;
        public ComponentDataArray<Position> Positions;
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public float yVel;
        public float deltaTime;

        public void Execute(int i)
        {
            Positions[i] = new Position { Value = Positions[i].Value + new float3(0, yVel, 0) * deltaTime };
        }
    }

    struct CollisionDetectionJob : IJobParallelFor
    {
        public void Execute(int i)
        {
            
        }
    }

    struct ApplyForceAndTorqueJob : IJobParallelFor
    {
        public void Execute(int i)
        {
           
        }
    }

    struct TriggerEvent : IJobParallelFor
    {
        public void Execute(int i)
        {

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) //on update, if true do this
    {
        var gravityJob = new ApplyGravityJob //add the applygravity job to the list of entities
        {
            Positions = _data.Positions,
            EntityArray = _data.Entities,
            EntityCommandBuffer = _gravBarrier.CreateCommandBuffer().ToConcurrent(),
            yVel = 1,
            deltaTime = Time.deltaTime

        }.Schedule(_data.Length, 64, inputDeps);
        gravityJob.Complete();

        return base.OnUpdate(inputDeps);
    }

}
