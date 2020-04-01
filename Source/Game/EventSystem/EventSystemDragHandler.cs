﻿using UnityEngine.EventSystems;

namespace LiteFramework.Game.EventSystem
{
    public class EventSystemDragHandler : EventSystemBaseHandler, IDragHandler
    {
        public void OnDrag(PointerEventData EventData)
        {
            // 穿透问题
            // if (EventData.rawPointerPress != null && gameObject != EventData.rawPointerPress)
            // {
            //     return;
            // }

            EventCallback_?.Invoke(new EventSystemData(EventData));
            EventCallbackEx_?.Invoke();
        }
    }
}