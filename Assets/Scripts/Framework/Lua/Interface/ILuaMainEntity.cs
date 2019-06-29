namespace Lite.Framework.Lua.Interface
{
    public interface ILuaMainEntity
    {
        bool OnStart();
        void OnStop();
        void OnTick(float DeltaTime);
    }
}