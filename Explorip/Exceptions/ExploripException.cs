using System;

namespace Explorip.Exceptions
{
    [Serializable()]
    public class ExploripException : Exception
    {
        public ExploripException() : base()
        {
        }

        public ExploripException(string message) : base(message)
        {
        }
    }
}
