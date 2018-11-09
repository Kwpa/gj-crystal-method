using System;
using System.Collections.Generic;

using Physics_Engine.Physics;

namespace Physics_Engine
{
    class ObjectManager
    {
        private static List<PhysicsGameObjectTraditional> m_Objects;
        public void Add(PhysicsGameObjectTraditional p_GameObject) { m_Objects.Add(p_GameObject); }
        public void Remove(PhysicsGameObjectTraditional p_GameObject) {
            PhysicsManager.Instance.Remove(p_GameObject.Body);
            m_Objects.Remove(p_GameObject); 
        }
        public void RemoveLast() {
            if (m_Objects.Count > 0)
            {
                PhysicsManager.Instance.RemoveLast();
                m_Objects.RemoveAt(m_Objects.Count - 1);
            }
        }
        public void Clear() { 
            m_Objects.Clear(); PhysicsManager.Instance.Clear(); 
        }
        public void Update()
        {
            foreach (PhysicsGameObjectTraditional gameObject in m_Objects)
                gameObject.Update();
        }

        //Singleton
        private static volatile ObjectManager m_Instance;
        //private static object m_SyncRoot = new Object();
        private ObjectManager()
        {
            m_Objects = new List<PhysicsGameObjectTraditional>();
        }
        public static ObjectManager Instance
        {
            get
            {
                if (m_Instance == null)//Make sure not null == not instantiated
                    //lock (m_SyncRoot)//For thread-safety
                        if (m_Instance == null)
                        {//Check again to avoid earlier instantiation by other thread
                            m_Instance = new ObjectManager();
                        }
                return m_Instance;
            }
        }
    }
}
