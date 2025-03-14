﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Securify.PropertyStore.Structures;

/// <summary>
/// The Serialized Property Storage structure is a sequence of Serialized Property Value structures. 
/// The sequence MUST be terminated by a Serialized Property Value structure that specifies 
/// 0x00000000 for the Value Size field.
/// </summary>
public class SerializedPropertyStorage : Structure
{
    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public SerializedPropertyStorage() : base()
    {
        this.PropertyStorage = [];
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="PropertyStorage">A sequence of one or more Serialized Property Storage structures</param>
    public SerializedPropertyStorage(List<SerializedPropertyValue> PropertyStorage) : base()
    {
        this.PropertyStorage = PropertyStorage;
    }
    #endregion // Constructor

    #region StorageSize
    /// <summary>
    /// Store Size (4 bytes): An unsigned integer that specifies the total size, 
    /// in bytes, of this structure, excluding the size of this field.
    /// </summary>
    public virtual uint StorageSize
    {
        get
        {
            uint Size = 28;
            for (int i = 0; i < PropertyStorage.Count; i++)
            {
                Size += PropertyStorage[i].ValueSize;
            }
            return Size;
        }
    }
    #endregion // StorageSize

    /// <summary>
    /// MUST be equal to 0x53505331.
    /// </summary>
    public static uint Version => 0x53505331;

    /// <summary>
    /// A GUID that specifies the semantics and expected usage of the properties contained in 
    /// this Serialized Property Storage structure. It MUST be unique in the set of serialized 
    /// property storage structures.
    /// </summary>
    public Guid FormatID { get; set; }

    /// <summary>
    /// A sequence of one or more Serialized Property Storage structures.
    /// </summary>
    public List<SerializedPropertyValue> PropertyStorage { get; set; }

    #region GetBytes
    /// <inheritdoc />
    public override byte[] GetBytes()
    {
        int Offset = 24;
        byte[] propertyStorage = new byte[StorageSize];
        Buffer.BlockCopy(BitConverter.GetBytes(StorageSize), 0, propertyStorage, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(Version), 0, propertyStorage, 4, 4);
        Buffer.BlockCopy(FormatID.ToByteArray(), 0, propertyStorage, 8, 16);
        for (int i = 0; i < this.PropertyStorage.Count; i++)
        {
            SerializedPropertyValue PropertyValue = this.PropertyStorage[i];
            Buffer.BlockCopy(PropertyValue.GetBytes(), 0, propertyStorage, Offset, (int)PropertyValue.ValueSize);
            Offset += (int)PropertyValue.ValueSize;
        }
        return propertyStorage;
    }
    #endregion // GetBytes

    #region ToString
    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.AppendFormat("StorageSize: {0} (0x{0:X})", StorageSize);
        builder.AppendLine();
        builder.AppendFormat("Version: 0x{0:X}", Version);
        builder.AppendLine();
        builder.AppendFormat("FormatID: {0}", FormatID);
        builder.AppendLine();
        for (int i = 0; i < this.PropertyStorage.Count; i++)
        {
            builder.Append(PropertyStorage[i].ToString());
        }
        return builder.ToString();
    }
    #endregion // ToString

    #region FromByteArray
    /// <summary>
    /// Create a LinkTargetIDList from a given byte array
    /// </summary>
    /// <param name="ba">The byte array</param>
    /// <returns>A LinkTargetIDList object</returns>
    public static SerializedPropertyStorage FromByteArray(byte[] ba)
    {
        SerializedPropertyStorage SerializedPropertyStorage = new();

        if (ba.Length < 28)
        {
            throw new ArgumentException(string.Format("Size of the SerializedPropertyStorage is less than 28 ({0})", ba.Length));
        }

        uint storageSize = BitConverter.ToUInt32(ba, 0);
        if (ba.Length < storageSize)
        {
            throw new ArgumentException(string.Format("Size of the SerializedPropertyStore is less than {0} ({1})", storageSize, ba.Length));
        }

        uint version = BitConverter.ToUInt32(ba, 4);
        if (SerializedPropertyStorage.Version != version)
        {
            throw new ArgumentException(string.Format("Version is not equal to {0} ({1})", SerializedPropertyStorage.Version, version));
        }

        byte[] formatID = new byte[16];
        Buffer.BlockCopy(ba, 8, formatID, 0, formatID.Length);
        SerializedPropertyStorage.FormatID = new Guid(formatID);

        ba = [.. ba.Skip(24)];
        uint ValueSize = BitConverter.ToUInt32(ba, 0);
        while (ValueSize > 5)
        {
            SerializedPropertyValue PropertyValue;
            if (SerializedPropertyStorage.FormatID.Equals(new Guid("{D5CDD505-2E9C-101B-9397-08002B2CF9AE}")))
            {
                PropertyValue = StringName.FromByteArray(ba);
            }
            else
            {
                PropertyValue = IntegerName.FromByteArray(ba);
            }
            SerializedPropertyStorage.PropertyStorage.Add(PropertyValue);
            ba = [.. ba.Skip((int)ValueSize)];
            ValueSize = BitConverter.ToUInt32(ba, 0);
        }

        return SerializedPropertyStorage;
    }
    #endregion // FromByteArray
}
