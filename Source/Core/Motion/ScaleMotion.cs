using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class ScaleMotion : BaseMotion
    {
        private readonly bool IsRelative_;
        private readonly float TotalTime_;
        private float CurrentTime_;
        private Vector3 BeginScale_;
        private Vector3 EndScale_;
        private Vector3 TargetScale_;

        public ScaleMotion(float Time, Vector3 Scale, bool IsRelative)
            : base()
        {
            IsRelative_ = IsRelative;
            TotalTime_ = Time;
            BeginScale_ = Scale;
            EndScale_ = Scale;
        }

        public override void Enter()
        {
            IsEnd = false;
            CurrentTime_ = 0;
            BeginScale_ = Master.localScale;
            TargetScale_ = EndScale_;

            if (IsRelative_)
            {
                TargetScale_ += BeginScale_;
            }
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
            CurrentTime_ += DeltaTime;
            var T = CurrentTime_ / TotalTime_;
            if (T >= 1.0f)
            {
                Master.localScale = TargetScale_;
                IsEnd = true;
            }

            Master.localScale = Vector3.Lerp(BeginScale_, TargetScale_, T);
        }
    }
}