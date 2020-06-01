using LiteFramework.Core.Log;
using Spine.Unity;
using UnityEngine;

namespace LiteFramework.Game.Sfx
{
    public class SkeletonSfx : BaseSfx
    {
        private SkeletonAnimation Animation_;

        public SkeletonSfx(string Name, Transform Trans)
            : base(Name, Trans)
        {
            Animation_ = GetComponent<SkeletonAnimation>();
            Animation_.Initialize(false);
        }

        public override void Tick(float DeltaTime)
        {
            if (!IsAlive)
            {
                return;
            }

            if (Animation_.AnimationState.GetCurrent(0).Loop)
            {
                return;
            }

            IsAlive = !IsEnd();
        }

        public override void Dispose()
        {
            if (Animation_ != null)
            {
                Animation_.Initialize(false);
                Animation_.AnimationState.ClearTracks();
                Animation_ = null;
            }

            base.Dispose();
        }

        protected override float GetZOrder(Vector2 Value)
        {
            return 0;
        }

        public override bool IsEnd()
        {
            return Animation_?.AnimationState?.GetCurrent(0)?.IsComplete ?? true;
        }

        public override void Pause()
        {
            Animation_.timeScale = 0;
        }

        public override void Resume()
        {
            Animation_.timeScale = 1;
        }

        public override void Play(string AnimationName, bool IsLoop = false, LiteAction Finished = null)
        {
            if (Animation_.Skeleton.Data.FindAnimation(AnimationName) == null)
            {
                LLogger.LError($"can't play animation '{AnimationName}'");
                return;
            }

            Animation_.AnimationState.SetAnimation(0, AnimationName, IsLoop);

            if (Finished == null)
            {
                Animation_.AnimationState.Complete = null;
            }
            else
            {
                Animation_.AnimationState.Complete = (Track) =>
                {
                    Finished.Invoke();
                    Animation_.AnimationState.Complete = null;
                };
            }
        }
    }
}