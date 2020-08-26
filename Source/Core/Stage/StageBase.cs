using System;
using LiteFramework.Interface;

namespace LiteFramework.Core.Stage
{
    public abstract class StageBase<T> : ITick where T : Enum
    {
        public T StageID { get; }
        public T NextStageID { get; private set; }

        protected StagePipeline<T> Pipeline_;
        protected StageCode StageCode_;

        protected StageBase(T stageID)
        {
            StageID = stageID;
            SetNextStageID(default);
        }

        public void SetPipeline(StagePipeline<T> pipeline)
        {
            Pipeline_ = pipeline;
        }

        public void SetNextStageID(T nextStageID)
        {
            NextStageID = nextStageID;
        }

        public void Enter()
        {
            StageCode_ = StageCode.Continue;
            OnEnter();
        }

        public void Exit()
        {
            OnExit();
        }

        public void Tick(float deltaTime)
        {
            OnTick(deltaTime);
        }

        public StageCode CheckStage()
        {
            return OnCheckStage();
        }

        public void SetStageCode(StageCode Code)
        {
            StageCode_ = Code;
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnExit()
        {
        }

        protected virtual void OnTick(float deltaTime)
        {
        }

        protected virtual StageCode OnCheckStage()
        {
            return StageCode_;
        }
    }
}