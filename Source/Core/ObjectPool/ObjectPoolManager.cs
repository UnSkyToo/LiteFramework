﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LiteFramework.Core.ObjectPool
{
    public static class ObjectPoolManager
    {
        private static readonly Dictionary<string, BaseObjectPool> PoolList_ = new Dictionary<string, BaseObjectPool>();
        private static Transform Root_ = null;

        public static bool Startup()
        {
            if (Root_ == null)
            {
                Root_ = new GameObject("ObjectPool").transform;
                Root_.localPosition = Vector3.zero;
                Root_.localRotation = Quaternion.identity;
                Root_.localScale = Vector3.one;
            }

            PoolList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            foreach (var Pool in PoolList_)
            {
                Pool.Value.Dispose();
            }
            PoolList_.Clear();

            if (Root_ != null)
            {
                Object.DestroyImmediate(Root_.gameObject);
                Root_ = null;
            }
        }

        public static void Tick(float DeltaTime)
        {
        }

        public static BaseObjectPool FindPool(string PoolName)
        {
            if (!PoolList_.ContainsKey(PoolName))
            {
                return null;
            }

            return PoolList_[PoolName];
        }

        public static T FindPool<T>(string PoolName) where T : BaseObjectPool
        {
            return FindPool(PoolName) as T;
        }

        public static GameObjectPool CreateGameObjectPool(string PoolName, UnityEngine.GameObject Prefab)
        {
            if (FindPool(PoolName) != null)
            {
                return FindPool(PoolName) as GameObjectPool;
            }

            Prefab.transform.SetParent(Root_, false);
            var Pool = new GameObjectPool(PoolName, Prefab);
            AddPool(Pool);
            return Pool;
        }

        public static BaseObjectPool AddPool(BaseObjectPool Pool)
        {
            if (!PoolList_.ContainsKey(Pool.PoolName))
            {
                PoolList_.Add(Pool.PoolName, Pool);
                return Pool;
            }

            return PoolList_[Pool.PoolName];
        }

        public static void DeletePool(string PoolName)
        {
            if (PoolList_.ContainsKey(PoolName))
            {
                PoolList_[PoolName].Dispose();
                PoolList_.Remove(PoolName);
            }
        }

        public static void DeletePool(BaseObjectPool Pool)
        {
            if (Pool != null)
            {
                DeletePool(Pool.PoolName);
            }
        }

        public static BaseObjectPool[] GetObjectPoolList()
        {
            return PoolList_.Values.ToArray();
        }
    }
}