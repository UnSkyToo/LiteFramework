using System;
using LiteFramework.Game.EventSystem;

namespace LiteFramework.Game.UI
{
    public abstract class LiteUIAttribute : Attribute
    {
        public string Path { get; }

        protected LiteUIAttribute(string Path)
        {
            this.Path = Path;
        }
    }

    public class LiteUINodeAttribute : LiteUIAttribute
    {
        public LiteUINodeAttribute(string Path)
            : base(Path)
        {
        }
    }

    public class LiteUIComponentAttribute : LiteUIAttribute
    {
        public LiteUIComponentAttribute(string Path)
            : base(Path)
        {
        }
    }

    public class LiteUIEventAttribute : LiteUIAttribute
    {
        public EventSystemType EventType { get; private set; }

        public LiteUIEventAttribute(string Path, EventSystemType EventType = EventSystemType.Click)
            : base(Path)
        {
            this.EventType = EventType;
        }
    }
}