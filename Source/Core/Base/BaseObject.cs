namespace LiteFramework.Core.Base
{
    public abstract class BaseObject
    {
        public uint SerialID { get; }

        protected BaseObject()
        {
            SerialID = IDGenerator.Get();
        }
    }
}