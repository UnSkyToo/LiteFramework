namespace LiteFramework.Core.Motion
{
    public class SetAlphaMotion : BaseMotion
    {
        private readonly float Alpha_;
        private MotionAlphaBox AlphaBox_;

        public SetAlphaMotion(float Alpha)
        {
            Alpha_ = Alpha;
            IsEnd = true;
        }

        public override void Enter()
        {
            AlphaBox_ = new MotionAlphaBox(Master);
            AlphaBox_.SetAlpha(Alpha_);
        }

        public override void Exit()
        {
        }

        public override void Tick(float DeltaTime)
        {
        }
    }
}