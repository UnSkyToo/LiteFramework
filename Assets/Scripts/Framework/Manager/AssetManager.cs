//#define LITE_USE_INTERNAL_RES

using System;
using UnityEngine;


#if LITE_USE_INTERNAL_RES
using AssetMgr = Lite.Framework.Manager.AssetInternalManager;
#else
using AssetMgr = Lite.Framework.Manager.AssetBundleManager;
#endif

namespace Lite.Framework.Manager
{
    public static class AssetManager
    {
        public static bool Startup()
        {
            return AssetMgr.Startup();
        }

        public static void Shutdown()
        {
            AssetMgr.Shutdown();
        }

        public static void Tick(float DeltaTime)
        {
            AssetMgr.Tick(DeltaTime);
        }

        public static void CreateAsset<T>(string BundlePath, string AssetName, Action<T> Callback = null) where T : UnityEngine.Object
        {
            AssetMgr.CreateAsset<T>(BundlePath, AssetName, Callback);
        }

        public static void CreateAsset<T>(string BundlePath, Action<T> Callback = null) where T : UnityEngine.Object
        {
            AssetMgr.CreateAsset(BundlePath, Callback);
        }

        public static T CreateAssetWithCache<T>(string BundlePath, string AssetName, Action<T> Callback = null) where T : UnityEngine.Object
        {
            return AssetMgr.CreateAssetWithCache<T>(BundlePath, AssetName, Callback);
        }

        public static void CreatePrefab(string BundlePath, string AssetName, Action<GameObject> Callback = null)
        {
            AssetMgr.CreatePrefab(BundlePath, AssetName, Callback);
        }

        public static void CreatePrefab(string BundlePath, Action<GameObject> Callback = null)
        {
            AssetMgr.CreatePrefab(BundlePath, Callback);
        }

        public static GameObject CreatePrefabWithCache(string BundlePath, string AssetName, Action<GameObject> Callback = null)
        {
            return AssetMgr.CreatePrefabWithCache(BundlePath, AssetName, Callback);
        }

        public static void CreateData(string BundlePath, Action<byte[]> Callback = null)
        {
            AssetMgr.CreateData(BundlePath, Callback);
        }

        public static byte[] CreateDataWithCache(string BundlePath, Action<byte[]> Callback = null)
        {
            return AssetMgr.CreateDataWithCache(BundlePath, Callback);
        }

        public static void DeleteAsset<T>(T Asset) where T : UnityEngine.Object
        {
            AssetMgr.DeleteAsset<T>(Asset);
        }

        public static void DeleteAsset(GameObject Asset)
        {
            AssetMgr.DeleteAsset(Asset);
        }

        public static void DeleteUnusedAssetBundle()
        {
            AssetMgr.DeleteUnusedAssetBundle();
        }
    }
}