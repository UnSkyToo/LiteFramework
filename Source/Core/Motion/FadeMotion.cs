﻿using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class FadeMotion : BaseMotion
    {
        private readonly float TotalTime_;
        private readonly float BeginAlpha_;
        private readonly float EndAlpha_;
        private float CurrentTime_;

        private MotionAlphaBox AlphaBox_;

        public FadeMotion(float Time, float BeginAlpha, float EndAlpha)
            : base()
        {
            TotalTime_ = Time;
            BeginAlpha_ = BeginAlpha;
            EndAlpha_ = EndAlpha;
            CurrentTime_ = 0;
        }

        public override void Enter()
        {
            IsEnd = false;
            CurrentTime_ = 0;
            AlphaBox_ = new MotionAlphaBox(Master);
            AlphaBox_.SetAlpha(BeginAlpha_);
        }

        public override void Tick(float DeltaTime)
        {
            CurrentTime_ += DeltaTime;
            var T = CurrentTime_ / TotalTime_;
            if (T >= 1.0f)
            {
                AlphaBox_.SetAlpha(EndAlpha_);
                IsEnd = true;
                return;
            }

            AlphaBox_.SetAlpha(Mathf.Lerp(BeginAlpha_, EndAlpha_, T));
        }
    }

    public class FadeInMotion : FadeMotion
    {
        public FadeInMotion(float Time)
            : base(Time, 0, 1)
        {
        }
    }

    public class FadeOutMotion : FadeMotion
    {
        public FadeOutMotion(float Time)
            : base(Time, 1, 0)
        {
        }
    }
}