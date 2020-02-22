using System.Collections.Generic;
using LiteFramework.Core.Motion;
using LiteFramework.Game.UI;
using UnityEngine;

namespace LiteFramework.Helper
{
    public static class DialogHelper
    {
        private static DialogUI CurrentUI_ = null;
        private static readonly Queue<DialogUI> OpenQueueList_ = new Queue<DialogUI>();

        public static void OpenDialog<T>(params object[] Params) where T : DialogUI, new()
        {
            if (IsOpenedDialog<T>())
            {
                return;
            }

            var Dialog = UIManager.OpenUI<T>(Params);
            if (CurrentUI_ != null)
            {
                Dialog.SetActive(false);
                OpenQueueList_.Enqueue(Dialog);
            }
            else
            {
                OpenDialog(Dialog);
            }
        }

        private static void OpenDialog(DialogUI Dialog)
        {
            CurrentUI_ = Dialog;
            var Root = UIHelper.FindChild(Dialog.UITransform, "Dialog");
            if (Root == null)
            {
                Dialog.OnDialogPopup();
                return;
            }

            var PopupAct = MotionHelper.Sequence()
                .Callback(MaskLayerHelper.CreateMaskLayer)
                .Scale(0.2f, new Vector3(1.1f, 1.1f), false)
                .Scale(0.05f, new Vector3(1.0f, 1.0f), false)
                .Callback(() =>
                {
                    Dialog.OnDialogPopup();
                    MaskLayerHelper.DeleteMaskLayer();
                })
                .Flush();

            Root.localScale = Vector3.zero;
            Root.ExecuteMotion(PopupAct);
        }

        public static void CloseDialog(BaseUI Dialog)
        {
            if (Dialog == null)
            {
                return;
            }

            var Root = UIHelper.FindChild(Dialog.UITransform, "Dialog");
            if (Root == null)
            {
                UIManager.CloseUI(Dialog);
                CurrentUI_ = null;
                CheckDialogQueueList();
                return;
            }

            var CloseAct = MotionHelper.Sequence()
                .Callback(MaskLayerHelper.CreateMaskLayer)
                .Scale(0.05f, new Vector3(1.1f, 1.1f), false)
                .Scale(0.2f, new Vector3(0.0f, 0.0f), false)
                .Callback(() =>
                {
                    UIManager.CloseUI(Dialog);
                    MaskLayerHelper.DeleteMaskLayer();
                    CurrentUI_ = null;
                    CheckDialogQueueList();
                })
                .Flush();

            Root.ExecuteMotion(CloseAct);
        }

        public static void CloseDialog<T>() where T : DialogUI, new()
        {
            var Dialog = UIManager.FindUI<T>();
            CloseDialog(Dialog);
        }

        public static bool IsOpenedDialog<T>() where T : DialogUI, new()
        {
            return UIManager.IsOpened<T>();
        }

        private static void CheckDialogQueueList()
        {
            if (OpenQueueList_.Count > 0)
            {
                var Dialog = OpenQueueList_.Dequeue();
                Dialog.SetActive(true);
                OpenDialog(Dialog);
            }
        }

        public static void Clear()
        {
            foreach (var Dialog in OpenQueueList_)
            {
                UIManager.CloseUI(Dialog);
            }
            OpenQueueList_.Clear();
            CurrentUI_ = null;
        }
    }
}
