using Lite.Framework.Base;
using Lite.Framework.Manager;
using Lite.Framework.Motion;

namespace Lite.Logic.UI
{
    public class LogoUI : UIBase
    {
        public LogoUI()
            : base()
        {
        }

        protected override void OnOpen(params object[] Params)
        {
            UITransform.ExecuteMotion(new MotionRepeatSequence(new MotionFadeOut(1), new MotionFadeIn(1)));
        }

        protected override void OnClose()
        {
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnTick(float DeltaTime)
        {
        }
    }
}