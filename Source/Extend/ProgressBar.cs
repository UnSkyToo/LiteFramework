#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace LiteFramework.Extend
{
    public class ProgressBar : MonoBehaviour
    {
        [Range(0, 1)]
        [SerializeField]
        private float Percent_;

        public float Percent
        {
            get => Percent_;
            set
            {
                if (BarImage != null)
                {
                    Percent_ = Mathf.Clamp01(value);

                    if (BarImage.type == Image.Type.Filled)
                    {
                        BarImage.fillAmount = Percent_;
                    }
                    else if (BarImage.type == Image.Type.Sliced)
                    {

                    }
                }
            }
        }

        public Image BgImage;
        public Image BarImage;

#if UNITY_EDITOR
        [MenuItem("GameObject/UI/Lite/ProgressBar")]
        public static void CreateProgressBar()
        {
            var Obj = new GameObject("ProgressBar");
            Obj.transform.SetParent(Selection.activeTransform, false);
            Obj.AddComponent<RectTransform>();

            var PBar = Obj.AddComponent<ProgressBar>();

            var BgImageObj = new GameObject("BgImage");
            BgImageObj.transform.SetParent(Obj.transform, false);
            PBar.BgImage = BgImageObj.AddComponent<Image>();
            PBar.BgImage.raycastTarget = false;
            PBar.BgImage.rectTransform.anchorMin = Vector2.zero;
            PBar.BgImage.rectTransform.anchorMax = Vector2.one;
            PBar.BgImage.rectTransform.anchoredPosition = Vector2.zero;
            PBar.BgImage.rectTransform.sizeDelta = Vector2.zero;

            var BarImageObj = new GameObject("BarImage");
            BarImageObj.transform.SetParent(Obj.transform, false);
            PBar.BarImage = BarImageObj.AddComponent<Image>();
            PBar.BarImage.raycastTarget = false;
            PBar.BarImage.rectTransform.anchorMin = Vector2.zero;
            PBar.BarImage.rectTransform.anchorMax = Vector2.one;
            PBar.BarImage.rectTransform.anchoredPosition = Vector2.zero;
            PBar.BarImage.rectTransform.sizeDelta = Vector2.zero;
            PBar.BarImage.type = Image.Type.Filled;
            PBar.BarImage.fillMethod = Image.FillMethod.Horizontal;
            PBar.BarImage.fillAmount = 1;
        }
#endif
    }
}