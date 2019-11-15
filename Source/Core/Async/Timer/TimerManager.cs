﻿using System;
using LiteFramework.Core.Base;
using UnityEngine;

namespace LiteFramework.Core.Async.Timer
{
    public static class TimerManager
    {
        private static readonly ListEx<TimerEntity> TimerList_ = new ListEx<TimerEntity>();

        public static bool Startup()
        {
            TimerList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            foreach (var Entity in TimerList_)
            {
                Entity.Dispose();
            }
            TimerList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            TimerList_.Foreach((Entity, Time) =>
            {
                Entity.Tick(Time);

                if (Entity.IsEnd)
                {
                    Entity.Dispose();
                    TimerList_.Remove(Entity);
                }
            }, DeltaTime);
        }

        public static TimerEntity AddTimer(float Interval, Action OnTick, int Count = -1)
        {
            var NewTimer = new TimerEntity(Interval, Count);
            NewTimer.OnTick += OnTick;
            TimerList_.Add(NewTimer);
            return NewTimer;
        }

        public static TimerEntity AddTimer(float Interval, Action OnTick, float TotalTime)
        {
            Interval = Mathf.Max(Interval, 0.0001f);
            return AddTimer(Interval, OnTick, (int)(TotalTime / Interval));
        }

        public static TimerEntity AddTimer(float Interval, Action OnTick, Action OnEnd, int Count = -1)
        {
            var NewTimer = new TimerEntity(Interval, Count);
            NewTimer.OnTick += OnTick;
            NewTimer.OnEnd += OnEnd;
            TimerList_.Add(NewTimer);
            return NewTimer;
        }

        public static TimerEntity AddTimer(float Interval, Action OnTick, Action OnEnd, float TotalTime)
        {
            Interval = Mathf.Max(Interval, 0.0001f);
            return AddTimer(Interval, OnTick, OnEnd, (int)(TotalTime / Interval));
        }

        public static TimerEntity AddTimerWithFrame(int FrameCount, Action OnTick)
        {
            return AddTimer(FrameCount * 0.016f, OnTick, 1);
        }

        public static void StopTimer(TimerEntity Entity)
        {
            Entity?.Stop();
        }

        public static void StopTimer(uint ID)
        {
            foreach (var Entity in TimerList_)
            {
                if (Entity.ID == ID)
                {
                    StopTimer(Entity);
                    return;
                }
            }
        }
    }
}