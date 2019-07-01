using System;
using Lite.Framework.Log;
using Lite.Framework.Lua;
using Lite.Framework.Manager;
using Lite.Logic;
using Lite.Logic.UI;

namespace Lite.Framework
{
    public static class LiteLauncher
    {
        private static bool IsLaunch_;

        public static bool Startup()
        {
            UIManager.OpenUI<LogoUI>();
            
            Logger.DInfo("Preload Start...");
            IsLaunch_ = false;
            Preload((IsDone) =>
            {
                UIManager.CloseUI<LogoUI>();

                Logger.DInfo("Preload End...");
                if (!IsDone)
                {
                    IsLaunch_ = false;
                    Logger.DWarning("Preload Failed");
                    return;
                }

                Logger.DInfo("LogicManager Startup");
                if (!LogicManager.Startup())
                {
                    IsLaunch_ = false;
                    Logger.DWarning("LogicManager Startup Failed");
                }

                Logger.DInfo("LuaRuntime Startup");
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
            TimerManager.AddTimer(1, () => { Callback?.Invoke(true); });
        }
    }
}