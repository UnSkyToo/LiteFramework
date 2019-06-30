using System;
using Lite.Framework.Base;
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

            var State = MainEntity_.Startup();
            if (!State)
            {
                Logger.DWarning("lua main start failed");
            }

            EventManager.Register<EnterForegroundEvent>(OnEnterForegroundEvent);
            EventManager.Register<EnterBackgroundEvent>(OnEnterBackgroundEvent);

            return State;
        }

        public static void Shutdown()
        {
            EventManager.UnRegister<EnterForegroundEvent>(OnEnterForegroundEvent);
            EventManager.UnRegister<EnterBackgroundEvent>(OnEnterBackgroundEvent);

            MainEntity_?.Shutdown();
            MainEntity_ = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            LuaEnv_.Dispose(true);
        }

        public static void Tick(float DeltaTime)
        {
            MainEntity_?.Tick(DeltaTime);
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

        private static void OnEnterForegroundEvent(EnterForegroundEvent Msg)
        {
            MainEntity_?.EnterForeground();
        }

        private static void OnEnterBackgroundEvent(EnterBackgroundEvent Msg)
        {
            MainEntity_?.EnterBackground();
        }
    }
}