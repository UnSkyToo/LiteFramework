using UnityEngine;

namespace Lite.Framework.Helper
{
    public static class UnityHelper
    {
        public static string GetDeviceID()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public static string GetPlatform()
        {
#if UNITY_IPHONE
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "Windows";
#endif
        }

        public static void ClearLog()
        {
#if UNITY_EDITOR
            var LogEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            var ClearMethod = LogEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            ClearMethod.Invoke(null, null);
#endif
        }
    }
}