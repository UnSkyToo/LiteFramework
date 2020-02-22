using LiteFramework.Game.Asset;

namespace LiteFramework.Core.Motion
{
    public class DestroyMotion : BaseMotion
    {
        public override void Enter()
        {
            if (Master?.gameObject != null)
            {
                AssetManager.DeleteAsset(Master.gameObject);
            }
        }
    }
}