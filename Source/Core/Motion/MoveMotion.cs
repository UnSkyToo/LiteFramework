using LiteFramework.Core.BezierCurve;
using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class MoveMotion : BaseMotion
    {
        private readonly bool IsRelative_;
        private readonly float TotalTime_;
        private float CurrentTime_;
        private Vector3 BeginPosition_;
        private Vector3 EndPosition_;
        private Vector3 TargetPosition_;

        public MoveMotion(float Time, Vector3 Position, bool IsRelative)
            : base()
        {
            IsRelative_ = IsRelative;
            TotalTime_ = Time;
            BeginPosition_ = Position;
            EndPosition_ = Position;
        }

        public override void Enter()
        {
            IsEnd = false;
            CurrentTime_ = 0;
            BeginPosition_ = Master.localPosition;
            TargetPosition_ = EndPosition_;

            if (IsRelative_)
            {
                TargetPosition_ += BeginPosition_;
            }
        }

        public override void Tick(float DeltaTime)
        {
            CurrentTime_ += DeltaTime;
            var T = CurrentTime_ / TotalTime_;
            if (T >= 1.0f)
            {
                T = 1.0f;
                IsEnd = true;
            }

            Master.localPosition = Vector3.Lerp(BeginPosition_, TargetPosition_, T);
        }
    }

    public class BezierMoveMotion : BaseMotion
    {
        private readonly bool IsRelative_;
        private readonly float TotalTime_;
        private readonly IBezierCurve BezierCurve_;
        private float CurrentTime_;
        private Vector3 BasePosition_;

        public BezierMoveMotion(float Time, IBezierCurve BezierCurve, bool IsRelative)
            : base()
        {
            IsRelative_ = IsRelative;
            TotalTime_ = Time;
            BezierCurve_ = BezierCurve;
        }

        public override void Enter()
        {
            IsEnd = false;
            CurrentTime_ = 0;
            BasePosition_ = IsRelative_ ? Master.localPosition : Vector3.zero;
        }

        public override void Tick(float DeltaTime)
        {
            CurrentTime_ += DeltaTime;
            var T = CurrentTime_ / TotalTime_;
            if (T >= 1.0f)
            {
                T = 1.0f;
                IsEnd = true;
            }

            Master.localPosition = BasePosition_ + BezierCurve_.Lerp(T);
        }
    }
}