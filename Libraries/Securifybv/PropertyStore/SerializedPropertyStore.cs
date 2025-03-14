﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Securify.PropertyStore.Structures;

namespace Securify.PropertyStore;

/// <summary>
/// The Property Store Binary File Format is a sequence of Serialized Property 
/// Storage structures. The sequence MUST be terminated by a Serialized Property 
/// Storage structure that specifies 0x00000000 for the Storage Size field.
/// </summary>
public class SerializedPropertyStore : Structure
{
    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public SerializedPropertyStore() : base()
    {
        PropertyStorage = [];
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="SerializedPropertyStorage">A sequence of one or more Serialized Property Storage structures</param>
    public SerializedPropertyStore(List<SerializedPropertyStorage> SerializedPropertyStorage) : base()
    {
        this.PropertyStorage = SerializedPropertyStorage;
    }
    #endregion // Constructor

    #region FromByteArray
    /// <summary>
    /// Create a LinkTargetIDList from a given byte array
    /// </summary>
    /// <param name="ba">The byte array</param>
    /// <returns>A LinkTargetIDList object</returns>
    public static SerializedPropertyStore FromByteArray(byte[] ba)
    {
        SerializedPropertyStore PropertyStore = new();

        if (ba.Length < 8)
        {
            throw new ArgumentException(string.Format("Size of the SerializedPropertyStore is less than 8 ({0})", ba.Length));
        }

        uint storeSize = BitConverter.ToUInt32(ba, 0);
        if (ba.Length < storeSize)
        {
            throw new ArgumentException(string.Format("Size of the SerializedPropertyStore is less than {0} ({1})", storeSize, ba.Length));
        }

        ba = [.. ba.Skip(4)];
        uint StorageSize = BitConverter.ToUInt32(ba, 0);
        while (StorageSize > 5)
        {
            SerializedPropertyStorage propertyStorage = SerializedPropertyStorage.FromByteArray(ba);
            PropertyStore.PropertyStorage.Add(propertyStorage);
            ba = [.. ba.Skip((int)StorageSize)];
            StorageSize = BitConverter.ToUInt32(ba, 0);
        }

        return PropertyStore;
    }
    #endregion // FromByteArray

    #region StorageSize
    /// <summary>
    /// Store Size (4 bytes): An unsigned integer that specifies the total size, 
    /// in bytes, of this structure, excluding the size of this field.
    /// </summary>
    public virtual uint StoreSize
    {
        get
        {
            uint Size = 8;
            for (int i = 0; i < PropertyStorage.Count; i++)
            {
                Size += PropertyStorage[i].StorageSize;
            }
            return Size;
        }
    }
    #endregion // StoreSize

    /// <summary>
    /// A sequence of one or more Serialized Property Storage structures
    /// </summary>
    public List<SerializedPropertyStorage> PropertyStorage { get; set; }

    #region GetBytes
    /// <inheritdoc />
    public override byte[] GetBytes()
    {
        int Offset = 4;
        byte[] PropertyStore = new byte[StoreSize];
        Buffer.BlockCopy(BitConverter.GetBytes(StoreSize), 0, PropertyStore, 0, 4);
        for (int i = 0; i < this.PropertyStorage.Count; i++)
        {
            SerializedPropertyStorage propertyStorage = this.PropertyStorage[i];
            Buffer.BlockCopy(propertyStorage.GetBytes(), 0, PropertyStore, Offset, (int)propertyStorage.StorageSize);
            Offset += (int)propertyStorage.StorageSize;
        }
        return PropertyStore;
    }
    #endregion // GetBytes

    #region ToString
    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.AppendFormat("StoreSize: {0}", StoreSize);
        builder.AppendLine();
        for (int i = 0; i < PropertyStorage.Count; i++)
        {
            builder.Append(PropertyStorage[i].ToString());
        }
        return builder.ToString();
    }
    #endregion // ToString
}
