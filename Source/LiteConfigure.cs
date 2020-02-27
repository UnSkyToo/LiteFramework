using System.Collections.Generic;
using UnityEngine;

namespace LiteFramework
{
    public static class LiteConfigure
    {
        public const string LiteFrameworkVersion = "20.02.22.0";
        private const bool IsDebugMode_ = true;
        public static bool IsDebugMode
        {
            get
            {
                if (Debug.isDebugBuild)
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
		public const bool EnableUIAutoBind = true;

        public static bool IsWidthMatch = true;

        public static readonly Transform CanvasRoot = GameObject.Find("Canvas").transform;
        public static readonly Transform UIRoot = CanvasRoot.Find("UI").transform;
        public static readonly Transform AudioRoot = GameObject.Find("Audio").transform;
        public static readonly Transform ObjectPoolRoot = GameObject.Find("ObjectPool").transform;

        public const float WindowWidth = 720;
        public const float WindowHeight = 1280;
        public const float WindowLeft = -WindowWidth / 2;
        public const float WindowRight = WindowWidth / 2;
        public const float WindowTop = WindowHeight / 2;
        public const float WindowBottom = -WindowHeight / 2;
        public static readonly Vector2 WindowSize = new Vector2(WindowWidth, WindowHeight);

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