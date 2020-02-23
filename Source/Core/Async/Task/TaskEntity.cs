using System.Collections;
using LiteFramework.Core.Base;

namespace LiteFramework.Core.Async.Task
{
    public class TaskEntity : BaseObject, System.IDisposable
    {
        public bool IsPause { get; set; }
        public bool IsEnd { get; private set; }

        private readonly IEnumerator TaskEntity_;
        private LiteAction Callback_;

        public TaskEntity(IEnumerator Entity, LiteAction Callback)
            : base()
        {
            IsPause = false;
            IsEnd = false;

            TaskEntity_ = Entity;
            Callback_ = Callback;
        }

        public void Dispose()
        {
            Callback_ = null;
        }

        public void Start()
        {
            IsPause = false;
        }

        public void Pause()
        {
            IsPause = true;
        }

        public void Stop()
        {
            Pause();
            IsEnd = true;
        }

        public IEnumerator Execute()
        {
            while (!IsEnd)
            {
                if (IsPause)
                {
                    yield return null;
                }
                else if (TaskEntity_ != null && TaskEntity_.MoveNext())
                {
                    yield return TaskEntity_.Current;
                }
                else
                {
                    IsEnd = true;
                }
            }

            Callback_?.Invoke();
        }
    }

    public class MainThreadTaskEntity : BaseObject
    {
        private readonly LiteAction<object> Func_;
        private readonly object Param_;

        public MainThreadTaskEntity(LiteAction<object> Func, object Param)
        {
            this.Func_ = Func;
            this.Param_ = Param;
        }

        public void Invoke()
        {
            Func_?.Invoke(Param_);
        }
    }
}