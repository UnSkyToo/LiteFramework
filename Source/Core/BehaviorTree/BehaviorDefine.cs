namespace LiteFramework.Core.BehaviorTree
{
    public enum BehaviorRunningState : byte
    {
        Running,
        Finish,
        Error
    }

    public enum BehaviorParallelMode : byte
    {
        Or,
        And
    }

    public enum BehaviorTerminalState : byte
    {
        Ready,
        Running,
        Finish
    }

    public class BehaviorInputData
    {
        public bool Enabled { get; set; }
        public float DeltaTime { get; set; }
        public float TickTime { get; set; }
        public BehaviorNode Node { get; set; }
        public object Params { get; set; }
    }
}