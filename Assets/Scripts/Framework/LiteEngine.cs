using Lite.Framework.Extend;
using Lite.Framework.Helper;
using Lite.Framework.Manager;
using Lite.Logic;
using UnityEngine;
using Logger = Lite.Framework.Log.Logger;

namespace Lite.Framework
{
    public static class LiteEngine
    {
        private static MonoBehaviour MonoBehaviourInstance { get; set; }
        private static float EnterBackgroundTime_ = 0.0f;

        public static bool Startup(MonoBehaviour Instance)
        {
            MonoBehaviourInstance = Instance;
            if (!TaskManager.Startup(MonoBehaviourInstance))
            {
                Logger.DError("TaskManager Startup Failed");
                return false;
            }

            if (!ObjectPoolManager.Startup())
            {
                Logger.DError("ObjectPoolManager Startup Failed");
                return false;
            }

            if (!EventManager.Startup())
            {
                Logger.DError("EventManager Startup Failed");
                return false;
            }

            if (!AssetManager.Startup())
            {
                Logger.DError("AssetManager Startup Failed");
                return false;
            }

            if (!TimerManager.Startup())
            {
                Logger.DError("TimerManager Startup Failed");
                return false;
            }

            if (!MotionManager.Startup())
            {
                Logger.DError("MotionManager Startup Failed");
                return false;
            }

            if (!UIManager.Startup())
            {
                Logger.DError("UIManager Startup Failed");
                return false;
            }

            Attach<Debugger>(MonoBehaviourInstance.gameObject);
            Attach<Fps>(MonoBehaviourInstance.gameObject);
            
            if (!LogicManager.Startup())
            {
                Logger.DError("LogicManager Startup Failed");
                return false;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            return true;
        }

        public static void Shutdown()
        {
            LogicManager.Shutdown();
            UIManager.Shutdown();
            MotionManager.Shutdown();
            TimerManager.Shutdown();
            AssetManager.Shutdown();
            EventManager.Shutdown();
            ObjectPoolManager.Shutdown();
            TaskManager.Shutdown();

            Detach<Debugger>(MonoBehaviourInstance.gameObject);
            Detach<Fps>(MonoBehaviourInstance.gameObject);

            PlayerPrefs.Save();
        }

        public static void Tick(float DeltaTime)
        {
            TaskManager.Tick(DeltaTime);
            ObjectPoolManager.Tick(DeltaTime);
            EventManager.Tick(DeltaTime);
            AssetManager.Tick(DeltaTime);
            TimerManager.Tick(DeltaTime);
            MotionManager.Tick(DeltaTime);
            UIManager.Tick(DeltaTime);
            LogicManager.Tick(DeltaTime);
        }

        public static void Restart()
        {
            UnityHelper.ClearLog();
            Shutdown();
            Startup(MonoBehaviourInstance);
        }

        public static T Attach<T>(GameObject Root) where T : MonoBehaviour
        {
            var Component = Root.GetComponent<T>();

            if (Component != null)
            {
                return Component;
            }

            return Root.AddComponent<T>();
        }

        public static void Detach<T>(GameObject Root) where T : MonoBehaviour
        {
            var Component = Root.GetComponent<T>();

            if (Component != null)
            {
                Object.DestroyImmediate(Component);
            }
        }

        public static void OnEnterForeground()
        {
            if (Time.realtimeSinceStartup - EnterBackgroundTime_ >= Configure.EnterBackgroundMaxTime)
            {
                Restart();
            }

            EnterBackgroundTime_ = Time.realtimeSinceStartup;
        }

        public static void OnEnterBackground()
        {
            EnterBackgroundTime_ = Time.realtimeSinceStartup;
        }
    }
}