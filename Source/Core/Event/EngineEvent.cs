using System;

namespace LiteFramework.Core.Event
{
    public sealed class EnterForegroundEvent : BaseEvent
    {
        public EnterForegroundEvent()
            : base()
        {
        }
    }

    public sealed class EnterBackgroundEvent : BaseEvent
    {
        public EnterBackgroundEvent()
            : base()
        {
        }
    }

    public sealed class OpenUIEvent : BaseEvent
    {
        public Type UIType { get; }

        public OpenUIEvent(Type UIType)
        {
            this.UIType = UIType;
        }
    }

    public sealed class CloseUIEvent : BaseEvent
    {
        public Type UIType { get; }

        public CloseUIEvent(Type UIType)
        {
            this.UIType = UIType;
        }
    }
}