using UnityEngine;

namespace LiteFramework.Extend
{
    public static class ComponentEx
    {
        public static void Reset(this Transform Master)
        {
            Master.localPosition = Vector3.zero;
            Master.localScale = Vector3.one;
            Master.localRotation = Quaternion.identity;
        }
    }
}