using UnityEngine;
using UnityEngine.UI;

namespace Lite.Framework.Motion
{
    public class MotionFade : MotionBase
    {
        private readonly float TotalTime_;
        private float CurrentTime_;
        private float BeginAlpha_;
        private float EndAlpha_;
        private bool IsRelative_;

        private Graphic UIGraphic_;
        private Color Color_;

        public MotionFade(float Time, float Alpha, bool IsRelative)
            : base()
        {
            TotalTime_ = Time;
            BeginAlpha_ = Alpha;
            EndAlpha_ = Alpha;
            IsRelative_ = IsRelative;
        }

        public override void Enter()
        {
            IsEnd = false;
            CurrentTime_ = 0;
            UIGraphic_ = Master.GetComponent<Graphic>();
            if (UIGraphic_ == null)
            {
                IsEnd = true;
                return;
            }

            Color_ = new Color(UIGraphic_.color.r, UIGraphic_.color.g, UIGraphic_.color.b, UIGraphic_.color.a);
            BeginAlpha_ = Color_.a;

            if (IsRelative_)
            {
                EndAlpha_ += BeginAlpha_;
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
                Color_.a = EndAlpha_;
                UIGraphic_.color = Color_;
                IsEnd = true;
            }

            Color_.a = Mathf.Lerp(BeginAlpha_, EndAlpha_, T);
            UIGraphic_.color = Color_;
        }
    }

    public class MotionFadeIn : MotionFade
    {
        public MotionFadeIn(float Time)
            : base(Time, 1.0f, false)
        {
        }
    }

    public class MotionFadeOut : MotionFade
    {
        public MotionFadeOut(float Time)
            : base(Time, 0.0f, false)
        {
        }
    }
}