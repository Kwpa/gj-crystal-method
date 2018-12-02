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

    public enum ColliderType
    {
        square1=0,
        square2=1,
        square3=2,
        square4=3
    }

    public class ApplyGravityBarrier : BarrierSystem { }
    public class ShapeBarrier : BarrierSystem { }

    struct Data
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<Position> Positions;
        public ComponentDataArray<Physics2DEntity> Physics2D;
    }

    struct VertData
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<ShapeSetupIncomplete> Setup;
        public BufferArray<VerticesBuffer> VerticesArray;
    }

    [Inject] Data _data;
    [Inject] VertData _vertData;
    [Inject] ApplyGravityBarrier _gravBarrier;
    [Inject] private ShapeBarrier _shapeBarrier;

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

    struct SetupPolygonShapeJob : IJobParallelFor
    {
        [ReadOnly] public EntityArray EntityArray;
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public BufferArray<VerticesBuffer> VertsArray;
        
        public void Execute(int i)
        {
            VerticesBuffer vertCoord = new VerticesBuffer();
            vertCoord.vertPosition = new float2(-0.5f, 0.5f);
            VertsArray[i].Add(vertCoord);
            vertCoord.vertPosition = new float2(0.5f, 0.5f);
            VertsArray[i].Add(vertCoord);
            vertCoord.vertPosition = new float2(0.5f, -0.5f);
            VertsArray[i].Add(vertCoord);
            vertCoord.vertPosition = new float2(-0.5f, -0.5f);
            VertsArray[i].Add(vertCoord);

            EntityCommandBuffer.RemoveComponent<ShapeSetupIncomplete>(i,EntityArray[i]);   //need entitycommandbuffer to access manager commands!!!

            //switch (VehicleType[i].Value)
            //{
            //    case 0:
            //        float x = 1; float y = 1; float xm = -1; float ym = -1;
            //        x *= width * 0.5f;
            //        y *= height * 0.5f;
            //        xm *= width * 0.5f;
            //        ym *= height * 0.5f;
            //        vertices[0] = new Vector2(x, y);
            //        vertices[1] = new Vector2(xm, y);
            //        vertices[2] = new Vector2(xm, ym);
            //        vertices[3] = new Vector2(x, ym);
            //        break;
            //}

        }
    }

    struct DrawPolygonShapeJob : IJobParallelFor
    {
        [ReadOnly] public EntityArray EntityArray;
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public BufferArray<VerticesBuffer> VertsArray;

        public void Execute(int i)
        {
            //VerticesBuffer vertCoord = EntityCommandBuffer.bu
            

            //Debug.DrawLine(new Vector3(vertCoord.vertPosition.x, vertCoord.vertPosition.y), new Vector3());
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
        var setupVertsJob = new SetupPolygonShapeJob
        {
            EntityArray = _vertData.Entities, 
            EntityCommandBuffer = _shapeBarrier.CreateCommandBuffer().ToConcurrent(),
            VertsArray = _vertData.VerticesArray
            
        }.Schedule(_vertData.Length, 64, inputDeps);

        setupVertsJob.Complete();

        //var gravityJob = new ApplyGravityJob //add the applygravity job to the list of entities
        //{
        //    Positions = _data.Positions,
        //    EntityArray = _data.Entities,
        //    EntityCommandBuffer = _gravBarrier.CreateCommandBuffer().ToConcurrent(),
        //    yVel = 1,
        //    deltaTime = Time.deltaTime

        //}.Schedule(_data.Length, 64, inputDeps);
        //gravityJob.Complete();

        return base.OnUpdate(inputDeps);
    }
}
