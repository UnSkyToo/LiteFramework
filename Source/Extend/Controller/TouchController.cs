using LiteFramework.Game.Base;
using LiteFramework.Game.EventSystem;
using LiteFramework.Helper;
using UnityEngine;

namespace LiteFramework.Extend.Controller
{
    public class TouchController : System.IDisposable
    {
        public bool Enabled { get; set; } = true;
        public float DoubleClickInterval { get; set; } = 0.3f;
        public float LongPressedInterval { get; set; } = 1.0f;

        public event LiteAction<Vector2> PointerDown;
        public event LiteAction<Vector2> Drag;
        public event LiteAction<Vector2> PointerUp;
        public event LiteAction PointerClick;
        public event LiteAction PointerDoubleClick;
        public event LiteAction PointerLongPressed; 

        private readonly Transform Master_;
        private bool IsDrag_;
        private bool IsClick_;
        private float PreviousClickTime_;
        private float PreviousDownTime_;

        public TouchController(Transform Master)
        {
            Master_ = Master;

            EventHelper.AddEvent(Master_, OnPointerDown, EventSystemType.Down);
            EventHelper.AddEvent(Master_, OnDrag, EventSystemType.Drag);
            EventHelper.AddEvent(Master_, OnPointerUp, EventSystemType.Up);
            EventHelper.AddEvent(Master_, OnPointerClick, EventSystemType.Click);
        }

        public TouchController(GameEntity Master)
            : this(Master.GetTransform())
        {
        }

        public void Dispose()
        {
            EventHelper.RemoveEvent(Master_, OnPointerDown, EventSystemType.Down);
            EventHelper.RemoveEvent(Master_, OnDrag, EventSystemType.Drag);
            EventHelper.RemoveEvent(Master_, OnPointerUp, EventSystemType.Up);
            EventHelper.RemoveEvent(Master_, OnPointerClick, EventSystemType.Click);
        }

        private void OnPointerDown(EventSystemData Data)
        {
            if (!Enabled)
            {
                return;
            }

            PointerDown?.Invoke(Data.Location);
            IsDrag_ = true;
            IsClick_ = true;
            PreviousDownTime_ = Time.realtimeSinceStartup;
        }

        private void OnDrag(EventSystemData Data)
        {
            if (!IsDrag_)
            {
                return;
            }

            IsClick_ = false;
            Drag?.Invoke(Data.Location);
        }

        private void OnPointerUp(EventSystemData Data)
        {
            if (!IsDrag_)
            {
                return;
            }

            PointerUp?.Invoke(Data.Location);
            CheckLongPressed();
            IsDrag_ = false;
            PreviousDownTime_ = 0;
        }

        private void OnPointerClick(EventSystemData Data)
        {
            if (!IsClick_)
            {
                return;
            }

            PointerClick?.Invoke();
            CheckDoubleClick();
        }

        private void CheckDoubleClick()
        {
            var CurrentClickTime = Time.realtimeSinceStartup;
            if (CurrentClickTime - PreviousClickTime_ <= DoubleClickInterval)
            {
                PointerDoubleClick?.Invoke();
            }
            PreviousClickTime_ = CurrentClickTime;
        }

        private void CheckLongPressed()
        {
            var CurrentClickTime = Time.realtimeSinceStartup;
            if (CurrentClickTime - PreviousDownTime_ <= LongPressedInterval)
            {
                IsClick_ = false;
                PointerLongPressed?.Invoke();
            }
        }
    }
}