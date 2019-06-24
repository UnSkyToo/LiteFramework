using Lite.Framework.Base;
using Lite.Framework.Motion;
using UnityEngine;

namespace Lite.Framework.Manager
{
    public static class MotionManager
    {
        private static readonly ListEx<MotionBase> MotionList_ = new ListEx<MotionBase>();

        public static bool Startup()
        {
            MotionList_.Clear();
            return true;
        }

        public static void Shutdown()
        {
            MotionList_.Clear();
        }

        public static void Tick(float DeltaTime)
        {
            MotionList_.Flush();
            foreach (var Motion in MotionList_)
            {
                if (Motion.IsEnd)
                {
                    Motion.Exit();
                    MotionList_.Remove(Motion);
                }
                else
                {
                    Motion.Tick(DeltaTime);
                }
            }
        }

        public static void Execute(Transform Master, MotionBase Motion)
        {
            Motion.Master = Master;
            Motion.Enter();
            MotionList_.Add(Motion);
        }

        public static void ExecuteMotion(this Transform Master, MotionBase Motion)
        {
            Execute(Master, Motion);
        }
    }
}