using Lite.Framework.Manager;
using Lite.Logic.UI;
using UnityEngine;

namespace Lite.Logic
{
    public static class LogicManager
    {
        public static bool Startup()
        {
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
            });

            AssetManager.CreatePrefab("anim/testani.prefab", (aa) =>
            {
                if (aa != null)
                {
                    aa.transform.SetParent(GameObject.Find("Canvas-Normal").transform);
                    aa.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                    aa.GetComponent<RectTransform>().localScale = Vector3.one;
                }
            });*/

            UIManager.OpenUI<LogoUI>();

            return true;
        }

        public static void Shutdown()
        {

        }

        public static void Tick(float DeltaTime)
        {
        }
    }
}