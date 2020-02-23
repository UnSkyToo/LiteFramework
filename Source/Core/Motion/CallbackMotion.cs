namespace LiteFramework.Core.Motion
{
    public class CallbackMotion : BaseMotion
    {
        private readonly LiteAction Callback_;

        public CallbackMotion(LiteAction Callback)
            : base()
        {
            Callback_ = Callback;
        }

        public override void Enter()
        {
            Callback_?.Invoke();
        }
    }

    public class CallbackMotion<T> : BaseMotion
    {
        private readonly LiteAction<T> Callback_;
        private readonly T Param_;

        public CallbackMotion(LiteAction<T> Callback, T Param)
            : base()
        {
            Callback_ = Callback;
            Param_ = Param;
        }

        public override void Enter()
        {
            Callback_?.Invoke(Param_);
        }
    }

    public class CallbackMotion<T1, T2> : BaseMotion
    {
        private readonly LiteAction<T1, T2> Callback_;
        private readonly T1 Param1_;
        private readonly T2 Param2_;

        public CallbackMotion(LiteAction<T1, T2> Callback, T1 Param1, T2 Param2)
            : base()
        {
            Callback_ = Callback;
            Param1_ = Param1;
            Param2_ = Param2;
        }

        public override void Enter()
        {
            Callback_?.Invoke(Param1_, Param2_);
        }
    }
}