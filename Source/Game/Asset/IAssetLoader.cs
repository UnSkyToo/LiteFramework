namespace LiteFramework.Game.Asset
{
    internal interface IAssetLoader
    {
        bool Startup();
        void Shutdown();
        void Tick(float DeltaTime);

        bool AssetCacheExisted(string AssetPath);
        void PreloadedAsset<T>(string AssetPath, LiteAction<bool> Callback) where T : UnityEngine.Object;

        void CreateAssetAsync<T>(AssetUri Uri, LiteAction<T> Callback = null) where T : UnityEngine.Object;
        T CreateAssetSync<T>(AssetUri Uri) where T : UnityEngine.Object;

        void CreatePrefabAsync(AssetUri Uri, LiteAction<UnityEngine.GameObject> Callback = null);
        UnityEngine.GameObject CreatePrefabSync(AssetUri Uri);

        void CreateDataAsync(AssetUri Uri, LiteAction<byte[]> Callback = null);
        byte[] CreateDataSync(AssetUri Uri);

        void DeleteAsset<T>(T Asset) where T : UnityEngine.Object;
        void DeleteAsset(UnityEngine.GameObject Asset);
        void DeleteUnusedAsset();
    }
}