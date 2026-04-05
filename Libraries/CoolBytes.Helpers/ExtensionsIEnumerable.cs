using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolBytes.Helpers;

public static class ExtensionsIEnumerable
{
    public static Type GetTypeOfList(this ICollection list)
    {
        if (list is IList)
            return list.GetType().GetGenericArguments()[0];
        else
            return null;
    }

    public static Type GetKeyTypeOfDictionary(this ICollection dictionary)
    {
        if (dictionary is IDictionary)
            return dictionary.GetType().GetGenericArguments()[0];
        else
            return null;
    }

    public static Type GetValueTypeOfDictionary(this ICollection dictionary)
    {
        if (dictionary is IDictionary)
            return dictionary.GetType().GetGenericArguments()[1];
        else
            return null;
    }

    public static string ToLongString(this ICollection @object)
    {
        string result = "";
        try
        {
            if (@object is IList list)
                result = DumpList<object>(list);
            else if (@object is IDictionary dict)
                result = DumpDictionnaire<object, object>(dict);
            else
                result = @object == null ? "null" : @object.ToString();
        }
        catch (Exception) { /* On ignore les erreurs, généralement un élément de la liste ne peut pas être casté en string */ }
        return result;
    }

    public static string ToLongString<T1, T2>(this Dictionary<T1, T2> dictionary)
    {
        string result = "";
        if (dictionary != null)
            result = DumpDictionnaire<T1, T2>(dictionary);
        return result;
    }

    public static string ToLongString<T1>(this List<T1> list)
    {
        string result = "";
        if (list != null)
            result = DumpList<T1>(list);
        return result;
    }

    private static string DumpList<T>(IList list)
    {
        StringBuilder result = new();
        foreach (T value in list)
        {
            if (result.Length > 0)
                result.Append(", ");
            result.Append(value is null ? "null" : value.ToString());
        }
        return result.ToString();
    }

    private static string DumpDictionnaire<T1, T2>(IDictionary dictionary)
    {
        StringBuilder result = new();
        T2 value;
        foreach (T1 key in dictionary.Keys.OfType<T1>())
        {
            value = (T2)dictionary[key];
            if (result.Length > 0)
                result.Append(", ");
            result.Append($"{key}={(value is null ? "null" : value.ToString())}");
        }
        return result.ToString();
    }

    public static bool Contains(this List<string> list, string subString, StringComparison comparer)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        if (string.IsNullOrWhiteSpace(subString))
            throw new ArgumentNullException(nameof(subString));
        return list.Exists(item => item.Equals(subString, comparer));
    }

    public static int Remove(this List<string> list, string subString, StringComparison comparer)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        if (subString == null)
            throw new ArgumentNullException(nameof(subString));
        return list.RemoveAll(item => item.Equals(subString, comparer));
    }

    public static Dictionary<TKey, TValue> Addition<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> toAdd) where TValue : struct
    {
        Dictionary<TKey, TValue> result = source;
        result ??= [];

        if (toAdd == null || toAdd.Count == 0)
            return result;

        foreach (TKey key in toAdd.Keys)
            if (result.ContainsKey(key))
                result[key] = (TValue)Convert.ChangeType(double.Parse(result[key].ToString()) + double.Parse(toAdd[key].ToString()), typeof(TValue));
            else
                result.Add(key, toAdd[key]);

        return result;
    }

    public static List<T1> Distinct<T1, T2>(this List<T1> source, Func<T1, T2> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        List<T1> result = [];
        HashSet<T2> key = [];
        foreach (T1 item in source.Where(i => key.Add(predicate(i))))
            result.Add(item);
        return result;
    }

    public static IEnumerable<T1> Distinct<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return Distinct([.. source], predicate).AsEnumerable();
    }

    public static T Random<T>(this IEnumerable<T> list, int min = int.MinValue, int max = int.MaxValue)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        if (!list.Any())
            return default;
        if (max > list.Count())
            max = list.Count();
        if (min < 0)
            min = 0;
        Random random = new(Guid.NewGuid().GetHashCode());
        int value = random.Next(min, max);
        return list.ElementAt(value);
    }

    public static void MoveRange<T>(this List<T> list, int start, int count, int newStart)
    {
        List<T> backup = list.GetRange(start, count);
        list.RemoveRange(start, count);
        list.InsertRange(newStart, backup);
    }

    public static List<T> ToList<T>(this ICollection list)
    {
        if (list == null)
            return [];
        List<T> result = [];
        IEnumerator enumerator = list.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is T item)
                result.Add(item);
        }
        return result;
    }

    public static int RemoveAll<T>(this IList<T> list, Predicate<T> predicate)
    {
        if (list == null || list.Count == 0)
            return 0;

        int nbDeleted = 0;
        for (int i = list.Count - 1; i >= 0; i--)
            if (predicate(list[i]))
            {
                list.RemoveAt(i);
                nbDeleted++;
            }

        return nbDeleted;
    }
}
