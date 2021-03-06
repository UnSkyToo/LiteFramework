﻿using System;
using LiteFramework.Core.Base;
using LiteFramework.Core.Event;
using LiteFramework.Core.Motion;
using LiteFramework.Game.Asset;
using LiteFramework.Game.EventSystem;
using LiteFramework.Helper;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LiteFramework.Game.UI
{
    public abstract class BaseUI : BaseObject
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Cached { get; set; }
        public Transform UITransform { get; set; }
        public RectTransform UIRectTransform { get; set; }
        public int DepthIndex { get; protected set; }
        public UIDepthMode DepthMode { get; protected set; }
        public bool IsClosed { get; private set; }

        private Canvas UICanvas_;
        public Canvas UICanvas
        {
            get
            {
                if (UICanvas_ != null)
                {
                    return UICanvas_;
                }

                UICanvas_ = UnityHelper.GetOrAddComponentSafe<Canvas>(UITransform);
                UICanvas_.overrideSorting = true;
                UnityHelper.GetOrAddComponentSafe<GraphicRaycaster>(UITransform);
                return UICanvas_;
            }
        }

        protected BaseUI(UIDepthMode Mode, int Index)
        {
            Name = GetType().Name;
            Cached = true;
            UITransform = null;
            UIRectTransform = null;
            DepthMode = Mode;
            DepthIndex = Index;
            IsClosed = false;
        }

        public void Open(params object[] Params)
        {
            IsClosed = false;
            // Auto Binder
            UIBinder.AutoBind(this);
            // Auto Binder
            OnOpen(Params);
            Show();
            EventManager.Send(new OpenUIEvent(GetType()));
        }

        public void Close()
        {
            IsClosed = true;
            Hide();
            UIHelper.RemoveAllEvent(UITransform, true);
            OnClose();
            EventManager.Send(new CloseUIEvent(GetType()));
        }

        public virtual void Show()
        {
            if (UITransform != null)
            {
                UITransform.gameObject.SetActive(true);
            }

            OnShow();
        }

        public virtual void Hide()
        {
            if (UITransform != null)
            {
                UITransform.gameObject.SetActive(false);
            }
            
            OnHide();
        }

        public void Tick(float DeltaTime)
        {
            OnTick(DeltaTime);
        }

        public Transform FindChild(string ChildPath)
        {
            return UIHelper.FindChild(UITransform, ChildPath);
        }

        public T GetComponent<T>() where T : Component
        {
            return UITransform.GetComponent<T>();
        }

        public T GetComponent<T>(string ChildPath) where T : Component
        {
            return UIHelper.GetComponent<T>(UITransform, ChildPath);
        }

        public Component GetComponent(string ChildParent, Type CType)
        {
            return UIHelper.GetComponent(UITransform, ChildParent, CType);
        }

        public Component GetComponent(string ChildParent, string CType)
        {
            return UIHelper.GetComponent(UITransform, ChildParent, CType);
        }

        public void AddEvent(LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            UIHelper.AddEvent(UITransform, Callback, Type);
        }

        public void AddEvent(UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            UIHelper.AddEvent(UITransform, Callback, Type);
        }

        public void AddClickEvent(UnityAction Callback, AssetUri AudioUri)
        {
            UIHelper.AddClickEvent(UITransform, Callback, AudioUri);
        }

        public void RemoveEvent(LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            UIHelper.RemoveEvent(UITransform, Callback, Type);
        }

        public void RemoveEvent(UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            UIHelper.RemoveEvent(UITransform, Callback, Type);
        }

        public void AddEventToChild(string ChildPath, LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            UIHelper.AddEventToChild(UITransform, ChildPath, Callback, Type);
        }

        public void AddEventToChild(string ChildPath, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            UIHelper.AddEventToChild(UITransform, ChildPath, Callback, Type);
        }

        public void AddClickEventToChild(string ChildPath, LiteAction<EventSystemData> Callback, AssetUri AudioUri)
        {
            UIHelper.AddClickEventToChild(UITransform, ChildPath, Callback, AudioUri);
        }

        public void AddClickEventToChild(string ChildPath, UnityAction Callback, AssetUri AudioUri)
        {
            UIHelper.AddClickEventToChild(UITransform, ChildPath, Callback, AudioUri);
        }

        public void RemoveEventFromChild(string ChildPath, LiteAction<EventSystemData> Callback, EventSystemType Type = EventSystemType.Click)
        {
            UIHelper.RemoveEventFromChild(UITransform, ChildPath, Callback, Type);
        }

        public void RemoveEventFromChild(string ChildPath, UnityAction Callback, EventSystemType Type = EventSystemType.Click)
        {
            UIHelper.RemoveEventFromChild(UITransform, ChildPath, Callback, Type);
        }

        public void SetActive(bool Value)
        {
            UITransform.gameObject.SetActive(Value);
        }

        public void SetActive(string ChildPath, bool Value)
        {
            UIHelper.SetActive(UITransform, ChildPath, Value);
        }

        public bool IsActive(string ChildPath)
        {
            return UIHelper.IsActive(UITransform, ChildPath);
        }

        public void EnableTouched(bool Enabled)
        {
            UIHelper.EnableTouched(UITransform, Enabled);
        }

        public void EnableTouched(string ChildPath, bool Enabled)
        {
            UIHelper.EnableTouched(UITransform, ChildPath, Enabled);
        }

        public BaseMotion ExecuteMotion(BaseMotion Motion)
        {
            return MotionManager.Execute(UITransform, Motion);
        }

        public BaseMotion ExecuteMotion(string ChildPath, BaseMotion Motion)
        {
            return MotionManager.Execute(FindChild(ChildPath), Motion);
        }

        public void AbandonMotion(BaseMotion Motion)
        {
            MotionManager.Abandon(Motion);
        }

        public void AbandonMotion(string ChildPath)
        {
            MotionManager.Abandon(FindChild(ChildPath));
        }

        public void ReplaceSprite(string ChildPath, bool IsNativeSize, AssetUri Uri)
        {
            UIHelper.ReplaceSprite(GetComponent<Image>(ChildPath), IsNativeSize, Uri);
        }

        protected virtual void OnOpen(params object[] Params)
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnTick(float DeltaTime)
        {
        }
    }
}