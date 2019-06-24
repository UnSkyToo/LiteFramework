using System;
using UnityEngine.EventSystems;

namespace Lite.Framework.Base
{
    public enum UIEventType
    {
        Click = 0,
        Down = 1,
        Up = 2,
        Count = 3,
    }

    public class UIEventTriggerListener : EventTrigger
    {
        private readonly Action<UnityEngine.GameObject>[] EventCallback_ = new Action<UnityEngine.GameObject>[(int)UIEventType.Count];
        private readonly Action[] EventCallbackWithoutParams_ = new Action[(int)UIEventType.Count];

        public static UIEventTriggerListener Get(UnityEngine.Transform Obj)
        {
            var Listener = Obj.GetComponent<UIEventTriggerListener>();
            if (Listener == null)
            {
                Listener = Obj.gameObject.AddComponent<UIEventTriggerListener>();
            }
            return Listener;
        }

        public static void Remove(UnityEngine.Transform Obj)
        {
            var Listener = Obj.GetComponent<UIEventTriggerListener>();
            if (Listener != null)
            {
                UnityEngine.Object.DestroyImmediate(Listener);
            }
        }

        public void AddCallback(UIEventType Type, Action<UnityEngine.GameObject> Callback)
        {
            EventCallback_[(int)Type] += Callback;
        }

        public void RemoveCallback(UIEventType Type, Action<UnityEngine.GameObject> Callback)
        {
            EventCallback_[(int)Type] -= Callback;
        }

        public void AddCallback(UIEventType Type, Action Callback)
        {
            EventCallbackWithoutParams_[(int)Type] += Callback;
        }

        public void RemoveCallback(UIEventType Type, Action Callback)
        {
            EventCallbackWithoutParams_[(int)Type] -= Callback;
        }

        public override void OnPointerClick(PointerEventData EventData)
        {
            EventCallback_[(int)UIEventType.Click]?.Invoke(EventData.pointerPress);
            EventCallbackWithoutParams_[(int)UIEventType.Click]?.Invoke();

        }

        public override void OnPointerDown(PointerEventData EventData)
        {
            EventCallback_[(int)UIEventType.Down]?.Invoke(EventData.pointerPress);
            EventCallbackWithoutParams_[(int)UIEventType.Down]?.Invoke();
        }

        public override void OnPointerUp(PointerEventData EventData)
        {
            EventCallback_[(int)UIEventType.Up]?.Invoke(EventData.pointerPress);
            EventCallbackWithoutParams_[(int)UIEventType.Up]?.Invoke();
        }
    }
}