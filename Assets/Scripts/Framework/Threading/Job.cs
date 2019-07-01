namespace Lite.Framework.Threading
{
    public class Job<TEntity>
    {
        public TEntity[] Entities { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public LiteException Ex { get; set; }

        internal void Set(TEntity[] Entities, int From, int To)
        {
            this.Entities = Entities;
            this.From = From;
            this.To = To;
            this.Ex = null;
        }
    }
}
