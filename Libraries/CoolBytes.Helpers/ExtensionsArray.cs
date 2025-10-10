using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolBytes.Helpers;

public static class ExtensionsArray
{
    public static T[] RemoveRange<T>(this T[] list, int index, int count)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

#pragma warning disable S112 // C'est pourtant la bonne exception. Il n'y en a pas de meilleur
        if (index > list.Length - 1)
            throw new IndexOutOfRangeException();
#pragma warning restore S112

        T[] ret = list;
        for (int i = index; i < index + count; i++)
            if (i < ret.Length)
                ret = ret.RemoveAt(index);
        return ret;
    }

    public static T[] RemoveAt<T>(this T[] list, int index)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
#pragma warning disable S112 // C'est pourtant la bonne exception. Il n'y en a pas de meilleur
        if (index >= list.Length)
            throw new IndexOutOfRangeException();
        if (list.Length == 0)
            throw new IndexOutOfRangeException();
#pragma warning restore S112 // C'est pourtant la bonne exception. Il n'y en a pas de meilleur

        T[] newListe = new T[list.Length - 1];
        for (int i = 0; i < list.Length; i++)
            if (i != index) newListe[i > index ? i - 1 : i] = list[i];

        return newListe;
    }

    public static T[] Remove<T>(this T[] list, object objectToRemove, bool all = true)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        if (objectToRemove == null)
            throw new ArgumentNullException(nameof(objectToRemove));

        if (list.Length > 0)
        {
            bool find = true;
            while (find)
            {
                find = false;
                for (int i = 0; i < list.Length; i++)
                    if (list[i] is not null && list[i].Equals(objectToRemove))
                    {
                        if (all)
                            find = true;
                        list = list.RemoveAt(i);
                        break;
                    }
            }
        }

        return list;
    }

    public static T[] RemoveAllNull<T>(this T[] list)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        if (list.Length > 0)
        {
            bool find = true;
            while (find)
            {
                find = false;
                for (int i = 0; i < list.Length; i++)
                    if (list[i] is not null)
                    {
                        find = true;
                        list = list.RemoveAt(i);
                        break;
                    }
            }
        }

        return list;
    }

    public static T[] RemoveAll<T>(this T[] list, Predicate<T> predicate)
    {
        if (list == null)
            return [];
        if (list.Length == 0)
            return list;

        for (int i = list.Length - 1; i >= 0; i--)
            if (predicate(list[i]))
                list = list.RemoveAt(i);
        return list;
    }

    public static T[] Add<T>(this T[] list, object objectToAdd)
    {
        if (objectToAdd == null)
            throw new ArgumentNullException(nameof(objectToAdd));

        if (typeof(T) == objectToAdd.GetType() || objectToAdd.GetType().IsSubclassOf(typeof(T)))
        {
            Array.Resize(ref list, list.Length + 1);
            list[list.Length - 1] = (T)objectToAdd;
        }
        return list;
    }

    public static T[] AddRange<T>(this T[] list, object[] objectsToAdd)
    {
        if (objectsToAdd == null)
            throw new ArgumentNullException(nameof(objectsToAdd));

        foreach (T curObj in objectsToAdd.Select(v => (T)v))
        {
            if (typeof(T) == curObj.GetType() || curObj.GetType().IsSubclassOf(typeof(T)))
            {
                Array.Resize(ref list, list.Length + 1);
                list[list.Length - 1] = curObj;
            }
        }
        return list;
    }

    public static T[] Insert<T>(this T[] list, object objectToInsert, int position)
    {
        if (objectToInsert == null)
            throw new ArgumentNullException(nameof(objectToInsert));

        if (position > list.Length - 1)
            position = list.Length;
        if (typeof(T) == objectToInsert.GetType() || objectToInsert.GetType().IsSubclassOf(typeof(T)))
        {
            Array.Resize(ref list, list.Length + 1);
            for (int i = list.Length - 1; i > position; i--)
                list[i] = list[i - 1];
            list[position] = (T)objectToInsert;
        }
        return list;
    }

    public static T[] InsertRange<T>(this T[] list, object[] objectsToInsert, int position)
    {
        if (objectsToInsert == null)
            throw new ArgumentNullException(nameof(objectsToInsert));

        if (position > list.Length - 1)
            position = list.Length;
        foreach (T curObj in objectsToInsert.Select(v => (T)v))
        {
            if (typeof(T) == curObj.GetType() || curObj.GetType().IsSubclassOf(typeof(T)))
            {
                Array.Resize(ref list, list.Length + 1);
                for (int i = list.Length - 1; i > position; i--)
                    list[i] = list[i - 1];
                list[position] = curObj;
            }
        }
        return list;
    }

    public static string HexToString(this byte[] buffer)
    {
        return buffer.HexToString(",");
    }

    public static string HexToString(this byte[] buffer, string separator)
    {
        if (buffer == null || buffer.Length == 0)
            return "";
        StringBuilder retour = new();
        foreach (byte b in buffer)
        {
            if (retour.Length > 0)
                retour.Append(separator);
            retour.Append($"0x{b:X}");
        }
        return retour.ToString();
    }

    public static T[] FromStart<T>(this T[] list, int start)
    {
        T[] newList = list;
        if (start > 0)
            for (int i = 1; i <= start; i++)
                newList = newList.RemoveAt(0);
        return newList;
    }

    public static T[] ToEnd<T>(this T[] list, int end)
    {
        T[] newList = list;
        if (end < list.Length - 1)
            Array.Resize(ref newList, list.Length - end);
        return newList;
    }

    public static T[] FromEnd<T>(this T[] list, int nb)
    {
        T[] newList = list;
        if (nb < list.Length)
            for (int i = 1; i <= list.Length - nb; i++)
                newList = newList.RemoveAt(0);
        return newList;
    }

    public static T1[] Distinct<T1, T2>(this T1[] list, Func<T1, T2> unique)
    {
        T1[] newList = [];
        HashSet<T2> cle = [];
        foreach (T1 item in list.Where(i => cle.Add(unique(i))))
            newList = newList.Add(item);
        return newList;
    }

    public static bool Contains(this object[] args, object objectToSearch, StringComparison comparisonMethod = StringComparison.OrdinalIgnoreCase)
    {
        if (args != null && args.Length > 0)
            if (objectToSearch is string str)
            {
                return args.OfType<string>().Any(s => s.Equals(str, comparisonMethod));
            }
            else
            {
                return Array.Exists(args, obj => obj == objectToSearch);
            }
        return false;
    }

    public static IEnumerable<T> AsEnumerable<T>(this Array array)
    {
        return array.OfType<T>();
    }

    public static List<T> ToList<T>(this Array array)
    {
        return [.. array.AsEnumerable<T>()];
    }
}
