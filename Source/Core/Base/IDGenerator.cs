namespace LiteFramework.Core.Base
{
    public static class IDGenerator
    {
        private static uint ID_ = 1;

        public static uint Get()
        {
            return ID_++;
        }
    }
}