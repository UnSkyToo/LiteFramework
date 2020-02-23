namespace LiteFramework.Core.Motion
{
    public class WaitTimeMotion : BaseMotion
    {
        private readonly float TotalTime_;
        private float CurrentTime_;

        public WaitTimeMotion(float Time)
            : base()
        {
            TotalTime_ = Time;
        }

        public override void Enter()
        {
            CurrentTime_ = TotalTime_;
            IsEnd = false;
        }

        public override void Tick(float DeltaTime)
        {
            CurrentTime_ -= DeltaTime;

            if (CurrentTime_ <= 0.0f)
            {
                IsEnd = true;
            }
        }
    }

    public class WaitConditionalMotion : BaseMotion
    {
        private readonly LiteFunc<bool> ConditionFunc_;

        public WaitConditionalMotion(LiteFunc<bool> ConditionFunc)
            : base()
        {
            ConditionFunc_ = ConditionFunc;
        }

        public override void Enter()
        {
            IsEnd = false;
        }

        public override void Tick(float DeltaTime)
        {
            IsEnd = ConditionFunc_?.Invoke() ?? true;
        }
    }
}