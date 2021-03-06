﻿using UnityEngine.EventSystems;

namespace LiteFramework.Game.EventSystem
{
    public class EventSystemBeginDragHandler : EventSystemBaseHandler, IBeginDragHandler
    {
        public void OnBeginDrag(PointerEventData EventData)
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