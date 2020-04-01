using LiteFramework.Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace LiteFramework.Helper
{
    public static class MaskLayerHelper
    {
        private static Image MaskLayer_ = null;
        private static int RefCount_;

        public static void CreateMaskLayer()
        {
            if (!MaskLayer_)
            {
                var Layer = UIHelper.CreateUIPanel(LiteConfigure.UIRoot, "Mask", (int)UIDepthMode.Top * 10);
                MaskLayer_ = Layer.GetOrAddComponent<Image>();
                MaskLayer_.color = Color.clear;
                RefCount_ = 0;
            }

            MaskLayer_.raycastTarget = true;
            RefCount_++;
        }

        public static void DeleteMaskLayer()
        {
            RefCount_--;

            if (RefCount_ <= 0 && MaskLayer_)
            {
                RefCount_ = 0;
                MaskLayer_.raycastTarget = false;
            }
        }

        public static void DisposeMaskLayer()
        {
            if (MaskLayer_ == null)
            {
                return;
            }

            Object.Destroy(MaskLayer_.gameObject);
            MaskLayer_ = null;
            RefCount_ = 0;
        }
    }
}