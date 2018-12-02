using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.SceneManagement;

public sealed class Bootstrap
{
    public static EntityArchetype PlayerArchetype;
    public static EntityArchetype RootArchetype;
    public static EntityArchetype AIPlayerArchetype;
    public static EntityArchetype BackgroundArchetype;
    public static MeshInstanceRenderer PlayerLook;
    public static MeshInstanceRenderer AIPlayerLook;
    public static MeshInstanceRenderer BackgroundLook;

    public static Settings Settings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        // This method creates archetypes for entities we will spawn frequently in this game.
        // Archetypes are optional but can speed up entity spawning substantially.

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        //create root
        RootArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Rotation), typeof(Scale));

        //create root
        BackgroundArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Rotation), typeof(Scale)); 

        // Create player archetype
        PlayerArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Rotation), typeof(PlayerInput),
            typeof(Health), typeof(Scale));

        //// Create aiplayer archetype
        //AIPlayerArchetype = entityManager.CreateArchetype(
        //    typeof(Position), typeof(Rotation), typeof(AIControlInput),
        //    typeof(Health), typeof(Scale));

        // Create aiplayer archetype with gravity
        AIPlayerArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Rotation), typeof(AIControlInput),
            typeof(Health), typeof(Scale), typeof(Physics2DEntity), typeof(Vertices), typeof(VehicleType), typeof(ShapeSetupIncomplete));
    }

    // Begin a new game.
    public static void NewGame()
    {
        // Access the ECS entity manager
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        World.Active.GetOrCreateManager<CameraMovementSystem>().FindCameraTransform();
        Entity background = SpawnBackground(entityManager); 
        Entity root = SpawnRoot(entityManager);
        Entity player = SpawnRoot(entityManager);
        for (int i = 0; i < Settings.numberOfPlayers; i++)
        {
            player = SpawnPlayer(entityManager);
            Parent(entityManager, root, player);
        }
        root = player;
        for (int i = 0; i < Settings.numberOfAIPlayers; i++)
        {
            player = SpawnAIPlayer(entityManager);
            Parent(entityManager, root, player);
        }
    }

    public static Entity SpawnRoot(EntityManager em)
    {
        Entity root = em.CreateEntity(RootArchetype);
        em.SetComponentData(root, new Position { Value = new float3(0.0f, 0.0f, 0.0f) });
        em.SetComponentData(root, new Rotation { Value = quaternion.identity });
        em.SetComponentData(root, new Scale { Value = new float3(1.0f, 1.0f, 1.0f) });
        return root;
    }

    public static Entity SpawnBackground(EntityManager em)
    {
        Entity background = em.CreateEntity(BackgroundArchetype);
        em.SetComponentData(background, new Position { Value = new float3(0.0f, 0.0f, 0.0f) });
        em.SetComponentData(background, new Rotation { Value = quaternion.identity });
        em.SetComponentData(background, new Scale { Value = new float3(1000.0f, 1000.0f, 1000.0f) });
        em.AddSharedComponentData(background, BackgroundLook);
        return background;
    }

    public static Entity SpawnPlayer (EntityManager em)
    {
        float x = UnityEngine.Random.Range(-1.0f, 1.0f) * Settings.xSpread;
        float y = UnityEngine.Random.Range(-1.0f, 1.0f) * Settings.ySpread;
        Entity player = em.CreateEntity(PlayerArchetype);
        em.SetComponentData(player, new Position { Value = new float3(x, y, 0.0f) });
        em.SetComponentData(player, new Rotation { Value = quaternion.identity });
        em.SetComponentData(player, new Health { Value = Settings.playerHealth });
        em.SetComponentData(player, new Scale { Value = new float3(1.0f, 1.0f, 1.0f) });
        em.AddSharedComponentData(player, PlayerLook);
        return player;
    }

    public static Entity SpawnAIPlayer(EntityManager em)
    {
        float x = UnityEngine.Random.Range(-1.0f, 1.0f) * Settings.xSpread;
        float y = UnityEngine.Random.Range(-1.0f, 1.0f) * Settings.ySpread;
        Entity player = em.CreateEntity(AIPlayerArchetype);
        em.SetComponentData(player, new Position { Value = new float3(x, y, 0.0f) });
        em.SetComponentData(player, new Rotation { Value = quaternion.identity });
        em.SetComponentData(player, new Health { Value = Settings.playerHealth });
        em.SetComponentData(player, new Scale { Value = new float3(1.0f, 1.0f, 1.0f) });
        em.AddSharedComponentData(player, AIPlayerLook);
        em.AddBuffer<VerticesBuffer>(player);
        return player;
    }

    public static void Parent(EntityManager em, Entity parent, Entity child)
    {
        var attach = em.CreateEntity(typeof(Attach));
        em.SetComponentData(attach, new Attach { Parent = parent, Child = child });
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
        BackgroundLook = GetLookFromPrototype("BackgroundRenderPrototype"); 

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