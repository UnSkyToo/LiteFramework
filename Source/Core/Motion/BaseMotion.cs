using LiteFramework.Core.Base;
using UnityEngine;

namespace LiteFramework.Core.Motion
{
    public abstract class BaseMotion : BaseObject
    {
        public Transform Master { get; set; }
        public bool IsEnd { get; protected set; }

        protected BaseMotion()
            : base()
        {
            IsEnd = true;
        }

        public void Stop()
        {
            IsEnd = true;
        }

        public virtual void Enter()
        {
        }

        public virtual void Tick(float DeltaTime)
        {
        }

        public virtual void Exit()
        {
        }
    }
}