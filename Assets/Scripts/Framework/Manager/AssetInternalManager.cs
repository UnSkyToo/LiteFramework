#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using Lite.Framework.Helper;
using UnityEditor;
using UnityEngine;

namespace Lite.Framework.Manager
{
    public static class AssetInternalManager
    {
        public enum AssetBundleType : byte
        {
            Asset,
            Prefab,
            Data,
        }

        public enum AssetRealType : byte
        {
            Asset,
            Prefab,
            Data,
            Sprite,
            Texture,
        }

        private static readonly Dictionary<string, AssetBundleCacheBase> AssetBundleCacheList_ = new Dictionary<string, AssetBundleCacheBase>();
        private static readonly Dictionary<string, List<Action>> LoadAssetBundleCallbackList_ = new Dictionary<string, List<Action>>();
        private static readonly Dictionary<int, string> AssetBundlePathCacheList_ = new Dictionary<int, string>();

        public static bool Startup()
        {
            AssetBundleCacheList_.Clear();
            LoadAssetBundleCallbackList_.Clear();
            AssetBundlePathCacheList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            if (AssetBundleCacheList_.Count > 0)
            {
                foreach (var Cache in AssetBundleCacheList_)
                {
                    Cache.Value.Unload();
                }
                AssetBundleCacheList_.Clear();
            }

            LoadAssetBundleCallbackList_.Clear();
            AssetBundlePathCacheList_.Clear();

            Resources.UnloadUnusedAssets();
            AssetBundle.UnloadAllAssetBundles(true);
            GC.Collect();
        }

        public static void Tick(float DeltaTime)
        {
        }

        private static AssetBundleType GetAssetBundleTypeWithName(string BundlePath)
        {
            var Ext = PathHelper.GetFileExt(BundlePath);

            switch (Ext)
            {
                case ".prefab":
                    return AssetBundleType.Prefab;
                case ".bytes":
                    return AssetBundleType.Data;
                default:
                    return AssetBundleType.Asset;
            }
        }

        private static AssetRealType GetAssetRealTypeWithName(string BundlePath)
        {
            var Ext = PathHelper.GetFileExt(BundlePath);

            switch (Ext)
            {
                case ".bytes":
                    return AssetRealType.Data;
                case ".prefab":
                    return AssetRealType.Prefab;
                default:
                    break;
            }

            var Importer = AssetImporter.GetAtPath($"Assets/StandaloneAssets/{BundlePath}");
            if (Importer is TextureImporter TexImporter)
            {
                if (TexImporter.textureType == TextureImporterType.Sprite)
                {
                    return AssetRealType.Sprite;
                }

                return AssetRealType.Texture;
            }

            return AssetRealType.Asset;
        }

        private static string TryGetAssetRealPath(string BundlePath)
        {
            var Ext = PathHelper.GetFileExt(BundlePath);

            switch (Ext)
            {
                case ".sprite":
                case ".texture":
                    BundlePath = PathHelper.GetDirectoryAndFileName(BundlePath);
                    if (File.Exists(PathHelper.GetStandaloneAssetsPath($"{BundlePath}.png")))
                    {
                        BundlePath = $"{BundlePath}.png";
                    }

                    if (File.Exists(PathHelper.GetStandaloneAssetsPath($"{BundlePath}.jpg")))
                    {
                        BundlePath = $"{BundlePath}.jpg";
                    }
                    break;
                default:
                    break;
            }

            return BundlePath;
        }

        private static AssetBundleCacheBase CreateAssetBundleCache<T>(AssetBundleType BundleType, string BundlePath) where T : UnityEngine.Object
        {
            AssetBundleCacheBase Cache = null;

            switch (BundleType)
            {
                case AssetBundleType.Asset:
                    var RealType = GetAssetRealTypeWithName(BundlePath);

                    switch (RealType)
                    {
                        case AssetRealType.Asset:
                            Cache = new AssetBundleCache<T>(BundleType, BundlePath);
                            break;
                        case AssetRealType.Prefab:
                            Cache = new PrefabBundleCache(BundleType, BundlePath);
                            break;
                        case AssetRealType.Data:
                            Cache = new DataBundleCache(BundleType, BundlePath);
                            break;
                        case AssetRealType.Sprite:
                            Cache = new AssetBundleCache<Sprite>(BundleType, BundlePath);
                            break;
                        case AssetRealType.Texture:
                            Cache = new AssetBundleCache<Texture>(BundleType, BundlePath);
                            break;
                        default:
                            break;
                    }
                    break;
                case AssetBundleType.Prefab:
                    Cache = new PrefabBundleCache(BundleType, BundlePath);
                    break;
                case AssetBundleType.Data:
                    Cache = new DataBundleCache(BundleType, BundlePath);
                    break;
                default:
                    break;
            }

            return Cache;
        }

        public static bool AssetBundleCacheExisted(string BundlePath)
        {
            BundlePath = BundlePath.ToLower();

            if (AssetBundleCacheList_.ContainsKey(BundlePath) && AssetBundleCacheList_[BundlePath].IsLoad)
            {
                return true;
            }

            return false;
        }

        public static bool LoadAssetBundle<T>(AssetBundleType BundleType, string BundlePath, Action Callback = null) where T : UnityEngine.Object
        {
            BundlePath = BundlePath.ToLower();
            if (AssetBundleCacheExisted(BundlePath))
            {
                Callback?.Invoke();
                return true;
            }
            return LoadAssetBundleCompleted<T>(BundleType, BundlePath, Callback);
        }

        private static bool LoadAssetBundleCompleted<T>(AssetBundleType BundleType, string BundlePath, Action Callback = null) where T : UnityEngine.Object
        {
            var Cache = CreateAssetBundleCache<T>(BundleType, BundlePath);
            if (Cache == null)
            {
                return false;
            }

            AssetBundleCacheList_.Add(BundlePath, Cache);
            LoadAssetBundleDependencies<T>(Cache, () =>
            {
                Cache.Load();
                Callback?.Invoke();
            });

            return true;
        }

        private static void LoadAssetBundleDependencies<T>(AssetBundleCacheBase Cache, Action Callback = null) where T : UnityEngine.Object
        {
            var LoadCompletedCount = 0;
            var Dependencies = Cache.GetAllDependencies();

            if (Dependencies == null || Dependencies.Length == 0)
            {
                Callback?.Invoke();
                return;
            }

            foreach (var Dependency in Dependencies)
            {
                var BundlePath = Dependency;
                var BundleType = GetAssetBundleTypeWithName(BundlePath);
                LoadAssetBundle<T>(BundleType, BundlePath, () =>
                {
                    Cache.AddDependencyCache(AssetBundleCacheList_[BundlePath]);
                    LoadCompletedCount++;

                    if (LoadCompletedCount >= Dependencies.Length)
                    {
                        Callback?.Invoke();
                    }
                });
            }
        }

        public static void CreateAsset<T>(string BundlePath, string AssetName, Action<T> Callback = null) where T : UnityEngine.Object
        {
            var IsLoaded = LoadAssetBundle<T>(AssetBundleType.Asset, BundlePath, () =>
            {
                CreateAssetWithCache(BundlePath, AssetName, Callback);
            });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public static void CreateAsset<T>(string BundlePath, Action<T> Callback = null) where T : UnityEngine.Object
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            CreateAsset<T>(BundlePath, AssetName, Callback);
        }

        public static T CreateAssetWithCache<T>(string BundlePath, string AssetName, Action<T> Callback = null) where T : UnityEngine.Object
        {
            T Asset = null;

            BundlePath = TryGetAssetRealPath(BundlePath);

            BundlePath = BundlePath.ToLower();
            AssetName = AssetName.ToLower();

            if (AssetBundleCacheList_.ContainsKey(BundlePath))
            {
                if (AssetBundleCacheList_[BundlePath] is AssetBundleCache<T> Cache)
                {
                    Asset = Cache.CreateAsset(AssetName);

                    if (Asset != null)
                    {
                        if (!AssetBundlePathCacheList_.ContainsKey(Asset.GetInstanceID()))
                        {
                            AssetBundlePathCacheList_.Add(Asset.GetInstanceID(), BundlePath);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Can't Create Asset : {BundlePath} - {AssetName}");
                    }
                }
            }

            Callback?.Invoke(Asset);
            return Asset;
        }

        public static void CreatePrefab(string BundlePath, string AssetName, Action<GameObject> Callback = null)
        {
            var IsLoaded = LoadAssetBundle<UnityEngine.Object>(AssetBundleType.Prefab, BundlePath, () =>
            {
                CreatePrefabWithCache(BundlePath, AssetName, Callback);
            });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public static void CreatePrefab(string BundlePath, Action<GameObject> Callback = null)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            CreatePrefab(BundlePath, AssetName, Callback);
        }

        public static GameObject CreatePrefabWithCache(string BundlePath, string AssetName, Action<GameObject> Callback = null)
        {
            GameObject Asset = null;
            BundlePath = BundlePath.ToLower();
            AssetName = AssetName.ToLower();

            if (AssetBundleCacheList_.ContainsKey(BundlePath))
            {
                if (AssetBundleCacheList_[BundlePath] is PrefabBundleCache Cache)
                {
                    Asset = Cache.CreateAsset(AssetName);

                    if (Asset != null)
                    {
                        AssetBundlePathCacheList_.Add(Asset.GetInstanceID(), BundlePath);
                    }
                    else
                    {
                        Debug.LogError($"can't create asset : {BundlePath} - {AssetName}");
                    }
                }
            }

            Callback?.Invoke(Asset);
            return Asset;
        }

        public static void CreateData(string BundlePath, Action<byte[]> Callback = null)
        {
            var IsLoaded = LoadAssetBundle<TextAsset>(AssetBundleType.Data, BundlePath, () =>
            {
                CreateDataWithCache(BundlePath, Callback);
            });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public static byte[] CreateDataWithCache(string BundlePath, Action<byte[]> Callback = null)
        {
            byte[] Asset = null;
            BundlePath = BundlePath.ToLower();

            if (AssetBundleCacheList_.ContainsKey(BundlePath))
            {
                if (AssetBundleCacheList_[BundlePath] is DataBundleCache Cache)
                {
                    Asset = Cache.Buffer;
                }
            }

            Callback?.Invoke(Asset);
            return Asset;
        }

        public static void DeleteAsset<T>(T Asset) where T : UnityEngine.Object
        {
            if (Asset == null)
            {
                return;
            }

            if (AssetBundlePathCacheList_.ContainsKey(Asset.GetInstanceID()))
            {
                var BundlePath = AssetBundlePathCacheList_[Asset.GetInstanceID()];
                AssetBundlePathCacheList_.Remove(Asset.GetInstanceID());

                if (AssetBundleCacheExisted(BundlePath))
                {
                    if (typeof(T) != typeof(GameObject))
                    {
                        if (AssetBundleCacheList_[BundlePath] is AssetBundleCache<T> Cache)
                        {
                            Cache.DeleteAsset(Asset);
                        }
                    }
                }
            }
        }

        public static void DeleteAsset(GameObject Asset)
        {
            if (Asset == null)
            {
                return;
            }

            if (AssetBundlePathCacheList_.ContainsKey(Asset.GetInstanceID()))
            {
                var BundlePath = AssetBundlePathCacheList_[Asset.GetInstanceID()];
                AssetBundlePathCacheList_.Remove(Asset.GetInstanceID());

                if (AssetBundleCacheExisted(BundlePath))
                {
                    if (AssetBundleCacheList_[BundlePath] is PrefabBundleCache Cache)
                    {
                        Cache.DeleteAsset(Asset);
                    }
                }
            }
        }

        public static void DeleteUnusedAssetBundle()
        {
            var RemoveList = new List<string>();

            foreach (var Cache in AssetBundleCacheList_)
            {
                if (Cache.Value.Unused)
                {
                    RemoveList.Add(Cache.Key);
                }
            }

            if (RemoveList.Count > 0)
            {
                foreach (var Key in RemoveList)
                {
                    AssetBundleCacheList_[Key].Unload();
                    AssetBundleCacheList_.Remove(Key);
                }

                Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }

        private class AssetBundleCacheBase
        {
            public AssetBundleType BundleType { get; }
            public string BundlePath { get; }

            public bool IsLoad { get; private set; }
            public bool Unused => (RefCount_ <= 0 && IsLoad == true);

            private int RefCount_;
            protected readonly List<AssetBundleCacheBase> DependenciesCache_;

            protected AssetBundleCacheBase(AssetBundleType BundleType, string BundlePath)
            {
                this.BundleType = BundleType;
                this.BundlePath = BundlePath;

                this.IsLoad = false;
                this.RefCount_ = 0;
                this.DependenciesCache_ = new List<AssetBundleCacheBase>();
            }

            protected virtual string GetInternalAssetPath()
            {
                return $"Assets/StandaloneAssets/{BundlePath}";
            }

            public void Load()
            {
                IsLoad = false;

                var AssetList = AssetDatabase.LoadAllAssetsAtPath(GetInternalAssetPath());
                RefCount_ = 0;
                IsLoad = true;
                OnLoad(AssetList);
            }

            public void Unload()
            {
                foreach (var Cache in DependenciesCache_)
                {
                    if (Cache.Unused)
                    {
                        Cache.Unload();
                    }
                }

                OnUnload();
                IsLoad = false;
                RefCount_ = 0;
            }

            protected void IncRef()
            {
                RefCount_++;
                foreach (var Cache in DependenciesCache_)
                {
                    Cache.IncRef();
                }
            }

            protected void DecRef()
            {
                if (RefCount_ > 0)
                {
                    RefCount_--;
                    foreach (var Cache in DependenciesCache_)
                    {
                        Cache.DecRef();
                    }
                }
            }

            public virtual string[] GetAllDependencies()
            {
                var Deps = new List<string>();
                Deps.AddRange(AssetDatabase.GetDependencies(GetInternalAssetPath()));

                for (var Index = 0; Index < Deps.Count; ++Index)
                {
                    Deps[Index] = Deps[Index].Substring("Assets/StandaloneAssets/".Length).ToLower();
                }

                Deps.Remove(BundlePath);

                return Deps.ToArray();
            }

            public void AddDependencyCache(AssetBundleCacheBase Cache)
            {
                if (!DependenciesCache_.Contains(Cache))
                {
                    DependenciesCache_.Add(Cache);
                }
            }

            protected virtual void OnLoad(UnityEngine.Object[] AssetList)
            {
            }

            protected virtual void OnUnload()
            {
            }
        }

        private class AssetBundleCache<T> : AssetBundleCacheBase where T : UnityEngine.Object
        {
            private readonly Dictionary<string, T> AssetList_ = null;
            private readonly List<int> AssetInstanceIDList_ = null;

            public AssetBundleCache(AssetBundleType BundleType, string BundlePath)
                : base(BundleType, BundlePath)
            {
                AssetList_ = new Dictionary<string, T>();
                AssetInstanceIDList_ = new List<int>();
            }

            protected override void OnLoad(UnityEngine.Object[] Assets)
            {
                if (Assets != null)
                {
                    foreach (var Asset in Assets)
                    {
                        if (Asset is T TAsset)
                        {
                            AssetList_.Add(Asset.name.ToLower(), TAsset);
                            AssetInstanceIDList_.Add(Asset.GetInstanceID());
                        }
                    }
                }
            }

            protected override void OnUnload()
            {
                if (AssetList_.Count > 0)
                {
                    foreach (var Asset in AssetList_)
                    {
                        Resources.UnloadAsset(Asset.Value);
                    }
                    AssetList_.Clear();
                }

                AssetInstanceIDList_.Clear();
            }

            public T CreateAsset(string AssetName)
            {
                AssetName = AssetName.ToLower();
                if (!AssetList_.ContainsKey(AssetName))
                {
                    return null;
                }

                IncRef();
                return AssetList_[AssetName];
            }

            public void DeleteAsset(T Asset)
            {
                if (Asset != null && AssetInstanceIDList_.Contains(Asset.GetInstanceID()))
                {
                    DecRef();
                }
            }
        }

        private class PrefabBundleCache : AssetBundleCacheBase
        {
            private readonly Dictionary<string, ObjectPoolManager.ObjectPool> ObjectPools_ = new Dictionary<string, ObjectPoolManager.ObjectPool>();
            private readonly Dictionary<int, string> GameObjectPoolNames_ = new Dictionary<int, string>();

            public PrefabBundleCache(AssetBundleType BundleType, string BundlePath)
                : base(BundleType, BundlePath)
            {
            }

            protected override void OnLoad(UnityEngine.Object[] AssetList)
            {
                if (AssetList != null)
                {
                    foreach (var Asset in AssetList)
                    {
                        if (Asset is GameObject ObjAsset)
                        {
                            var Pool = ObjectPoolManager.AddPool($"{BundlePath}_{Asset.name}".ToLower(), ObjAsset);
                            ObjectPools_.Add(Pool.PoolName, Pool);
                        }
                    }
                }
            }

            protected override void OnUnload()
            {
                if (ObjectPools_.Count > 0)
                {
                    foreach (var Pool in ObjectPools_)
                    {
                        ObjectPoolManager.DeletePool(Pool.Value);
                    }

                    ObjectPools_.Clear();
                }

                GameObjectPoolNames_.Clear();
            }

            public GameObject CreateAsset(string AssetName)
            {
                var PoolName = $"{BundlePath}_{AssetName}".ToLower();
                if (!ObjectPools_.ContainsKey(PoolName))
                {
                    return null;
                }

                var Obj = ObjectPools_[PoolName].Spawn();
                if (!GameObjectPoolNames_.ContainsKey(Obj.GetInstanceID()))
                {
                    GameObjectPoolNames_.Add(Obj.GetInstanceID(), PoolName);
                }
                IncRef();
                return Obj;
            }

            public void DeleteAsset(GameObject Asset)
            {
                if (GameObjectPoolNames_.ContainsKey(Asset.GetInstanceID()))
                {
                    var AssetName = GameObjectPoolNames_[Asset.GetInstanceID()];
                    ObjectPools_[AssetName].Recycle(Asset);
                    DecRef();
                }
            }
        }

        private class DataBundleCache : AssetBundleCacheBase
        {
            public byte[] Buffer { get; private set; }

            public DataBundleCache(AssetBundleType BundleType, string BundlePath)
                : base(BundleType, BundlePath)
            {
                Buffer = null;
            }

            protected override void OnLoad(UnityEngine.Object[] AssetList)
            {
                if (AssetList != null && AssetList.Length > 0)
                {
                    if (AssetList[0] is TextAsset TAsset)
                    {
                        Buffer = TAsset.bytes;
                    }
                }
            }

            protected override void OnUnload()
            {
                Buffer = null;
            }
        }
    }
}

#endif