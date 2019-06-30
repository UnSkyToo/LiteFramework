﻿using Lite.Framework.Base;
using UnityEngine;

namespace Lite.Framework.Motion
{
    public abstract class MotionBase
    {
        public uint ID { get; }
        public Transform Master { get; set; }
        public bool IsEnd { get; protected set; }

        protected MotionBase()
        {
            ID = IDGenerator.Get();
            IsEnd = true;
        }

        public void Stop()
        {
            IsEnd = true;
        }

        public abstract void Enter();

        public abstract void Tick(float DeltaTime);

        public abstract void Exit();
    }
}