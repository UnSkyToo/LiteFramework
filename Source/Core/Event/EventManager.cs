﻿using System;
using System.Collections.Generic;
using LiteFramework.Core.Log;

namespace LiteFramework.Core.Event
{
    public static class EventManager
    {
        //public static event Action<BaseEvent> OnSend; 

        private abstract class EventListener
        {
            public abstract void Trigger(BaseEvent Msg);

#if UNITY_EDITOR
            public abstract void Check();
#endif
        }

        private class EventListenerImpl<T> : EventListener where T : BaseEvent
        {
            public event Action<T> OnEvent = null;

            public override void Trigger(BaseEvent Msg)
            {
                OnEvent?.Invoke((T)Msg);
            }

#if UNITY_EDITOR
            public override void Check()
            {
                if (OnEvent != null)
                {
                    var CallbackList = OnEvent.GetInvocationList();

                    foreach (var Callback in CallbackList)
                    {
                        LLogger.LWarning($"{Callback.Method.ReflectedType.Name} : {Callback.Method.Name} UnRegister");
                    }
                }
            }
#endif
        }

        private static readonly Dictionary<string, EventListener> EventList_ = new Dictionary<string, EventListener>();

        public static bool Startup()
        {
            EventList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
#if UNITY_EDITOR
            foreach (var EventEntity in EventList_)
            {
                EventEntity.Value.Check();
            }
#endif

            //OnSend = null;
        }

        private static string GetEventName<T>()
        {
            return typeof(T).Name;
        }

        public static void Send<T>(T Event) where T : BaseEvent
        {
            if (EventList_.ContainsKey(Event.EventName))
            {
                ((EventListenerImpl<T>)EventList_[Event.EventName]).Trigger(Event);
                //OnSend?.Invoke(Event);
            }
        }

        public static void Send<T>() where T : BaseEvent, new()
        {
            var Event = new T();
            Send(Event);
        }

        public static void Register<T>(Action<T> Callback) where T : BaseEvent
        {
            var EventName = GetEventName<T>();
            if (!EventList_.ContainsKey(EventName))
            {
                EventList_.Add(EventName, new EventListenerImpl<T>());
            }

            ((EventListenerImpl<T>)EventList_[EventName]).OnEvent += Callback;
        }

        public static void UnRegister<T>(Action<T> Callback) where T : BaseEvent
        {
            var EventName = GetEventName<T>();
            if (EventList_.ContainsKey(EventName))
            {
                ((EventListenerImpl<T>)EventList_[EventName]).OnEvent -= Callback;
            }
        }
    }
}