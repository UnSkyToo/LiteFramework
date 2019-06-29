﻿using System;
using System.Collections;
using System.Collections.Generic;
using Lite.Framework.Helper;
using Lite.Framework.Log;

namespace Lite.Framework.Manager
{
    public static class AssetBundleManager
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

        private static UnityEngine.AssetBundleManifest Manifest_ = null;
        private static readonly List<string> AssetBundlePathList_ = new List<string>();
        private static readonly Dictionary<string, AssetBundleCacheBase> AssetBundleCacheList_ = new Dictionary<string, AssetBundleCacheBase>();
        private static readonly Dictionary<string, List<Action>> LoadAssetBundleCallbackList_ = new Dictionary<string, List<Action>>();
        private static readonly Dictionary<int, string> AssetBundlePathCacheList_ = new Dictionary<int, string>();

        public static bool Startup()
        {
            Manifest_ = null;
            AssetBundlePathList_.Clear();
            AssetBundleCacheList_.Clear();
            LoadAssetBundleCallbackList_.Clear();
            AssetBundlePathCacheList_.Clear();

            if (!LoadAssetBundleManifest(Configure.AssetBundleManifestName))
            {
                return false;
            }
            
            return true;
        }

        public static void Shutdown()
        {
            if (Manifest_ != null)
            {
                UnityEngine.Resources.UnloadAsset(Manifest_);
                Manifest_ = null;
            }

            AssetBundlePathList_.Clear();

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

            UnityEngine.Resources.UnloadUnusedAssets();
            UnityEngine.AssetBundle.UnloadAllAssetBundles(true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void Tick(float DeltaTime)
        {
        }

        private static UnityEngine.AssetBundleCreateRequest CreateBundleRequestAsync(string Path)
        {
            var FullPath = PathHelper.GetAssetFullPath(Path);
            if (string.IsNullOrEmpty(FullPath))
            {
                return null;
            }
            return UnityEngine.AssetBundle.LoadFromFileAsync(FullPath);
        }

        private static UnityEngine.AssetBundle CreateBundleRequestSync(string Path)
        {
            var FullPath = PathHelper.GetAssetFullPath(Path);
            if (string.IsNullOrEmpty(FullPath))
            {
                return null;
            }
            return UnityEngine.AssetBundle.LoadFromFile(FullPath);
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
                case ".sprite":
                    return AssetRealType.Sprite;
                case ".texture":
                    return AssetRealType.Texture;
                case ".bytes":
                    return AssetRealType.Data;
                case ".prefab":
                    return AssetRealType.Prefab;
                default:
                    return AssetRealType.Asset;
            }
        }

        private static bool LoadAssetBundleManifest(string ResPath)
        {
            var FullPath = PathHelper.GetAssetFullPath(ResPath);
            if (string.IsNullOrEmpty(FullPath))
            {
                return false;
            }

            var Bundle = UnityEngine.AssetBundle.LoadFromFile(FullPath);
            if (Bundle != null)
            {
                Manifest_ = Bundle.LoadAsset<UnityEngine.AssetBundleManifest>("AssetBundleManifest");
                Bundle.Unload(false);
                AssetBundlePathList_.AddRange(Manifest_.GetAllAssetBundles());
                return true;
            }
            else
            {
                Logger.DError($"LoadAssetBundleManifest Failed : {ResPath}");
            }

            return false;
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
                            Cache = new AssetBundleCache<UnityEngine.Sprite>(BundleType, BundlePath);
                            break;
                        case AssetRealType.Texture:
                            Cache = new AssetBundleCache<UnityEngine.Texture>(BundleType, BundlePath);
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

        private static bool LoadAssetBundleAsync<T>(AssetBundleType BundleType, string BundlePath, Action Callback = null) where T : UnityEngine.Object
        {
            BundlePath = BundlePath.ToLower();
            if (AssetBundleCacheExisted(BundlePath))
            {
                Callback?.Invoke();
                return true;
            }

            if (!AssetBundlePathList_.Contains(BundlePath))
            {
                Callback?.Invoke();
                return false;
            }

            if (!LoadAssetBundleCallbackList_.ContainsKey(BundlePath))
            {
                LoadAssetBundleCallbackList_.Add(BundlePath, new List<Action> {Callback});

                return LoadAssetBundleCacheCompletedAsync<T>(BundleType, BundlePath, () =>
                {
                    foreach (var LoadCallback in LoadAssetBundleCallbackList_[BundlePath])
                    {
                        LoadCallback?.Invoke();
                    }
                    LoadAssetBundleCallbackList_.Remove(BundlePath);
                });
            }
            else
            {
                LoadAssetBundleCallbackList_[BundlePath].Add(Callback);
            }

            return true;
        }

        private static AssetBundleCacheBase LoadAssetBundleSync<T>(AssetBundleType BundleType, string BundlePath) where T : UnityEngine.Object
        {
            BundlePath = BundlePath.ToLower();
            if (AssetBundleCacheExisted(BundlePath))
            {
                return AssetBundleCacheList_[BundlePath];
            }

            if (!AssetBundlePathList_.Contains(BundlePath))
            {
                return null;
            }

            return LoadAssetBundleCacheCompletedSync<T>(BundleType, BundlePath);
        }

        private static bool LoadAssetBundleCacheCompletedAsync<T>(AssetBundleType BundleType, string BundlePath, Action Callback = null) where T : UnityEngine.Object
        {
            var Cache = CreateAssetBundleCache<T>(BundleType, BundlePath);
            if (Cache == null)
            {
                Callback?.Invoke();
                return false;
            }

            AssetBundleCacheList_.Add(BundlePath, Cache);
            LoadAssetBundleCacheDependenciesAsync<T>(Cache, () =>
            {
                TaskManager.AddTask(LoadAssetBundleCacheAsync<T>(Cache), () =>
                {
                    Callback?.Invoke();
                });
            });

            return true;
        }

        private static AssetBundleCacheBase LoadAssetBundleCacheCompletedSync<T>(AssetBundleType BundleType, string BundlePath) where T : UnityEngine.Object
        {
            var Cache = CreateAssetBundleCache<T>(BundleType, BundlePath);
            if (Cache == null)
            {
                return null;
            }

            AssetBundleCacheList_.Add(BundlePath, Cache);
            LoadAssetBundleCacheDependenciesSync<T>(Cache);
            return LoadAssetBundleCacheSync<T>(Cache); ;
        }

        private static IEnumerator LoadAssetBundleCacheAsync<T>(AssetBundleCacheBase Cache) where T : UnityEngine.Object
        {
            if (!AssetBundleCacheList_.ContainsKey(Cache.BundlePath))
            {
                yield break;
            }

            yield return TaskManager.WaitTask(Cache.LoadAsync());

            if (!Cache.IsLoad)
            {
                AssetBundleCacheList_.Remove(Cache.BundlePath);
            }
            else
            {
                //AssetBundleCacheList_[Cache.BundlePath].UnloadAssetBundle();
            }
        }

        private static AssetBundleCacheBase LoadAssetBundleCacheSync<T>(AssetBundleCacheBase Cache) where T : UnityEngine.Object
        {
            if (!AssetBundleCacheList_.ContainsKey(Cache.BundlePath))
            {
                return null;
            }

            Cache.LoadSync();

            if (!Cache.IsLoad)
            {
                AssetBundleCacheList_.Remove(Cache.BundlePath);
                Cache = null;
            }
            else
            {
                //AssetBundleCacheList_[Cache.BundlePath].UnloadAssetBundle();
            }

            return Cache;
        }

        private static void LoadAssetBundleCacheDependenciesAsync<T>(AssetBundleCacheBase Cache, Action Callback = null) where T : UnityEngine.Object
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
                LoadAssetBundleAsync<T>(BundleType, BundlePath, () =>
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

        private static void LoadAssetBundleCacheDependenciesSync<T>(AssetBundleCacheBase Cache) where T : UnityEngine.Object
        {
            var Dependencies = Cache.GetAllDependencies();

            if (Dependencies == null || Dependencies.Length == 0)
            {
                return;
            }

            foreach (var Dependency in Dependencies)
            {
                var BundlePath = Dependency;
                var BundleType = GetAssetBundleTypeWithName(BundlePath);

                var DependencyBundle = LoadAssetBundleSync<T>(BundleType, BundlePath);
                if (DependencyBundle != null)
                {
                    Cache.AddDependencyCache(DependencyBundle);
                }
            }
        }

        public static void CreateAssetAsync<T>(string BundlePath, string AssetName, Action<T> Callback = null) where T : UnityEngine.Object
        {
            var IsLoaded = LoadAssetBundleAsync<T>(AssetBundleType.Asset, BundlePath, () =>
            {
                Callback?.Invoke(CreateAssetSync<T>(BundlePath, AssetName));
            });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public static void CreateAssetAsync<T>(string BundlePath, Action<T> Callback = null) where T : UnityEngine.Object
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            CreateAssetAsync<T>(BundlePath, AssetName, Callback);
        }

        public static T CreateAssetSync<T>(string BundlePath, string AssetName) where T : UnityEngine.Object
        {
            T Asset = null;
            BundlePath = BundlePath.ToLower();
            AssetName = AssetName.ToLower();

            AssetBundleCache<T> AssetCache = null;
            if (!AssetBundleCacheList_.ContainsKey(BundlePath))
            {
                AssetCache = LoadAssetBundleSync<T>(AssetBundleType.Asset, BundlePath) as AssetBundleCache<T>;
            }
            else
            {
                AssetCache = AssetBundleCacheList_[BundlePath] as AssetBundleCache<T>;
            }

            if (AssetCache != null)
            {
                Asset = AssetCache.CreateAsset(AssetName);

                if (Asset != null)
                {
                    if (!AssetBundlePathCacheList_.ContainsKey(Asset.GetInstanceID()))
                    {
                        AssetBundlePathCacheList_.Add(Asset.GetInstanceID(), BundlePath);
                    }
                }
                else
                {
                    Logger.DWarning($"Can't Create Asset : {BundlePath} - {AssetName}");
                }
            }

            return Asset;
        }

        public static T CreateAssetSync<T>(string BundlePath) where T : UnityEngine.Object
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            return CreateAssetSync<T>(BundlePath, AssetName);
        }

        public static void CreatePrefabAsync(string BundlePath, string AssetName, Action<UnityEngine.GameObject> Callback = null)
        {
            var IsLoaded = LoadAssetBundleAsync<UnityEngine.Object>(AssetBundleType.Prefab, BundlePath, () =>
            {
                Callback?.Invoke(CreatePrefabSync(BundlePath, AssetName));
            });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public static void CreatePrefabAsync(string BundlePath, Action<UnityEngine.GameObject> Callback = null)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            CreatePrefabAsync(BundlePath, AssetName, Callback);
        }

        public static UnityEngine.GameObject CreatePrefabSync(string BundlePath, string AssetName)
        {
            UnityEngine.GameObject Asset = null;
            BundlePath = BundlePath.ToLower();
            AssetName = AssetName.ToLower();

            PrefabBundleCache PrefabCache = null;
            if (!AssetBundleCacheList_.ContainsKey(BundlePath))
            {
                PrefabCache = LoadAssetBundleSync<UnityEngine.Object>(AssetBundleType.Prefab, BundlePath) as PrefabBundleCache;
            }
            else
            {
                PrefabCache = AssetBundleCacheList_[BundlePath] as PrefabBundleCache;
            }

            if (PrefabCache != null)
            {
                Asset = PrefabCache.CreateAsset(AssetName);

                if (Asset != null)
                {
                    AssetBundlePathCacheList_.Add(Asset.GetInstanceID(), BundlePath);
                }
                else
                {
                    Logger.DWarning($"can't create asset : {BundlePath} - {AssetName}");
                }
            }

            return Asset;
        }

        public static UnityEngine.GameObject CreatePrefabSync(string BundlePath)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            return CreatePrefabSync(BundlePath, AssetName);
        }

        public static void CreateDataAsync(string BundlePath, string AssetName, Action<byte[]> Callback = null)
        {
            var IsLoaded = LoadAssetBundleAsync<UnityEngine.TextAsset>(AssetBundleType.Data, BundlePath, () =>
            {
                Callback?.Invoke(CreateDataSync(BundlePath, AssetName));
            });

            if (!IsLoaded)
            {
                Callback?.Invoke(null);
            }
        }

        public static void CreateDataAsync(string BundlePath, Action<byte[]> Callback = null)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            CreateDataAsync(BundlePath, AssetName, Callback);
        }

        public static byte[] CreateDataSync(string BundlePath, string AssetName)
        {
            UnityEngine.TextAsset Asset = null;
            BundlePath = BundlePath.ToLower();
            AssetName = AssetName.ToLower();

            DataBundleCache DataCache = null;
            if (!AssetBundleCacheList_.ContainsKey(BundlePath))
            {
                DataCache = LoadAssetBundleSync<UnityEngine.TextAsset>(AssetBundleType.Data, BundlePath) as DataBundleCache;
            }
            else
            {
                DataCache = AssetBundleCacheList_[BundlePath] as DataBundleCache;
            }

            if (DataCache != null)
            {
                Asset = DataCache.CreateAsset(AssetName);

                if (Asset != null)
                {
                    AssetBundlePathCacheList_.Add(Asset.GetInstanceID(), BundlePath);
                }
                else
                {
                    Logger.DWarning($"can't create asset : {BundlePath} - {AssetName}");
                }
            }

            return Asset?.bytes;
        }

        public static byte[] CreateDataSync(string BundlePath)
        {
            var AssetName = PathHelper.GetFileNameWithoutExt(BundlePath);
            return CreateDataSync(BundlePath, AssetName);
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
                    if (typeof(T) != typeof(UnityEngine.GameObject))
                    {
                        if (AssetBundleCacheList_[BundlePath] is AssetBundleCache<T> Cache)
                        {
                            Cache.DeleteAsset(Asset);
                        }
                    }
                }
            }
        }

        public static void DeleteAsset(UnityEngine.GameObject Asset)
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

                UnityEngine.Resources.UnloadUnusedAssets();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private class AssetBundleCacheBase
        {
            public AssetBundleType BundleType { get; }
            public string BundlePath { get; }
            public UnityEngine.AssetBundle Bundle { get; private set; }

            public bool IsLoad { get; private set; }
            public bool Unused => (RefCount_ <= 0 && IsLoad == true);

            private int RefCount_;
            protected readonly List<AssetBundleCacheBase> DependenciesCache_;

            protected AssetBundleCacheBase(AssetBundleType BundleType, string BundlePath)
            {
                this.BundleType = BundleType;
                this.BundlePath = BundlePath;
                this.Bundle = null;

                this.IsLoad = false;
                this.RefCount_ = 0;
                this.DependenciesCache_ = new List<AssetBundleCacheBase>();
            }

            public IEnumerator LoadAsync()
            {
                IsLoad = false;
                var Request = CreateBundleRequestAsync(BundlePath);
                yield return Request;

                Logger.DInfo($"Load AssetBundle : {BundlePath}");

                if (!Request.isDone)
                {
                    Logger.DWarning($"Load AssetBundle : {BundlePath} Failed");
                }
                else
                {
                    RefCount_ = 0;
                    IsLoad = true;
                    Bundle = Request.assetBundle;
                    OnLoad();
                }

                yield break;
            }

            public void LoadSync()
            {
                IsLoad = false;
                var Request = CreateBundleRequestSync(BundlePath);
                //Logger.DInfo($"Load AssetBundle : {BundlePath}");

                if (!Request)
                {
                    Logger.DWarning($"Load AssetBundle : {BundlePath} Failed");
                }
                else
                {
                    RefCount_ = 0;
                    IsLoad = true;
                    Bundle = Request;
                    OnLoad();
                }
            }

            public void Unload()
            {
                OnUnload();
                UnloadAssetBundle();
                IsLoad = false;
                RefCount_ = 0;
                DependenciesCache_.Clear();
            }

            private void UnloadAssetBundle()
            {
                if (Bundle != null)
                {
                    Bundle.Unload(false);
                    Bundle = null;
                }

                foreach (var Cache in DependenciesCache_)
                {
                    if (Cache.Unused)
                    {
                        Cache.Unload();
                    }
                }
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
                return Manifest_.GetAllDependencies(BundlePath);
            }

            public void AddDependencyCache(AssetBundleCacheBase Cache)
            {
                if (Cache == null)
                {
                    return;
                }

                if (!DependenciesCache_.Contains(Cache))
                {
                    DependenciesCache_.Add(Cache);
                }
            }

            protected virtual void OnLoad()
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

            protected override void OnLoad()
            {
                var AssetList = Bundle.LoadAllAssets<T>();
                if (AssetList != null)
                {
                    foreach (var Asset in AssetList)
                    {
                        AssetList_.Add(Asset.name.ToLower(), Asset);
                        AssetInstanceIDList_.Add(Asset.GetInstanceID());
                    }
                }
            }

            protected override void OnUnload()
            {
                if (AssetList_.Count > 0)
                {
                    foreach (var Asset in AssetList_)
                    {
                        UnityEngine.Resources.UnloadAsset(Asset.Value);
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

            protected override void OnLoad()
            {
                var AssetList = Bundle.LoadAllAssets<UnityEngine.GameObject>();

                if (AssetList != null)
                {
                    foreach (var Asset in AssetList)
                    {
                        var Pool = ObjectPoolManager.AddPool($"{BundlePath}_{Asset.name}".ToLower(), Asset);
                        ObjectPools_.Add(Pool.PoolName, Pool);
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
                        UnityEngine.Object.DestroyImmediate(Pool.Value.Prefab, true);
                    }

                    ObjectPools_.Clear();
                }

                GameObjectPoolNames_.Clear();
            }

            public UnityEngine.GameObject CreateAsset(string AssetName)
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

            public void DeleteAsset(UnityEngine.GameObject Asset)
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
            private readonly Dictionary<string, UnityEngine.TextAsset> AssetList_ = null;
            private readonly List<int> AssetInstanceIDList_ = null;

            public DataBundleCache(AssetBundleType BundleType, string BundlePath)
                : base(BundleType, BundlePath)
            {
                AssetList_ = new Dictionary<string, UnityEngine.TextAsset>();
                AssetInstanceIDList_ = new List<int>();
            }

            protected override void OnLoad()
            {
                var AssetList = Bundle.LoadAllAssets<UnityEngine.TextAsset>();
                if (AssetList != null && AssetList.Length > 0)
                {
                    foreach (var Asset in AssetList)
                    {
                        AssetList_.Add(Asset.name.ToLower(), Asset);
                        AssetInstanceIDList_.Add(Asset.GetInstanceID());
                    }
                }
            }

            protected override void OnUnload()
            {
                if (AssetList_.Count > 0)
                {
                    foreach (var Asset in AssetList_)
                    {
                        UnityEngine.Resources.UnloadAsset(Asset.Value);
                    }
                    AssetList_.Clear();
                }

                AssetInstanceIDList_.Clear();
            }

            public UnityEngine.TextAsset CreateAsset(string AssetName)
            {
                AssetName = AssetName.ToLower();
                if (!AssetList_.ContainsKey(AssetName))
                {
                    return null;
                }

                IncRef();
                return AssetList_[AssetName];
            }

            public void DeleteAsset(UnityEngine.TextAsset Asset)
            {
                if (Asset != null && AssetInstanceIDList_.Contains(Asset.GetInstanceID()))
                {
                    DecRef();
                }
            }
        }
    }
}