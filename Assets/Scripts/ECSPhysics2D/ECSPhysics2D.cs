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
        public BufferArray<EntityBuffer> AssociatedEntityBuffer;
    }

    struct VertData
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<ShapeSetupIncomplete> Setup;
        public BufferArray<VerticesBuffer> VerticesArray;
    }

    struct LineData
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<NodeIncomplete> Setup;
        public SubtractiveComponent<ShapeSetupIncomplete> Sub;
        public BufferArray<VerticesBuffer> VerticesArray;
    }

    struct FinalData
    {
        public readonly int Length;
        public EntityArray Entities;
        public SubtractiveComponent<ShapeSetupIncomplete> Sub_shape;
        public SubtractiveComponent<NodeIncomplete> Sub_line;
        public BufferArray<VerticesBuffer> VerticesArray;
    }


    [Inject] Data _data;
    [Inject] VertData _vertData;
    [Inject] LineData _lineData;
    [Inject] FinalData _finalData;

    [Inject] ApplyGravityBarrier _gravBarrier;
    [Inject] ShapeBarrier _shapeBarrier;

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
        public ComponentDataArray<Position> PositionArray;
        public void Execute(int i)
        {
            VerticesBuffer vertCoord = new VerticesBuffer();
            float x = 0; float y = 0;
            float3 pos = PositionArray[i].Value;

            x = -0.5f; y = 0.5f;
            vertCoord.vertPositions = new float4(x, y, x + pos.x, y +pos.y);
            VertsArray[i].Add(vertCoord);

            x = 0.5f; y = 0.5f;
            vertCoord.vertPositions = new float4(x, y, x + pos.x, y + pos.y);
            VertsArray[i].Add(vertCoord);

            x = 0.5f; y = -0.5f;
            vertCoord.vertPositions = new float4(x, y, x + pos.x, y + pos.y);
            VertsArray[i].Add(vertCoord);

            x = -0.5f; y = -0.5f;
            vertCoord.vertPositions = new float4(x, y, x + pos.x, y + pos.y);
            VertsArray[i].Add(vertCoord);

            EntityCommandBuffer.RemoveComponent<ShapeSetupIncomplete>(i,EntityArray[i]);   //need entitycommandbuffer to access manager commands!!!
        }
    }

    struct UpdateVerticesJob : IJobParallelFor
    {
        [ReadOnly] public EntityArray EntityArray;
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public BufferArray<VerticesBuffer> VertsArray;
        public ComponentDataArray<Position> PositionArray;
        public ComponentDataArray<NodeID> LineIDArray;
        //public NativeArray<float2> meshVerts;

        public void Execute(int i)
        {
            var vertArray = VertsArray[i].Reinterpret<float4>();
            float3 pos = PositionArray[i].Value;

            for (int j = 0; j<vertArray.Length; j++)
            {
                float x = vertArray[j].x; float y = vertArray[j].y;
                float z = x + pos.x; float w = y + pos.y;
                vertArray[j] = new float4(x, y, z, w);
                
                float zLast = 0; float wLast = 0;

                if (j == 0)
                {
                    zLast = vertArray[vertArray.Length-1].z;
                    wLast = vertArray[vertArray.Length-1].w;
                }
                else
                {
                    zLast = vertArray[j-1].z;
                    wLast = vertArray[j-1].w;
                }

                //EntityCommandBuffer.CreateEntity(i, Bootstrap.NodeArchetype);

            }
        }
    }

    struct CreateLinesJob : IJobParallelFor
    {
        [ReadOnly] public EntityArray EntityArray;
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public BufferArray<VerticesBuffer> VertsArray;

        public void Execute(int i)
        {
            int count = VertsArray.Length;
            for(int j = 0; j < count; j++)
            {
                EntityCommandBuffer.CreateEntity(i, Bootstrap.NodeArchetype);
            }

            EntityCommandBuffer.RemoveComponent<NodeIncomplete>(i, EntityArray[i]);   //need entitycommandbuffer to access manager commands!!!

        }
    }

    struct UpdatePolygonShapeJob : IJobParallelFor
    {
        [ReadOnly] public EntityArray EntityArray;
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public BufferArray<VerticesBuffer> VertsArray;
        public ComponentDataArray<Position> PositionArray;
        public int countVerts;

        public void Execute(int index)
        {

            // for each poly get a group of nodes and edit their position
            // each vert is a position we can enter into this job!
            // so... job instance 1 should be for node 1 which should be assigned to shape 1
            // and...job instance 1 should also be for node 2

            int count = VertsArray.Length;
            var vertArray = VertsArray[index].Reinterpret<float4>();

            for (int j = 0; j < count; j++)
            {
                int mod = j * count + j % count;
                PositionArray[mod] = new Position {Value = new float3(vertArray[index].x, vertArray[index].y, 0) };
            }
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
            VertsArray = _vertData.VerticesArray,
            PositionArray = _data.Positions
            
        }.Schedule(_vertData.Length, 64, inputDeps);
        setupVertsJob.Complete();

        var updateVertsJob = new UpdateVerticesJob
        {
            EntityArray = _vertData.Entities,
            EntityCommandBuffer = _shapeBarrier.CreateCommandBuffer().ToConcurrent(),
            VertsArray = _vertData.VerticesArray,
            PositionArray = _data.Positions

        }.Schedule(_vertData.Length, 64, inputDeps);

        updateVertsJob.Complete();

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
