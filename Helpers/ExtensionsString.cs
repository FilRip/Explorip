using System;

namespace Explorip.Helpers
{
    public static class ExtensionsString
    {
        public static string EffaceCaracteres(this string chaine, string caracteres)
        {
            return chaine.Replace(caracteres, "");
        }

        public static string EffaceCaracteres(this string chaine, char caracteres)
        {
            return EffaceCaracteres(chaine, caracteres.ToString());
        }

        public static bool Contains(this string str, string subString, StringComparison comparateur)
        {
            if ((!string.IsNullOrWhiteSpace(str)) && (!string.IsNullOrWhiteSpace(subString)))
                return (str.IndexOf(subString, comparateur) >= 0);
            return false;
        }

        public static string Replace(this string source, string sousChaine, string Remplacement, StringComparison comparateur)
        {
            string retour = "";

            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Trim().Length == 0) return source;

            if (string.IsNullOrWhiteSpace(sousChaine)) throw new ArgumentNullException(nameof(sousChaine));

            int trouve;
            int pos = 0;
            while ((trouve = source.IndexOf(sousChaine, pos, comparateur)) >= 0)
            {
                retour = source.Substring(pos, trouve);
                retour += Remplacement;
                pos += trouve + sousChaine.Length;
            }

            if (source.Length > pos) retour += source.Substring(pos);
            return retour;
        }

        public static string SupprimeDoublons(this string source, char caractere)
        {
            string retour = source;
            while (retour.IndexOf(caractere.ToString() + caractere.ToString()) >= 0)
                retour = retour.Replace(caractere.ToString() + caractere.ToString(), caractere.ToString());
            return retour;
        }

        public static string RetourneTailleMax(this string texte, int tailleMaximum)
        {
            if (texte == null)
                throw new ArgumentNullException(nameof(texte));
            if (texte.Length > tailleMaximum)
                return texte.Substring(0, tailleMaximum);
            else
                return texte;
        }
    }
}
