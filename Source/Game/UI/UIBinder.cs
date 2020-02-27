using System;
using System.Reflection;
using LiteFramework.Core.Log;
using LiteFramework.Game.EventSystem;
using UnityEngine;
using UnityEngine.Events;

namespace LiteFramework.Game.UI
{
    internal static class UIBinder
    {
        private const BindingFlags CustomFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly Type TransformType = typeof(Transform);
        private static readonly Type ComponentType = typeof(Component);

        internal static void AutoBind(BaseUI UI)
        {
            if (UI == null || !LiteConfigure.EnableUIAutoBind)
            {
                return;
            }

            BindNode(UI);
            BindComponent(UI);
            BindEvent(UI);
        }

        private static void BindNode(BaseUI UI)
        {
            var FieldList = UI.GetType().GetFields(CustomFlags);
            foreach (var Field in FieldList)
            {
                if (Field.FieldType != TransformType)
                {
                    continue;
                }

                var Attr = Field.GetCustomAttribute<LiteUINodeAttribute>(false);
                if (Attr != null)
                {
                    var NodeValue = UI.FindChild(Attr.Path);
                    if (NodeValue == null)
                    {
                        LLogger.LWarning($"can't bind node : {Attr.Path}");
                        continue;
                    }

                    Field.SetValue(UI, NodeValue);
                }
            }
        }

        private static void BindComponent(BaseUI UI)
        {
            var FieldList = UI.GetType().GetFields(CustomFlags);
            foreach (var Field in FieldList)
            {
                if (!Field.FieldType.IsSubclassOf(ComponentType))
                {
                    continue;
                }

                var Attr = Field.GetCustomAttribute<LiteUIComponentAttribute>(false);
                if (Attr != null)
                {
                    var ComponentValue = UI.GetComponent(Attr.Path, Field.FieldType);
                    if (ComponentValue == null)
                    {
                        LLogger.LWarning($"can't bind component : {Attr.Path}");
                        continue;
                    }

                    Field.SetValue(UI, ComponentValue);
                }
            }
        }

        private static void BindEvent(BaseUI UI)
        {
            var MethodList = UI.GetType().GetMethods(CustomFlags);
            foreach (var Method in MethodList)
            {
                var Attr = Method.GetCustomAttribute<LiteUIEventAttribute>(false);
                if (Attr != null)
                {
                    try
                    {
                        if (Method.GetParameters().Length == 0)
                        {
                            UI.AddEventToChild(Attr.Path, Delegate.CreateDelegate(typeof(UnityAction), UI, Method) as UnityAction, Attr.EventType);
                        }
                        else
                        {
                            UI.AddEventToChild(Attr.Path, Delegate.CreateDelegate(typeof(Action<EventSystemData>), UI, Method) as UnityAction, Attr.EventType);
                        }
                    }
                    catch (Exception Ex)
                    {
                        LLogger.LWarning(Ex.Message);
                        LLogger.LWarning($"can't bind component : {Attr.Path}");
                    }
                }
            }
        }
    }
}