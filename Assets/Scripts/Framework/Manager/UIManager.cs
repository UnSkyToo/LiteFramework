using System;
using System.Collections.Generic;
using Lite.Framework.Base;
using UnityEngine;
using Logger = Lite.Framework.Log.Logger;

namespace Lite.Framework.Manager
{
    public static class UIManager
    {
        private static Transform CanvasNormalTransform_ = null;
        private static Transform CanvasNormalTransform
        {
            get
            {
                if (CanvasNormalTransform_ == null)
                {
                    CanvasNormalTransform_ = GameObject.Find("Canvas-Normal").transform;
                }

                return CanvasNormalTransform_;
            }
        }

        private static readonly Dictionary<uint, UIBase> UIList_ = new Dictionary<uint, UIBase>();
        private static readonly Dictionary<string, Transform> CacheList_ = new Dictionary<string, Transform>();
        private static readonly List<UIBase> OpenList_ = new List<UIBase>();
        private static readonly List<UIBase> CloseList_ = new List<UIBase>();

        public static bool Startup()
        {
            UIList_.Clear();
            CacheList_.Clear();
            OpenList_.Clear();
            CloseList_.Clear();

            return true;
        }

        public static void Shutdown()
        {
            CloseAllUI();

            var ChildCount = CanvasNormalTransform.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = CanvasNormalTransform.GetChild(Index);
                AssetManager.DeleteAsset(Child.gameObject);
            }

            UIList_.Clear();
            CacheList_.Clear();
            OpenList_.Clear();
            CloseList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            if (OpenList_.Count > 0)
            {
                foreach (var UI in OpenList_)
                {
                    UIList_.Add(UI.ID, UI);
                }

                OpenList_.Clear();
                ResortUIList();
            }

            if (UIList_.Count > 0)
            {
                foreach (var UI in UIList_)
                {
                    UI.Value.Tick(DeltaTime);
                }
            }

            if (CloseList_.Count > 0)
            {
                foreach (var UI in CloseList_)
                {
                    UIList_.Remove(UI.ID);

                    if (!CacheList_.ContainsKey(UI.Path))
                    {
                        CacheList_.Add(UI.Path, UI.UITransform);
                    }
                }

                CloseList_.Clear();
            }
        }

        public static void OpenUI<T>(params object[] Params) where T : UIBase, new()
        {
            var ScriptType = typeof(T);
            if (!Configure.UIList.ContainsKey(ScriptType))
            {
                Logger.DWarning($"Can't find UIPath : {ScriptType.Name}");
                return;
            }

            var UI = FindUI<T>();
            if (UI == null)
            {
                CreateUI<T>(Configure.UIList[ScriptType], Params);
            }
        }

        public static void OpenUI<T>(string Path, params object[] Params) where T : UIBase, new()
        {
            CreateUI<T>(Path, Params);
        }

        public static void OpenUI<T>(T Script, string Path, params object[] Params) where T : UIBase
        {
            CreateUI<T>(Script, Path, Params);
        }

        public static void CloseUI<T>() where T : UIBase
        {
            var UI = FindUI<T>();
            if (UI != null)
            {
                CloseUI(UI);
            }
        }

        public static void CloseUI(UIBase UI)
        {
            UI.Close();
            CloseList_.Add(UI);
        }

        public static void CloseAllUI()
        {
            if (UIList_.Count > 0)
            {
                foreach (var UI in UIList_)
                {
                    CloseUI(UI.Value);
                }

                UIList_.Clear();
            }

            if (OpenList_.Count > 0)
            {
                foreach (var UI in OpenList_)
                {
                    CloseUI(UI);
                }

                OpenList_.Clear();
            }
        }

        public static void DeleteUnusedUI()
        {
            foreach (var Cache in CacheList_)
            {
                AssetManager.DeleteAsset(Cache.Value.gameObject);
            }

            CacheList_.Clear();
        }

        public static T FindUI<T>() where T : UIBase
        {
            var ScriptType = typeof(T);

            foreach (var Data in UIList_)
            {
                if (Data.Value.Name == ScriptType.Name)
                {
                    return Data.Value as T;
                }
            }

            foreach (var Data in OpenList_)
            {
                if (Data.Name == ScriptType.Name)
                {
                    return Data as T;
                }
            }

            return null;
        }

        public static bool IsClosed<T>() where T : UIBase
        {
            var UI = FindUI<T>();
            if (UI == null)
            {
                return true;
            }

            return false;
        }

        private static void GetOrCreateGameObject(string Path, Action<GameObject> Callback)
        {
            Transform UIObj = null;
            if (CacheList_.ContainsKey(Path))
            {
                UIObj = CacheList_[Path];
                CacheList_.Remove(Path);
            }

            if (UIObj == null)
            {
                var UIPath = $"ui/{Path.ToLower()}.prefab";
                AssetManager.CreatePrefabAsync(UIPath, Obj =>
                {
                    if (Obj == null)
                    {
                        Logger.DWarning("Can't Create UI : " + UIPath);
                        return;
                    }
                    Callback?.Invoke(Obj);
                });
            }
            else
            {
                Callback?.Invoke(UIObj.gameObject);
            }
        }

        public static void CreateUI<T>(string Path, params object[] Params) where T : UIBase, new()
        {
            GetOrCreateGameObject(Path, (Obj) =>
            {
                CreateUI<T>(Obj, Path, Params);
            });
        }

        public static void CreateUI<T>(T Script, string Path, params object[] Params) where T : UIBase
        {
            GetOrCreateGameObject(Path, (Obj) =>
            {
                CreateUI<T>(Obj, Script, Path, Params);
            });
        }

        public static void CreateUI<T>(GameObject Obj, string Path, params object[] Params) where T : UIBase, new()
        {
            var Script = new T();
            CreateUI<T>(Obj, Script, Path, Params);
        }

        public static void CreateUI<T>(GameObject Obj, T Script, string Path, params object[] Params) where T : UIBase
        {
            Obj.name = Path;
            Obj.transform.SetParent(CanvasNormalTransform, false);

            Script.Path = Path;
            Script.UITransform = Obj.transform;
            Script.UIRectTransform = Obj.GetComponent<RectTransform>();
            Script.UIRectTransform.SetSiblingIndex(Script.SortOrder + UIList_.Count + CacheList_.Count);

            OpenList_.Add(Script);
            Script.Open(Params);
        }

        private static void ResortUIList()
        {
            var CountInfo = GetUICountInfo();
            RebuildUIList();

            var BottomIndex = CacheList_.Count;
            var NormalIndex = BottomIndex + CountInfo[UIDepthMode.Bottom];
            var TopIndex = NormalIndex + CountInfo[UIDepthMode.Normal];

            foreach (var UI in UIList_)
            {
                var NewIndex = UI.Value.UIRectTransform.GetSiblingIndex();
                switch (UI.Value.DepthMode)
                {
                    case UIDepthMode.Bottom:
                        NewIndex += BottomIndex++;
                        break;
                    case UIDepthMode.Normal:
                        NewIndex += NormalIndex++;
                        break;
                    case UIDepthMode.Top:
                        NewIndex += TopIndex++;
                        break;
                    default:
                        break;
                }

                UI.Value.UIRectTransform.SetSiblingIndex(NewIndex);
            }
        }

        private static Dictionary<UIDepthMode, int> GetUICountInfo()
        {
            var Result = new Dictionary<UIDepthMode, int>
            {
                {UIDepthMode.Bottom, 0},
                {UIDepthMode.Normal, 0},
                {UIDepthMode.Top, 0}
            };

            foreach (var UI in UIList_)
            {
                Result[UI.Value.DepthMode]++;
            }

            return Result;
        }

        private static void RebuildUIList()
        {
            var NewList = new List<UIBase>(UIList_.Values);
            NewList.Sort((X, Y) =>
            {
                if (X.UIRectTransform.GetSiblingIndex() + X.SortOrder < Y.UIRectTransform.GetSiblingIndex() + Y.SortOrder)
                {
                    return -1;
                }

                if (X.UIRectTransform.GetSiblingIndex() + X.SortOrder > Y.UIRectTransform.GetSiblingIndex() + Y.SortOrder)
                {
                    return 1;
                }

                return 0;
            });

            UIList_.Clear();
            foreach (var UI in NewList)
            {
                UIList_.Add(UI.ID, UI);
            }
        }
    }
}