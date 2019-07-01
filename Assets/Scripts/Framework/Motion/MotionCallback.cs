using System;
using UnityEngine;

namespace Lite.Framework.Motion
{
    public class MotionCallback : MotionBase
    {
        private readonly Action Callback_;

        public MotionCallback()
            : base()
        {
            IsEnd = true;
        }

        public override void Enter()
        {
            Callback_?.Invoke();
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}