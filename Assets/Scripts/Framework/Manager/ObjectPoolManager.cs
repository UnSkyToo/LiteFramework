using System.Collections.Generic;
using UnityEngine;

namespace Lite.Framework.Manager
{
    public class ObjectPoolManager
    {
        public class ObjectPoolCache
        {
            public GameObject Obj { get; set; }
            public bool Used { get; set; }
            public uint Count { get; set; }

            public ObjectPoolCache(GameObject Obj)
            {
                this.Obj = Obj;
                this.Used = false;
                this.Count = 1;
            }
        }

        public class ObjectPool
        {
            public delegate void PoolEventHandler(GameObject Obj);
            public event PoolEventHandler OnSpawn;
            public event PoolEventHandler OnRecycle;

            public int InfrequentCount { get; set; } = 2;
            public string PoolName { get; }
            public GameObject Prefab { get; }

            private readonly Dictionary<int, ObjectPoolCache> ObjectCache_ = null;

            public ObjectPool(string PoolName, GameObject Prefab)
            {
                this.PoolName = PoolName;
                this.Prefab = Prefab;
                this.ObjectCache_ = new Dictionary<int, ObjectPoolCache>();
            }

            public GameObject Spawn()
            {
                foreach (var Cache in ObjectCache_)
                {
                    if (!Cache.Value.Used)
                    {
                        Cache.Value.Used = true;
                        Cache.Value.Count++;
                        Cache.Value.Obj.SetActive(true);
                        TriggerSpawn(Cache.Value.Obj);
                        return Cache.Value.Obj;
                    }
                }

                GameObject Obj = null;
                if (Prefab != null)
                {
                    Obj = Object.Instantiate(Prefab);
                }
                else
                {
                    Obj = new GameObject();
                }

                var NewCache = new ObjectPoolCache(Obj);
                ObjectCache_.Add(Obj.GetInstanceID(), NewCache);
                NewCache.Used = true;
                Obj.SetActive(true);
                Obj.transform.localPosition = Vector3.zero;
                Obj.transform.localRotation = Quaternion.identity;
                Obj.transform.localScale = Vector3.one;
                TriggerSpawn(Obj);

                return Obj;
            }

            public void Recycle(GameObject Obj)
            {
                TriggerRecycle(Obj);

                if (Obj != null && ObjectCache_.ContainsKey(Obj.GetInstanceID()))
                {
                    ObjectCache_[Obj.GetInstanceID()].Obj.SetActive(false);
                    ObjectCache_[Obj.GetInstanceID()].Used = false;
                }
                else
                {
                    Object.Destroy(Obj);
                }
            }

            public void DestroyObjects()
            {
                foreach (var Cache in ObjectCache_)
                {
                    Object.Destroy(Cache.Value.Obj);
                }

                ObjectCache_.Clear();
            }

            public void DestroyUnuseObjects()
            {
                var DestroyKeys = new List<int>();

                foreach (var Cache in ObjectCache_)
                {
                    if (!Cache.Value.Used)
                    {
                        DestroyKeys.Add(Cache.Key);
                    }
                }

                DestroyList(DestroyKeys);
            }

            public void DestroyInfrequentObjects()
            {
                var DestroyKeys = new List<int>();

                foreach (var Cache in ObjectCache_)
                {
                    if (!Cache.Value.Used && Cache.Value.Count <= InfrequentCount)
                    {
                        DestroyKeys.Add(Cache.Key);
                    }
                }

                DestroyList(DestroyKeys);
            }

            private void DestroyList(List<int> Keys)
            {
                foreach (var Key in Keys)
                {
                    if (ObjectCache_.ContainsKey(Key))
                    {
                        Object.Destroy(ObjectCache_[Key].Obj);
                        ObjectCache_.Remove(Key);
                    }
                }
            }

            private void TriggerSpawn(GameObject Obj)
            {
                OnSpawn?.Invoke(Obj);
            }

            private void TriggerRecycle(GameObject Obj)
            {
                OnRecycle?.Invoke(Obj);
            }
        }

        private static readonly Dictionary<string, ObjectPool> Pools_ = new Dictionary<string, ObjectPool>();

        public static bool Startup()
        {
            Pools_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            foreach (var Pool in Pools_)
            {
                Pool.Value.DestroyObjects();
            }
            Pools_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
        }

        public static ObjectPool AddPool(string PoolName, GameObject Prefab)
        {
            if (!Pools_.ContainsKey(PoolName))
            {
                var Pool = new ObjectPool(PoolName, Prefab);
                Pools_.Add(PoolName, Pool);
                return Pool;
            }

            return Pools_[PoolName];
        }

        public static void DeletePool(string PoolName)
        {
            if (Pools_.ContainsKey(PoolName))
            {
                Pools_[PoolName].DestroyObjects();
                Pools_.Remove(PoolName);
            }
        }

        public static void DeletePool(ObjectPool Pool)
        {
            if (Pool != null)
            {
                DeletePool(Pool.PoolName);
            }
        }
    }
}