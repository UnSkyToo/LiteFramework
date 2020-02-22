namespace LiteFramework.Interface
{
    public interface ILogic : ITick
    {
        bool Startup();
        void Shutdown();
    }

    public abstract class BaseLogic : ILogic
    {
        public abstract bool Startup();
        public abstract void Shutdown();
        public abstract void Tick(float DeltaTime);
    }
}