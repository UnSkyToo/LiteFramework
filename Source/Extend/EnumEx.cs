using System;

namespace LiteFramework.Extend
{
    public static class EnumEx
    {
        public static int Count<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T)).Length;
        }
    }
}