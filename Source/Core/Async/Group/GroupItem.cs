using LiteFramework.Core.Async.Timer;
using LiteFramework.Core.Base;

namespace LiteFramework.Core.Async.Group
{
    public class GroupItem : BaseObject, System.IDisposable
    {
        public bool IsEnd { get; private set; }
        protected readonly GroupEntity Master_;
        protected LiteAction<GroupItem> Func_;

        public GroupItem(GroupEntity Master, LiteAction<GroupItem> Func)
            : base()
        {
            Master_ = Master;
            Func_ = Func;

            IsEnd = false;
        }

        public virtual void Dispose()
        {
            Func_ = null;
        }

        public virtual void Execute()
        {
            Func_?.Invoke(this);
        }

        public void Done()
        {
            Master_?.ItemDone();
        }
    }

    public class GroupParamItem<T> : GroupItem
    {
        private readonly LiteAction<GroupItem, T> NewFunc_;
        private readonly T Param_;

        public GroupParamItem(GroupEntity Master, LiteAction<GroupItem, T> Func, T Param)
            : base(Master, null)
        {
            NewFunc_ = Func;
            Param_ = Param;
        }

        public override void Execute()
        {
            NewFunc_?.Invoke(this, Param_);
        }
    }

    public class GroupWaitTime : GroupItem
    {
        private readonly float WaitTime_;
        private TimerEntity Timer_;

        public GroupWaitTime(GroupEntity Master, float WaitTime)
            : base(Master, null)
        {
            WaitTime_ = WaitTime;
        }

        public override void Dispose()
        {
            TimerManager.StopTimer(Timer_);
            Timer_ = null;

            base.Dispose();
        }

        public override void Execute()
        {
            Timer_ = TimerManager.AddTimer(WaitTime_, Done, 1);
        }
    }

    public class GroupWaitConditional : GroupItem
    {
        private readonly LiteFunc<bool> ConditionFunc_;
        private TimerEntity Timer_;

        public GroupWaitConditional(GroupEntity Master, LiteFunc<bool> ConditionFunc)
            : base(Master, null)
        {
            ConditionFunc_ = ConditionFunc;
        }

        public override void Dispose()
        {
            TimerManager.StopTimer(Timer_);
            Timer_ = null;

            base.Dispose();
        }

        public override void Execute()
        {
            Timer_ = TimerManager.AddTimer(0, TickFunc);
        }

        private void TickFunc()
        {
            if (ConditionFunc_?.Invoke() ?? true)
            {
                Done();
            }
        }
    }
}