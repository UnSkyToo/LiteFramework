using System;
using System.Collections.Generic;

namespace LiteFramework.Core.Base
{
    public class DictionaryEx<TKey, TValue>
    {
        private bool Dirty_;
        private bool InEach_;

        private readonly Dictionary<TKey, TValue> Values_;
        private readonly Dictionary<TKey, TValue> AddList_;
        private readonly List<TKey> RemoveList_;

        public int Count => AddList_.Count + Values_.Count - RemoveList_.Count;

        public TValue this[TKey Key]
        {
            get
            {
                if (Values_.ContainsKey(Key))
                {
                    return Values_[Key];
                }

                if (AddList_.ContainsKey(Key))
                {
                    return AddList_[Key];
                }

                return default;
            }
        }

        public DictionaryEx()
        {
            Dirty_ = false;
            InEach_ = false;

            Values_ = new Dictionary<TKey, TValue>();
            AddList_ = new Dictionary<TKey, TValue>();
            RemoveList_ = new List<TKey>();
        }

        public void Add(TKey Key, TValue Value)
        {
            AddList_.Add(Key, Value);
            Dirty_ = true;
        }

        public void Remove(TKey Key)
        {
            RemoveList_.Add(Key);
            Dirty_ = true;
        }

        public void Clear()
        {
            RemoveList_.Clear();
            AddList_.Clear();
            Values_.Clear();
        }

        public bool ContainsKey(TKey Key)
        {
            return Values_.ContainsKey(Key) || AddList_.ContainsKey(Key);
        }

        public void Foreach(Action<TKey, TValue> TickFunc)
        {
            Flush();

            InEach_ = true;
            foreach (var Item in Values_)
            {
                TickFunc?.Invoke(Item.Key, Item.Value);
            }
            InEach_ = false;
        }

        public void Foreach<P>(Action<TKey, TValue, P> TickFunc, P Param)
        {
            Flush();

            InEach_ = true;
            foreach (var Item in Values_)
            {
                TickFunc?.Invoke(Item.Key, Item.Value, Param);
            }
            InEach_ = false;
        }

        public void Flush()
        {
            if (Dirty_ && !InEach_)
            {
                foreach (var Item in RemoveList_)
                {
                    Values_.Remove(Item);
                }

                RemoveList_.Clear();

                foreach (var Item in AddList_)
                {
                    Values_.Add(Item.Key, Item.Value);
                }

                AddList_.Clear();
            }
        }
    }
}