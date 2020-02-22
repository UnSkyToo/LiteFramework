using System;
using LiteFramework.Game.Base;
using LiteFramework.Interface;
using UnityEngine;

namespace LiteFramework.Extend.Controller
{
    public class MoveController : ITick
    {
        private readonly Transform Master_;

        private bool IsMoving_;
        private Vector3 BeginPos_;
        private Vector3 EndPos_;
        private float MoveTime_;
        private float CurrentTime_;
        private Action FinishCallback_;

        public MoveController(Transform Master)
        {
            Master_ = Master;
            Stop();
        }

        public MoveController(GameEntity Master)
            : this(Master.GetTransform())
        {
        }

        public void Tick(float DeltaTime)
        {
            if (!IsMoving_)
            {
                return;
            }

            CurrentTime_ += DeltaTime;

            var T = CurrentTime_ / MoveTime_;
            Master_.localPosition = Vector3.Lerp(BeginPos_, EndPos_, Mathf.Clamp01(T));

            if (T >= 1.0f)
            {
                Stop();
                FinishCallback_?.Invoke();
            }
        }

        public void MoveTo(Vector3 TargetPos, float MoveTime, bool Force, Action Finished)
        {
            if (IsMoving_ && !Force)
            {
                return;
            }

            if ((new Vector3(EndPos_.x, EndPos_.y) - TargetPos).magnitude < 0.00001f)
            {
                Finished?.Invoke();
                return;
            }

            IsMoving_ = true;
            BeginPos_ = Master_.localPosition;
            EndPos_ = new Vector3(TargetPos.x, TargetPos.y, BeginPos_.z);
            MoveTime_ = Mathf.Clamp(MoveTime, 0.016f, float.MaxValue);
            CurrentTime_ = 0;
            FinishCallback_ = Finished;
        }

        public void Stop()
        {
            IsMoving_ = false;
            BeginPos_ = Vector3.zero;
            EndPos_ = Vector3.negativeInfinity;
            MoveTime_ = 0;
            CurrentTime_ = 0;
        }
    }
}