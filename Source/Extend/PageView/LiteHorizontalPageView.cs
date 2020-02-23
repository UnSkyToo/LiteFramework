#if UNITY_EDITOR
using UnityEditor;
#endif
using LiteFramework.Extend.LoopView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LiteFramework.Extend.PageView
{
    [AddComponentMenu("UI/Lite/Horizontal PageView")]
    public class LiteHorizontalPageView : LiteHorizontalLoopView
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/Lite/Horizontal PageView")]
        private static void CreatePageView()
        {
            var Root = new GameObject("PageViewH").AddComponent<RectTransform>();
            Root.transform.SetParent(Selection.activeTransform, false);

            var Viewport = new GameObject("Viewport").AddComponent<RectTransform>();
            Viewport.SetParent(Root.transform, false);
            Viewport.anchorMin = Vector2.zero;
            Viewport.anchorMax = Vector2.one;
            Viewport.pivot = Vector2.zero;
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
            Content.anchorMin = Vector2.zero;
            Content.anchorMax = new Vector2(0, 1);
            Content.pivot = new Vector2(0, 1);
            Content.rotation = Quaternion.identity;
            Content.localScale = Vector3.one;
            Content.anchoredPosition = Vector2.zero;
            Content.sizeDelta = Vector2.zero;

            var Script = Root.gameObject.AddComponent<LiteHorizontalPageView>();
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

            var Rate = (Content.anchoredPosition.x + ItemOffset) / (ItemSize.x + ItemSpace);
            var Index = Mathf.Abs((int)Rate);
            var SubRate = Rate + Index;

            if (SubRate > 0)
            {
                return;
            }

            var Dir = Content.anchoredPosition.x - ContentStartPosition_.x;
            if (Dir < 0)
            {
                if (SubRate > -0.5f)
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
                if (SubRate < -0.5f)
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

                var Offset = InternalCalculatePositionWithIndex(TargetIndex_).x;
                var AnchoredPositionX = Mathf.SmoothDamp(Content.anchoredPosition.x,
                    Offset, ref CurrentVelocity_,
                    Elasticity, float.PositiveInfinity, Time.unscaledDeltaTime);
                if (Mathf.Abs(AnchoredPositionX - Offset) < 5)
                {
                    OnPageIndexChanged?.Invoke(TargetIndex_);
                    AnchoredPositionX = Offset;
                    TargetIndex_ = -1;
                }

                SetContentAnchoredPosition(new Vector2(AnchoredPositionX, Content.anchoredPosition.y));

                PrevPosition_ = Content.anchoredPosition;
            }
            else
            {
                base.LateUpdate();
            }
        }
    }
}