﻿using LiteFramework.Core;
using LiteFramework.Core.Event;
using LiteFramework.Core.Log;
using LiteFramework.Extend.Debug;
using LiteFramework.Game;
using LiteFramework.Helper;
using LiteFramework.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace LiteFramework
{
    public static class LiteManager
    {
        public static bool IsPause { get; set; }
        public static bool IsRestart { get; private set; }
        public static bool IsFocus { get; private set; }

        public static float TimeScale
        {
            get => Time.timeScale;
            set => Time.timeScale = value;
        }

        private static MonoBehaviour MonoBehaviourInstance { get; set; }
        private static float EnterBackgroundTime_ = 0.0f;

        private static ILogic MainLogic_;

        public static bool Startup(MonoBehaviour Instance, ILogic Logic)
        {
            IsPause = true;
            IsRestart = false;
            IsFocus = true;
            TimeScale = 1.0f;
            MonoBehaviourInstance = Instance;
            LLogger.Enabled = LiteConfigure.IsDebugMode;

            LiteConfigure.UIDescList.Clear();

            if (LiteConfigure.IsDebugMode)
            {
                Attach<Debugger>();
            }

            LLogger.LInfo("Lite Framework Startup");
            if (!LiteCoreManager.Startup(MonoBehaviourInstance))
            {
                return false;
            }

            if (!LiteGameManager.Startup())
            {
                return false;
            }

            MainLogic_ = Logic;
            if (MainLogic_ == null || !MainLogic_.Startup())
            {
                LLogger.LError("Logic Startup Failed");
                return false;
            }

            InitConfigure();
            IsPause = false;
            return true;
        }

        public static void Shutdown()
        {
            OnEnterBackground();

            MainLogic_?.Shutdown();

            DialogHelper.Clear();
            MaskLayerHelper.DisposeMaskLayer();
            LiteGameManager.Shutdown();
            LiteCoreManager.Shutdown();

            if (LiteConfigure.IsDebugMode)
            {
                Detach<Debugger>();
            }

            PlayerPrefs.Save();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            LLogger.LInfo("Lite Framework Shutdown");
        }

        public static void Tick(float DeltaTime)
        {
            if (IsRestart)
            {
                RestartGameManager();
                return;
            }

            if (IsPause)
            {
                return;
            }

            var Dt = DeltaTime/* * TimeScale*/;
            LiteCoreManager.Tick(Dt);
            LiteGameManager.Tick(Dt);
            MainLogic_?.Tick(Dt);
        }

        private static void InitConfigure()
        {
            var Ratio = (float)Screen.height / (float)Screen.width;
            LiteConfigure.IsWidthMatch = Ratio >= (LiteConfigure.WindowHeight / LiteConfigure.WindowWidth);
            LiteConfigure.CanvasRoot.GetComponent<CanvasScaler>().matchWidthOrHeight = LiteConfigure.IsWidthMatch ? 0 : 1;
            Camera.main.orthographicSize = Screen.height / 100.0f / 2.0f;

            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Random.InitState((int)System.DateTime.Now.Ticks);
        }

        public static void Restart()
        {
            IsRestart = true;
        }

        private static void RestartGameManager()
        {
            IsRestart = false;
            Debug.ClearDeveloperConsole();
            Shutdown();
            IsPause = !Startup(MonoBehaviourInstance, MainLogic_);
        }

        public static T Attach<T>() where T : MonoBehaviour
        {
            var Component = MonoBehaviourInstance?.gameObject.GetComponent<T>();

            if (Component != null)
            {
                return Component;
            }

            return MonoBehaviourInstance?.gameObject.AddComponent<T>();
        }

        public static void Detach<T>() where T : MonoBehaviour
        {
            var Component = MonoBehaviourInstance?.gameObject.GetComponent<T>();

            if (Component != null)
            {
                Object.DestroyImmediate(Component);
            }
        }

        public static void OnEnterForeground()
        {
            if (IsFocus)
            {
                return;
            }

            IsPause = false;
            IsFocus = true;

            LLogger.LWarning("OnEnterForeground");
            EventManager.Send<EnterForegroundEvent>();

            if (LiteConfigure.EnterBackgroundAutoRestart && Time.realtimeSinceStartup - EnterBackgroundTime_ >= LiteConfigure.EnterBackgroundMaxTime)
            {
                Restart();
                return;
            }

            EnterBackgroundTime_ = Time.realtimeSinceStartup;
        }

        public static void OnEnterBackground()
        {
            if (!IsFocus)
            {
                return;
            }
            IsPause = true;
            IsFocus = false;

            LLogger.LWarning("OnEnterBackground");
            EventManager.Send<EnterBackgroundEvent>();
            EnterBackgroundTime_ = Time.realtimeSinceStartup;
        }
    }
}