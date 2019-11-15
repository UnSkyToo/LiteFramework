using LiteFramework.Core.Log;
using LiteFramework.Game.Asset;
using LiteFramework.Game.Audio;
using LiteFramework.Game.Data;
using LiteFramework.Game.Sfx;
using LiteFramework.Game.UI;

namespace LiteFramework.Game
{
    public static class LiteGameManager
    {
        public static bool Startup()
        {
            LLogger.LInfo($"{nameof(DataManager)} Startup");
            if (!DataManager.Startup())
            {
                LLogger.LError($"{nameof(DataManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(AssetManager)} Startup");
            if (!AssetManager.Startup())
            {
                LLogger.LError($"{nameof(AssetManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(AudioManager)} Startup");
            if (!AudioManager.Startup())
            {
                LLogger.LError($"{nameof(AudioManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(SfxManager)} Startup");
            if (!SfxManager.Startup())
            {
                LLogger.LError($"{nameof(SfxManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(UIManager)} Startup");
            if (!UIManager.Startup())
            {
                LLogger.LError($"{nameof(UIManager)} Startup Failed");
                return false;
            }

            return true;
        }

        public static void Shutdown()
        {
            UIManager.Shutdown();
            SfxManager.Shutdown();
            AudioManager.Shutdown();
            AssetManager.Shutdown();
            DataManager.Shutdown();
        }

        public static void Tick(float DeltaTime)
        {
            AssetManager.Tick(DeltaTime);
            AudioManager.Tick(DeltaTime);
            SfxManager.Tick(DeltaTime);
            UIManager.Tick(DeltaTime);
        }
    }
}