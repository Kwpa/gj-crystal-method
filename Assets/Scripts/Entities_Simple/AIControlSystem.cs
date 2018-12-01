using Unity.Entities;
using UnityEngine;
using System;


public class AIControlSystem : ComponentSystem
{
    struct PlayerData
    {
#pragma warning disable 649
        public readonly int Length;
#pragma warning restore 649
        public ComponentDataArray<AIControlInput> Input;
        public SubtractiveComponent<PreventTransform> PreventTransforms;
    }

    [Inject] private PlayerData m_Players;

    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < m_Players.Length; ++i)
        {
            UpdatePlayerInput(i, dt);
        }
    }

    private void UpdatePlayerInput(int i, float dt)
    {
        AIControlInput ai;

        var settings = Bootstrap.Settings;

        ai.Move.x = 0.0f;
        ai.Move.y = 1;
        ai.Move.z = 0.0f;
        ai.Scale = new Unity.Mathematics.float3(1, 1, 1);
        ai.Look = 1;
        m_Players.Input[i] = ai;
    }
}

