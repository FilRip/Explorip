using System;
using System.Runtime.Serialization;

namespace Explorip.Exceptions
{
    [Serializable()]
    public class ScreenManagerException : ExploripException
    {
        private readonly string _erreur;

        public ScreenManagerException(string erreur)
        {
            _erreur = erreur;
        }

        public override string Message
        {
            get
            {
                return _erreur;
            }
        }

        protected ScreenManagerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
