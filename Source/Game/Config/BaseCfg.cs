using System.Collections.Generic;
using LiteFramework.Core.Log;
using LiteFramework.Game.Data;

namespace LiteFramework.Game.Config
{
    public class BaseCfg<T> where T : IBaseCfgLine, new()
    {
        protected readonly Dictionary<int, T> LineList_;

        public T this[int ID]
        {
            get
            {
                if (LineList_.ContainsKey(ID))
                {
                    return LineList_[ID];
                }

                return default;
            }
        }

        public int LineCount => LineList_.Count;

        public BaseCfg()
        {
            LineList_ = new Dictionary<int, T>();
        }

        public Dictionary<int, T> GetLineList()
        {
            return LineList_;
        }

        public bool Parse(DataTable Table)
        {
            try
            {
                var Keys = Table.Keys();

                foreach (var Key in Keys)
                {
                    var Line = new T();
                    Line.Parse(Table.Line(Key));
                    LineList_.Add(Key, Line);
                }

                return true;
            }
            catch (LiteException Ex)
            {
                LLogger.LError(Ex.Message);
                return false;
            }
        }
    }
}