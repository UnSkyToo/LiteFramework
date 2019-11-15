﻿using LiteFramework.Core.Async.Group;
using LiteFramework.Core.Async.Task;
using LiteFramework.Core.Async.Timer;
using LiteFramework.Core.Event;
using LiteFramework.Core.Log;
using LiteFramework.Core.Motion;
using LiteFramework.Core.Net;
using LiteFramework.Core.ObjectPool;

namespace LiteFramework.Core
{
    public static class LiteCoreManager
    {
        public static bool Startup(UnityEngine.MonoBehaviour Instance)
        {
            LLogger.LInfo($"{nameof(EventManager)} Startup");
            if (!EventManager.Startup())
            {
                LLogger.LError($"{nameof(EventManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(ObjectPoolManager)} Startup");
            if (!ObjectPoolManager.Startup())
            {
                LLogger.LError($"{nameof(ObjectPoolManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(GroupManager)} Startup");
            if (!GroupManager.Startup())
            {
                LLogger.LError($"{nameof(GroupManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(TaskManager)} Startup");
            if (!TaskManager.Startup(Instance))
            {
                LLogger.LError($"{nameof(TaskManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(TimerManager)} Startup");
            if (!TimerManager.Startup())
            {
                LLogger.LError($"{nameof(TimerManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(MotionManager)} Startup");
            if (!MotionManager.Startup())
            {
                LLogger.LError($"{nameof(MotionManager)} Startup Failed");
                return false;
            }

            LLogger.LInfo($"{nameof(NetManager)} Startup");
            if (!NetManager.Startup())
            {
                LLogger.LError($"{nameof(NetManager)} Startup Failed");
                return false;
            }

            return true;
        }

        public static void Shutdown()
        {
            NetManager.Shutdown();
            MotionManager.Shutdown();
            TimerManager.Shutdown();
            TaskManager.Shutdown();
            GroupManager.Shutdown();
            ObjectPoolManager.Shutdown();
            EventManager.Shutdown();
        }

        public static void Tick(float DeltaTime)
        {
            ObjectPoolManager.Tick(DeltaTime);
            GroupManager.Tick(DeltaTime);
            TaskManager.Tick(DeltaTime);
            TimerManager.Tick(DeltaTime);
            MotionManager.Tick(DeltaTime);
            NetManager.Tick(DeltaTime);
        }
    }
}