using System;
using System.Runtime.Serialization;

namespace Explorip.Exceptions
{
    /// <summary>
    /// Exception levée si erreur dans une méthode de gestionFenetres
    /// </summary>
    [Serializable()]
    public class GestionFenetresException : Exception
    {
        private readonly string _erreur;

        public GestionFenetresException(string erreur)
        {
            _erreur = erreur;
        }

        protected GestionFenetresException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public override string Message
        {
            get
            {
                return _erreur;
            }
        }
    }
}
