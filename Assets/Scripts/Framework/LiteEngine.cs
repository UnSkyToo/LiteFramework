using Lite.Framework.Extend;
using Lite.Framework.Helper;
using Lite.Framework.Manager;
using UnityEngine;

namespace Lite.Framework
{
    public static class LiteEngine
    {
        public static MonoBehaviour MonoBehaviourInstance { get; private set; }

        private static float EnterBackgroundTime_ = 0.0f;
        private const float EnterBackgroundMaxTime = 90.0f;

        public static bool Startup(MonoBehaviour Instance)
        {
            MonoBehaviourInstance = Instance;

            if (!TaskManager.Startup())
            {
                Debug.LogError("TaskManager Startup Failed");
                return false;
            }

            if (!ObjectPoolManager.Startup())
            {
                Debug.LogError("ObjectPoolManager Startup Failed");
                return false;
            }

            if (!AssetManager.Startup())
            {
                Debug.LogError("ResourceManager Startup Failed");
                return false;
            }

            Attach<Debugger>(Camera.main.gameObject);
            Attach<Fps>(Camera.main.gameObject);

            /*AssetManager.CreatePrefab("ui/testimage.prefab", (Obj) =>
            {
                Obj.transform.SetParent(GameObject.Find("Canvas").transform);
                Obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                var AA = AssetManager.CreateAssetWithCache<Sprite>("res1/arena.sprite", "arena_5", (Spr) =>
                {
                    if (Spr != null)
                    {
                        Obj.GetComponent<Image>().sprite = Spr;
                    }
                });
            });*/

            AssetManager.CreatePrefab("anim/testani.prefab", (aa) =>
            {
                if (aa != null)
                {
                    aa.transform.SetParent(GameObject.Find("Canvas").transform);
                    aa.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
            });

            return true;
        }

        public static void Shutdown()
        {
            AssetManager.Shutdown();
            ObjectPoolManager.Shutdown();
            TaskManager.Shutdown();

            Detach<Debugger>(Camera.main.gameObject);
            Detach<Fps>(Camera.main.gameObject);

            MonoBehaviourInstance.StopAllCoroutines();
            PlayerPrefs.Save();
        }

        public static void Tick(float DeltaTime)
        {
            TaskManager.Tick(DeltaTime);
            ObjectPoolManager.Tick(DeltaTime);
            AssetManager.Tick(DeltaTime);
        }

        public static void Restart()
        {
            UnityHelper.ClearLog();
            Shutdown();
            Startup(MonoBehaviourInstance);
        }

        public static T Attach<T>(GameObject Root) where T : MonoBehaviour
        {
            var Component = Root.GetComponent<T>();

            if (Component != null)
            {
                return Component;
            }

            return Root.AddComponent<T>();
        }

        public static void Detach<T>(GameObject Root) where T : MonoBehaviour
        {
            var Component = Root.GetComponent<T>();

            if (Component != null)
            {
                Object.DestroyImmediate(Component);
            }
        }

        public static void OnEnterForeground()
        {
            if (Time.realtimeSinceStartup - EnterBackgroundTime_ >= EnterBackgroundMaxTime)
            {
                Restart();
            }

            EnterBackgroundTime_ = Time.realtimeSinceStartup;
        }

        public static void OnEnterBackground()
        {
            EnterBackgroundTime_ = Time.realtimeSinceStartup;
        }
    }
}