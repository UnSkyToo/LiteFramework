using System;
using Lite.Framework.Log;
using Lite.Framework.Lua;
using Lite.Logic;

namespace Lite.Framework
{
    public static class GameLauncher
    {
        private static bool IsLaunch_;

        public static bool Startup()
        {
            IsLaunch_ = false;
            Preload((IsDone) =>
            {
                if (!IsDone)
                {
                    IsLaunch_ = false;
                    Logger.DWarning("Preload Failed");
                    return;
                }

                if (!LogicManager.Startup())
                {
                    IsLaunch_ = false;
                    Logger.DWarning("LogicManager Startup Failed");
                }

                if (!LuaRuntime.Startup())
                {
                    IsLaunch_ = false;
                    Logger.DWarning("LuaRuntime Startup Failed");
                }

                IsLaunch_ = true;
            });

            return true;
        }

        public static void Shutdown()
        {
            if (!IsLaunch_)
            {
                return;
            }

            LuaRuntime.Shutdown();
            LogicManager.Shutdown();
            IsLaunch_ = false;
        }

        public static void Tick(float DeltaTime)
        {
            if (!IsLaunch_)
            {
                return;
            }

            LogicManager.Tick(DeltaTime);
            LuaRuntime.Tick(DeltaTime);
        }

        private static void Preload(Action<bool> Callback)
        {
            Callback?.Invoke(true);
        }
    }
}