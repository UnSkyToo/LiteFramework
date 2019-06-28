using Lite.Framework.Log;
using Lite.Framework.Manager;
using XLua;

namespace Lite.Logic.Lua
{
    public class LuaLogic : ILogic
    {
        private LuaEnv LuaEnv_ = null;
        private ILuaMainEntity MainEntity_ = null;

        public LuaLogic()
        {
        }

        public bool Startup()
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

        public void Shutdown()
        {
            MainEntity_.OnStop();
            MainEntity_ = null;
            LuaEnv_.Dispose(true);
        }

        public void Tick(float DeltaTime)
        {
            MainEntity_.OnTick(DeltaTime);
        }

        private static byte[] StandaloneLuaLoader(ref string LuaPath)
        {
            var FullPath = $"scripts/{LuaPath}.lua";
            return AssetManager.CreateDataSync(FullPath);
        }
    }
}