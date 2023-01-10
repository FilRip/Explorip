using System;
using System.Runtime.Serialization;

namespace Explorip.Exceptions
{
    [Serializable()]
    public class GestionEcransException : ExploripException
    {
        private readonly string _erreur;

        public GestionEcransException(string erreur)
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

        protected GestionEcransException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
