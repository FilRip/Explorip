using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorip.Helpers
{
    /// <summary>
    /// Classe contenant des méthodes d'extension de gestion des tableaux
    /// </summary>
    public static class ExtensionsArray
    {
        /// <summary>
        /// Supprime une liste d'objet qui se suive d'un tableau
        /// </summary>
        /// <typeparam name="T">Type de la liste</typeparam>
        /// <param name="liste">Liste contenant l'index à supprimer</param>
        /// <param name="index">Numéro du début dans le tableau de(s) objet(s) à supprimer de la liste</param>
        /// <param name="nb">Nombre d'éléments à supprimer de la liste</param>
        /// <returns>Un nouveau tableau avec les éléments demandé supprimé</returns>
        public static T[] RemoveRange<T>(this T[] liste, int index, int nb)
        {
            if (liste == null)
                throw new ArgumentNullException(nameof(liste));

#pragma warning disable S112 // C'est pourtant la bonne exception. Il n'y en a pas de meilleur
            if (index > liste.Length - 1)
                throw new IndexOutOfRangeException();
#pragma warning restore S112

            T[] retour = liste;
            for (int i = index; i < index + nb; i++)
                if (i < retour.Length)
                    retour = retour.RemoveAt(index);
            return retour;
        }

        /// <summary>
        /// Supprime un objet du tableau, en fonction de sa position
        /// </summary>
        /// <typeparam name="T">Type de la liste</typeparam>
        /// <param name="liste">Liste contenant l'index à supprimer</param>
        /// <param name="index">Numéro index dans le tableau de l'objet à supprimer de la liste</param>
        /// <returns>Un nouveau tableau, sans l'index spécifié</returns>
        public static T[] RemoveAt<T>(this T[] liste, int index)
        {
            if (liste == null)
                throw new ArgumentNullException(nameof(liste));
#pragma warning disable S112 // C'est pourtant la bonne exception. Il n'y en a pas de meilleur
            if (index >= liste.Length)
                throw new IndexOutOfRangeException();
            if (liste.Length == 0)
                throw new IndexOutOfRangeException();
#pragma warning restore S112 // C'est pourtant la bonne exception. Il n'y en a pas de meilleur

            T[] newListe = new T[liste.Length - 1];
            if (liste.Length > 0)
                for (int i = 0; i < liste.Length; i++)
                    if (i != index) newListe[(i > index ? i - 1 : i)] = liste[i];

            return newListe;
        }

        /// <summary>
        /// Supprime un objet spécifié du tableau
        /// </summary>
        /// <typeparam name="T">Type du tableau</typeparam>
        /// <param name="liste">Le tableau</param>
        /// <param name="objetASupprimer">Objet à supprimer</param>
        /// <param name="toutesLesOccurences">Vrai pour supprimer toutes les occurences de cet objet du tableau, sinon Faux pour ne supprimer que le premier trouvé</param>
        /// <returns>Un nouveau tableau, sans l'objet spécifié</returns>
        public static T[] Remove<T>(this T[] liste, object objetASupprimer, bool toutesLesOccurences = true)
        {
            if (liste == null) throw new ArgumentNullException(nameof(liste));
            if (objetASupprimer == null) throw new ArgumentNullException(nameof(objetASupprimer));

            if (liste.Length > 0)
            {
                bool trouve = true;
                while (trouve)
                {
                    trouve = false;
                    for (int i = 0; i < liste.Length; i++)
                        if ((liste[i] != null) && (liste[i].Equals(objetASupprimer)))
                        {
                            if (toutesLesOccurences) trouve = true;
                            liste = liste.RemoveAt(i);
                            break;
                        }
                }
            }

            return liste;
        }

        /// <summary>
        /// Supprime toutes les occurences de null(nothing) du tableau
        /// </summary>
        /// <typeparam name="T">Type du tableau</typeparam>
        /// <param name="liste">Le tableau</param>
        /// <returns>Un nouveau tableau, sans les valeurs à null</returns>
        public static T[] RemoveAllNull<T>(this T[] liste)
        {
            if (liste == null) throw new ArgumentNullException(nameof(liste));

            if (liste.Length > 0)
            {
                bool trouve = true;
                while (trouve)
                {
                    trouve = false;
                    for (int i = 0; i < liste.Length; i++)
                        if (liste[i] == null)
                        {
                            trouve = true;
                            liste = liste.RemoveAt(i);
                            break;
                        }
                }
            }

            return liste;
        }

        /// <summary>
        /// Retourne une liste après avoir supprimé tous les éléments de la liste dont la(les) condition(s) du prédicat sont remplies
        /// </summary>
        /// <typeparam name="T">Type d'objet dans la liste</typeparam>
        /// <param name="liste">Liste à parcourir</param>
        /// <param name="predicate">Predicat (conditions) à remplir pour que l'élément soit supprimé</param>
        public static T[] RemoveAll<T>(this T[] liste, Predicate<T> predicate)
        {
            if (liste == null)
                return null;
            if (liste.Length == 0)
                return liste;

            for (int i = liste.Length - 1; i >= 0; i--)
                if (predicate(liste[i]))
                    liste = liste.RemoveAt(i);
            return liste;
        }

        /// <summary>
        /// Ajoute un élément en fin de tableau
        /// </summary>
        /// <typeparam name="T">Type d'objet du tableau</typeparam>
        /// <param name="liste">Le tableau</param>
        /// <param name="objetAAjouter">L'objet à ajouter au tableau</param>
        /// <returns>Un nouveau tableau, avec l'objet ajouté</returns>
        public static T[] Add<T>(this T[] liste, object objetAAjouter)
        {
            if (objetAAjouter == null)
                throw new ArgumentNullException(nameof(objetAAjouter));

            if ((typeof(T) == objetAAjouter.GetType()) || (objetAAjouter.GetType().IsSubclassOf(typeof(T))))
            {
                Array.Resize(ref liste, liste.Length + 1);
                liste[liste.Length - 1] = (T)objetAAjouter;
            }
            return liste;
        }

        /// <summary>
        /// Ajoute des éléments en fin de tableau
        /// </summary>
        /// <typeparam name="T">Type d'objet du tableau</typeparam>
        /// <param name="liste">Le tableau</param>
        /// <param name="objetsAAjouter">Les objets à ajouter au tableau</param>
        /// <returns>Un nouveau tableau, avec les objets ajoutés</returns>
        public static T[] AddRange<T>(this T[] liste, object[] objetsAAjouter)
        {
            if (objetsAAjouter == null)
                throw new ArgumentNullException(nameof(objetsAAjouter));

            foreach (T objet in objetsAAjouter.Select(v => (T)v))
            {
                if ((typeof(T) == objet.GetType()) || (objet.GetType().IsSubclassOf(typeof(T))))
                {
                    Array.Resize(ref liste, liste.Length + 1);
                    liste[liste.Length - 1] = objet;
                }
            }
            return liste;
        }

        /// <summary>
        /// Insère un élément dans le tableau à la position indiquée
        /// </summary>
        /// <typeparam name="T">Type d'objet du tableau</typeparam>
        /// <param name="liste">Le tableau</param>
        /// <param name="objetAInserer">L'objet à insérer au tableau</param>
        /// <param name="position">Position dans le tableau ou insérer l'objet (de base zéro). Si la position est supérieur à la taille du tableau, il sera ajouté à la fin</param>
        /// <returns>Un nouveau tableau, avec l'objet inséré</returns>
        public static T[] Insert<T>(this T[] liste, object objetAInserer, int position)
        {
            if (objetAInserer == null) throw new ArgumentNullException(nameof(objetAInserer));

            if (position > liste.Length - 1) position = liste.Length;
            if ((typeof(T) == objetAInserer.GetType()) || (objetAInserer.GetType().IsSubclassOf(typeof(T))))
            {
                Array.Resize(ref liste, liste.Length + 1);
                for (int i = liste.Length - 1; i > position; i--)
                    liste[i] = liste[i - 1];
                liste[position] = (T)objetAInserer;
            }
            return liste;
        }

        /// <summary>
        /// Insère des éléments dans le tableau à la position indiquée
        /// </summary>
        /// <typeparam name="T">Type d'objet du tableau</typeparam>
        /// <param name="liste">Le tableau</param>
        /// <param name="objetsAInserer">Les objets à insérer au tableau</param>
        /// <param name="position">Position dans le tableau ou insérer les objets (de base zéro). Si la position est supérieur à la taille du tableau, il seront ajouté à la fin</param>
        /// <returns>Un nouveau tableau, avec les objets insérés</returns>
        public static T[] InsertRange<T>(this T[] liste, object[] objetsAInserer, int position)
        {
            if (objetsAInserer == null)
                throw new ArgumentNullException(nameof(objetsAInserer));

            if (position > liste.Length - 1) position = liste.Length;
            foreach (T objet in objetsAInserer.Select(v => (T)v))
            {
                if ((typeof(T) == objet.GetType()) || (objet.GetType().IsSubclassOf(typeof(T))))
                {
                    Array.Resize(ref liste, liste.Length + 1);
                    for (int i = liste.Length - 1; i > position; i--)
                        liste[i] = liste[i - 1];
                    liste[position] = objet;
                }
            }
            return liste;
        }

        /// <summary>
        /// Retourne une suite de valeur hexadecimale du buffer d'octets sous forme d'une chaine de caractère, séparées par une virgule
        /// </summary>
        /// <param name="buffer">buffer d'octet (de byte) source</param>
        public static string HexToString(this byte[] buffer)
        {
            return HexToString(buffer, ",");
        }

        /// <summary>
        /// Retourne une suite de valeur hexadecimale du buffer d'octets sous forme d'une chaine de caractère
        /// </summary>
        /// <param name="buffer">buffer d'octet (de byte) source</param>
        /// <param name="separateur">Séparateur à inserer entre chaque valeur</param>
        public static string HexToString(this byte[] buffer, string separateur)
        {
            if ((buffer == null) || (buffer.Length == 0))
                return "";
            StringBuilder retour = new();
            foreach (byte octet in buffer)
            {
                if (retour.Length > 0)
                    retour.Append(separateur);
                retour.Append($"0x{octet:X}");
            }
            return retour.ToString();
        }

        /// <summary>
        /// Retourne le contenu du tableau en partant du premier indice donné en paramètre
        /// </summary>
        /// <typeparam name="T">Type du tableau</typeparam>
        /// <param name="liste">Tableau source</param>
        /// <param name="start">Position de départ par rapport à zéro (par rapport au premier élement du tableau=</param>
        public static T[] FromStart<T>(this T[] liste, int start)
        {
            T[] nouvelleListe = liste;
            if (start > 0)
                for (int i = 1; i <= start; i++)
                    nouvelleListe = nouvelleListe.RemoveAt(0);
            return nouvelleListe;
        }

        /// <summary>
        /// Retourne le contenu du tableau en partant du premier indice jusqu'à la fin moins le nombre spécifié en paramètre
        /// </summary>
        /// <typeparam name="T">Type du tableau</typeparam>
        /// <param name="liste">Tableau source</param>
        /// <param name="end">Nombre d'élémente du tableau, en partant de la fin, à retourner</param>
        public static T[] ToEnd<T>(this T[] liste, int end)
        {
            T[] nouvelleListe = liste;
            if (end < liste.Length - 1)
                Array.Resize(ref nouvelleListe, liste.Length - end);
            return nouvelleListe;
        }

        /// <summary>
        /// Retourne le contenu du tableau en partant de la fin moins le nombre spécifié en paramètre
        /// </summary>
        /// <typeparam name="T">Type du tableau</typeparam>
        /// <param name="liste">Tableau source</param>
        /// <param name="nb">Nombre d'élémente du tableau, en partant de la fin, à retourner</param>
        public static T[] FromEnd<T>(this T[] liste, int nb)
        {
            T[] nouvelleListe = liste;
            if (nb < liste.Length)
                for (int i = 1; i <= liste.Length - nb; i++)
                    nouvelleListe = nouvelleListe.RemoveAt(0);
            return nouvelleListe;
        }

        /// <summary>
        /// Effectue un distinct sur un tableau d'élément d'après une propriété des l'éléments
        /// </summary>
        /// <typeparam name="T1">Type de l'élement</typeparam>
        /// <typeparam name="T2">Type de la propriété de l'élément à comparer</typeparam>
        /// <param name="liste">Le tableau d'élément</param>
        /// <param name="unique">Function retournant la propriété de l'élément à comparer</param>
        public static T1[] Distinct<T1, T2>(this T1[] liste, Func<T1, T2> unique)
        {
            T1[] nouvelleListe = new T1[0] { };
            HashSet<T2> cle = new();
            foreach (T1 item in liste.Where(i => cle.Add(unique(i))))
                nouvelleListe = nouvelleListe.Add(item);
            return nouvelleListe;
        }
    }
}
