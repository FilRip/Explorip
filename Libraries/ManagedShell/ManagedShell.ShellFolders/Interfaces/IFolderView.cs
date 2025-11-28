using System;
using System.Runtime.InteropServices;

using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Structs;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.ShellFolders.Interfaces;

#nullable enable
#pragma warning disable IDE0079
#pragma warning disable S125 // Sections of code should not be commented out
[ComImport(), Guid("cde725b0-ccc9-4519-917e-325d72fab4ce"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IFolderView
{
    /// <summary>Gets an address containing a value representing the folder's current view mode.</summary>
    /// <returns>The folder's current view mode.</returns>
    FolderViewMode GetCurrentViewMode();

    /// <summary>Sets the selected folder's view mode.</summary>
    /// <param name="ViewMode">One of the following values from the FolderViewMode enumeration.</param>
    void SetCurrentViewMode(FolderViewMode ViewMode);

    /// <summary>Gets the folder object.</summary>
    /// <param name="riid">Reference to the desired IID to represent the folder.</param>
    /// <returns>
    /// When this method returns, contains the interface Pointer requested in <paramref name="riid"/>. This is typically
    /// IShellFolder or a related interface. This can also be an IShellItemArray with a single element.
    /// </returns>
    [return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 0)]
    object GetFolder(in Guid riid);

    /// <summary>Gets the identifier of a specific item in the folder view, by index.</summary>
    /// <param name="iItemIndex">The index of the item in the view.</param>
    /// <returns>The address of a Pointer to a IntPtr containing the item's identifier information.</returns>
    IntPtr Item(int iItemIndex);

    /// <summary>
    /// Gets the number of items in the folder. This can be the number of all items, or a subset such as the number of selected items.
    /// </summary>
    /// <param name="uFlags">Flags from the _ShellViewGetItemObject enumeration that limit the count to certain types of items.</param>
    /// <returns>The number of items (files and folders) displayed in the folder view.</returns>
    int ItemCount(ShellViewGetItemObject uFlags);

    /// <summary>Gets the address of an enumeration object based on the collection of items in the folder view.</summary>
    /// <param name="uFlags">_ShellViewGetItemObject values that limit the enumeration to certain types of items.</param>
    /// <param name="riid">Reference to the desired IID to represent the folder.</param>
    /// <returns>
    /// When this method returns, contains the interface Pointer requested in <paramref name="riid"/>. This is typically an
    /// IEnumIDList, IDataObject, or IShellItemArray. If an error occurs, this value is NULL.
    /// </returns>
    [return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 1)]
    object? Items(ShellViewGetItemObject uFlags, in Guid riid);

    /// <summary>Gets the index of an item in the folder's view which has been marked by using the SVSI_SELECTIONMARK in IFolderView::SelectItem.</summary>
    /// <returns>The index of the marked item.</returns>
    int GetSelectionMarkedItem();

    /// <summary>Gets the index of the item that currently has focus in the folder's view.</summary>
    /// <returns>The index of the item.</returns>
    int GetFocusedItem();

    /// <summary>Gets the position of an item in the folder's view.</summary>
    /// <param name="IntPtr">A Pointer to an ITEMIDLIST interface.</param>
    /// <returns>The position of the item's upper-left corner.</returns>
    Point GetItemPosition(IntPtr IntPtr);

    /// <summary>
    /// Gets a Point structure containing the width (x) and height (y) dimensions, including the surrounding white space, of an item.
    /// </summary>
    /// <returns>The current sizing dimensions of the items in the folder's view.</returns>
    Point GetSpacing();

    /// <summary>
    /// Gets a Pointer to a Point structure containing the default width (x) and height (y) measurements of an item, including the
    /// surrounding white space.
    /// </summary>
    /// <returns>The default sizing dimensions of the items in the folder's view.</returns>
    Point GetDefaultSpacing();

    /// <summary>Gets the current state of the folder's Auto Arrange mode.</summary>
    /// <returns>Returns S_OK if the folder is in Auto Arrange mode; S_FALSE if it is not.</returns>
    [PreserveSig()]
    int GetAutoArrange();

    /// <summary>Selects an item in the folder's view.</summary>
    /// <param name="iItem">The index of the item to select in the folder's view.</param>
    /// <param name="dwFlags">One of the _SVSIF constants that specify the type of selection to apply.</param>
    void SelectItem(int iItem, SVSIF dwFlags);

    /// <summary>Allows the selection and positioning of items visible in the folder's view.</summary>
    /// <param name="cidl">The number of items to select.</param>
    /// <param name="aIntPtr">A Pointer to an array of size <paramref name="cidl"/> that contains the IntPtrs of the items.</param>
    /// <param name="apt">
    /// A Pointer to an array of <paramref name="cidl"/> structures containing the locations each corresponding element in <paramref
    /// name="aIntPtr"/> should be positioned.
    /// </param>
    /// <param name="dwFlags">One of the _SVSIF constants that specifies the type of selection to apply.</param>
    void SelectAndPositionItems(uint cidl, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] aIntPtr, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Point[] apt, SVSIF dwFlags);
}

/// <summary>
/// Exposes methods that retrieve information about a folder's display options, select specified items in that folder, and set the
/// folder's view mode.
/// </summary>
/// <seealso cref="IFolderView"/>
[ComImport(), Guid("1af3a467-214f-4298-908e-06b03e0b39f9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IFolderView2 : IFolderView
{
    /// <summary>Gets an address containing a value representing the folder's current view mode.</summary>
    /// <returns>The folder's current view mode.</returns>
    new FolderViewMode GetCurrentViewMode();

    /// <summary>Sets the selected folder's view mode.</summary>
    /// <param name="ViewMode">One of the following values from the FolderViewMode enumeration.</param>
    new void SetCurrentViewMode(FolderViewMode ViewMode);

    /// <summary>Gets the folder object.</summary>
    /// <param name="riid">Reference to the desired IID to represent the folder.</param>
    /// <returns>
    /// When this method returns, contains the interface Pointer requested in <paramref name="riid"/>. This is typically
    /// IShellFolder or a related interface. This can also be an IShellItemArray with a single element.
    /// </returns>
    [return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 0)]
    new object GetFolder(in Guid riid);

    /// <summary>Gets the identifier of a specific item in the folder view, by index.</summary>
    /// <param name="iItemIndex">The index of the item in the view.</param>
    /// <returns>The address of a Pointer to a IntPtr containing the item's identifier information.</returns>
    new IntPtr Item(int iItemIndex);

    /// <summary>
    /// Gets the number of items in the folder. This can be the number of all items, or a subset such as the number of selected items.
    /// </summary>
    /// <param name="uFlags">Flags from the _ShellViewGetItemObject enumeration that limit the count to certain types of items.</param>
    /// <returns>The number of items (files and folders) displayed in the folder view.</returns>
    new int ItemCount(ShellViewGetItemObject uFlags);

    /// <summary>Gets the address of an enumeration object based on the collection of items in the folder view.</summary>
    /// <param name="uFlags">_ShellViewGetItemObject values that limit the enumeration to certain types of items.</param>
    /// <param name="riid">Reference to the desired IID to represent the folder.</param>
    /// <returns>
    /// When this method returns, contains the interface Pointer requested in <paramref name="riid"/>. This is typically an
    /// IEnumIDList, IDataObject, or IShellItemArray. If an error occurs, this value is NULL.
    /// </returns>
    [return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 1)]
    new object? Items(ShellViewGetItemObject uFlags, in Guid riid);

    /// <summary>Gets the index of an item in the folder's view which has been marked by using the SVSI_SELECTIONMARK in IFolderView::SelectItem.</summary>
    /// <returns>The index of the marked item.</returns>
    new int GetSelectionMarkedItem();

    /// <summary>Gets the index of the item that currently has focus in the folder's view.</summary>
    /// <returns>The index of the item.</returns>
    new int GetFocusedItem();

    /// <summary>Gets the position of an item in the folder's view.</summary>
    /// <param name="IntPtr">A Pointer to an ITEMIDLIST interface.</param>
    /// <returns>The position of the item's upper-left corner.</returns>
    new Point GetItemPosition(IntPtr IntPtr);

    /// <summary>
    /// Gets a Point structure containing the width (x) and height (y) dimensions, including the surrounding white space, of an item.
    /// </summary>
    /// <returns>The current sizing dimensions of the items in the folder's view.</returns>
    new Point GetSpacing();

    /// <summary>
    /// Gets a Pointer to a Point structure containing the default width (x) and height (y) measurements of an item, including the
    /// surrounding white space.
    /// </summary>
    /// <returns>The default sizing dimensions of the items in the folder's view.</returns>
    new Point GetDefaultSpacing();

    /// <summary>Gets the current state of the folder's Auto Arrange mode.</summary>
    /// <returns>Returns S_OK if the folder is in Auto Arrange mode; S_FALSE if it is not.</returns>
    [PreserveSig()]
    new int GetAutoArrange();

    /// <summary>Selects an item in the folder's view.</summary>
    /// <param name="iItem">The index of the item to select in the folder's view.</param>
    /// <param name="dwFlags">One of the _SVSIF constants that specify the type of selection to apply.</param>
    new void SelectItem(int iItem, SVSIF dwFlags);

    /// <summary>Allows the selection and positioning of items visible in the folder's view.</summary>
    /// <param name="cidl">The number of items to select.</param>
    /// <param name="aIntPtr">A Pointer to an array of size <paramref name="cidl"/> that contains the IntPtrs of the items.</param>
    /// <param name="apt">
    /// A Pointer to an array of <paramref name="cidl"/> structures containing the locations each corresponding element in <paramref
    /// name="aIntPtr"/> should be positioned.
    /// </param>
    /// <param name="dwFlags">One of the _SVSIF constants that specifies the type of selection to apply.</param>
    new void SelectAndPositionItems(uint cidl, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] aIntPtr, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Point[] apt, SVSIF dwFlags);

    /// <summary>Groups the view by the given property key and direction.</summary>
    /// <param name="key">
    /// <para>Type: <c>REFPropertyKey</c></para>
    /// <para>A PropertyKey by which the view should be grouped.</para>
    /// </param>
    /// <param name="fAscending">
    /// <para>Type: <c>BOOL</c></para>
    /// <para>A value of type <c>BOOL</c> to indicate sort order of the groups.</para>
    /// </param>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-setgroupby int
    // SetGroupBy( REFPropertyKey key, BOOL fAscending );
    void SetGroupBy(in PropertyKey key, [MarshalAs(UnmanagedType.Bool)] bool fAscending);

    /// <summary>Retrieves the property and sort order used for grouping items in the folder display.</summary>
    /// <param name="pkey">
    /// <para>Type: <c>PropertyKey*</c></para>
    /// <para>A Pointer to the PropertyKey by which the view is grouped.</para>
    /// </param>
    /// <param name="pfAscending">
    /// <para>Type: <c>BOOL*</c></para>
    /// <para>A Pointer to a value of type <c>BOOL</c> that indicates sort order of the groups.</para>
    /// </param>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getgroupby int
    // GetGroupBy( PropertyKey *pkey, BOOL *pfAscending );
    void GetGroupBy(out PropertyKey pkey, [MarshalAs(UnmanagedType.Bool)] out bool pfAscending);

    /// <summary>
    /// <para>
    /// [This method is still implemented, but should be considered deprecated as of Windows 7. It might not be implemented in
    /// future versions of Windows. It cannot be used with items in search results or library views, so consider using the item's
    /// existing properties or, if applicable, emitting properties from your namespace or property handler. See Developing Property
    /// Handlers for Windows Search for more information.]
    /// </para>
    /// <para>Caches a property for an item in the view's property cache.</para>
    /// </summary>
    /// <param name="IntPtr">
    /// <para>Type: <c>PCUITEMID_CHILD</c></para>
    /// <para>A IntPtr that identifies the item.</para>
    /// </param>
    /// <param name="propkey">
    /// <para>Type: <c>REFPropertyKey</c></para>
    /// <para>The PropertyKey which is to be stored.</para>
    /// </param>
    /// <param name="propvar">
    /// <para>Type: <c>const PROPVARIANT*</c></para>
    /// <para>A Pointer to a PROPVARIANT structure in which the PropertyKey is stored.</para>
    /// </param>
    /// <remarks>The property is displayed in the view, but not written to the underlying item.</remarks>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-setviewproperty
    // DEPRECATED_int SetViewProperty( PCUITEMID_CHILD IntPtr, REFPropertyKey propkey, REFPROPVARIANT propvar );
    [Obsolete()]
    void SetViewProperty(IntPtr IntPtr, in PropertyKey propkey, object propvar);

    /// <summary>
    /// <para>
    /// [This method is still implemented, but should be considered deprecated as of Windows 7. It might not be implemented in
    /// future versions of Windows. It cannot be used with items in search results or library views, so consider using the item's
    /// existing properties or, if applicable, emitting properties from your namespace or property handler. See Developing Property
    /// Handlers for Windows Search for more information.]
    /// </para>
    /// <para>Gets a property value for a given property key from the view's cache.</para>
    /// </summary>
    /// <param name="IntPtr">
    /// <para>Type: <c>PCUITEMID_CHILD</c></para>
    /// <para>A Pointer to an item identifier list (IntPtr).</para>
    /// </param>
    /// <param name="propkey">
    /// <para>Type: <c>REFPropertyKey</c></para>
    /// <para>The PropertyKey to be retrieved.</para>
    /// </param>
    /// <param name="ppropvar">
    /// <para>Type: <c>PROPVARIANT*</c></para>
    /// <para>A Pointer to a PROPVARIANT structure in which the PropertyKey is stored.</para>
    /// </param>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getviewproperty
    // DEPRECATED_int GetViewProperty( PCUITEMID_CHILD IntPtr, REFPropertyKey propkey, PROPVARIANT *ppropvar );
    [Obsolete()]
    void GetViewProperty(IntPtr IntPtr, in PropertyKey propkey, [In, Out] object ppropvar);

    /// <summary>
    /// <para>
    /// [This method is still implemented, but should be considered deprecated as of Windows 7. It might not be implemented in
    /// future versions of Windows. It cannot be used with items in search results or library views, so consider using the item's
    /// existing properties or, if applicable, emitting properties from your namespace or property handler. See Developing Property
    /// Handlers for Windows Search for more information.]
    /// </para>
    /// <para>Set the list of tile properties for an item.</para>
    /// </summary>
    /// <param name="IntPtr">
    /// <para>Type: <c>PCUITEMID_CHILD</c></para>
    /// <para>A Pointer to an item identifier list (IntPtr).</para>
    /// </param>
    /// <param name="pszPropList">
    /// <para>Type: <c>LPCWSTR</c></para>
    /// <para>A Pointer to a Unicode string containing a list of properties.</para>
    /// </param>
    /// <remarks>
    /// The pszPropList parameter must be of the form "prop:&lt;canonical-property-name&gt;;&lt;canonical-property-name&gt;" where
    /// "&lt;canonical-property-name&gt;" is replaced by an actual canonical property name. The parameter can contain one or more
    /// properties delimited by semicolons.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-settileviewproperties
    // DEPRECATED_int SetTileViewProperties( PCUITEMID_CHILD IntPtr, LPCWSTR pszPropList );
    [Obsolete()]
    void SetTileViewProperties(IntPtr IntPtr, [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropList);

    /// <summary>
    /// <para>
    /// [This method is still implemented, but should be considered deprecated as of Windows 7. It might not be implemented in
    /// future versions of Windows. It cannot be used with items in search results or library views, so consider using the item's
    /// existing properties or, if applicable, emitting properties from your namespace or property handler. See Developing Property
    /// Handlers for Windows Search for more information.]
    /// </para>
    /// <para>Sets the list of extended tile properties for an item.</para>
    /// </summary>
    /// <param name="IntPtr">
    /// <para>Type: <c>PCUITEMID_CHILD</c></para>
    /// <para>A Pointer to an item identifier list (IntPtr).</para>
    /// </param>
    /// <param name="pszPropList">
    /// <para>Type: <c>LPCWSTR</c></para>
    /// <para>A Pointer to a Unicode string containing a list of properties.</para>
    /// </param>
    /// <remarks>
    /// The pszPropList parameter must be of the form "prop:&lt;canonical-property-name&gt;;&lt;canonical-property-name&gt;" where
    /// "&lt;canonical-property-name&gt;" is an actual canonical property name. It can contain one or more properties delimited by semicolons.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-setextendedtileviewproperties
    // DEPRECATED_int SetExtendedTileViewProperties( PCUITEMID_CHILD IntPtr, LPCWSTR pszPropList );
    [Obsolete()]
    void SetExtendedTileViewProperties(IntPtr IntPtr, [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropList);

    /// <summary>Sets the default text to be used when there are no items in the view.</summary>
    /// <param name="iType">
    /// <para>Type: <c>FVTEXTTYPE</c></para>
    /// <para>This value should be set to the following flag.</para>
    /// <para>FVST_EMPTYTEXT</para>
    /// <para>Set the text to display when there are no items in the view.</para>
    /// </param>
    /// <param name="pwszText">
    /// <para>Type: <c>LPCWSTR</c></para>
    /// <para>A Pointer to a Unicode string that contains the text to be used.</para>
    /// </param>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-settext int SetText(
    // FVTEXTTYPE iType, LPCWSTR pwszText );
    void SetText(FVTEXTTYPE iType, [In, MarshalAs(UnmanagedType.LPWStr)] string? pwszText);

    /// <summary>Sets and applies specified folder flags.</summary>
    /// <param name="dwMask">
    /// <para>Type: <c>DWORD</c></para>
    /// <para>The value of type <c>DWORD</c> that specifies the bitmask indicating which items in the structure are desired or valid.</para>
    /// </param>
    /// <param name="dwFlags">
    /// <para>Type: <c>DWORD</c></para>
    /// <para>The value of type <c>DWORD</c> that contains one or more FOLDERFLAGS.</para>
    /// </param>
    /// <remarks>
    /// <c>For Windows 7 or later:</c> This method must be used in combination with the FVO_CUSTOMPOSITION flag from the
    /// FOLDERVIEWOPTIONS enumeration.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-setcurrentfolderflags
    // int SetCurrentFolderFlags( DWORD dwMask, DWORD dwFlags );
    void SetCurrentFolderFlags(EFolder dwMask, EFolder dwFlags);

    /// <summary>Gets the currently applied folder flags.</summary>
    /// <returns>
    /// <para>Type: <c>DWORD*</c></para>
    /// <para>A Pointer to a <c>DWORD</c> with any FOLDERFLAGS that have been applied to the folder.</para>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getcurrentfolderflags
    // int GetCurrentFolderFlags( DWORD *pdwFlags );
    EFolder GetCurrentFolderFlags();

    /// <summary>Gets the count of sort columns currently applied to the view.</summary>
    /// <returns>
    /// <para>Type: <c>int*</c></para>
    /// <para>A Pointer to an <c>int</c>.</para>
    /// </returns>
    /// <remarks>Returns E_INVALIDARG if the column count provided does not equal the count of sort columns in the view.</remarks>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getsortcolumncount int
    // GetSortColumnCount( int *pcColumns );
    int GetSortColumnCount();

    /// <summary>Sets and sorts the view by the given sort columns.</summary>
    /// <param name="rgSortColumns">
    /// <para>Type: <c>const SORTCOLUMN*</c></para>
    /// <para>A Pointer to a SORTCOLUMN structure. The size of this structure is determined by cColumns.</para>
    /// </param>
    /// <param name="cColumns">
    /// <para>Type: <c>int</c></para>
    /// <para>The count of columns to sort by.</para>
    /// </param>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-setsortcolumns int
    // SetSortColumns( const SORTCOLUMN *rgSortColumns, int cColumns );
    void SetSortColumns([In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] SortColumn[] rgSortColumns, int cColumns);

    /// <summary>Gets the sort columns currently applied to the view.</summary>
    /// <param name="rgSortColumns">
    /// <para>Type: <c>const SORTCOLUMN*</c></para>
    /// <para>A Pointer to a SORTCOLUMN structure. The size of this structure is determined by cColumns.</para>
    /// </param>
    /// <param name="cColumns">
    /// <para>Type: <c>int</c></para>
    /// <para>The count of columns to sort by.</para>
    /// </param>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getsortcolumns int
    // GetSortColumns( SORTCOLUMN *rgSortColumns, int cColumns );
    void GetSortColumns([In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] SortColumn[] rgSortColumns, int cColumns);

    /// <summary>Retrieves an object that represents a specified item.</summary>
    /// <param name="iItem">
    /// <para>Type: <c>int</c></para>
    /// <para>The zero-based index of the item to retrieve.</para>
    /// </param>
    /// <param name="riid">
    /// <para>Type: <c>REFIID</c></para>
    /// <para>Reference to the desired IID to represent the item, such as IID_IShellItem.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <c>void**</c></para>
    /// <para>When this method returns, contains the interface Pointer requested in <paramref name="riid"/>. This is typically IShellItem.</para>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getitem int GetItem( int
    // iItem, REFIID riid, void **ppv );
    [return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 1)]
    object GetItem(int iItem, in Guid riid);

    /// <summary>Gets the next visible item in relation to a given index in the view.</summary>
    /// <param name="iStart">
    /// <para>Type: <c>int</c></para>
    /// <para>The zero-based position at which to start searching for a visible item.</para>
    /// </param>
    /// <param name="fPrevious">
    /// <para>Type: <c>BOOL</c></para>
    /// <para><c>TRUE</c> to find the first visible item before iStart. <c>FALSE</c> to find the first visible item after iStart.</para>
    /// </param>
    /// <param name="piItem">
    /// <para>Type: <c>int*</c></para>
    /// <para>When this method returns, contains a Pointer to a value that receives the index of the visible item in the view.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <c>int</c></para>
    /// <para>This method can return one of these values.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>Item retrieved.</term>
    /// </item>
    /// <item>
    /// <term>S_FALSE</term>
    /// <term>Item not found. Note that this is a success code.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getvisibleitem int
    // GetVisibleItem( int iStart, BOOL fPrevious, int *piItem );
    [PreserveSig()]
    int GetVisibleItem(int iStart, [In, MarshalAs(UnmanagedType.Bool)] bool fPrevious, out int piItem);

    /// <summary>Locates the currently selected item at or after a given index.</summary>
    /// <param name="iStart">The index position from which to start searching for the currently selected item.</param>
    /// <param name="piItem">A Pointer to a value that receives the index of the item in the view.</param>
    /// <returns>
    /// <para>Type: <c>int</c></para>
    /// <para>Returns S_OK if a selected item was found, or an error value otherwise, including the following:</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_FALSE</term>
    /// <term>
    /// Item not found. Note that this is a success code. The operation was successful in searching the view, it simply did not find
    /// a currently selected item after the given index (iStart). It is possible that no item was selected, or that the selected
    /// item had an index less than iStart.
    /// </term>
    /// </item>
    /// </list>
    /// </returns>
    [PreserveSig()]
    int GetSelectedItem(int iStart, out int piItem);

    /// <summary>
    /// <para>Gets the current selection as an IShellItemArray.</para>
    /// </summary>
    /// <param name="fNoneImpliesFolder">
    /// <para>Type: <c>BOOL</c></para>
    /// <para>If <c>TRUE</c>, this method returns an IShellItemArray containing the parent folder when there is no current selection.</para>
    /// </param>
    /// <param name="ppsia">
    /// <para>Type: <c>IShellItemArray**</c></para>
    /// <para>The address of a Pointer to an IShellItemArray.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <c>int</c></para>
    /// <para>Returns one of the following values, or an error otherwise.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <term>S_OK</term>
    /// <term>The operation completed successfully.</term>
    /// </item>
    /// <item>
    /// <term>S_FALSE</term>
    /// <term>The IShellItemArray returned has zero items.</term>
    /// </item>
    /// </list>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getselection int
    // GetSelection( BOOL fNoneImpliesFolder, IShellItemArray **ppsia );
    [PreserveSig()]
    int GetSelection([In, MarshalAs(UnmanagedType.Bool)] bool fNoneImpliesFolder, out IShellItemArray ppsia);

    /// <summary>Gets the selection state including check state.</summary>
    /// <param name="IntPtr">
    /// <para>Type: <c>PCUITEMID_CHILD</c></para>
    /// <para>A IntPtr of the item.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <c>DWORD*</c></para>
    /// <para>
    /// Zero or one of the following _SVSIF constants that specify the current type of selection: <c>SVSI_FOCUSED</c>,
    /// <c>SVSI_SELECT</c>, <c>SVSI_CHECK</c>, or <c>SVSI_CHECK2</c>. Other <c>_SVSIF</c> constants are not returned by this API.
    /// </para>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getselectionstate int
    // GetSelectionState( PCUITEMID_CHILD IntPtr, DWORD *pdwFlags );
    SVSIF GetSelectionState(IntPtr IntPtr);

    /// <summary>Invokes the given verb on the current selection.</summary>
    /// <param name="pszVerb">
    /// <para>Type: <c>LPCSTR</c></para>
    /// <para>A Pointer to a Unicode string containing a verb.</para>
    /// </param>
    /// <remarks>If pszVerb is <c>NULL</c>, then the default verb is invoked on the selection.</remarks>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-invokeverbonselection
    // int InvokeVerbOnSelection( LPCSTR pszVerb );
    void InvokeVerbOnSelection([In, MarshalAs(UnmanagedType.LPStr)] string? pszVerb);

    /// <summary>Sets and applies the view mode and image size.</summary>
    /// <param name="uViewMode">
    /// <para>Type: <c>FolderViewMode</c></para>
    /// <para>The FolderViewMode to be applied.</para>
    /// </param>
    /// <param name="iImageSize">
    /// <para>Type: <c>int</c></para>
    /// <para>The size of the image in pixels.</para>
    /// </param>
    /// <remarks>If iImageSize is -1 then the current default icon size for the view mode is used.</remarks>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-setviewmodeandiconsize
    // int SetViewModeAndIconSize( FolderViewMode uViewMode, int iImageSize );
    void SetViewModeAndIconSize(FolderViewMode uViewMode, int iImageSize = -1);

    /// <summary>Gets the current view mode and icon size applied to the view.</summary>
    /// <param name="puViewMode">
    /// <para>Type: <c>FolderViewMode*</c></para>
    /// <para>A Pointer to the current FolderViewMode.</para>
    /// </param>
    /// <param name="piImageSize">
    /// <para>Type: <c>int*</c></para>
    /// <para>A Pointer to the size of the icon in pixels.</para>
    /// </param>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getviewmodeandiconsize
    // int GetViewModeAndIconSize( FolderViewMode *puViewMode, int *piImageSize );
    void GetViewModeAndIconSize(out FolderViewMode puViewMode, out int piImageSize);

    /// <summary>Turns on group subsetting and sets the number of visible rows of items in each group.</summary>
    /// <param name="cVisibleRows">
    /// <para>Type: <c>UINT</c></para>
    /// <para>The number of rows to be visible.</para>
    /// </param>
    /// <remarks>If cVisibleRows is zero, subsetting is turned off.</remarks>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-setgroupsubsetcount int
    // SetGroupSubsetCount( UINT cVisibleRows );
    void SetGroupSubsetCount(uint cVisibleRows);

    /// <summary>Gets the count of visible rows displayed for a group's subset.</summary>
    /// <returns>
    /// <para>Type: <c>UINT*</c></para>
    /// <para>The number of rows currently visible.</para>
    /// </returns>
    /// <remarks>If group subsetting is disabled the number of rows is zero.</remarks>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-getgroupsubsetcount int
    // GetGroupSubsetCount( UINT *pcVisibleRows );
    uint GetGroupSubsetCount();

    /// <summary>Sets redraw on and off.</summary>
    /// <param name="fRedrawOn">
    /// <para>Type: <c>BOOL</c></para>
    /// <para>a <c>BOOL</c> value.</para>
    /// </param>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-setredraw int SetRedraw(
    // BOOL fRedrawOn );
    void SetRedraw([In, MarshalAs(UnmanagedType.Bool)] bool fRedrawOn);

    /// <summary>
    /// Checks to see if this view sourced the current drag-and-drop or cut-and-paste operation (used by drop target objects).
    /// </summary>
    /// <returns>
    /// <para>Type: <c>int</c></para>
    /// <para>If this method succeeds, it returns <c>S_OK</c>. Otherwise, it returns an <c>int</c> error code.</para>
    /// </returns>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-ismoveinsamefolder int
    // IsMoveInSameFolder( );
    [PreserveSig()]
    int IsMoveInSameFolder();

    /// <summary>Starts a rename operation on the current selection.</summary>
    // https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ifolderview2-dorename int DoRename( );
    void DoRename();
}
#pragma warning restore S125 // Sections of code should not be commented out
#pragma warning restore IDE0079
