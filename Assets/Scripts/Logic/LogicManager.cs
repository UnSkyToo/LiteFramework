using Lite.Framework.Base;
using Lite.Framework.Manager;
using Lite.Logic.Lua;
using Lite.Logic.UI;

namespace Lite.Logic
{
    public static class LogicManager
    {
        private static readonly ListEx<ILogic> LogicList_ = new ListEx<ILogic>();

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

            //UIManager.OpenUI<LogoUI>();

            LogicList_.Clear();
            RegisterLogic(new LuaLogic());

            return true;
        }

        public static void Shutdown()
        {
            foreach (var Logic in LogicList_)
            {
                Logic.Shutdown();
            }
            LogicList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            LogicList_.Flush();
            foreach (var Logic in LogicList_)
            {
                Logic.Tick(DeltaTime);
            }
        }

        public static void RegisterLogic(ILogic Logic)
        {
            if (Logic.Startup())
            {
                LogicList_.Add(Logic);
            }
        }

        public static void UnRegisterLogic(ILogic Logic)
        {
            Logic.Shutdown();
            LogicList_.Remove(Logic);
        }
    }
}