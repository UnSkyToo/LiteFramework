namespace LiteFramework.Game.UI
{
    public abstract class DialogUI : BaseUI
    {
        protected DialogUI(UIDepthMode Mode, int Index)
            : base(Mode, Index)
        {
        }

        public override void Show()
        {
            if (UITransform != null)
            {
                UITransform.gameObject.SetActive(true);
            }
        }

        public void OnDialogPopup()
        {
            OnShow();
        }
    }
}