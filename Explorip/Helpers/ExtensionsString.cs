namespace Explorip.Helpers
{
    /// <summary>
    /// Classe contenant des extensions pour la classe String
    /// </summary>
    public static class ExtensionsString
    {
        /// <summary>
        /// Supprime tous les doublons, boucle tant qu'ils y en a
        /// Exemple : supprime 3, 4, 5, ... même caractère spécifié, à la suite jusqu'à ce qu'il n'en reste plus qu'un
        /// </summary>
        /// <param name="source">Chaine de caractères contenant les doublons à supprimer</param>
        /// <param name="caractere">Caractère dont il faut supprimer les doulons à la suite</param>
        public static string RemoveDuplicate(this string source, char caractere)
        {
            string retour = source;
            while (retour.IndexOf(caractere.ToString() + caractere.ToString()) >= 0)
                retour = retour.Replace(caractere.ToString() + caractere.ToString(), caractere.ToString());
            return retour;
        }
    }
}
