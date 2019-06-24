using System;
using System.Collections.Generic;
using Lite.Logic.UI;

namespace Lite
{
    public static class Configure
    {
        public const float EnterBackgroundMaxTime = 90.0f;
        public const string AssetBundleManifestName = "StreamingAssets";

        public static readonly Dictionary<Type, string> UIList = new Dictionary<Type, string>
        {
            {typeof(LogoUI), "LogoUI"},
        };
    }
}