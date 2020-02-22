using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public class SetPositionMotion : BaseMotion
    {
        private readonly Vector3 Position_;

        public SetPositionMotion(Vector3 Position)
            : base()
        {
            Position_ = Position;
        }

        public override void Enter()
        {
            Master.localPosition = Position_;
        }
    }

    public class SetRotationMotion : BaseMotion
    {
        private readonly Quaternion Rotation_;

        public SetRotationMotion(Quaternion Rotation)
            : base()
        {
            Rotation_ = Rotation;
        }

        public override void Enter()
        {
            Master.localRotation = Rotation_;
        }
    }

    public class SetScaleMotion : BaseMotion
    {
        private readonly Vector3 Scale_;

        public SetScaleMotion(Vector3 Scale)
            : base()
        {
            Scale_ = Scale;
        }

        public override void Enter()
        {
            Master.localScale = Scale_;
        }
    }

    public class SetAlphaMotion : BaseMotion
    {
        private readonly float Alpha_;
        private MotionAlphaBox AlphaBox_;

        public SetAlphaMotion(float Alpha)
            : base()
        {
            Alpha_ = Alpha;
        }

        public override void Enter()
        {
            AlphaBox_ = new MotionAlphaBox(Master);
            AlphaBox_.SetAlpha(Alpha_);
        }
    }

    public class SetActiveMotion : BaseMotion
    {
        private readonly bool Value_;

        public SetActiveMotion(bool Value)
            : base()
        {
            Value_ = Value;
        }

        public override void Enter()
        {
            Master?.gameObject.SetActive(Value_);
        }
    }
}