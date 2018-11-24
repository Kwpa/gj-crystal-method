using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Physics_Engine.Physics;
using Physics_Engine.Physics.Properties;
using Physics_Engine.Helpers;

namespace Physics_Engine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 600;
        private const int NR_OF_PARTICLES = 0;
        private Texture2D m_Particle;
        public Texture2D m_Background;
        private ObjectManager m_ObjectManager;
        private PhysicsManager m_PhysicsManager;

        //Random m_RandomGenerator;

        private float m_DeltaTime = 0;

        //public GraphicsDeviceManager m_Graphics { get; private set; }

        private void CollisionListener(PhysicsManager.CollisionInfo cInfo, PhysicsBody body1, PhysicsBody body2){
            Debug.Log("Body " + body1.GetID() + " collided with Body " + body2.GetID());
            foreach (Vector2 cPoint in cInfo.CollisionPoints)
            {
                Debug.Log("Collisionpoint: " + cPoint);
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        void Awake()
        {
            // TODO: Add your initialization logic here
            m_ObjectManager = ObjectManager.Instance;
            m_PhysicsManager = PhysicsManager.Instance;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>

        void Start()
        {
            LoadContent();
        }

        public void LoadContent()
        {
            //PhysicsManager.Instance.RegisterListener(CollisionListener);
            // TODO: use this.Content to load your game content here
            
            /*
            for(int i=0; i<NR_OF_PARTICLES; i++){
                Vector2 randomPos = new Vector2(m_RandomGenerator.Next(0, m_Graphics.PreferredBackBufferWidth), m_RandomGenerator.Next(0, m_Graphics.PreferredBackBufferHeight));
                float randomSize = (float)m_RandomGenerator.Next(12, 32);
                m_ObjectManager.Add(new PhysicsGameObjectNormal(randomPos, m_Particle, new PhysicsCircleDef(randomSize)));
            }
            */          //??????

            
            
            
            
            Vector2 pos1 = new Vector2(0, 0);
            Vector2 pos2 = new Vector2(0, 3);
            Vector2 pos3 = new Vector2(0.5f, 6);
            m_ObjectManager.Add(new PhysicsGameObjectTraditional(pos1, m_Particle, new PhysicsPolygonDef(DefineRectangle(4, 1), true)));
            m_ObjectManager.Add(new PhysicsGameObjectTraditional(pos2, m_Particle, new PhysicsPolygonDef(DefineRectangle(1, 1), false)));
            m_ObjectManager.Add(new PhysicsGameObjectTraditional(pos3, m_Particle, new PhysicsPolygonDef(DefineRectangle(1, 1), false, true)));

            //m_ObjectManager.Add(new GameObject(new Vector2(SCREEN_WIDTH * 0.5f, SCREEN_HEIGHT * 0.8f), m_Particle, new PhysicsPolygonDef(vertices, true, Material.FLUID)));

            //THREE PHYSOBJECTS!

            //m_ObjectManager.Add(new PhysicsGameObjectTraditional(new Vector2(SCREEN_WIDTH * 0.5f, SCREEN_HEIGHT*0.3f), m_Particle, new PhysicsPolygonDef(vertices3, true)));

        }

        public Vector2[] DefineRectangle(float width, float height)
        {
            Vector2[] vertices = new Vector2[4];
            float x = 1; float y = 1; float xm = -1; float ym = -1;
            x *= width * 0.5f;
            y *= height * 0.5f;
            xm *= width * 0.5f;
            ym *= height * 0.5f;
            vertices[0] = new Vector2(x, y);
            vertices[1] = new Vector2(xm, y);
            vertices[2] = new Vector2(xm, ym);
            vertices[3] = new Vector2(x, ym);
            return vertices;
        }

        private void OnDrawGizmos()
        {
            int j = -1;
            foreach (PhysicsGameObjectTraditional g in ObjectManager.m_Objects)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(g.Position, 0.05f);
                j++;
                switch(j)
                {
                    case 0: Gizmos.color = Color.white; break;
                    case 1: Gizmos.color = Color.blue; break;
                    case 2: Gizmos.color = Color.green; break;
                    case 3: Gizmos.color = Color.red; break; 
                }

                for (int i = 0; i < g.Body.PolygonDef.VertexCount; i++)
                {
                    Gizmos.DrawSphere(g.Body.PolygonDef.Vertices[i]+g.Body.Position, 0.05f);
                    int iModulus = (i+1) % g.Body.PolygonDef.VertexCount;
                    Gizmos.DrawLine(g.Body.PolygonDef.Vertices[i] + g.Body.Position, g.Body.PolygonDef.Vertices[iModulus] + g.Body.Position);
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>

        //protected override void UnloadContent()
        //{
        //    Content.Unload();
        //    // TODO: Unload any non ContentManager content here
        //}

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
        public float force = 2;
        //temp


        void FixedUpdate()
        {
            m_DeltaTime = Time.deltaTime;

            /*
            //temp FPS counter
            double endtime = gameTime.TotalGameTime.Milliseconds - lastUpdateUpdate;
            RenderManager.Instance.DrawString("FPS: " + ((int)(1000 / endtime)).ToString());
            //RenderManager.Instance.DrawString("Delta: " + gameTime.ElapsedGameTime.TotalSeconds.ToString());
            lastUpdateUpdate = gameTime.TotalGameTime.Milliseconds;
            */

            if (Input.GetKey(KeyCode.UpArrow))
            {
                ObjectManager.m_Objects[1].Body.AddForce(new Vector2(0, force), Vector2.zero, FORCE_TYPE.FORCE, false);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                ObjectManager.m_Objects[1].Body.AddForce(new Vector2(0, -force), Vector2.zero, FORCE_TYPE.FORCE, false);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                ObjectManager.m_Objects[1].Body.AddForce(new Vector2(-force, 0), Vector2.zero, FORCE_TYPE.FORCE, false);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                ObjectManager.m_Objects[1].Body.AddForce(new Vector2(force, 0), Vector2.zero, FORCE_TYPE.FORCE, false);
            }

            if (Input.GetKey(KeyCode.R))
            {

                ObjectManager.m_Objects[1].Body.AddTorque(100000f);
            }

            #region MakeShapes

            //if (Input.GetKeyDown(KeyCode.C))
            //    m_ObjectManager.Clear();
            //if (Input.GetKeyDown(KeyCode.Z))
            //    m_ObjectManager.RemoveLast();

            //if (Input.GetKeyDown(KeyCode.Q))
            //    isStatic = !isStatic;
            //if (Input.GetKeyDown(KeyCode.W))
            //    isWater = !isWater;
            //if (isDrawingPoly == true || isDrawingCircle == true)
            //{
            //    Debug.Log("Static: " + isStatic.ToString());
            //    Debug.Log("Material: " + (isWater ? Physics.Properties.Material.FLUID : Physics.Properties.Material.SOLID).ToString());
            //}
            //if (Input.GetKeyDown(KeyCode.A)){
            //    if (isDrawingPoly == false)
            //        isDrawingPoly = true;
            //    else if (polygonVertices.Count > 2)
            //    {
            //        Vector2 startPos = new Vector2();
            //        foreach (Vector2 vertex in polygonVertices)
            //            startPos += vertex;
            //        startPos.x /= polygonVertices.Count;
            //        startPos.y /= polygonVertices.Count;
            //        m_ObjectManager.Add(new PhysicsGameObjectTraditional(startPos, m_Particle, new PhysicsPolygonDef(polygonVertices.ToArray(), 
            //            isStatic, false, isWater ? Physics.Properties.Material.FLUID : Physics.Properties.Material.SOLID)));
            //        polygonVertices.Clear();
            //        isDrawingPoly = false;
            //    }
            //    else
            //    {
            //        isDrawingPoly = false;
            //        polygonVertices.Clear();
            //    }
            //}
            //else if (Input.GetKeyDown(KeyCode.S))
            //{
            //    if (isDrawingCircle == false)
            //        isDrawingCircle = true;
            //    else if (circleStartPos != Vector2.zero)
            //    {
            //        m_ObjectManager.Add(new PhysicsGameObjectTraditional(circleStartPos, m_Particle, new PhysicsCircleDef(circleRadius, isStatic, Physics.Properties.Material.SOLID)));
            //        circleStartPos = Vector2.zero;
            //        circleRadius = 0;
            //        isDrawingCircle = false;
            //    }
            //    else
            //        isDrawingCircle = false;
            //}
            //if (isDrawingPoly)
            //{
            //    Debug.Log("Drawing polygon");
            //    foreach (Vector2 vertex in polygonVertices)
            //    {
            //        Debug.DrawLine(vertex, vertex+new Vector2(-6,-6));
            //    }
            //    if (polygonVertices.Count > 2)
            //    {
            //        Vector2 edgeNormal1 = peMath.LeftPerp(polygonVertices[polygonVertices.Count - 1] - polygonVertices[polygonVertices.Count - 2]);
            //        Vector2 edgeNormal2 = peMath.LeftPerp(polygonVertices[1] - polygonVertices[0]);
            //        Vector2 edgeNormal3 = peMath.LeftPerp(polygonVertices[0] - polygonVertices[polygonVertices.Count - 1]);
            //        edgeNormal1.Normalize();
            //        edgeNormal2.Normalize();
            //        Vector2 newEdge1 = polygonVertices[polygonVertices.Count - 1] - (Vector2)Input.mousePosition;
            //        Vector2 newEdge2 = polygonVertices[0] - (Vector2)Input.mousePosition;
            //        if (Vector2.Dot(newEdge1, edgeNormal1) < 0 && Vector2.Dot(newEdge2, edgeNormal2) < 0 && Vector2.Dot(newEdge2, edgeNormal3) > 0)
            //        {
            //            Debug.DrawLine(Input.mousePosition, Input.mousePosition + new Vector3(7,7), Color.green);
            //            if (Input.GetMouseButtonDown(0))
            //                polygonVertices.Add(Input.mousePosition);
            //        }
            //        else
            //            Debug.DrawLine(Input.mousePosition, Input.mousePosition + new Vector3(7, 7), Color.red);
            //    }
            //    else
            //    {
            //        if (polygonVertices.Count == 2)
            //        {
            //            Vector2 edge = (Vector2)Input.mousePosition - polygonVertices[1];
            //            Vector2 edgeNormal = peMath.LeftPerp(polygonVertices[1] - polygonVertices[0]);
            //            edgeNormal.Normalize();
            //            if (Vector2.Dot(edge, edgeNormal) > 0)
            //            {
            //                Debug.DrawLine(Input.mousePosition, Input.mousePosition + new Vector3(7, 7), Color.green);
            //                if (Input.GetMouseButtonDown(0))
            //                    polygonVertices.Add(Input.mousePosition);
            //            }
            //            else
            //                Debug.DrawLine(Input.mousePosition, Input.mousePosition + new Vector3(7, 7), Color.red);
            //        }
            //        else
            //        {
            //            Debug.DrawLine(Input.mousePosition, Input.mousePosition + new Vector3(7, 7), Color.green);
            //            if (Input.GetMouseButtonDown(0))
            //                polygonVertices.Add(Input.mousePosition);
            //        }
            //    }
            //}
            //else if (isDrawingCircle)
            //{
            //    Debug.Log("Drawing circle");
            //    if (Input.GetMouseButtonUp(0))
            //        circleStartPos = (Vector2)Input.mousePosition;
            //    if (Input.GetMouseButtonDown(0))
            //        circleRadius = (circleStartPos - (Vector2)Input.mousePosition).magnitude;
            //    Debug.DrawLine(Input.mousePosition, Input.mousePosition + new Vector3(circleRadius, circleRadius), Color.green);
            //}

            #endregion

            // TODO: Add your update logic here
            m_PhysicsManager.Update();
            m_ObjectManager.Update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        //protected override void Draw(GameTime gameTime)
        //{
        //    //Update renderer, make it draw!
        //    Physics_Engine.Graphics.RenderManager.Instance.Update();
        //    //temp FPS counter
        //    double endtime = gameTime.TotalGameTime.Milliseconds - lastUpdateDraw;
        //    //RenderManager.Instance.DrawString("Draw: "+((int)(1000 / endtime)).ToString());
        //    lastUpdateDraw = gameTime.TotalGameTime.Milliseconds;

        //    base.Draw(gameTime);
        //}

        public float    DeltaTime       { get { return m_DeltaTime; } }
        public int      ScreenHeight    { get { return SCREEN_HEIGHT; } }
        public int      ScreenWidth     { get { return SCREEN_WIDTH; } }

        //Singleton
        private static volatile GameManager m_Instance;
        //private static object m_SyncRoot = new Object();
        private GameManager()
        {
        }
        public static GameManager Instance
        {
            get
            {
                if (m_Instance == null)//Make sure not null == not instantiated
                    //lock (m_SyncRoot)//For thread-safety
                        if (m_Instance == null){//Check again to avoid earlier instantiation by other thread
                            m_Instance = new GameManager();
                        }
                return m_Instance;
            }
        }
    }
}
