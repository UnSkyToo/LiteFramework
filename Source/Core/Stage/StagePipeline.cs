using System;
using System.Collections.Generic;
using LiteFramework.Interface;

namespace LiteFramework.Core.Stage
{
    public class StagePipeline<T> : ISubstance where T : Enum
    {
        public object Extra { get; set; }
        
        private readonly Dictionary<T, StageBase<T>> Stages_ = new Dictionary<T, StageBase<T>>();
        private readonly T ErrorStageID_;
        private StageBase<T> _pre = null;
        private StageBase<T> _cur = null;
        private StageBase<T> _dst = null;

        public StagePipeline(T errorStageID)
        {
            ErrorStageID_ = errorStageID;
        }

        public void Dispose()
        {
            End();
        }

        public void AddStage(StageBase<T> Stage, T NextStageID)
        {
            if (!Stages_.ContainsKey(Stage.StageID))
            {
                Stages_.Add(Stage.StageID, Stage);
                Stage.SetPipeline(this);
                Stage.SetNextStageID(NextStageID);
            }
        }
        
        public void ClearStage()
        {
            Stages_.Clear();
        }
        
        public void Begin(T DefaultID)
        {
            TranslateTo(DefaultID);
        }

        public void End()
        {
            _cur?.Exit();
            _pre = null;
            _cur = null;
            _dst = null;
        }
        
        public void Tick(float DeltaTime)
        {
            if (Stages_.Count == 0)
            {
                return;
            }

            if (_dst != null)
            {
                _cur?.Exit();

                _pre = _cur;
                _cur = _dst;
                _dst = null;

                _cur.Enter();
            }

            if (_cur != null)
            {
                _cur.Tick(DeltaTime);
                
                var Code = _cur.CheckStage();
                if (Code == StageCode.Failed)
                {
                    TranslateTo(ErrorStageID_);
                }
                else if (Code == StageCode.Succeeded)
                {
                    TranslateTo(_cur.NextStageID);
                }
            }
        }

        private void TranslateTo(T NextID)
        {
            if (Stages_.ContainsKey(NextID))
            {
                _dst = Stages_[NextID];
            }
            else
            {
                TranslateTo(ErrorStageID_);
            }
        }
    }
}