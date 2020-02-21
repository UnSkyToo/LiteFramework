using UnityEngine;
using LiteFramework.Helper;

namespace LiteFramework.Core.Motion
{
    public class MotionAlphaBox
    {
        private SpriteRenderer Renderer_;
        private Color OriginColor_;
        private CanvasGroup Group_;

        public MotionAlphaBox(Transform Master)
        {
            Renderer_ = Master.GetComponent<SpriteRenderer>();
            if (Renderer_ == null)
            {
                Group_ = Master.GetOrAddComponent<CanvasGroup>();
            }
            else
            {
                OriginColor_ = Renderer_.color;
            }
        }

        public void SetAlpha(float Alpha)
        {
            if (Renderer_ != null)
            {
                OriginColor_.a = Alpha;
                Renderer_.color = OriginColor_;
            }
            else
            {
                Group_.alpha = Alpha;
            }
        }
    }
}