﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ExploripConfig.Helpers;

/// <summary>
/// Class for extensions methods for Array
/// </summary>
public static class ExtensionsArray
{
    /// <summary>
    /// Remove a list of items who follow each other, from a start index and a number of items to remove
    /// </summary>
    /// <typeparam name="T">Type of object in Array</typeparam>
    /// <param name="list">Array from where to remove instance</param>
    /// <param name="index">First index in array where start remove items</param>
    /// <param name="nb">Number of items to remove</param>
    /// <returns>A new array without all items specified</returns>
    public static T[] RemoveRange<T>(this T[] list, int index, int nb)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

#pragma warning disable S112 // It's the best exception. There is no better
        if (index > list.Length - 1)
            throw new IndexOutOfRangeException("The index is greater than the number of item in list");
#pragma warning restore S112

        T[] retour = list;
        for (int i = index; i < index + nb; i++)
            if (i < retour.Length)
                retour = retour.RemoveAt(index);
        return retour;
    }

    /// <summary>
    /// Remove an item from an Array by it's index in list
    /// </summary>
    /// <typeparam name="T">Type of object in Array</typeparam>
    /// <param name="list">Array from where to remove instance</param>
    /// <param name="index">Index of item to remove in Array</param>
    /// <returns>A new array without item specified</returns>
    public static T[] RemoveAt<T>(this T[] list, int index)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
#pragma warning disable S112 // It's the best exception to use, remove Sonar warning
        if (index >= list.Length || list.Length == 0)
            throw new IndexOutOfRangeException("The index is greater than the number of item in list");
#pragma warning restore S112

        T[] newListe = new T[list.Length - 1];
        for (int i = 0; i < list.Length; i++)
            if (i != index) newListe[(i > index ? i - 1 : i)] = list[i];

        return newListe;
    }

    /// <summary>
    /// Remove an item of an object from the array
    /// </summary>
    /// <typeparam name="T">Type of object in Array</typeparam>
    /// <param name="list">Array from where to remove instance</param>
    /// <param name="itemToRemove">Instance of object to remove</param>
    /// <param name="allOccurrences">True to remove all iteration from the Array, or False to remove only the first one find</param>
    /// <param name="comparisonType">If it's a list of string, choice the comparison type (culture and case sensitive)</param>
    /// <returns>Return new Array, without the instance of object to remove</returns>
    public static T[] Remove<T>(this T[] list, object itemToRemove, bool allOccurrences = true, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        if (itemToRemove == null)
            throw new ArgumentNullException(nameof(itemToRemove));

        if (list.Length > 0)
        {
            bool find = true;
            while (find)
            {
                find = false;
                for (int i = 0; i < list.Length; i++)
                    if ((list[i] is not null) && (list[i].Equals(itemToRemove) || (typeof(T) == typeof(string) && string.Compare(list[i].ToString(), itemToRemove.ToString(), comparisonType) == 0)))
                    {
                        if (allOccurrences)
                            find = true;
                        list = list.RemoveAt(i);
                        break;
                    }
            }
        }

        return list;
    }

    /// <summary>
    /// Remove all "null" item from an Array
    /// </summary>
    /// <typeparam name="T">Type of object in Array</typeparam>
    /// <param name="list">Array from where to remove instance</param>
    /// <returns>A new array without null instance</returns>
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
                    if (list[i] is null)
                    {
                        find = true;
                        list = list.RemoveAt(i);
                        break;
                    }
            }
        }

        return list;
    }

    /// <summary>
    /// Remove all item that response to a predicate from an array
    /// </summary>
    /// <typeparam name="T">Type of object in Array</typeparam>
    /// <param name="list">Array from where to remove instance</param>
    /// <param name="predicate">Predicat (condition) that item must response to be removed from list</param>
    public static T[] RemoveAll<T>(this T[] list, Predicate<T> predicate)
    {
        if (list == null)
            return null;
        if (list.Length == 0)
            return list;

        for (int i = list.Length - 1; i >= 0; i--)
            if (predicate(list[i]))
                list = list.RemoveAt(i);
        return list;
    }

    /// <summary>
    /// Add an item at the end of Array
    /// </summary>
    /// <typeparam name="T">Type of object in Array</typeparam>
    /// <param name="list">Array from where to add item</param>
    /// <param name="itemToAdd">Item to add to Array</param>
    /// <returns>Return a new Array with the item added to the end</returns>
    public static T[] Add<T>(this T[] list, object itemToAdd)
    {
        if (itemToAdd == null)
            throw new ArgumentNullException(nameof(itemToAdd));

        if ((typeof(T) == itemToAdd.GetType()) || (itemToAdd.GetType().IsSubclassOf(typeof(T))))
        {
            Array.Resize(ref list, list.Length + 1);
            list[list.Length - 1] = (T)itemToAdd;
        }
        return list;
    }

    /// <summary>
    /// Add a list of items at the end of Array
    /// </summary>
    /// <typeparam name="T">Type of object in Array</typeparam>
    /// <param name="list">Array from where to add items</param>
    /// <param name="itemsToAdd">List of items to add to Array</param>
    /// <returns>Return a new Array with the items added to the end</returns>
    public static T[] AddRange<T>(this T[] list, object[] itemsToAdd)
    {
        if (itemsToAdd == null)
            throw new ArgumentNullException(nameof(itemsToAdd));

        foreach (T obj in itemsToAdd.Select(v => (T)v))
        {
            if ((typeof(T) == obj.GetType()) || (obj.GetType().IsSubclassOf(typeof(T))))
            {
                Array.Resize(ref list, list.Length + 1);
                list[list.Length - 1] = obj;
            }
        }
        return list;
    }

    /// <summary>
    /// Insert an item at a specified index in the Array
    /// </summary>
    /// <typeparam name="T">Type of object in Array</typeparam>
    /// <param name="list">Array from where to insert item</param>
    /// <param name="itemToInsert">Item to insert in Array</param>
    /// <param name="position">Index (of base zero) where to insert item in Array. If position is greater than the size of the Array, the item is added at the end of Array</param>
    /// <returns>Return a new Array with the item insert at the specified index</returns>
    public static T[] Insert<T>(this T[] list, object itemToInsert, int position)
    {
        if (itemToInsert == null)
            throw new ArgumentNullException(nameof(itemToInsert));

        if (position > list.Length - 1)
            position = list.Length;
        if ((typeof(T) == itemToInsert.GetType()) || (itemToInsert.GetType().IsSubclassOf(typeof(T))))
        {
            Array.Resize(ref list, list.Length + 1);
            for (int i = list.Length - 1; i > position; i--)
                list[i] = list[i - 1];
            list[position] = (T)itemToInsert;
        }
        return list;
    }

    /// <summary>
    /// Insert a list of items at a specified index in the Array
    /// </summary>
    /// <typeparam name="T">Type of object in Array</typeparam>
    /// <param name="list">Array from where to insert items</param>
    /// <param name="itemsToInsert">Item to insert in Array</param>
    /// <param name="position">Index (of base zero) where to insert the list of items in Array. If position is greater than the size of the Array, the items will be added at the end of Array</param>
    /// <returns>Return a new Array with the item insert at the specified index</returns>
    public static T[] InsertRange<T>(this T[] list, object[] itemsToInsert, int position)
    {
        if (itemsToInsert == null)
            throw new ArgumentNullException(nameof(itemsToInsert));

        if (position > list.Length - 1) position = list.Length;
        foreach (T obj in itemsToInsert.Select(v => (T)v))
        {
            if ((typeof(T) == obj.GetType()) || (obj.GetType().IsSubclassOf(typeof(T))))
            {
                Array.Resize(ref list, list.Length + 1);
                for (int i = list.Length - 1; i > position; i--)
                    list[i] = list[i - 1];
                list[position] = obj;
            }
        }
        return list;
    }

    /// <summary>
    /// Return a new Array with a distinct apply by a predicate executed on the array
    /// </summary>
    /// <typeparam name="T1">Type of Array</typeparam>
    /// <typeparam name="T2">Type of the member of the item array to use as distinct</typeparam>
    /// <param name="list">Array where execute the distinct</param>
    /// <param name="predicate">Predicate that return the member of item array where to make the distinct</param>
    public static T1[] Distinct<T1, T2>(this T1[] list, Func<T1, T2> predicate)
    {
        T1[] newList = [];
        HashSet<T2> key = [];
        foreach (T1 item in list.Where(i => key.Add(predicate(i))))
            newList = newList.Add(item);
        return newList;
    }
}
