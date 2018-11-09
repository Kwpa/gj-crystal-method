using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Physics_Engine.Physics;
using Physics_Engine.Physics.Properties;
using Physics_Engine.Graphics;
using Physics_Engine.Input;
using Physics_Engine.Helpers;

namespace Physics_Engine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameManager : Microsoft.Xna.Framework.Game
    {
        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 600;
        private const int NR_OF_PARTICLES = 0;
        private Texture2D m_Particle;
        public Texture2D m_Background;
        private ObjectManager m_ObjectManager;
        private PhysicsManager m_PhysicsManager;
        private InputManager m_InputManager;
        Random m_RandomGenerator;
        private float m_DeltaTime = 0;

        public GraphicsDeviceManager m_Graphics { get; private set; }

        private void CollisionListener(PhysicsManager.CollisionInfo cInfo, PhysicsBody body1, PhysicsBody body2){
            RenderManager.Instance.DrawString("Body " + body1.GetID() + " collided with Body " + body2.GetID());
            foreach (Vector2 cPoint in cInfo.CollisionPoints)
            {
                RenderManager.Instance.DrawString("Collisionpoint: " + cPoint);
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            m_ObjectManager = ObjectManager.Instance;
            m_PhysicsManager = PhysicsManager.Instance;
            m_InputManager = InputManager.Instance;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //PhysicsManager.Instance.RegisterListener(CollisionListener);
            // TODO: use this.Content to load your game content here
            m_Background = Content.Load<Texture2D>("background");
            for(int i=0; i<NR_OF_PARTICLES; i++){
                Vector2 randomPos = new Vector2(m_RandomGenerator.Next(0, m_Graphics.PreferredBackBufferWidth), m_RandomGenerator.Next(0, m_Graphics.PreferredBackBufferHeight));
                float randomSize = (float)m_RandomGenerator.Next(12, 32);
                m_ObjectManager.Add(new PhysicsGameObjectNormal(randomPos, m_Particle, new PhysicsCircleDef(randomSize)));
            }
            Vector2[] vertices = new Vector2[4];
            vertices[3] = new Vector2(-SCREEN_WIDTH * 0.5f, 50);
            vertices[2] = new Vector2(SCREEN_WIDTH*0.5f, 50);
            vertices[1] = new Vector2(SCREEN_WIDTH*0.5f, -200);
            vertices[0] = new Vector2(-SCREEN_WIDTH*0.5f, -200);
            Vector2[] vertices2 = new Vector2[4];
            vertices2[3] = new Vector2(0, SCREEN_HEIGHT);
            vertices2[2] = new Vector2(50, SCREEN_HEIGHT);
            vertices2[1] = new Vector2(50, 0);
            vertices2[0] = new Vector2(0, 0);
            Vector2[] vertices3 = new Vector2[4];
            vertices3[3] = new Vector2(0, 50);
            vertices3[2] = new Vector2(SCREEN_WIDTH, 50);
            vertices3[1] = new Vector2(SCREEN_WIDTH, 0);
            vertices3[0] = new Vector2(0, 0);

            //m_ObjectManager.Add(new GameObject(new Vector2(SCREEN_WIDTH * 0.5f, SCREEN_HEIGHT * 0.8f), m_Particle, new PhysicsPolygonDef(vertices, true, Material.FLUID)));
            m_ObjectManager.Add(new PhysicsGameObjectNormal(new Vector2(0, SCREEN_HEIGHT * 0.5f), m_Particle, new PhysicsPolygonDef(vertices2, true)));
            m_ObjectManager.Add(new PhysicsGameObjectNormal(new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT * 0.5f), m_Particle, new PhysicsPolygonDef(vertices2, true)));
            m_ObjectManager.Add(new PhysicsGameObjectNormal(new Vector2(SCREEN_WIDTH * 0.5f, SCREEN_HEIGHT), m_Particle, new PhysicsPolygonDef(vertices3, true)));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        //temp
        private double lastUpdateUpdate = 0;
        private double lastUpdateDraw = 0;
        bool isDrawingPoly = false;
        bool isDrawingCircle = false;
        bool isStatic = false;
        bool isWater = false;
        float circleRadius;
        Vector2 circleStartPos;
        List<Vector2> polygonVertices = new List<Vector2>();
        //temp
        protected override void Update(GameTime gameTime)
        {
            m_DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            //temp FPS counter
            double endtime = gameTime.TotalGameTime.Milliseconds - lastUpdateUpdate;
            RenderManager.Instance.DrawString("FPS: " + ((int)(1000 / endtime)).ToString());
            //RenderManager.Instance.DrawString("Delta: " + gameTime.ElapsedGameTime.TotalSeconds.ToString());
            lastUpdateUpdate = gameTime.TotalGameTime.Milliseconds;

            if (m_InputManager.KeyWasPressed(Keys.C))
                m_ObjectManager.Clear();
            if (m_InputManager.KeyWasPressed(Keys.Z))
                m_ObjectManager.RemoveLast();
            
            if (m_InputManager.KeyWasPressed(Keys.Q))
                isStatic = !isStatic;
            if (m_InputManager.KeyWasPressed(Keys.W))
                isWater = !isWater;
            if (isDrawingPoly == true || isDrawingCircle == true)
            {
                RenderManager.Instance.DrawString("Static: " + isStatic.ToString());
                RenderManager.Instance.DrawString("Material: " + (isWater ? Material.FLUID : Material.SOLID).ToString());
            }
            if (m_InputManager.KeyWasPressed(Keys.A)){
                if (isDrawingPoly == false)
                    isDrawingPoly = true;
                else if (polygonVertices.Count > 2)
                {
                    Vector2 startPos = new Vector2();
                    foreach (Vector2 vertex in polygonVertices)
                        startPos += vertex;
                    startPos.X /= polygonVertices.Count;
                    startPos.Y /= polygonVertices.Count;
                    m_ObjectManager.Add(new PhysicsGameObjectNormal(startPos, m_Particle, new PhysicsPolygonDef(polygonVertices.ToArray(), isStatic, isWater ? Material.FLUID : Material.SOLID)));
                    polygonVertices.Clear();
                    isDrawingPoly = false;
                }
                else
                {
                    isDrawingPoly = false;
                    polygonVertices.Clear();
                }
            }
            else if (m_InputManager.KeyWasPressed(Keys.S))
            {
                if (isDrawingCircle == false)
                    isDrawingCircle = true;
                else if (circleStartPos != Vector2.Zero)
                {
                    m_ObjectManager.Add(new PhysicsGameObjectNormal(circleStartPos, m_Particle, new PhysicsCircleDef(circleRadius, isStatic, Material.SOLID)));
                    circleStartPos = Vector2.Zero;
                    circleRadius = 0;
                    isDrawingCircle = false;
                }
                else
                    isDrawingCircle = false;
            }
            if (isDrawingPoly)
            {
                RenderManager.Instance.DrawString("Drawing polygon");
                foreach (Vector2 vertex in polygonVertices)
                {
                    RenderManager.Instance.DrawCircle(vertex, 6);
                }
                if (polygonVertices.Count > 2)
                {
                    Vector2 edgeNormal1 = peMath.LeftPerp(polygonVertices[polygonVertices.Count - 1] - polygonVertices[polygonVertices.Count - 2]);
                    Vector2 edgeNormal2 = peMath.LeftPerp(polygonVertices[1] - polygonVertices[0]);
                    Vector2 edgeNormal3 = peMath.LeftPerp(polygonVertices[0] - polygonVertices[polygonVertices.Count - 1]);
                    edgeNormal1.Normalize();
                    edgeNormal2.Normalize();
                    Vector2 newEdge1 = polygonVertices[polygonVertices.Count - 1] - m_InputManager.GetMousePos();
                    Vector2 newEdge2 = polygonVertices[0] - m_InputManager.GetMousePos();
                    if (Vector2.Dot(newEdge1, edgeNormal1) < 0 && Vector2.Dot(newEdge2, edgeNormal2) < 0 && Vector2.Dot(newEdge2, edgeNormal3) > 0)
                    {
                        RenderManager.Instance.DrawCircle(m_InputManager.GetMousePos(), 6, Color.Green);
                        if (m_InputManager.MouseLeftWasPressed())
                            polygonVertices.Add(m_InputManager.GetMousePos());
                    }
                    else
                        RenderManager.Instance.DrawCircle(m_InputManager.GetMousePos(), 6, Color.Red);
                }
                else
                {
                    if (polygonVertices.Count == 2)
                    {
                        Vector2 edge = m_InputManager.GetMousePos() - polygonVertices[1];
                        Vector2 edgeNormal = peMath.LeftPerp(polygonVertices[1] - polygonVertices[0]);
                        edgeNormal.Normalize();
                        if (Vector2.Dot(edge, edgeNormal) > 0)
                        {
                            RenderManager.Instance.DrawCircle(m_InputManager.GetMousePos(), 6, Color.Green);
                            if (m_InputManager.MouseLeftWasPressed())
                                polygonVertices.Add(m_InputManager.GetMousePos());
                        }
                        else
                            RenderManager.Instance.DrawCircle(m_InputManager.GetMousePos(), 6, Color.Red);
                    }
                    else
                    {
                        RenderManager.Instance.DrawCircle(m_InputManager.GetMousePos(), 6, Color.Green);
                        if (m_InputManager.MouseLeftWasPressed())
                            polygonVertices.Add(m_InputManager.GetMousePos());
                    }
                }
            }
            else if (isDrawingCircle)
            {
                RenderManager.Instance.DrawString("Drawing circle");
                if (m_InputManager.MouseLeftWasPressed())
                    circleStartPos = m_InputManager.GetMousePos();
                if (m_InputManager.MouseLeftIsPressed())
                    circleRadius = (circleStartPos - m_InputManager.GetMousePos()).Length();
                RenderManager.Instance.DrawCircle(circleStartPos, circleRadius, Color.Green);
            }

            // TODO: Add your update logic here
            m_PhysicsManager.Update();
            m_ObjectManager.Update();
            m_InputManager.Update();
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Update renderer, make it draw!
            Physics_Engine.Graphics.RenderManager.Instance.Update();
            //temp FPS counter
            double endtime = gameTime.TotalGameTime.Milliseconds - lastUpdateDraw;
            //RenderManager.Instance.DrawString("Draw: "+((int)(1000 / endtime)).ToString());
            lastUpdateDraw = gameTime.TotalGameTime.Milliseconds;

            base.Draw(gameTime);
        }

        public float    DeltaTime       { get { return m_DeltaTime; } }
        public int      ScreenHeight    { get { return SCREEN_HEIGHT; } }
        public int      ScreenWidth     { get { return SCREEN_WIDTH; } }

        //Singleton
        private static volatile GameManager m_Instance;
        private static object m_SyncRoot = new Object();
        private GameManager()
        {
            m_RandomGenerator = new Random();
            m_Graphics = new GraphicsDeviceManager(this);
            Window.Title = "Physics_Engine";
            m_Graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            m_Graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            m_Graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            m_Graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
        }
        public static GameManager Instance
        {
            get
            {
                if (m_Instance == null)//Make sure not null == not instantiated
                    lock (m_SyncRoot)//For thread-safety
                        if (m_Instance == null){//Check again to avoid earlier instantiation by other thread
                            m_Instance = new GameManager();
                        }
                return m_Instance;
            }
        }
    }
}
