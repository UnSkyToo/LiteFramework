namespace Lite.Logic.Lua
{
    public interface ILuaMainEntity
    {
        bool OnStart();
        void OnStop();
        void OnTick(float DeltaTime);
    }
}