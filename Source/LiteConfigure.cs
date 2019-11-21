using System.Collections.Generic;

namespace LiteFramework
{
    public static class LiteConfigure
    {
        public const string LiteFrameworkVersion = "19.10.31.0";
        private const bool IsDebugMode_ = true;
        public static bool IsDebugMode
        {
            get
            {
                if (UnityEngine.Debug.isDebugBuild)
                {
                    return IsDebugMode_;
                }

                return false;
            }
        }
        public const bool EnterBackgroundAutoRestart = false;
        public const float EnterBackgroundMaxTime = 90.0f;
        public const string AssetBundleManifestName = "StreamingAssets.lite";
        public const string StandaloneAssetsName = "StandaloneAssets";
        public const string CanvasBottomName = "Canvas-Bottom";
        public const string CanvasNormalName = "Canvas-Normal";
        public const string CanvasTopName = "Canvas-Top";
        public const bool EnableButtonClick = true;

        public static readonly Dictionary<System.Type, Game.UI.UIDescriptor> UIDescList = new Dictionary<System.Type, Game.UI.UIDescriptor>();

        public static void RegisterUI(Dictionary<System.Type, Game.UI.UIDescriptor> DescList)
        {
            foreach (var Desc in DescList)
            {
                UIDescList.Add(Desc.Key, Desc.Value);
            }
        }
    }
}