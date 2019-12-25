﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace LiteFramework.Core.Base
{
    public class ListEx<T> : IEnumerable<T>
    {
        private bool Dirty_;
        private bool InEach_;

        private readonly List<T> Values_;
        private readonly List<T> AddList_;
        private readonly List<T> RemoveList_;

        public int Count => AddList_.Count + Values_.Count - RemoveList_.Count;

        public T this[int Index] => Values_[Index];

        public ListEx()
        {
            Dirty_ = false;
            InEach_ = false;

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

        public void Foreach(Action<T> TickFunc)
        {
            Flush();
            InEach_ = true;
            foreach (var Item in Values_)
            {
                TickFunc?.Invoke(Item);
            }
            InEach_ = false;
        }

        /// <summary>
        /// Return T when TickFunc return true
        /// </summary>
        public T ForeachReturn(Func<T, bool> TickFunc)
        {
            Flush();
            InEach_ = true;
            foreach (var Item in Values_)
            {
                if (TickFunc?.Invoke(Item) == true)
                {
                    return Item;
                }
            }
            InEach_ = false;
            return default;
        }

        public void Foreach<P>(Action<T, P> TickFunc, P Param)
        {
            Flush();
            InEach_ = true;
            foreach (var Item in Values_)
            {
                TickFunc?.Invoke(Item, Param);
            }
            InEach_ = false;
        }

        /// <summary>
        /// Return T when TickFunc return true
        /// </summary>
        public T ForeachReturn<P>(Func<T, P, bool> TickFunc, P Param)
        {
            Flush();
            InEach_ = true;
            foreach (var Item in Values_)
            {
                if (TickFunc?.Invoke(Item, Param) == true)
                {
                    return Item;
                }
            }
            InEach_ = false;
            return default;
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
                    Values_.Add(Item);
                }

                AddList_.Clear();
            }
        }

        public T Where(Func<T, bool> ConditionFunc)
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

        public List<T> All(Func<T, bool> ConditionFunc)
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