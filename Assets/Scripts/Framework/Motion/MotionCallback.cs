using System;
using UnityEngine;

namespace Lite.Framework.Motion
{
    public class MotionCallback : MotionBase
    {
        private readonly Action<Transform> Callback_;

        public MotionCallback(Action<Transform> Callback)
            : base()
        {
            Callback_ = Callback;
            IsEnd = true;
        }

        public override void Enter()
        {
            Callback_?.Invoke(Master);
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}