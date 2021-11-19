using System;

namespace Explorip.Helpers
{
    /// <summary>
    /// Classe contenant des méthodes d'extension pour les types (classes)
    /// </summary>
    public static class ExtensionsClass
    {
        /// <summary>
        /// Indique si un type/une classe est d'un type spécifique, ou enfant d'un type spécifique
        /// </summary>
        /// <param name="type">Type/classe à tester</param>
        /// <param name="classe">Type/classe à rechercher</param>
        /// <returns>True si 'type' est de type 'classe' (ou enfant de 'classe'), sinon False</returns>
        public static bool EstTypeOuSousTypeDe(this Type type, Type classe)
        {
            return (type.IsSubclassOf(classe) || (type == classe));
        }

        /// <summary>
        /// Convertir un objet vers un autre type.
        /// Attention, pour savoir si ca a marché = il n'y a pas d'exception.
        /// Vous devez donc appeler cette méthode dans un Try/Catch pour savoir si ca a échoué (et pour éviter une exception non gérée)
        /// </summary>
        /// <param name="objet">Objet à convertir</param>
        /// <param name="nouveauType">Nouveau type pour cet objet</param>
        public static object ConvertToType(this object objet, Type nouveauType)
        {
            if (nouveauType.IsEnum)
            {
                return Enum.Parse(nouveauType, objet.ToString());
            }
            else
            {
                return Convert.ChangeType(objet, nouveauType);
            }
        }
    }
}
