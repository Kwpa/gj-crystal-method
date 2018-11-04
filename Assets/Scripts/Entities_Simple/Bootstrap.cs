using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.SceneManagement;

public sealed class Bootstrap
{
    public static EntityArchetype PlayerArchetype;
    public static EntityArchetype AIPlayerArchetype;
    public static MeshInstanceRenderer PlayerLook;
    public static MeshInstanceRenderer AIPlayerLook;

    public static Settings Settings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        // This method creates archetypes for entities we will spawn frequently in this game.
        // Archetypes are optional but can speed up entity spawning substantially.

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        // Create player archetype
        PlayerArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Rotation), typeof(PlayerInput),
            typeof(Health), typeof(Scale), typeof(TransformBasedMovement));
        AIPlayerArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Rotation), typeof(AIControlInput),
            typeof(Health), typeof(Scale), typeof(TransformBasedMovement));
    }

    // Begin a new game.
    public static void NewGame()
    {
        // Access the ECS entity manager
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        for(int i = 0; i < Settings.numberOfPlayers; i++)
        {
            SpawnPlayer(entityManager);
        }
        for (int i = 0; i < Settings.numberOfAIPlayers; i++)
        {
            SpawnAIPlayer(entityManager);
        }
    }

    
    public static void SpawnPlayer (EntityManager em)
    {
        float x = UnityEngine.Random.Range(-1.0f, 1.0f) * Settings.xSpread;
        float y = UnityEngine.Random.Range(-1.0f, 1.0f) * Settings.ySpread;
        Entity player = em.CreateEntity(PlayerArchetype);
        em.SetComponentData(player, new Position { Value = new float3(x, y, 0.0f) });
        em.SetComponentData(player, new Rotation { Value = quaternion.identity });
        em.SetComponentData(player, new Health { Value = Settings.playerHealth });
        em.SetComponentData(player, new Scale { Value = new float3(1.0f, 1.0f, 1.0f) });
        em.AddSharedComponentData(player, PlayerLook);
    }

    public static void SpawnAIPlayer(EntityManager em)
    {
        float x = UnityEngine.Random.Range(-1.0f, 1.0f) * Settings.xSpread;
        float y = UnityEngine.Random.Range(-1.0f, 1.0f) * Settings.ySpread;
        Entity player = em.CreateEntity(AIPlayerArchetype);
        em.SetComponentData(player, new Position { Value = new float3(x, y, 0.0f) });
        em.SetComponentData(player, new Rotation { Value = quaternion.identity });
        em.SetComponentData(player, new Health { Value = Settings.playerHealth });
        em.SetComponentData(player, new Scale { Value = new float3(1.0f, 1.0f, 1.0f) });
        em.AddSharedComponentData(player, AIPlayerLook);
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeAfterSceneLoad()
    {
        var settingsGO = GameObject.Find("Settings");
        if (settingsGO == null)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            return;
        }

        InitializeWithScene();
    }

    public static void InitializeWithScene()
    {
        var settingsGO = GameObject.Find("Settings");
        if (settingsGO == null)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            return;
        }
        Settings = settingsGO?.GetComponent<Settings>();
        if (!Settings)
            return;

        PlayerLook = GetLookFromPrototype("PlayerRenderPrototype");
        AIPlayerLook = GetLookFromPrototype("AIPlayerRenderPrototype");

        NewGame();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        InitializeWithScene();
    }

    private static MeshInstanceRenderer GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
        Object.Destroy(proto);
        return result;
    }
}