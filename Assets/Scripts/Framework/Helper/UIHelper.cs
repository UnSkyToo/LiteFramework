using System;
using Lite.Framework.Base;
using UnityEngine;

namespace Lite.Framework.Helper
{
    public static class UIHelper
    {
        public static Transform FindChild(Transform Parent, string ChildPath)
        {
            var Paths = ChildPath.Split('/');
            var Index = 0;
            var Current = Parent;

            while (Index < Paths.Length)
            {
                var Name = Paths[Index];
                var Child = Current.Find(Name);

                if (Child == null)
                {
                    return null;
                }

                Current = Child;
                Index++;
            }

            return Current;
        }

        public static T FindComponent<T>(Transform Parent, string ChildPath)
        {
            var Obj = FindChild(Parent, ChildPath);

            if (Obj != null)
            {
                return Obj.GetComponent<T>();
            }

            return default(T);
        }

        public static void AddEvent(Transform Obj, Action<GameObject> Callback, UIEventType Type = UIEventType.Click)
        {
            UIEventTriggerListener.Get(Obj).AddCallback(Type, Callback);
        }

        public static void RemoveEvent(Transform Obj, Action<GameObject> Callback, UIEventType Type = UIEventType.Click)
        {
            UIEventTriggerListener.Get(Obj).RemoveCallback(Type, Callback);
        }

        public static void AddEventToChild(Transform Parent, string ChildPath, Action<GameObject> Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventTriggerListener.Get(Obj).AddCallback(Type, Callback);
            }
        }

        public static void RemoveEventFromChild(Transform Parent, string ChildPath, Action<GameObject> Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventTriggerListener.Get(Obj).RemoveCallback(Type, Callback);
            }
        }

        public static void AddEvent(Transform Obj, Action Callback, UIEventType Type = UIEventType.Click)
        {
            UIEventTriggerListener.Get(Obj).AddCallback(Type, Callback);
        }

        public static void RemoveEvent(Transform Obj, Action Callback, UIEventType Type = UIEventType.Click)
        {
            UIEventTriggerListener.Get(Obj).RemoveCallback(Type, Callback);
        }

        public static void AddEventToChild(Transform Parent, string ChildPath, Action Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventTriggerListener.Get(Obj).AddCallback(Type, Callback);
            }
        }

        public static void ShowChild(Transform Parent, string ChildPath)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                Obj.gameObject.SetActive(true);
            }
        }

        public static void HideChild(Transform Parent, string ChildPath)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                Obj.gameObject.SetActive(false);
            }
        }

        public static void EnableTouched(Transform Target, bool Enabled)
        {
            var Listener = Target.GetComponent<UIEventTriggerListener>();
            if (Listener)
            {
                Listener.enabled = Enabled;
            }
        }

        public static void EnableTouched(Transform Parent, string ChildPath, bool Enabled)
        {
            var Listener = FindComponent<UIEventTriggerListener>(Parent, ChildPath);
            if (Listener)
            {
                Listener.enabled = Enabled;
            }
        }

        public static void RemoveEventFromChild(Transform Parent, string ChildPath, Action Callback, UIEventType Type = UIEventType.Click)
        {
            var Obj = FindChild(Parent, ChildPath);
            if (Obj != null)
            {
                UIEventTriggerListener.Get(Obj).RemoveCallback(Type, Callback);
            }
        }

        public static void RemoveAllEvent(Transform Parent, bool Recursively)
        {
            UIEventTriggerListener.Remove(Parent);

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

        public static void RemoveAllChildren(Transform Parent)
        {
            if (Parent != null)
            {
                var ChildCount = Parent.childCount;

                for (var Index = 0; Index < ChildCount; ++Index)
                {
                    UnityEngine.Object.Destroy(Parent.GetChild(Index).gameObject);
                }
            }
        }

        public static void HideAllChildren(Transform Parent)
        {
            if (Parent != null)
            {
                var ChildCount = Parent.childCount;

                for (var Index = 0; Index < ChildCount; ++Index)
                {
                    Parent.GetChild(Index).gameObject.SetActive(false);
                }
            }
        }
    }
}