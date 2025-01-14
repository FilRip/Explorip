using System.Runtime.InteropServices;

using ManagedShell.Interop;
using ManagedShell.ShellFolders.Enums;

namespace ManagedShell.ShellFolders.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct SortColumn
{
    /// <summary>
    /// <para>Type: <c>PROPERTYKEY</c></para>
    /// <para>
    /// The ID of the column by which the user will sort. A PROPERTYKEY structure. For example, for the "Name" column, the property
    /// key is PKEY_ItemNameDisplay.
    /// </para>
    /// </summary>
    public NativeMethods.PropertyKey propkey;

    /// <summary>
    /// <para>Type: <c>SORTDIRECTION</c></para>
    /// <para>The direction in which the items are sorted. One of the following values.</para>
    /// <para>SORT_DESCENDING</para>
    /// <para>
    /// The items are sorted in ascending order. Whether the sort is alphabetical, numerical, and so on, is determined by the data
    /// type of the column indicated in <c>propkey</c>.
    /// </para>
    /// <para>SORT_ASCENDING</para>
    /// <para>
    /// The items are sorted in descending order. Whether the sort is alphabetical, numerical, and so on, is determined by the data
    /// type of the column indicated in <c>propkey</c>.
    /// </para>
    /// </summary>
    public SORTDIRECTION direction;
}
