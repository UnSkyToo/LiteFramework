using System;
using System.Collections;
using System.Collections.Generic;
using Lite.Framework.Base;

namespace Lite.Framework.Manager
{
    public class Task
    {
        public uint ID { get; }
        public bool IsPause { get; set; }
        public bool IsEnd { get; private set; }

        private readonly IEnumerator TaskEntity_;
        private readonly Action Callback_;

        public Task(IEnumerator Entity, Action Callback)
        {
            ID = IDGenerator.Get();
            IsPause = false;
            IsEnd = false;

            TaskEntity_ = Entity;
            Callback_ = Callback;
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

    public static class TaskManager
    {
        private static readonly List<Task> TaskList_ = new List<Task>();

        public static bool Startup()
        {
            TaskList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            TaskList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            for (var Index = 0; Index < TaskList_.Count;)
            {
                if (TaskList_[Index].IsEnd)
                {
                    TaskList_.RemoveAt(Index);
                }
                else
                {
                    Index++;
                }
            }
        }

        public static Task AddTask(IEnumerator TaskFunc, Action Callback = null)
        {
            var NewTask = new Task(TaskFunc, Callback);
            TaskList_.Add(NewTask);
            LiteEngine.MonoBehaviourInstance.StartCoroutine(NewTask.Execute());
            return NewTask;
        }

        public static IEnumerator WaitTask(IEnumerator TaskFunc, Action Callback = null)
        {
            var NewTask = new Task(TaskFunc, Callback);
            TaskList_.Add(NewTask);
            yield return LiteEngine.MonoBehaviourInstance.StartCoroutine(NewTask.Execute());
        }
    }
}