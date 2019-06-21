using System;

namespace Lite
{
    public class LiteException : Exception
    {
        public LiteException()
        {
        }

        public LiteException(string Message)
            : base(Message)
        {
        }

        public LiteException(string Message, Exception InnerException)
            : base(Message, InnerException)
        {
        }
    }
}