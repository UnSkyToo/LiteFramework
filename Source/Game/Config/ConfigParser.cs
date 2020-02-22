using LiteFramework.Game.Asset;
using LiteFramework.Game.Data;

namespace LiteFramework.Game.Config
{
    public static class ConfigParser
    {
        public static BaseCfg<T> Parse<T>(AssetUri Uri) where T : IBaseCfgLine, new()
        {
            if (!DataManager.LoadSync(Uri))
            {
                throw new LiteException($"can't get data table : {Uri}");
            }

            return Parse<T>(Uri.AssetName);
        }

        public static BaseCfg<T> Parse<T>(string TableName) where T : IBaseCfgLine, new()
        {
            var Table = DataManager.GetTable(TableName);
            if (Table == null)
            {
                throw new LiteException($"can't get data table : {TableName}");
            }

            var Cfg = new BaseCfg<T>();
            if (!Cfg.Parse(Table))
            {
                throw new LiteException($"{TableName} cfg parse error");
            }

            return Cfg;
        }
    }
}