using LiteFramework.Core.Log;
using LiteFramework.Game.Asset;
using LiteFramework.Game.Base;
using UnityEngine;

namespace LiteFramework.Helper
{
    public static class SpriteHelper
    {
        public static void ReplaceSprite(SpriteRenderer Master, AssetUri Uri)
        {
            if (Master == null || Uri == null)
            {
                return;
            }

            AssetManager.CreateAssetAsync<Sprite>(Uri, (Spr) =>
            {
                if (Spr == null)
                {
                    LLogger.LWarning($"can't load {Uri}");
                    return;
                }

                Master.sprite = Spr;
            });
        }

        public static void ChangeColor(Transform Parent, Color NewColor, bool Recursively)
        {
            var Graphics = Parent?.GetComponent<SpriteRenderer>();
            if (Graphics == null)
            {
                return;
            }

            Graphics.color = NewColor;

            if (!Recursively)
            {
                return;
            }

            var ChildCount = Parent.childCount;
            for (var Index = 0; Index < ChildCount; ++Index)
            {
                var Child = Parent.GetChild(Index);
                ChangeColor(Child, NewColor, Recursively);
            }
        }

        public static Vector3 ScreenPosToWorldPos(Transform Master, Vector2 ScreenPos)
        {
            var ScreenZ = Camera.main.WorldToScreenPoint(Master.position).z;
            return Camera.main.ScreenToWorldPoint(new Vector3(ScreenPos.x, ScreenPos.y, ScreenZ));
        }

        public static Vector3 ScreenPosToLocalPos(Transform Master, Vector2 ScreenPos)
        {
            if (Master == null)
            {
                return Vector3.zero;
            }

            var WorldPos = ScreenPosToWorldPos(Master, ScreenPos);
            return Master.parent == null ? WorldPos : Master.parent.InverseTransformPoint(WorldPos);
        }

        public static Vector3 ScreenPosToLocalPos(GameEntity Master, Vector2 ScreenPos)
        {
            return ScreenPosToLocalPos(Master.GetTransform(), ScreenPos);
        }

        public static Vector2 WorldToScreenPos(Vector3 WorldPos)
        {
            return Camera.main.WorldToScreenPoint(WorldPos);
        }

        public static Vector2 WorldToScreenPos(Transform Master)
        {
            return WorldToScreenPos(Master.position);
        }

        public static Vector2 WorldToScreenPos(GameEntity Master)
        {
            return WorldToScreenPos(Master.GetTransform());
        }
    }
}