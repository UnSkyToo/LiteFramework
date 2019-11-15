using EasyButtons;
using UnityEngine;
using UnityEngine.UI;

namespace LiteFramework.Extend
{
    public class UGUIEx : MonoBehaviour
    {
        [Button("Remove Self", ButtonMode.DisabledInPlayMode, ButtonSpacing.After)]
        private void OnRemoveSelf()
        {
            DestroyImmediate(this);
        }

        [Button("Add ContentSize Filter", ButtonMode.DisabledInPlayMode, ButtonSpacing.After)]
        private void OnAddFilter()
        {
            if (gameObject.GetComponent<ContentSizeFitter>() != null)
            {
                return;
            }

            var Filter = gameObject.AddComponent<ContentSizeFitter>();
            Filter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            Filter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        [Button("Remove ContentSize Filter", ButtonMode.DisabledInPlayMode, ButtonSpacing.After)]
        private void OnRemoveFilter()
        {
            var Filter = gameObject.GetComponent<ContentSizeFitter>();
            if (Filter != null)
            {
                DestroyImmediate(Filter);
            }
        }
    }
}