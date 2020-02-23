using LiteFramework.Game.Base;
using LiteFramework.Game.EventSystem;
using UnityEngine;
using UnityEngine.Events;

namespace LiteFramework.Helper
{
    public static class EventHelper
    {
        public static void AddEvent(Transform Obj, LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            EventSystemListener.AddCallback(Obj, Type, Callback);
        }

        public static void AddEvent(GameEntity Entity, LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            AddEvent(Entity?.GetTransform(), Callback, Type);
        }

        public static void AddEvent(Transform Obj, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            EventSystemListener.AddCallback(Obj, Type, Callback);
        }

        public static void AddEvent(GameEntity Entity, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            AddEvent(Entity?.GetTransform(), Callback, Type);
        }

        public static void RemoveEvent(Transform Obj, LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            EventSystemListener.RemoveCallback(Obj, Type, Callback);
        }

        public static void RemoveEvent(GameEntity Entity, LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            RemoveEvent(Entity?.GetTransform(), Callback, Type);
        }

        public static void RemoveEvent(Transform Obj, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            EventSystemListener.RemoveCallback(Obj, Type, Callback);
        }

        public static void RemoveEvent(GameEntity Entity, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            RemoveEvent(Entity?.GetTransform(), Callback, Type);
        }

        public static void RemoveEvent(Transform Obj, EventSystemType Type = EventSystemType.Click)
        {
            if (Obj == null)
            {
                return;
            }

            EventSystemListener.ClearCallback(Obj, Type);
        }

        public static void RemoveEvent(GameEntity Entity, EventSystemType Type = EventSystemType.Click)
        {
            RemoveEvent(Entity?.GetTransform(), Type);
        }

        public static void ClearEvent(Transform Obj, EventSystemType Type)
        {
            EventSystemListener.ClearCallback(Obj, Type);
        }

        public static void ClearEvent(GameEntity Entity, EventSystemType Type)
        {
            ClearEvent(Entity?.GetTransform(), Type);
        }

        public static void AddEventToChild(Transform Parent, string ChildPath, LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            var Obj = Parent?.Find(ChildPath);
            if (Obj != null)
            {
                EventSystemListener.AddCallback(Obj, Type, Callback);
            }
        }

        public static void AddEventToChild(GameEntity Entity, string ChildPath, LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            AddEventToChild(Entity?.GetTransform(), ChildPath, Callback, Type);
        }

        public static void AddEventToChild(Transform Parent, string ChildPath, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            var Obj = Parent?.Find(ChildPath);
            if (Obj != null)
            {
                EventSystemListener.AddCallback(Obj, Type, Callback);
            }
        }

        public static void AddEventToChild(GameEntity Entity, string ChildPath, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            AddEventToChild(Entity?.GetTransform(), ChildPath, Callback, Type);
        }

        public static void RemoveEventFromChild(Transform Parent, string ChildPath, LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            var Obj = Parent?.Find(ChildPath);
            if (Obj != null)
            {
                EventSystemListener.RemoveCallback(Obj, Type, Callback);
            }
        }

        public static void RemoveEventFromChild(GameEntity Entity, string ChildPath, LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            RemoveEventFromChild(Entity?.GetTransform(), ChildPath, Callback, Type);
        }

        public static void RemoveEventFromChild(Transform Parent, string ChildPath, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            var Obj = Parent?.Find(ChildPath);
            if (Obj != null)
            {
                EventSystemListener.RemoveCallback(Obj, Type, Callback);
            }
        }

        public static void RemoveEventFromChild(GameEntity Entity, string ChildPath, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            RemoveEventFromChild(Entity?.GetTransform(), ChildPath, Callback, Type);
        }

        public static void ClearEventFromChild(Transform Parent, string ChildPath, EventSystemType Type)
        {
            var Obj = Parent?.Find(ChildPath);
            if (Obj != null)
            {
                EventSystemListener.ClearCallback(Obj, Type);
            }
        }

        public static void ClearEventFromChild(GameEntity Entity, string ChildPath, EventSystemType Type)
        {
            ClearEventFromChild(Entity?.GetTransform(), ChildPath, Type);
        }

        public static void RemoveAllEvent(Transform Parent, bool Recursively)
        {
            if (Parent == null)
            {
                return;
            }

            EventSystemListener.ClearCallback(Parent);

            if (!Recursively)
            {
                return;
            }

            var ChildCount = Parent.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = Parent.GetChild(Index);
                RemoveAllEvent(Child, Recursively);
            }
        }

        public static void RemoveAllEvent(GameEntity Entity, bool Recursively)
        {
            RemoveAllEvent(Entity?.GetTransform(), Recursively);
        }
    }
}