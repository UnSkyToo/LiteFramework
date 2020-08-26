using System;

namespace LiteFramework.Core.ObjectPool
{
    public abstract class BaseObjectPool : IDisposable
    {
        public string FullName { get; }
        public string PoolName { get; }
        public Type ObjectType { get; }
        public int UsedCount { get; protected set; }
        public int IdleCount { get; protected set; }


        protected BaseObjectPool(string PoolName, Type ObjectType)
        {
            this.FullName = $"{ObjectType.Name}_{PoolName}";
            this.PoolName = PoolName;
            this.ObjectType = ObjectType;
            this.UsedCount = 0;
            this.IdleCount = 0;
        }

        public abstract void Dispose();
    }
}