using LiteFramework.Game.Data;

namespace LiteFramework.Game.Config
{
    public interface IBaseCfgLine
    {
        void Parse(DataLine Line);
    }
}