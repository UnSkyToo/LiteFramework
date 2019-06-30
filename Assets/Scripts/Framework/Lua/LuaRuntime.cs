using System;
using Lite.Framework.Log;
using Lite.Framework.Lua.Interface;
using Lite.Framework.Manager;
using XLua;

namespace Lite.Framework.Lua
{
    public static class LuaRuntime
    {
        private static LuaEnv LuaEnv_ = null;
        private static ILuaMainEntity MainEntity_ = null;

        public static bool Startup()
        {
            LuaEnv_ = new LuaEnv();
            LuaEnv_.AddLoader(StandaloneLuaLoader);

            LuaEnv_.DoString("_lite_main_entity_ = require 'main'", "main");

            MainEntity_ = LuaEnv_.Global.GetInPath<ILuaMainEntity>("_lite_main_entity_");
            if (MainEntity_ == null)
            {
                Logger.DWarning("can't load main.lua file");
                return false;
            }

            var State = MainEntity_.OnStart();
            if (!State)
            {
                Logger.DWarning("lua main start failed");
            }

            return State;
        }

        public static void Shutdown()
        {
            MainEntity_?.OnStop();
            MainEntity_ = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            LuaEnv_.Dispose(true);
        }

        public static void Tick(float DeltaTime)
        {
            MainEntity_?.OnTick(DeltaTime);
        }

        private static byte[] StandaloneLuaLoader(ref string LuaPath)
        {
#if UNITY_EDITOR && LITE_USE_INTERNAL_ASSET
            var FullPath = $"scripts/{LuaPath.Replace('.', '/')}.lua";
#else
            var FullPath = $"scripts/{LuaPath.Replace('.', '/')}.lua.bytes";
#endif
            //LuaPath = Helper.PathHelper.GetAssetFullPath(FullPath);
            return AssetManager.CreateDataSync(FullPath);
        }

        public static void OpenLuaUI(string Path, LuaTable LuaEntity)
        {
            var UI = new UIBaseLua(LuaEntity);
            UIManager.OpenUI<UIBaseLua>(UI, Path, null);
        }

        public static void CloseLuaUI(LuaTable LuaEntity)
        {
            var UI = LuaEntity?.GetInPath<UIBaseLua>("_CSEntity_");
            if (UI != null)
            {
                UIManager.CloseUI(UI);
            }
        }
    }
}