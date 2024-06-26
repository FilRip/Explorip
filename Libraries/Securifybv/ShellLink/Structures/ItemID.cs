﻿using System;
using System.Runtime.InteropServices;
using System.Text;

using Securify.ShellLink.Internal;

namespace Securify.ShellLink.Structures;

/// <summary>
/// An ItemID is an element in an IDList structure. The data stored in a given ItemID is defined 
/// by the source that corresponds to the location in the target namespace of the preceding ItemIDs. 
/// This data uniquely identifies the items in that part of the namespace.
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
/// <param name="itemID">An ItemID value</param>
public class ItemID(byte[] itemID) : Structure
{
    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public ItemID() : this([]) { }
    #endregion // Constructor

    #region GetBytes
    /// <inheritdoc />
    public override byte[] GetBytes()
    {
        byte[] ItemID = new byte[ItemIDSize];
        Buffer.BlockCopy(BitConverter.GetBytes(ItemIDSize), 0, ItemID, 0, 2);
        Buffer.BlockCopy(Data, 0, ItemID, 2, Data.Length);
        return ItemID;
    }
    #endregion // GetBytes

    /// <summary>
    /// ItemIDSize (2 bytes): A 16-bit, unsigned integer that specifies the size, in bytes, of the 
    /// ItemID structure, including the ItemIDSize field.
    /// </summary>
    public ushort ItemIDSize => (ushort)(Data.Length + 2);

    /// <summary>
    /// Data (variable): The shell data source-defined data that specifies an item.
    /// </summary>
    public byte[] Data { get; set; } = itemID;
    #region DisplayName
    /// <summary>
    /// Retrieves the display name of an item identified by its IDList.
    /// </summary>
    public string DisplayName
    {
        get
        {
            if (Win32.SHGetNameFromIDList(GetBytes(), SIGDN.SIGDN_PARENTRELATIVE, out IntPtr pszName) == 0)
            {
                try
                {
                    return Marshal.PtrToStringAuto(pszName);
                }
                catch (Exception)
                {
                    return "";
                }
                finally
                {
                    Win32.CoTaskMemFree(pszName);
                }
            }
            return "";
        }
    }
    #endregion // DisplayName

    #region ToString
    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.AppendFormat("ItemIDSize: {0}", ItemIDSize);
        builder.AppendLine();
        builder.AppendFormat("DisplayName: {0}", DisplayName);
        builder.AppendLine();
        builder.AppendFormat("Data: {0}", BitConverter.ToString(Data).Replace("-", " "));
        builder.AppendLine();
        return builder.ToString();
    }
    #endregion // ToString

    #region FromByteArray
    /// <summary>
    /// Create an ItemID from a given byte array
    /// </summary>
    /// <param name="ba">The byte array</param>
    /// <returns>An ItemID object</returns>
    public static ItemID FromByteArray(byte[] ba)
    {
        ItemID ItemId = new();

        if (ba.Length < 2)
        {
            throw new ArgumentException(string.Format("Size of the ItemID is less than 2 ({0})", ba.Length));
        }

        ushort itemIDSize = BitConverter.ToUInt16(ba, 0);
        if (ba.Length < itemIDSize)
        {
            throw new ArgumentException(string.Format("Size of the ItemID is not equal to {0} ({1})", itemIDSize, ba.Length));
        }

        ItemId.Data = new byte[itemIDSize - 2];
        Buffer.BlockCopy(ba, 2, ItemId.Data, 0, ItemId.Data.Length);

        return ItemId;
    }
    #endregion // FromByteArray
}
