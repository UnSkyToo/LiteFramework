using System.Collections;
using System.Collections.Generic;

namespace LiteFramework.Core.Base
{
    public class ListEx<T> : IEnumerable<T>
    {
        private bool Dirty_;
        private int InEach_;

        private readonly List<T> Values_;
        private readonly List<T> AddList_;
        private readonly List<T> RemoveList_;

        public int Count => AddList_.Count + Values_.Count - RemoveList_.Count;

        public T this[int Index] => Values_[Index];

        public ListEx()
        {
            Dirty_ = false;
            InEach_ = 0;

            Values_ = new List<T>();
            AddList_ = new List<T>();
            RemoveList_ = new List<T>();
        }

        public void Add(T Item)
        {
            AddList_.Add(Item);
            Dirty_ = true;
        }

        public void Remove(T Item)
        {
            RemoveList_.Add(Item);
            Dirty_ = true;
        }

        public void Clear()
        {
            RemoveList_.Clear();
            AddList_.Clear();
            Values_.Clear();
        }

        public bool Contains(T Item)
        {
            return Values_.Contains(Item) || AddList_.Contains(Item);
        }

        public void Foreach(LiteAction<T> TickFunc)
        {
            Flush();
            InEach_++;
            foreach (var Item in Values_)
            {
                TickFunc?.Invoke(Item);
            }
            InEach_--;
        }

        /// <summary>
        /// Return T when TickFunc return true
        /// </summary>
        public T ForeachReturn(LiteFunc<T, bool> TickFunc)
        {
            Flush();
            InEach_++;
            foreach (var Item in Values_)
            {
                if (TickFunc?.Invoke(Item) == true)
                {
                    return Item;
                }
            }
            InEach_--;
            return default;
        }

        public void Foreach<P>(LiteAction<T, P> TickFunc, P Param)
        {
            Flush();
            InEach_++;
            foreach (var Item in Values_)
            {
                TickFunc?.Invoke(Item, Param);
            }
            InEach_--;
        }

        public void Foreach<P1, P2>(LiteAction<T, P1, P2> TickFunc, P1 Param1, P2 Param2)
        {
            Flush();
            InEach_++;
            foreach (var Item in Values_)
            {
                TickFunc?.Invoke(Item, Param1, Param2);
            }
            InEach_--;
        }
        
        public void Foreach<P1, P2, P3>(LiteAction<T, P1, P2, P3> TickFunc, P1 Param1, P2 Param2, P3 Param3)
        {
            Flush();
            InEach_++;
            foreach (var Item in Values_)
            {
                TickFunc?.Invoke(Item, Param1, Param2, Param3);
            }
            InEach_--;
        }

        /// <summary>
        /// Return T when TickFunc return true
        /// </summary>
        public T ForeachReturn<P>(LiteFunc<T, P, bool> TickFunc, P Param)
        {
            Flush();

            InEach_++;
            foreach (var Item in Values_)
            {
                if (TickFunc?.Invoke(Item, Param) == true)
                {
                    return Item;
                }
            }
            InEach_--;
            return default;
        }

        public void Flush()
        {
            if (Dirty_ && InEach_ == 0)
            {
                foreach (var Item in RemoveList_)
                {
                    Values_.Remove(Item);
                }

                RemoveList_.Clear();

                foreach (var Item in AddList_)
                {
                    Values_.Add(Item);
                }

                AddList_.Clear();
            }
        }

        public T Where(LiteFunc<T, bool> ConditionFunc)
        {
            Flush();

            foreach (var Item in Values_)
            {
                if (ConditionFunc?.Invoke(Item) == true)
                {
                    return Item;
                }
            }

            return default;
        }

        public List<T> All(LiteFunc<T, bool> ConditionFunc)
        {
            Flush();
            var Result = new List<T>();

            foreach (var Item in Values_)
            {
                if (ConditionFunc?.Invoke(Item) == true)
                {
                    Result.Add(Item);
                }
            }

            return Result;
        }

        // GC Alloc 40B
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (var Index = 0; Index < Values_.Count; ++Index)
            {
                yield return Values_[Index];
            }
        }

        // GC Alloc 40B
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var Index = 0; Index < Values_.Count; ++Index)
            {
                yield return Values_[Index];
            }
        }
    }
}