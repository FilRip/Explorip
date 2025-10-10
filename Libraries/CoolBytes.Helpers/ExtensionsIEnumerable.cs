using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolBytes.Helpers;

public static class ExtensionsIEnumerable
{
    public static Type GetTypeOfList(this ICollection maListe)
    {
        if (maListe is IList)
            return maListe.GetType().GetGenericArguments()[0];
        else
            return null;
    }

    public static Type GetKeyTypeOfDictionary(this ICollection monDictionnaire)
    {
        if (monDictionnaire is IDictionary)
            return monDictionnaire.GetType().GetGenericArguments()[0];
        else
            return null;
    }

    public static Type GetValueTypeOfDictionary(this ICollection monDictionnaire)
    {
        if (monDictionnaire is IDictionary)
            return monDictionnaire.GetType().GetGenericArguments()[1];
        else
            return null;
    }

    public static string ToLongString(this ICollection objet)
    {
        string retour = "";
        try
        {
            if (objet is IList list)
                retour = DumpList<object>(list);
            else if (objet is IDictionary dict)
                retour = DumpDictionnaire<object, object>(dict);
            else
                retour = objet == null ? "null" : objet.ToString();
        }
        catch (Exception) { /* On ignore les erreurs, généralement un élément de la liste ne peut pas être casté en string */ }
        return retour;
    }

    public static string ToLongString<T1, T2>(this Dictionary<T1, T2> dictionnaire)
    {
        string retour = "";
        if (dictionnaire != null)
            retour = DumpDictionnaire<T1, T2>(dictionnaire);
        return retour;
    }

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
            retour.Append(valeur is null ? "null" : valeur.ToString());
        }
        return retour.ToString();
    }

    private static string DumpDictionnaire<T1, T2>(IDictionary monDictionnaire)
    {
        StringBuilder retour = new();
        T2 valeur;
        foreach (T1 cle in monDictionnaire.Keys.OfType<T1>())
        {
            valeur = (T2)monDictionnaire[cle];
            if (retour.Length > 0)
                retour.Append(", ");
            retour.Append($"{cle}={(valeur is null ? "null" : valeur.ToString())}");
        }
        return retour.ToString();
    }

    public static bool Contains(this List<string> liste, string sousChaine, StringComparison comparateur)
    {
        if (liste == null) throw new ArgumentNullException(nameof(liste));
        if (string.IsNullOrWhiteSpace(sousChaine)) throw new ArgumentNullException(nameof(sousChaine));
        return liste.Exists(item => item.Equals(sousChaine, comparateur));
    }

    public static int Remove(this List<string> liste, string sousChaine, StringComparison comparateur)
    {
        if (liste == null) throw new ArgumentNullException(nameof(liste));
        if (sousChaine == null) throw new ArgumentNullException(nameof(sousChaine));
        return liste.RemoveAll(item => item.Equals(sousChaine, comparateur));
    }

    public static Dictionary<TKey, TValue> Addition<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> AAjouter) where TValue : struct
    {
        Dictionary<TKey, TValue> retour = source;
        retour ??= [];

        if (AAjouter == null || AAjouter.Count == 0)
            return retour;

        foreach (TKey cle in AAjouter.Keys)
            if (retour.ContainsKey(cle))
                retour[cle] = (TValue)Convert.ChangeType(double.Parse(retour[cle].ToString()) + double.Parse(AAjouter[cle].ToString()), typeof(TValue));
            else
                retour.Add(cle, AAjouter[cle]);

        return retour;
    }

    public static List<T1> Distinct<T1, T2>(this List<T1> source, Func<T1, T2> unique)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        List<T1> retour = [];
        HashSet<T2> cle = [];
        foreach (T1 item in source.Where(i => cle.Add(unique(i))))
            retour.Add(item);
        return retour;
    }

    public static IEnumerable<T1> Distinct<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> unique)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return Distinct([.. source], unique).AsEnumerable();
    }

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

    public static void MoveRange<T>(this List<T> list, int start, int count, int newStart)
    {
        List<T> backup = list.GetRange(start, count);
        list.RemoveRange(start, count);
        list.InsertRange(newStart, backup);
    }

    public static List<T> ToList<T>(this ICollection liste)
    {
        if (liste == null)
            return [];
        List<T> retour = [];
        IEnumerator enumerer = liste.GetEnumerator();
        while (enumerer.MoveNext())
        {
            if (enumerer.Current is T item)
                retour.Add(item);
        }
        return retour;
    }

    public static int RemoveAll<T>(this IList<T> liste, Predicate<T> predicate)
    {
        if (liste == null || liste.Count == 0)
            return 0;

        int nbSuppr = 0;
        for (int i = liste.Count - 1; i >= 0; i--)
            if (predicate(liste[i]))
            {
                liste.RemoveAt(i);
                nbSuppr++;
            }

        return nbSuppr;
    }
}
