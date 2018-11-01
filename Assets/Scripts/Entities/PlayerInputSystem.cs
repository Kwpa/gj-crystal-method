using Unity.Entities;
using UnityEngine;
using System;


public class PlayerInputSystem : ComponentSystem
{
    struct PlayerData
    {
#pragma warning disable 649
        public readonly int Length;
#pragma warning restore 649

        public ComponentDataArray<PlayerInput> Input;
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
        PlayerInput pi;

        pi.Move.x = 0.0f;
        pi.Move.y = Input.GetAxis("Vertical");
        pi.Move.z = 0.0f;
        pi.Look = Input.GetAxis("Horizontal");

        m_Players.Input[i] = pi;
    }
}

