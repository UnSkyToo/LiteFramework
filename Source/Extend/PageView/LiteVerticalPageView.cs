#if UNITY_EDITOR
using UnityEditor;
#endif
using LiteFramework.Extend.LoopView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LiteFramework.Extend.PageView
{
    [AddComponentMenu("UI/Lite/Vertical PageView")]
    public class LiteVerticalPageView : LiteVerticalLoopView
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/Lite/Vertical PageView")]
        private static void CreatePageView()
        {
            var Root = new GameObject("PageViewV").AddComponent<RectTransform>();
            Root.transform.SetParent(Selection.activeTransform, false);

            var Viewport = new GameObject("Viewport").AddComponent<RectTransform>();
            Viewport.SetParent(Root.transform, false);
            Viewport.anchorMin = Vector2.zero;
            Viewport.anchorMax = Vector2.one;
            Viewport.pivot = new Vector2(0, 1);
            Viewport.rotation = Quaternion.identity;
            Viewport.localScale = Vector3.one;
            Viewport.anchoredPosition = Vector2.zero;
            Viewport.sizeDelta = Vector2.zero;
            var ViewportMaskImage = Viewport.gameObject.AddComponent<Image>();
            ViewportMaskImage.sprite = Resources.Load<Sprite>("UIMask");
            ViewportMaskImage.type = Image.Type.Sliced;
            ViewportMaskImage.fillCenter = true;
            ViewportMaskImage.raycastTarget = true;
            var ViewportMask = Viewport.gameObject.AddComponent<Mask>();
            ViewportMask.showMaskGraphic = false;

            var Content = new GameObject("Content").AddComponent<RectTransform>();
            Content.SetParent(Viewport, false);
            Content.anchorMin = new Vector2(0, 1);
            Content.anchorMax = Vector2.one;
            Content.pivot = new Vector2(0, 1);
            Content.rotation = Quaternion.identity;
            Content.localScale = Vector3.one;
            Content.anchoredPosition = Vector2.zero;
            Content.sizeDelta = Vector2.zero;

            var Script = Root.gameObject.AddComponent<LiteVerticalPageView>();
            Script.Viewport = Viewport;
            Script.Content = Content;
            Script.TotalCount = 1;

            Selection.activeGameObject = Root.gameObject;
        }
#endif

        public event LiteAction<int> OnPageIndexChanged;

        private int TargetIndex_;
        private float CurrentVelocity_;

        protected override void Awake()
        {
            base.Awake();
            Inertia_ = false;
            TargetIndex_ = -1;
            CurrentVelocity_ = 0;
        }

        public override void OnEndDrag(PointerEventData EventData)
        {
            base.OnEndDrag(EventData);

            var Rate = Content.anchoredPosition.y / (ItemSize.y + ItemSpace);
            var Index = (int)Rate;
            var SubRate = Rate - Index;

            if (SubRate < 0)
            {
                return;
            }

            var Dir = Content.anchoredPosition.y - ContentStartPosition_.y;
            if (Dir < 0)
            {
                if (SubRate < 0.5f)
                {
                    SetTargetIndex(Index);
                }
                else
                {
                    SetTargetIndex(Index + 1);
                }
            }
            else if (Dir > 0)
            {
                if (SubRate > 0.5f)
                {
                    SetTargetIndex(Index + 1);
                }
                else
                {
                    SetTargetIndex(Index);
                }
            }
        }
        public void SetTargetIndex(int Index)
        {
            TargetIndex_ = Index;
        }

        public int GetCurrentIndex()
        {
            return CurrentIndex_;
        }

        protected override void FillItemList()
        {
            base.FillItemList();
            OnPageIndexChanged?.Invoke(CurrentIndex_);
        }

        protected override void LateUpdate()
        {
            if (!IsActive())
            {
                return;
            }

            if (!Dragging_ && TargetIndex_ != -1)
            {
                UpdateBounds();

                var Offset = InternalCalculatePositionWithIndex(TargetIndex_).y;
                var AnchoredPositionY = Mathf.SmoothDamp(Content.anchoredPosition.y,
                    Offset, ref CurrentVelocity_,
                    Elasticity, float.PositiveInfinity, Time.unscaledDeltaTime);
                if (Mathf.Abs(AnchoredPositionY - Offset) < 5)
                {
                    OnPageIndexChanged?.Invoke(TargetIndex_);
                    AnchoredPositionY = Offset;
                    TargetIndex_ = -1;
                }

                SetContentAnchoredPosition(new Vector2(Content.anchoredPosition.x, AnchoredPositionY));

                PrevPosition_ = Content.anchoredPosition;
            }
            else
            {
                base.LateUpdate();
            }
        }
    }
}