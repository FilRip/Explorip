// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace Microsoft.WindowsAPICodePack.Shell.Common;

/// <summary>
/// Stores information about how to sort a column that is displayed in the folder view.
/// </summary>    
[StructLayout(LayoutKind.Sequential)]
public struct SortColumn
{

    /// <summary>
    /// Creates a sort column with the specified direction for the given property.
    /// </summary>
    /// <param name="propertyKey">Property key for the property that the user will sort.</param>
    /// <param name="direction">The direction in which the items are sorted.</param>
    public SortColumn(PropertyKey propertyKey, SortDirection direction)
        : this()
    {
        PropertyKey = propertyKey;
        Direction = direction;
    }

    /// <summary>
    /// The ID of the column by which the user will sort. A PropertyKey structure. 
    /// For example, for the "Name" column, the property key is PKEY_ItemNameDisplay or
    /// <see cref="SystemProperties.System.ItemName"/>.
    /// </summary>                
    public PropertyKey PropertyKey { get; set; }

    /// <summary>
    /// The direction in which the items are sorted.
    /// </summary>                        
    public SortDirection Direction { get; set; }


    /// <summary>
    /// Implements the == (equality) operator.
    /// </summary>
    /// <param name="col1">First object to compare.</param>
    /// <param name="col2">Second object to compare.</param>
    /// <returns>True if col1 equals col2; false otherwise.</returns>
    public static bool operator ==(SortColumn col1, SortColumn col2)
    {
        return col1.Direction == col2.Direction &&
            col1.PropertyKey == col2.PropertyKey;
    }

    /// <summary>
    /// Implements the != (unequality) operator.
    /// </summary>
    /// <param name="col1">First object to compare.</param>
    /// <param name="col2">Second object to compare.</param>
    /// <returns>True if col1 does not equals col1; false otherwise.</returns>
    public static bool operator !=(SortColumn col1, SortColumn col2)
    {
        return !(col1 == col2);
    }

    /// <summary>
    /// Determines if this object is equal to another.
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>Returns true if the objects are equal; false otherwise.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not SortColumn result) { return false; }
        return this == result;
    }

    /// <summary>
    /// Generates a nearly unique hashcode for this structure.
    /// </summary>
    /// <returns>A hash code.</returns>
    public override int GetHashCode()
    {
        int hash = Direction.GetHashCode();
        hash = hash * 31 + PropertyKey.GetHashCode();
        return hash;
    }
}
