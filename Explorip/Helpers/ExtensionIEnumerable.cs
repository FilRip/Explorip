using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilRip.Utils.Extensions
{
    /// <summary>
    /// Classe d'extension des types de classes Enumerable (List, Dictionary) pour encapsuler des méthodes ToString plus explicite pour ces classes
    /// </summary>
    public static class ExtensionIEnumerable
    {
        /// <summary>
        /// Indique si un objet est de type Dictionary (générique)
        /// </summary>
        /// <param name="objet">Objet à tester</param>
        /// <returns>True si 'objet' est de type Dictionary, sinon False</returns>
        public static bool IsDictionary(this object objet)
        {
            return (objet != null && objet.GetType().IsGenericType && objet.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>));
        }

        /// <summary>
        /// Indique si un objet est de type List (générique)
        /// </summary>
        /// <param name="objet">Objet à tester</param>
        /// <returns>True si 'objet' est de type List, sinon False</returns>
        public static bool IsList(this object objet)
        {
            return (objet != null && objet.GetType().IsGenericType && objet.GetType().GetGenericTypeDefinition() == typeof(List<>));
        }

        /// <summary>
        /// Retourne le type d'une List générique
        /// </summary>
        /// <param name="maListe">Objet de type List source</param>
        /// <returns>Le type des objets stockés dans cette List</returns>
        public static Type GetTypeOfList(this ICollection maListe)
        {
            if (IsList(maListe))
                return maListe.GetType().GetGenericArguments()[0];
            else
                return null;
        }

        /// <summary>
        /// Retourne le type de l'objet clé d'un objet Dictionary générique
        /// </summary>
        /// <param name="monDictionnaire">Objet de type Dictionary source</param>
        /// <returns>Le type des objets stockés en clé dans ce Dictionary</returns>
        public static Type GetKeyTypeOfDictionary(this ICollection monDictionnaire)
        {
            if (IsDictionary(monDictionnaire))
                return monDictionnaire.GetType().GetGenericArguments()[0];
            else
                return null;
        }

        /// <summary>
        /// Retourne le type de l'objet valeur d'un objet Dictionary générique
        /// </summary>
        /// <param name="monDictionnaire">Objet de type Dictionary source</param>
        /// <returns>Le type des objets stockés en valeur dans ce Dictionary</returns>
        public static Type GetValueTypeOfDictionary(this ICollection monDictionnaire)
        {
            if (IsDictionary(monDictionnaire))
                return monDictionnaire.GetType().GetGenericArguments()[1];
            else
                return null;
        }

        /// <summary>
        /// Retourne une réprésentation en chaine de caractères, d'un objet de type collection générique (List, Dictionary)
        /// </summary>
        /// <param name="objet">Objet collection source</param>
        /// <returns>Chaine de caractères contenant chaque élément de la collection source</returns>
        public static string ToLongString(this ICollection objet)
        {
            string retour = "";
            try
            {
                if (IsList(objet))
                    retour = DumpList<object>((IList)objet);
                else if (IsDictionary(objet))
                    retour = DumpDictionnaire<object, object>((IDictionary)objet);
                else
                    retour = (objet == null ? "null" : objet.ToString());
            }
            catch (Exception) { /* On ignore les erreurs, généralement un élément de la liste ne peut pas être casté en string */ }
            return retour;
        }

        /// <summary>
        /// Retourne une réprésentation en chaine de caractères, d'un objet de type dictionary générique
        /// </summary>
        /// <param name="dictionnaire">Objet Dictionary source</param>
        /// <returns>Chaine de caractères contenant chaque élément du dictionnaire source</returns>
        public static string ToLongString<T1, T2>(this Dictionary<T1, T2> dictionnaire)
        {
            string retour = "";
            if (dictionnaire != null)
                retour = DumpDictionnaire<T1, T2>(dictionnaire);
            return retour;
        }

        /// <summary>
        /// Retourne une réprésentation en chaine de caractères, d'un objet de type List générique
        /// </summary>
        /// <param name="liste">Objet List source</param>
        /// <returns>Chaine de caractères contenant chaque élément de la liste source</returns>
        public static string ToLongString<T1>(this List<T1> liste)
        {
            string retour = "";
            if (liste != null)
                retour = DumpList<T1>(liste);
            return retour;
        }

        private static string DumpList<T>(IList maListe)
        {
            StringBuilder retour = new();
            foreach (T valeur in maListe)
            {
                if (retour.Length > 0)
                    retour.Append(", ");
                retour.Append(valeur == null ? "null" : valeur.ToString());
            }
            return retour.ToString();
        }

        private static string DumpDictionnaire<T1, T2>(IDictionary monDictionnaire)
        {
            StringBuilder retour = new();
            T2 valeur;
            foreach (T1 cle in monDictionnaire.Keys)
            {
                if (cle != null)
                {
                    valeur = (T2)monDictionnaire[cle];
                    if (retour.Length > 0)
                        retour.Append(", ");
                    retour.Append($"{cle}={(valeur == null ? "null" : valeur.ToString())}");
                }
            }
            return retour.ToString();
        }

        /// <summary>
        /// Retourne si une chaine de charactères est présente dans une liste en spécifiant si on doit prendre en compte la culture et la casse
        /// </summary>
        /// <param name="liste">Liste de chaines en entrée</param>
        /// <param name="sousChaine">La chaine de caractères à rechercher</param>
        /// <param name="comparateur">Comparateur à utiliser (prendre en compte ou pas : la culture, la casse)</param>
        /// <returns></returns>
        public static bool Contains(this List<string> liste, string sousChaine, StringComparison comparateur)
        {
            if (liste == null) throw new ArgumentNullException(nameof(liste));
            if (string.IsNullOrWhiteSpace(sousChaine)) throw new ArgumentNullException(nameof(sousChaine));
            return liste.Any(item => item.Equals(sousChaine, comparateur));
        }

        /// <summary>
        /// Supprime toutes itérations d'une chaine de caractères dans une liste de chaines de caractères
        /// En spécifiant si on doit tenir compte de la culture et de la casse
        /// </summary>
        /// <param name="liste">Liste de chaines de caractères en entrée</param>
        /// <param name="sousChaine">La chaine de caractères à supprimer</param>
        /// <param name="comparateur">Comparateur à utiliser (prendre compte ou pas : la culture, la casse)</param>
        /// <returns>Le nombre d'occurence supprimée</returns>
        public static int Remove(this List<string> liste, string sousChaine, StringComparison comparateur)
        {
            if (liste == null) throw new ArgumentNullException(nameof(liste));
            if (sousChaine == null) throw new ArgumentNullException(nameof(sousChaine));
            return liste.RemoveAll(item => item.Equals(sousChaine, comparateur));
        }

        /// <summary>
        /// Additionne les valeurs d'un dictionaire, en liant les pair de clé identique
        /// </summary>
        /// <typeparam name="TKey">Type de la clé du dictionaire</typeparam>
        /// <typeparam name="TValue">Type de valeur. Doit être une valeur numérique</typeparam>
        /// <param name="source">Dictionaire servant de source de base</param>
        /// <param name="AAjouter">Dictionaire servant de source pour lesquelles les valeurs de ce dictionaire doivent être ajoutées au dictionaire source</param>
        /// <returns>Retourne un dictionaire pourvu du même type et contenant la somme des valeurs pour chaque clé, plus les valeurs se trouvant dans le dictionaire AAjouter mais absent du dictionaire source</returns>
        public static Dictionary<TKey, TValue> Addition<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> AAjouter) where TValue : struct
        {
            Dictionary<TKey, TValue> retour = source ?? new Dictionary<TKey, TValue>();
            if ((AAjouter == null) || (AAjouter.Count == 0))
                return retour;

            foreach (TKey cle in AAjouter.Keys)
                if (retour.ContainsKey(cle))
                    retour[cle] = (TValue)Convert.ChangeType(double.Parse(retour[cle].ToString()) + double.Parse(AAjouter[cle].ToString()), typeof(TValue));
                else
                    retour.Add(cle, AAjouter[cle]);

            return retour;
        }

        /// <summary>
        /// Effectue un distinct sur une liste d'élément d'après une propriété des l'éléments
        /// </summary>
        /// <typeparam name="T1">Type de l'élement</typeparam>
        /// <typeparam name="T2">Type de la propriété de l'élément à comparer</typeparam>
        /// <param name="source">La liste d'élément</param>
        /// <param name="unique">Function retournant la propriété de l'élément à comparer</param>
        public static List<T1> Distinct<T1, T2>(this List<T1> source, Func<T1, T2> unique)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            List<T1> retour = new();
            HashSet<T2> cle = new();
            foreach (T1 item in source.Where(i => cle.Add(unique(i))))
                retour.Add(item);
            return retour;
        }

        /// <summary>
        /// Effectue un distinct sur une liste d'élément d'après une propriété des l'éléments
        /// </summary>
        /// <typeparam name="T1">Type de l'élement</typeparam>
        /// <typeparam name="T2">Type de la propriété de l'élément à comparer</typeparam>
        /// <param name="source">La liste d'élément</param>
        /// <param name="unique">Function retournant la propriété de l'élément à comparer</param>
        public static IEnumerable<T1> Distinct<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> unique)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Distinct(source.ToList(), unique).AsEnumerable();
        }

        /// <summary>
        /// Retourne aléatoirement (après avoir réinitialisé le randomizer) un objet d'une liste
        /// </summary>
        /// <typeparam name="T">Type des objets dans la liste</typeparam>
        /// <param name="liste">liste pour laquelle on veut un objet de cette liste</param>
        /// <param name="min">Valeur minimum (inclus). Si cette valeur est inférieur à zéro, zéro sera pris</param>
        /// <param name="max">Valeur maximum (exclus). Si cette valeur est supérieur au nombre d'objet dans la liste, le nombre d'objet de la liste sera pris</param>
        public static T Random<T>(this IEnumerable<T> liste, int min = int.MinValue, int max = int.MaxValue)
        {
            if (liste == null)
                throw new ArgumentNullException(nameof(liste));
            if (!liste.Any())
                return default;
            if (max > liste.Count())
                max = liste.Count();
            if (min < 0)
                min = 0;
            Random random = new(Guid.NewGuid().GetHashCode());
            int valeur = random.Next(min, max);
            return liste.ElementAt(valeur);
        }

        /// <summary>
        /// Déplace une  liste d'item (qui se suivent) dans la liste
        /// </summary>
        /// <typeparam name="T">Type d'item dans la liste</typeparam>
        /// <param name="list">Liste contenant, entre autres, les items à déplacer</param>
        /// <param name="start">Position du premier élément à déplacer</param>
        /// <param name="count">Nombre d'éléments à déplacer</param>
        /// <param name="newStart">Nouvelle position des éléments dans la liste (liste sans le nombre d'éléments à déplacer)</param>
        public static void MoveRange<T>(this List<T> list, int start, int count, int newStart)
        {
            List<T> backup = list.GetRange(start, count);
            list.RemoveRange(start, count);
            list.InsertRange(newStart, backup);
        }

        /// <summary>
        /// Retourne une liste d'une collection enumerable
        /// </summary>
        /// <typeparam name="T">Type d'objet stockés dans cette liste</typeparam>
        /// <param name="liste">Collection (supportant l'interface IEnumerable)</param>
        public static List<T> ToList<T>(this ICollection liste)
        {
            if (liste == null)
                return null;
            List<T> retour = new();
            IEnumerator enumerer = liste.GetEnumerator();
            while (enumerer.MoveNext())
            {
                if (enumerer.Current is T item)
                    retour.Add(item);
            }
            return retour;
        }
    }
}
