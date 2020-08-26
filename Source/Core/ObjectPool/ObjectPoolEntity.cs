using System.Collections.Generic;

namespace LiteFramework.Core.ObjectPool
{
    public class ObjectPoolEntity<T> : BaseObjectPool where T : UnityEngine.Object
    {
        public int InfrequentCount { get; set; } = 2;

		protected readonly Queue<T> ObjectCacheList_ = null;

        protected LiteFunc<T> CreateFunc_ = null;
        protected LiteAction<T> SpawnFunc_ = null;
        protected LiteAction<T> RecycleFunc_ = null;
        protected LiteAction<T> DisposeFunc_ = null;

        public ObjectPoolEntity(string PoolName, LiteFunc<T> CreateFunc, LiteAction<T> SpawnFunc, LiteAction<T> RecycleFunc, LiteAction<T> DisposeFunc)
            : base(PoolName, typeof(T))
        {
			ObjectCacheList_ = new Queue<T>();
			
            this.CreateFunc_ = CreateFunc;
            this.SpawnFunc_ = SpawnFunc;
            this.RecycleFunc_ = RecycleFunc;
            this.DisposeFunc_ = DisposeFunc;
        }

        public T Spawn()
        {
            if (ObjectCacheList_.Count > 0)
            {
                UsedCount++;
                IdleCount--;
                var TCache = ObjectCacheList_.Dequeue();
                TriggerSpawn(TCache);
                return TCache;
            }
            
            var NewCache = CreateFunc_.Invoke();
            TriggerSpawn(NewCache);
            UsedCount++;

            return NewCache;
        }

        public void Recycle(T Obj)
        {
            if (Obj != null)
            {
                UsedCount--;
                IdleCount++;
                ObjectCacheList_.Enqueue(Obj);
                TriggerRecycle(Obj);
            }
        }

        public override void Dispose()
        {
            foreach (var Cache in ObjectCacheList_)
            {
                TriggerDispose(Cache);
            }

            ObjectCacheList_.Clear();
            UsedCount = 0;
            IdleCount = 0;

            CreateFunc_ = null;
            SpawnFunc_ = null;
            RecycleFunc_ = null;
            DisposeFunc_ = null;
        }

        private void TriggerSpawn(T Cache)
        {
            SpawnFunc_?.Invoke(Cache);
        }

        private void TriggerRecycle(T Cache)
        {
            RecycleFunc_?.Invoke(Cache);
        }

        private void TriggerDispose(T Cache)
        {
            DisposeFunc_?.Invoke(Cache);
        }
    }
}