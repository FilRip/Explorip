using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorip.Helpers
{
    /// <summary>
    /// Classe contenant des extensions pour la classe String
    /// </summary>
    public static class ExtensionsString
    {
        /// <summary>
        /// Efface toutes les occurences d'une chaine de caractère, d'une autre chaine de caractère
        /// </summary>
        /// <param name="chaine">Chaine source, contenant les caractères à effacer</param>
        /// <param name="caracteres">Caractère ou chaine de caractères à supprimer</param>
        public static string EffaceCaracteres(this string chaine, string caracteres)
        {
            return chaine.Replace(caracteres, "");
        }
        /// <summary>
        /// Efface toutes les occurences d'un caractère, d'une autre chaine de caractère
        /// </summary>
        /// <param name="chaine">Chaine source, contenant le caractère à effacer</param>
        /// <param name="caracteres">Caractère à supprimer</param>
        public static string EffaceCaracteres(this string chaine, char caracteres)
        {
            return EffaceCaracteres(chaine, caracteres.ToString());
        }

        /// <summary>
        /// Indique si une chaine de caractères précise est présente dans une chaine de charactères
        /// </summary>
        /// <param name="str">Chaine de caratères source</param>
        /// <param name="subString">Chaine de caractères à rechercher</param>
        /// <param name="comparateur">Comparateur à utiliser (généralement type de chaine et spécifie si l'on doit ignorer la casse ou pas)</param>
        /// <returns>True si chaine trouvée, sinon False</returns>
        public static bool Contains(this string str, string subString, StringComparison comparateur)
        {
            if ((!string.IsNullOrWhiteSpace(str)) && (!string.IsNullOrWhiteSpace(subString)))
                return str.IndexOf(subString, comparateur) >= 0;
            return false;
        }

        /// <summary>
        /// Remplace une chaine de caractère(s) par une autre avec prise en compte de la culture et de la casse ou pas
        /// </summary>
        /// <param name="source">Chaine source contenant la(les) chaine(s) de caractère(s) à remplacer</param>
        /// <param name="sousChaine">Chaine de caractère(s) à remplacer</param>
        /// <param name="Remplacement">Nouvelle chaine de caractère(s) de remplacement</param>
        /// <param name="comparateur">Comparateur à utiliser</param>
        public static string Replace(this string source, string sousChaine, string Remplacement, StringComparison comparateur)
        {
            StringBuilder retour = new();

            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Trim().Length == 0) return source;

            if (string.IsNullOrWhiteSpace(sousChaine)) throw new ArgumentNullException(nameof(sousChaine));

            bool boucle = true;
            while (boucle)
            {
                boucle = false;
                int trouve;
                if ((trouve = source.IndexOf(sousChaine, 0, comparateur)) >= 0)
                {
                    retour.Clear();
                    retour = new(source.Substring(0, trouve));
                    retour.Append(Remplacement);
                    retour.Append(source.Substring(trouve + sousChaine.Length));
                    source = retour.ToString();
                    boucle = true;
                }
            }

            return retour.Length == 0 ? source : retour.ToString();
        }

        /// <summary>
        /// Ajouter autant de fois le caractère demandé, au début de la chaine, pour que la chaine obtienne la taille demandée
        /// </summary>
        /// <param name="source">Chaine pour laquelle ajouter des caractères</param>
        /// <param name="caractere">Caractère à ajouter</param>
        /// <param name="taille">Taille que doit faire la chaine de caractères</param>
        public static string RemplirDebut(this string source, char caractere, int taille)
        {
            return RemplirDebut(source, caractere, taille, false);
        }

        /// <summary>
        /// Ajouter autant de fois le caractère demandé, au début de la chaine, pour que la chaine obtienne la taille demandée
        /// </summary>
        /// <param name="source">Chaine pour laquelle ajouter des caractères</param>
        /// <param name="caractere">Caractère à ajouter</param>
        /// <param name="taille">Taille que doit faire la chaine de caractères</param>
        /// <param name="restrictTaille">Restreint la taille de la chaine à la taille spécifiée (si celle-ci est plus grande que la taille spécifiée)</param>
        public static string RemplirDebut(this string source, char caractere, int taille, bool restrictTaille)
        {
            if (source.Length < taille)
                return new string(caractere, taille - source.Length) + source;
            else
                return (restrictTaille ? source.Substring(0, taille) : source);
        }

        /// <summary>
        /// Ajouter autant de fois le caractère demandé, à la fin de la chaine, pour que la chaine obtienne la taille demandée
        /// </summary>
        /// <param name="source">Chaine pour laquelle ajouter des caractères</param>
        /// <param name="caractere">Caractère à ajouter</param>
        /// <param name="taille">Taille que doit faire la chaine de caractère</param>
        public static string RemplirFin(this string source, char caractere, int taille)
        {
            return RemplirFin(source, caractere, taille, false);
        }

        /// <summary>
        /// Ajouter autant de fois le caractère demandé, à la fin de la chaine, pour que la chaine obtienne la taille demandée
        /// </summary>
        /// <param name="source">Chaine pour laquelle ajouter des caractères</param>
        /// <param name="caractere">Caractère à ajouter</param>
        /// <param name="taille">Taille que doit faire la chaine de caractère</param>
        /// <param name="restrictTaille">Restreint la taille de la chaine à la taille spécifiée (si celle-ci est plus grande que la taille spécifiée)</param>
        public static string RemplirFin(this string source, char caractere, int taille, bool restrictTaille)
        {
            if (source.Length < taille)
                return source + new string(caractere, taille - source.Length);
            else
                return (restrictTaille ? source.Substring(0, taille) : source);
        }

        /// <summary>
        /// Supprime les caractères spécifié au début et à la fin d'une chaine
        /// </summary>
        /// <param name="source">Chaine de caractères sources, contenant les caractères à supprimer</param>
        /// <param name="chaine">Chaine de caractères à supprimer</param>
        public static string Trim(this string source, string chaine)
        {
            string retour = source;

            retour = retour.TrimStart(chaine).TrimEnd(chaine);

            return retour;
        }

        /// <summary>
        /// Supprime les caractères spécifié au début d'une chaine
        /// </summary>
        /// <param name="source">Chaine de caractères sources, contenant les caractères à supprimer</param>
        /// <param name="chaine">Chaine de caractères à supprimer</param>
        public static string TrimStart(this string source, string chaine)
        {
            string retour = source;
            bool boucle = true;

            while (boucle)
            {
                boucle = false;
                if (retour.StartsWith(chaine))
                {
                    boucle = true;
                    retour = retour.Substring(chaine.Length);
                }
            }

            return retour;
        }

        /// <summary>
        /// Supprime les caractères spécifié à la fin d'une chaine
        /// </summary>
        /// <param name="source">Chaine de caractères sources, contenant les caractères à supprimer</param>
        /// <param name="chaine">Chaine de caractères à supprimer</param>
        public static string TrimEnd(this string source, string chaine)
        {
            string retour = source;
            bool boucle = true;

            while (boucle)
            {
                boucle = false;
                if (retour.EndsWith(chaine))
                {
                    boucle = true;
                    retour = retour.Substring(0, retour.Length - chaine.Length);
                }
            }

            return retour;
        }

        /// <summary>
        /// Supprime tous les doublons, boucle tant qu'ils y en a
        /// Exemple : supprime 3, 4, 5, ... même caractère spécifié, à la suite jusqu'à ce qu'il n'en reste plus qu'un
        /// </summary>
        /// <param name="source">Chaine de caractères contenant les doublons à supprimer</param>
        /// <param name="caractere">Caractère dont il faut supprimer les doulons à la suite</param>
        public static string SupprimeDoublons(this string source, char caractere)
        {
            string retour = source;
            while (retour.IndexOf(caractere.ToString() + caractere.ToString()) >= 0)
                retour = retour.Replace(caractere.ToString() + caractere.ToString(), caractere.ToString());
            return retour;
        }

        /// <summary>
        /// Retournela même chaine mais tronquée si jamais elle dépasse la taille maximum donnée en paramètre
        /// </summary>
        /// <param name="texte">Chaine a tronquer</param>
        /// <param name="tailleMaximum">Taille maximum acceptée de la chaine</param>
        public static string RetourneTailleMax(this string texte, int tailleMaximum)
        {
            if (texte == null)
                throw new ArgumentNullException(nameof(texte));
            if (texte.Length > tailleMaximum)
                return texte.Substring(0, tailleMaximum);
            else
                return texte;
        }

        /// <summary>
        /// Retourne une chaine de caractères, avec des caractères aléatoire (lettres et chiffres), de 10 caractères taille spécifiée
        /// </summary>
        public static string RandomString()
        {
            return RandomString(10, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        }

        /// <summary>
        /// Retourne une chaine de caractères, avec des caractères aléatoire (lettres et chiffres), d'une taille spécifiée
        /// </summary>
        /// <param name="taille">Taille de la chaine de caractère</param>
        public static string RandomString(int taille)
        {
            return RandomString(taille, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        }

        /// <summary>
        /// Retourne une chaine de caractères, avec des caractères aléatoire (lettres et chiffres), d'une taille spécifiée
        /// </summary>
        /// <param name="taille">Taille de la chaine de caractère</param>
        /// <param name="caracteresAutorises">Liste des caractères possibles pour générer la chaine de caractère aléatoire</param>
        public static string RandomString(int taille, string caracteresAutorises)
        {
            return new string(Enumerable.Repeat(caracteresAutorises, taille).Select(s => s[new Random(Guid.NewGuid().GetHashCode()).Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Retourne un tableau de chaines qui contient les sous-chaines de cette chaine, délimitées par la chaine spécifiée
        /// </summary>
        /// <param name="chaine">Chaine à séparer</param>
        /// <param name="separateur">Chaine séparateur</param>
        public static string[] Split(this string chaine, string separateur)
        {
            return Split(chaine, separateur, StringSplitOptions.None);
        }

        /// <summary>
        /// Retourne un tableau de chaines qui contient les sous-chaines de cette chaine, délimitées par la chaine spécifiée
        /// </summary>
        /// <param name="chaine">Chaine à séparer</param>
        /// <param name="separateur">Chaine séparateur</param>
        /// <param name="option">Option pour retourner ou non les sous-chaines vides</param>
        public static string[] Split(this string chaine, string separateur, StringSplitOptions option)
        {
            List<string> retour = new();

            while (chaine.IndexOf(separateur) >= 0)
            {
                retour.Add(chaine.Substring(0, chaine.IndexOf(separateur)));
                chaine = chaine.Substring(chaine.IndexOf(separateur) + separateur.Length);
            }
            retour.Add(chaine);
            if (option == StringSplitOptions.RemoveEmptyEntries)
                retour.RemoveAll(ligne => string.IsNullOrWhiteSpace(ligne));

            return retour.ToArray();
        }

        /// <summary>
        /// Retourne le mot précédent dans une chaine de caractères, à partir de la position donnée
        /// </summary>
        /// <param name="chaine">Chaine de caractères</param>
        /// <param name="position">Position dans la chaine de caractère ou commencer à rechercher le mot précédent</param>
        public static string MotPrecedent(this string chaine, ref int position)
        {
            return chaine.MotPrecedent(ref position, true);
        }

        /// <summary>
        /// Retourne le mot précédent dans une chaine de caractères, à partir de la position donnée
        /// </summary>
        /// <param name="chaine">Chaine de caractères</param>
        /// <param name="position">Position dans la chaine de caractère ou commencer à rechercher le mot précédent</param>
        /// <param name="ignoreRetourALaLigne">Ignore ou non les retours à la ligne</param>
        public static string MotPrecedent(this string chaine, ref int position, bool ignoreRetourALaLigne)
        {
            string retour = "";
            if (string.IsNullOrWhiteSpace(chaine))
                return null;
            if (position > chaine.Length - 1)
                position = chaine.Length - 1;
            if (position == 0)
                return retour;
            while ((chaine[position] != ' ' && position > 0 && ignoreRetourALaLigne && chaine[position] != '\r' && chaine[position] != '\n') || retour == "")
            {
                retour = chaine[position] + retour.Trim();
                position--;
            }
            return retour;
        }

        /// <summary>
        /// Retourne le mot suivant dans une chaine de caractères, à partir de la position donnée
        /// </summary>
        /// <param name="chaine">Chaine de caractères</param>
        /// <param name="position">Position dans la chaine de caractère ou commencer à rechercher le mot suivant</param>
        public static string MotSuivant(this string chaine, ref int position)
        {
            return chaine.MotSuivant(ref position, true);
        }

        /// <summary>
        /// Retourne le mot suivant dans une chaine de caractères, à partir de la position donnée
        /// </summary>
        /// <param name="chaine">Chaine de caractères</param>
        /// <param name="position">Position dans la chaine de caractère ou commencer à rechercher le mot suivant</param>
        /// <param name="ignoreRetourALaLigne">Ignore ou non les retours à la ligne</param>
        public static string MotSuivant(this string chaine, ref int position, bool ignoreRetourALaLigne)
        {
            StringBuilder retour = new();
            if (string.IsNullOrWhiteSpace(chaine))
                return null;
            if (position > chaine.Length - 1)
                return retour.ToString();
            while ((chaine[position] != ' ' && position < chaine.Length - 1 && ignoreRetourALaLigne && chaine[position] != '\r' && chaine[position] != '\n') || retour.Length == 0)
            {
                retour.Append(chaine[position].ToString().Trim());
                position++;
            }
            return retour.ToString();
        }
    }
}
