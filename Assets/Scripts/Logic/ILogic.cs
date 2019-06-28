namespace Lite.Logic
{
    public interface ILogic
    {
        bool Startup();
        void Shutdown();
        void Tick(float DeltaTime);
    }
}