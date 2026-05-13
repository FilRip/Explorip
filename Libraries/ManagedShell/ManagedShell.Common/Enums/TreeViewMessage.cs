using static ManagedShell.Common.Common.Constants;

namespace ManagedShell.Common.Enums;

public enum TreeViewMessage
{
    /// <summary>
    /// Removes an item and all its children from a tree-view control. You can send this message explicitly or by using the
    /// <c>TreeView_DeleteItem</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>
    /// <c>HTREEITEM</c> handle to the item to delete. If lParam is set to TVI_ROOT or to <c>NULL</c>, all items are deleted. You can
    /// also use the <c>TreeView_DeleteAllItems</c> macro to delete all items.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns <c>TRUE</c> if successful, or <c>FALSE</c> otherwise.</para>
    /// </summary>
    /// <remarks>
    /// <para>It is not safe to delete items in response to a notification such as TVN_SELCHANGING.</para>
    /// <para>Once an item is deleted, its handle is invalid and cannot be used.</para>
    /// <para>The parent window receives a TVN_DELETEITEM notification code when each item is removed.</para>
    /// <para>
    /// If the item label is being edited, the edit operation is canceled and the parent window receives the TVN_ENDLABELEDIT
    /// notification code.
    /// </para>
    /// <para>
    /// If you delete all items in a tree-view control that has the <c>TVS_NOSCROLL</c> style, items subsequently added may not display
    /// properly. For more information, see <c>TreeView_DeleteAllItems</c>.
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-deleteitem
    TVM_DELETEITEM = TV_FIRST + 1,

    /// <summary>
    /// The <c>TVM_EXPAND</c> message expands or collapses the list of child items associated with the specified parent item, if any. You
    /// can send this message explicitly or by using the <c>TreeView_Expand</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Action flag. This parameter can be one or more of the following values:</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <term>Meaning</term>
    /// </listheader>
    /// <item>
    /// <term><c>TVE_COLLAPSE</c></term>
    /// <term>Collapses the list.</term>
    /// </item>
    /// <item>
    /// <term><c>TVE_COLLAPSERESET</c></term>
    /// <term>
    /// Collapses the list and removes the child items. The <c>TVIS_EXPANDEDONCE</c> state flag is reset. This flag must be used with the
    /// TVE_COLLAPSE flag.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVE_EXPAND</c></term>
    /// <term>Expands the list.</term>
    /// </item>
    /// <item>
    /// <term><c>TVE_EXPANDPARTIAL</c></term>
    /// <term>
    /// Version 4.70. Partially expands the list. In this state the child items are visible and the parent item's plus sign (+),
    /// indicating that it can be expanded, is displayed. This flag must be used in combination with the TVE_EXPAND flag.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVE_TOGGLE</c></term>
    /// <term>Collapses the list if it is expanded or expands it if it is collapsed.</term>
    /// </item>
    /// </list>
    /// <para><em>lParam</em></para>
    /// <para>Handle to the parent item to expand or collapse.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns nonzero if the operation was successful, or zero otherwise.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Expanding a node that is already expanded is considered a successful operation and <c>SendMessage</c> returns a nonzero value.
    /// Collapsing a node returns zero if the node is already collapsed; otherwise it returns nonzero. Attempting to expand or collapse a
    /// node that has no children is considered a failure and <c>SendMessage</c> returns zero.
    /// </para>
    /// <para>
    /// When an item is first expanded by a <c>TVM_EXPAND</c> message, the action generates TVN_ITEMEXPANDING and TVN_ITEMEXPANDED
    /// notification codes and the item's <c>TVIS_EXPANDEDONCE</c> state flag is set. As long as this state flag remains set, subsequent
    /// <c>TVM_EXPAND</c> messages do not generate TVN_ITEMEXPANDING or TVN_ITEMEXPANDED notifications. To reset the
    /// <c>TVIS_EXPANDEDONCE</c> state flag, you must send a <c>TVM_EXPAND</c> message with the TVE_COLLAPSE and TVE_COLLAPSERESET flags
    /// set. Attempting to explicitly set <c>TVIS_EXPANDEDONCE</c> will result in unpredictable behavior.
    /// </para>
    /// <para>
    /// The expand operation may fail if the owner of the treeview control denies the operation in response to a TVN_ITEMEXPANDING notification.
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-expand
    TVM_EXPAND = TV_FIRST + 2,

    /// <summary>
    /// Retrieves the bounding rectangle for a tree-view item and indicates whether the item is visible. You can send this message
    /// explicitly or by using the <c>TreeView_GetItemRect</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>
    /// Value specifying the portion of the item for which to retrieve the bounding rectangle. If this parameter is <c>TRUE</c>, the
    /// bounding rectangle includes only the text of the item. Otherwise, it includes the entire line that the item occupies in the
    /// tree-view control.
    /// </para>
    /// <para><em>lParam</em></para>
    /// <para>
    /// Pointer to a <c>RECT</c> structure that, when sending the message, contains the handle of the item to retrieve the rectangle for.
    /// See the example below for more information on how to place the item handle in this parameter. After returning from the message,
    /// this parameter contains the bounding rectangle. The coordinates are relative to the upper-left corner of the tree-view control.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// If the item is visible and the bounding rectangle was successfully retrieved, the return value is <c>TRUE</c>. Otherwise, the
    /// message returns <c>FALSE</c> and does not retrieve the bounding rectangle.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// When sending this message, the lParam parameter contains the handle of the item that the rectangle is being retrieved for. The
    /// handle is placed in lParam as shown in the following example:
    /// </para>
    /// <para>
    /// <code>RECT rc; *(HTREEITEM*)&amp;rc = hTreeItem; SendMessage(hwndTreeView, TVM_GETITEMRECT, FALSE, (LPARAM)&amp;rc);</code>
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getitemrect
    TVM_GETITEMRECT = TV_FIRST + 4,

    /// <summary>
    /// Retrieves a count of the items in a tree-view control. You can send this message explicitly or by using the
    /// <c>TreeView_GetCount</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the count of items.</para>
    /// </summary>
    /// <remarks>
    /// The node count returned by <c>TreeView_GetCount</c> is limited to integer values. If you add a node beyond 32767 the macro
    /// returns a negative value. After adding 65536 nodes the count returns to zero. When this occurs, the tree-view control appears
    /// empty with no scrollbars.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getcount
    TVM_GETCOUNT = TV_FIRST + 5,

    /// <summary>
    /// Retrieves the amount, in pixels, that child items are indented relative to their parent items. You can send this message
    /// explicitly or by using the <c>TreeView_GetIndent</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the amount of indentation.</para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getindent
    TVM_GETINDENT = TV_FIRST + 6,

    /// <summary>
    /// Sets the width of indentation for a tree-view control and redraws the control to reflect the new width. You can send this message
    /// explicitly or by using the <c>TreeView_SetIndent</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>
    /// Width, in pixels, of the indentation. If this parameter is less than the system-defined minimum width, the new width is set to
    /// the system-defined minimum.
    /// </para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>No return value.</para>
    /// </summary>
    /// <remarks>
    /// The system-defined minimum indent value is typically five pixels, but it is not fixed. To retrieve the exact value of the minimum
    /// indent on a particular system, send a <c>TVM_SETINDENT</c> message with wParam set to zero. Then send a <c>TVM_GETINDENT</c>
    /// message to retrieve the minimum indent value.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setindent
    TVM_SETINDENT = TV_FIRST + 7,

    /// <summary>
    /// Retrieves the handle to the normal or state image list associated with a tree-view control. You can send this message explicitly
    /// or by using the <c>TreeView_GetImageList</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Type of image list to retrieve. This parameter can be one of the following values:</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <term>Meaning</term>
    /// </listheader>
    /// <item>
    /// <term><c>TVSIL_NORMAL</c></term>
    /// <term>
    /// Indicates the normal image list, which contains selected, nonselected, and overlay images for the items of a tree-view control.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVSIL_STATE</c></term>
    /// <term>
    /// Indicates the state image list. You can use state images to indicate application-defined item states. A state image is displayed
    /// to the left of an item's selected or nonselected image.
    /// </term>
    /// </item>
    /// </list>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns an HIMAGELIST handle to the specified image list.</para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getimagelist
    TVM_GETIMAGELIST = TV_FIRST + 8,

    /// <summary>
    /// Sets the normal or state image list for a tree-view control and redraws the control using the new images. You can send this
    /// message explicitly or by using the <c>TreeView_SetImageList</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Type of image list to set. This parameter can be one of the following values:</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <term>Meaning</term>
    /// </listheader>
    /// <item>
    /// <term><c>TVSIL_NORMAL</c></term>
    /// <term>
    /// Indicates the normal image list, which contains selected, nonselected, and overlay images for the items of a tree-view control.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVSIL_STATE</c></term>
    /// <term>
    /// Indicates the state image list. You can use state images to indicate application-defined item states. A state image is displayed
    /// to the left of an item's selected or nonselected image.
    /// </term>
    /// </item>
    /// </list>
    /// <para><em>lParam</em></para>
    /// <para>Handle to the image list. If lParam is <c>NULL</c>, the message removes the specified image list from the tree-view control.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the handle to the previous image list, if any, or <c>NULL</c> otherwise.</para>
    /// </summary>
    /// <remarks>
    /// The tree-view control will not destroy the image list specified with this message. Your application must destroy the image list
    /// when it is no longer needed.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setimagelist
    TVM_SETIMAGELIST = TV_FIRST + 9,

    /// <summary>
    /// Retrieves the tree-view item that bears the specified relationship to a specified item. You can send this message explicitly, by
    /// using the <c>TreeView_GetNextItem</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Flag specifying the item to retrieve. This parameter can be one of the following values:</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <term>Meaning</term>
    /// </listheader>
    /// <item>
    /// <term><c>TVGN_CARET</c></term>
    /// <term>Retrieves the currently selected item. You can use the <c>TreeView_GetSelection</c> macro to send this message.</term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_CHILD</c></term>
    /// <term>
    /// Retrieves the first child item of the item specified by the <c>hitem</c> parameter. You can use the <c>TreeView_GetChild</c>
    /// macro to send this message.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_DROPHILITE</c></term>
    /// <term>
    /// Retrieves the item that is the target of a drag-and-drop operation. You can use the <c>TreeView_GetDropHilight</c> macro to send
    /// this message.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_FIRSTVISIBLE</c></term>
    /// <term>
    /// Retrieves the first item that is visible in the tree-view window. You can use the <c>TreeView_GetFirstVisible</c> macro to send
    /// this message.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_LASTVISIBLE</c></term>
    /// <term>
    /// Version 4.71. Retrieves the last expanded item in the tree. This does not retrieve the last item visible in the tree-view window.
    /// You can use the <c>TreeView_GetLastVisible</c> macro to send this message.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_NEXT</c></term>
    /// <term>Retrieves the next sibling item. You can use the <c>TreeView_GetNextSibling</c> macro to send this message.</term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_NEXTSELECTED</c></term>
    /// <term>
    /// <c>Windows Vista and later.</c> Retrieves the next selected item. You can use the <c>TreeView_GetNextSelected</c> macro to send
    /// this message.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_NEXTVISIBLE</c></term>
    /// <term>
    /// Retrieves the next visible item that follows the specified item. The specified item must be visible. Use the
    /// <c>TVM_GETITEMRECT</c> message to determine whether an item is visible. You can use the <c>TreeView_GetNextVisible</c> macro to
    /// send this message.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_PARENT</c></term>
    /// <term>Retrieves the parent of the specified item. You can use the <c>TreeView_GetParent</c> macro to send this message.</term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_PREVIOUS</c></term>
    /// <term>Retrieves the previous sibling item. You can use the <c>TreeView_GetPrevSibling</c> macro to send this message.</term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_PREVIOUSVISIBLE</c></term>
    /// <term>
    /// Retrieves the first visible item that precedes the specified item. The specified item must be visible. Use the
    /// <c>TVM_GETITEMRECT</c> message to determine whether an item is visible. You can use the <c>TreeView_GetPrevVisible</c> macro to
    /// send this message.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_ROOT</c></term>
    /// <term>
    /// Retrieves the topmost or very first item of the tree-view control. You can use the <c>TreeView_GetRoot</c> macro to send this message.
    /// </term>
    /// </item>
    /// </list>
    /// <para><em>lParam</em></para>
    /// <para>Handle to an item.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// Returns the handle to the item if successful. For most cases, the message returns a <c>NULL</c> value to indicate an error. See
    /// the Remarks section for details.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This message will return <c>NULL</c> if the item being retrieved is the root node of the tree. For example, if you use this
    /// message with the TVGN_PARENT flag on a first-level child of the tree view's root node, the message will return <c>NULL</c>.
    /// </para>
    /// <para>You can also use one of these related macros:</para>
    /// <list type="table">
    /// <listheader>
    /// <term/>
    /// </listheader>
    /// <item>
    /// <term><c>TreeView_GetChild</c></term>
    /// </item>
    /// <item>
    /// <term><c>TreeView_GetDropHilight</c></term>
    /// </item>
    /// <item>
    /// <term><c>TreeView_GetFirstVisible</c></term>
    /// </item>
    /// <item>
    /// <term><c>TreeView_GetLastVisible</c></term>
    /// </item>
    /// <item>
    /// <term><c>TreeView_GetNextSibling</c></term>
    /// </item>
    /// <item>
    /// <term><c>TreeView_GetNextVisible</c></term>
    /// </item>
    /// <item>
    /// <term><c>TreeView_GetParent</c></term>
    /// </item>
    /// <item>
    /// <term><c>TreeView_GetPrevSibling</c></term>
    /// </item>
    /// <item>
    /// <term><c>TreeView_GetPrevVisible</c></term>
    /// </item>
    /// <item>
    /// <term><c>TreeView_GetRoot</c></term>
    /// </item>
    /// <item>
    /// <term><c>TreeView_GetSelection</c></term>
    /// </item>
    /// </list>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getnextitem
    TVM_GETNEXTITEM = TV_FIRST + 10,

    /// <summary>
    /// Selects the specified tree-view item, scrolls the item into view, or redraws the item in the style used to indicate the target of
    /// a drag-and-drop operation. You can send this message explicitly or by using the <c>TreeView_Select</c>,
    /// <c>TreeView_SelectItem</c>, or <c>TreeView_SelectDropTarget</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Action flag. This parameter can be one of the following values:</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <term>Meaning</term>
    /// </listheader>
    /// <item>
    /// <term><c>TVGN_CARET</c></term>
    /// <term>
    /// Sets the selection to the specified item. The tree-view control's parent window receives the TVN_SELCHANGING and TVN_SELCHANGED
    /// notification codes.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_DROPHILITE</c></term>
    /// <term>Redraws the specified item in the style used to indicate the target of a drag-and-drop operation.</term>
    /// </item>
    /// <item>
    /// <term><c>TVGN_FIRSTVISIBLE</c></term>
    /// <term>
    /// Ensures that the specified item is visible, and, if possible, displays it at the top of the control's window. Tree-view controls
    /// display as many items as will fit in the window. If the specified item is near the bottom of the control's hierarchy of items, it
    /// might not become the first visible item, depending on how many items fit in the window.
    /// </term>
    /// </item>
    /// <item>
    /// <term><c>TVSI_NOSINGLEEXPAND</c></term>
    /// <term>
    /// When a single item is selected, ensures that the treeview does not expand the children of that item. This is valid only if used
    /// with the TVGN_CARET flag.
    /// </term>
    /// </item>
    /// </list>
    /// <para><em>lParam</em></para>
    /// <para>Handle to an item. If lParam is <c>NULL</c>, the control is set to have no selected item.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns <c>TRUE</c> if successful, or <c>FALSE</c> otherwise.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the specified item is the child of a collapsed parent item, the parent's list of child items is expanded to reveal the
    /// specified item. In this case, the control's parent window receives the TVN_ITEMEXPANDING and TVN_ITEMEXPANDED notification codes.
    /// </para>
    /// <para>
    /// Using the <c>TreeView_SelectItem</c> macro is equivalent to sending the <c>TVM_SELECTITEM</c> message with wParam set to the
    /// TVGN_CARET value. Using the <c>TreeView_SelectDropTarget</c> macro is equivalent to sending the <c>TVM_SELECTITEM</c> message
    /// with wParam set to the TVGN_DROPHILITE value. Using <c>TreeView_SelectSetFirstVisible</c> is equivalent to sending the
    /// <c>TVM_SELECTITEM</c> message with wParam set to the TVGN_FIRSTVISIBLE value.
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-selectitem
    TVM_SELECTITEM = TV_FIRST + 11,

    /// <summary>
    /// Retrieves the handle to the edit control being used to edit a tree-view item's text. You can send this message explicitly or by
    /// using the <c>TreeView_GetEditControl</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the handle to the edit control if successful, or <c>NULL</c> otherwise.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// When label editing begins, an edit control is created, but not positioned or displayed. Before it is displayed, the tree-view
    /// control sends its parent window an TVN_BEGINLABELEDIT notification code.
    /// </para>
    /// <para>
    /// To customize label editing, implement a handler for TVN_BEGINLABELEDIT and have it send a <c>TVM_GETEDITCONTROL</c> message to
    /// the tree-view control. If a label is being edited, the return value will be a handle to the edit control. Use this handle to
    /// customize the edit control by sending the usual <c>EM_XXX</c> messages.
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-geteditcontrol
    TVM_GETEDITCONTROL = TV_FIRST + 15,

    /// <summary>
    /// Obtains the number of items that can be fully visible in the client window of a tree-view control. You can send this message
    /// explicitly or by using the <c>TreeView_GetVisibleCount</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the number of items that can be fully visible in the client window of the tree-view control.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The number of items that can be fully visible may be greater than the number of items in the control. The control calculates this
    /// value by dividing the height of the client window by the height of an item.
    /// </para>
    /// <para>
    /// Note that the return value is the number of items that can be fully visible. If you can see all of 20 items and part of one more
    /// item, the return value is 20.
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getvisiblecount
    TVM_GETVISIBLECOUNT = TV_FIRST + 16,

    /// <summary>
    /// Determines the location of the specified point relative to the client area of a tree-view control. You can send this message
    /// explicitly or by using the <c>TreeView_HitTest</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>
    /// Pointer to a <c>TVHITTESTINFO</c> structure. When the message is sent, the <c>pt</c> member specifies the coordinates of the
    /// point to test. When the message returns, the <c>hItem</c> member is the handle to the item at the specified point or <c>NULL</c>
    /// if no item occupies the point. Also, when the message returns, the <c>flags</c> member is a hit test value that indicates the
    /// location of the specified point. For a list of hit test values, see the description of the <c>TVHITTESTINFO</c> structure.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the handle to the tree-view item that occupies the specified point, or <c>NULL</c> if no item occupies the point.</para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-hittest
    TVM_HITTEST = TV_FIRST + 17,

    /// <summary>
    /// Creates a dragging bitmap for the specified item in a tree-view control. The message also creates an image list for the bitmap
    /// and adds the bitmap to the image list. An application can display the image when dragging the item by using the image list
    /// functions. You can send this message explicitly or by using the <c>TreeView_CreateDragImage</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Handle to the item that receives the new dragging bitmap.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the handle to the image list to which the dragging bitmap was added if successful, or <c>NULL</c> otherwise.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// If you create a tree-view control without an associated image list, you cannot use the <c>TVM_CREATEDRAGIMAGE</c> message to
    /// create the image to display during a drag operation. You must implement your own method of creating a drag cursor.
    /// </para>
    /// <para>Your application is responsible for destroying the image list when it is no longer needed.</para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-createdragimage
    TVM_CREATEDRAGIMAGE = TV_FIRST + 18,

    /// <summary>
    /// Sorts the child items of the specified parent item in a tree-view control. You can send this message explicitly or by using the
    /// <c>TreeView_SortChildren</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>
    /// Value that specifies whether the sorting is recursive. Set wParam to <c>TRUE</c> to sort all levels of child items below the
    /// parent item. Otherwise, only the parent's immediate children are sorted.
    /// </para>
    /// <para><em>lParam</em></para>
    /// <para>Handle to the parent item whose child items are to be sorted.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns <c>TRUE</c> if successful, or <c>FALSE</c> otherwise.</para>
    /// </summary>
    /// <remarks>
    /// This message alphabetizes the tree items using <c>lstrcmpi</c> on the item name. You can use the <c>TVM_SORTCHILDRENCB</c>
    /// message to customize the ordering behavior.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-sortchildren
    TVM_SORTCHILDREN = TV_FIRST + 19,

    /// <summary>
    /// Ensures that a tree-view item is visible, expanding the parent item or scrolling the tree-view control, if necessary. You can
    /// send this message explicitly or by using the <c>TreeView_EnsureVisible</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Handle to the item.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// Returns nonzero if the system scrolled the items in the tree-view control and no items were expanded. Otherwise, the message
    /// returns zero.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If the TVM_ENSUREVISIBLE message expands the parent item, the parent window receives the TVN_ITEMEXPANDING and TVN_ITEMEXPANDED
    /// notification codes.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-ensurevisible
    TVM_ENSUREVISIBLE = TV_FIRST + 20,

    /// <summary>
    /// Sorts tree-view items using an application-defined callback function that compares the items. You can send this message
    /// explicitly or by using the <c>TreeView_SortChildrenCB</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Reserved. Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>
    /// Pointer to a <c>TVSORTCB</c> structure. The <c>lpfnCompare</c> member is the address of the application-defined callback
    /// function, which is called during the sort operation each time the relative order of two list items needs to be compared. For more
    /// information about the callback function, see the description of <c>TVSORTCB</c>.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns <c>TRUE</c> if successful, or <c>FALSE</c> otherwise.</para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-sortchildrencb
    TVM_SORTCHILDRENCB = TV_FIRST + 21,

    /// <summary>
    /// Ends the editing of a tree-view item's label. You can send this message explicitly or by using the
    /// <c>TreeView_EndEditLabelNow</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>
    /// Variable that indicates whether the editing is canceled without being saved to the label. If this parameter is <c>TRUE</c>, the
    /// system cancels editing without saving the changes. Otherwise, the system saves the changes to the label.
    /// </para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns <c>TRUE</c> if successful, or <c>FALSE</c> otherwise.</para>
    /// </summary>
    /// <remarks>This message causes the TVN_ENDLABELEDIT notification code to be sent to the parent window of the tree-view control.</remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-endeditlabelnow
    TVM_ENDEDITLABELNOW = TV_FIRST + 22,

    /// <summary>
    /// Sets a tree-view control's child tooltip control. You can send this message explicitly or by using the
    /// <c>TreeView_SetToolTips</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Handle to a tooltip control.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// Returns the handle to tooltip control previously set for the tree-view control, or <c>NULL</c> if tooltips were not previously used.
    /// </para>
    /// </summary>
    /// <remarks>
    /// When created, tree-view controls automatically create a child tooltip control. To prevent a tree-view control from using
    /// tooltips, create the control with the <c>TVS_NOTOOLTIPS</c> style.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-settooltips
    TVM_SETTOOLTIPS = TV_FIRST + 24,

    /// <summary>
    /// Retrieves the handle to the child tooltip control used by a tree-view control. You can send this message explicitly or by using
    /// the <c>TreeView_GetToolTips</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the handle to the child tooltip control, or <c>NULL</c> if the control is not using tooltips.</para>
    /// </summary>
    /// <remarks>
    /// When created, tree-view controls automatically create a child tooltip control. To cause a tree-view control not to use tooltips,
    /// create the control with the <c>TVS_NOTOOLTIPS</c> style.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-gettooltips
    TVM_GETTOOLTIPS = TV_FIRST + 25,

    /// <summary>
    /// Sets the insertion mark in a tree-view control. You can send this message explicitly or by using the
    /// <c>TreeView_SetInsertMark</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>
    /// <c>BOOL</c> value that specifies if the insertion mark is placed before or after the specified item. If this argument is nonzero,
    /// the insertion mark will be placed after the item. If this argument is zero, the insertion mark will be placed before the item.
    /// </para>
    /// <para><em>lParam</em></para>
    /// <para>
    /// <c>HTREEITEM</c> that specifies at which item the insertion mark will be placed. If this argument is <c>NULL</c>, the insertion
    /// mark is removed.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns nonzero if successful, or zero otherwise.</para>
    /// </summary>
    /// <remarks>
    /// In some circumstances, the insert mark can appear in two places after a node is expanded. If you are using insertion marks, it is
    /// recommended that you force a refresh of the control after expanding a node.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setinsertmark
    TVM_SETINSERTMARK = TV_FIRST + 26,

    /// <summary>
    /// Sets the Unicode character format flag for the control. This message allows you to change the character set used by the control
    /// at run time rather than having to re-create the control. You can send this message explicitly or use the
    /// <c>TreeView_SetUnicodeFormat</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>
    /// Determines the character set that is used by the control. If this value is nonzero, the control will use Unicode characters. If
    /// this value is zero, the control will use ANSI characters.
    /// </para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the previous Unicode format flag for the control.</para>
    /// </summary>
    /// <remarks>See the remarks for <c>CCM_SETUNICODEFORMAT</c> for a discussion of this message.</remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setunicodeformat
    TVM_SETUNICODEFORMAT = CommonControlMessage.CCM_SETUNICODEFORMAT,

    /// <summary>
    /// Retrieves the Unicode character format flag for the control. You can send this message explicitly or use the
    /// <c>TreeView_GetUnicodeFormat</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// Returns the Unicode format flag for the control. If this value is nonzero, the control is using Unicode characters. If this value
    /// is zero, the control is using ANSI characters.
    /// </para>
    /// </summary>
    /// <remarks>See the remarks for <c>CCM_GETUNICODEFORMAT</c> for a discussion of this message.</remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getunicodeformat
    TVM_GETUNICODEFORMAT = CommonControlMessage.CCM_GETUNICODEFORMAT,

    /// <summary>
    /// Sets the height of the tree-view items. You can send this message explicitly or by using the <c>TreeView_SetItemHeight</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>
    /// New height of every item in the tree view, in pixels. Heights less than 1 will be set to 1. If this argument is not even and the
    /// tree-view control does not have the <c>TVS_NONEVENHEIGHT</c> style, this value will be rounded down to the nearest even value. If
    /// this argument is -1, the control will revert to using its default item height.
    /// </para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the previous height of the items, in pixels.</para>
    /// </summary>
    /// <remarks>
    /// The tree-view control uses this value for the height of all items. To modify the height of individual items, see the description
    /// of the <c>iIntegral</c> member of the <c>TVITEMEX</c> structure.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setitemheight
    TVM_SETITEMHEIGHT = TV_FIRST + 27,

    /// <summary>
    /// Retrieves the current height of the each tree-view item. You can send this message explicitly or by using the
    /// <c>TreeView_GetItemHeight</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the height of each item, in pixels.</para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getitemheight
    TVM_GETITEMHEIGHT = TV_FIRST + 28,

    /// <summary>
    /// Sets the background color of the control. You can send this message explicitly or by using the <c>TreeView_SetBkColor</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>
    /// <c>COLORREF</c> value that contains the new background color. If this value is -1, the control will revert to using the system
    /// color for the background color.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// Returns a <c>COLORREF</c> value that represents the previous background color. If this value is -1, the control was using the
    /// system color for the background color.
    /// </para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setbkcolor
    TVM_SETBKCOLOR = TV_FIRST + 29,

    /// <summary>
    /// Sets the text color of the control. You can send this message explicitly or by using the <c>TreeView_SetTextColor</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>
    /// <c>COLORREF</c> value that contains the new text color. If this argument is -1, the control will revert to using the system color
    /// for the text color.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// Returns a <c>COLORREF</c> value that represents the previous text color. If this value is -1, the control was using the system
    /// color for the text color.
    /// </para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-settextcolor
    TVM_SETTEXTCOLOR = TV_FIRST + 30,

    /// <summary>
    /// Retrieves the current background color of the control. You can send this message explicitly or by using the
    /// <c>TreeView_GetBkColor</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// Returns a <c>COLORREF</c> value that represents the current background color. If this value is -1, the control is using the
    /// system color for the background color.
    /// </para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getbkcolor
    TVM_GETBKCOLOR = TV_FIRST + 31,

    /// <summary>
    /// Retrieves the current text color of the control. You can send this message explicitly or by using the
    /// <c>TreeView_GetTextColor</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// Returns a <c>COLORREF</c> value that represents the current text color. If this value is -1, the control is using the system
    /// color for the text color.
    /// </para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-gettextcolor
    TVM_GETTEXTCOLOR = TV_FIRST + 32,

    /// <summary>
    /// Sets the maximum scroll time for the tree-view control. You can send this message explicitly or by using the
    /// <c>TreeView_SetScrollTime</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>New maximum scroll time, in milliseconds.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the previous maximum scroll time, in milliseconds.</para>
    /// </summary>
    /// <remarks>
    /// The maximum scroll time is the longest amount of time that a scroll operation can take. Scrolling will be adjusted so that the
    /// scroll will take place within the maximum scroll time. A scroll operation may take less time than the maximum.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setscrolltime
    TVM_SETSCROLLTIME = TV_FIRST + 33,

    /// <summary>
    /// Retrieves the maximum scroll time for the tree-view control. You can send this message explicitly or by using the
    /// <c>TreeView_GetScrollTime</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the maximum scroll time, in milliseconds.</para>
    /// </summary>
    /// <remarks>
    /// The maximum scroll time is the longest amount of time that a scroll operation can take. The scrolling will be adjusted so that
    /// the scroll will take place within the maximum scroll time. A scroll operation may take less time than the maximum.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getscrolltime
    TVM_GETSCROLLTIME = TV_FIRST + 34,

    /// <summary>
    /// Sets the color used to draw the insertion mark for the tree view. You can send this message explicitly or by using the
    /// <c>TreeView_SetInsertMarkColor</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para><c>COLORREF</c> value that contains the new insertion mark color.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns a <c>COLORREF</c> value that contains the previous insertion mark color.</para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setinsertmarkcolor
    TVM_SETINSERTMARKCOLOR = TV_FIRST + 37,

    /// <summary>
    /// Retrieves the color used to draw the insertion mark for the tree view. You can send this message explicitly or by using the
    /// <c>TreeView_GetInsertMarkColor</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns a <c>COLORREF</c> value that contains the current insertion mark color.</para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getinsertmarkcolor
    TVM_GETINSERTMARKCOLOR = TV_FIRST + 38,

    /// <summary>
    /// <para><c>Intended for internal use; not recommended for use in applications.</c></para>
    /// <para>
    /// Sets the size of the border for the items in a tree-view control. You can send the message explicitly or by using the
    /// <c>TreeView_SetBorder</c> macro.
    /// </para>
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Action flags. This parameter can be one or more of the following values:</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <term>Meaning</term>
    /// </listheader>
    /// <item>
    /// <term><c>TVSBF_XBORDER</c></term>
    /// <term>Applies the specified border size to the left side of the items in the tree-view control.</term>
    /// </item>
    /// <item>
    /// <term><c>TVSBF_YBORDER</c></term>
    /// <term>Applies the specified border size to the top of the items in the tree-view control.</term>
    /// </item>
    /// </list>
    /// <para><em>lParam</em></para>
    /// <para>
    /// The <c>LOWORD</c> is a <c>SHORT</c> that specifies the size of the left border, in pixels. The <c>HIWORD</c> is a <c>SHORT</c>
    /// that specifies the size of the top border, in pixels.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// Returns a <c>LONG</c> value that contains the previous border size, in pixels. The <c>LOWORD</c> contains the previous size of
    /// the horizontal border, and the <c>HIWORD</c> contains the previous size of the vertical border.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para><c>Security Warning:</c> Using this message might compromise the security of your program.</para>
    /// <para>The item border is set just for spacing purposes. A successful setting triggers a recalculation of the scroll bars.</para>
    /// <para>
    /// This message may not be supported in future versions of Comctl32.dll. Also, this message is not defined in commctrl.h. Add the
    /// following definitions to the source files of your application to use the message:
    /// </para>
    /// <para>
    /// <code>#define TVM_SETBORDER (TV_FIRST + 35) #define TVSBF_XBORDER 0x00000001 #define TVSBF_YBORDER 0x00000002</code>
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setborder
    TVM_SETBORDER = TV_FIRST + 35,

    /// <summary>
    /// Retrieves some or all of a tree-view item's state attributes. You can send this message explicitly or by using the
    /// <c>TreeView_GetItemState</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Handle to the item.</para>
    /// <para><em>lParam</em></para>
    /// <para>Mask used to specify the states to query for. It is equivalent to the <c>stateMask</c> member of <c>TVITEMEX</c>.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>
    /// Returns a <c>UINT</c> value with the appropriate state bits set to <c>TRUE</c>. Only those bits that are specified by lParam and
    /// that are <c>TRUE</c> will be set. This value is equivalent to the <c>state</c> member of <c>TVITEMEX</c>.
    /// </para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getitemstate
    TVM_GETITEMSTATE = TV_FIRST + 39,

    /// <summary>
    /// The <c>TVM_SETLINECOLOR</c> message sets the current line color.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>New line color. Use the CLR_DEFAULT value to restore the system default colors.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the previous line color.</para>
    /// </summary>
    /// <remarks>
    /// This message only changes line colors. To change the colors of the '+' and '-' inside the buttons, use the
    /// <c>TVM_SETTEXTCOLOR</c> message.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setlinecolor
    TVM_SETLINECOLOR = TV_FIRST + 40,

    /// <summary>
    /// The <c>TVM_GETLINECOLOR</c> message gets the current line color.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the current line color, or the CLR_DEFAULT value if none has been specified.</para>
    /// </summary>
    /// <remarks>
    /// This message only retrieves line colors. To retrieve the colors of the '+' and '-' inside the buttons, use the
    /// <c>TVM_GETTEXTCOLOR</c> message.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getlinecolor
    TVM_GETLINECOLOR = TV_FIRST + 41,

    /// <summary>
    /// Maps an accessibility ID to an <c>HTREEITEM</c>.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>**UINT** that contains the accessibility ID to map to an **HTREEITEM**.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the <c>HTREEITEM</c> that the specified accessibility ID is mapped to.</para>
    /// </summary>
    /// <remarks>
    /// <para>When you add an item to a tree-view control an <c>HTREEITEM</c> returns, which uniquely identifies the item.</para>
    /// <para>
    /// <para>Note</para>
    /// <para>
    /// To use this message, you must provide a manifest specifying Comclt32.dll version 6.0. For more information on manifests, see
    /// Enabling Visual Styles.
    /// </para>
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-mapaccidtohtreeitem
    TVM_MAPACCIDTOHTREEITEM = TV_FIRST + 42,

    /// <summary>
    /// Maps an <c>HTREEITEM</c> to an accessibility ID.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>**HTREEITEM** that is mapped to an accessibility ID.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns an accessibility ID.</para>
    /// </summary>
    /// <remarks>
    /// <para>When you add an item to a tree-view control an <c>HTREEITEM</c> handle is returned that uniquely identifies the item.</para>
    /// <para>
    /// <para>Note</para>
    /// <para>
    /// To use this message, you must provide a manifest specifying Comclt32.dll version 6.0. For more information on manifests, see
    /// Enabling Visual Styles.
    /// </para>
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-maphtreeitemtoaccid
    TVM_MAPHTREEITEMTOACCID = TV_FIRST + 43,

    /// <summary>
    /// Informs the tree-view control to set extended styles. Send this message or use the macro <c>TreeView_SetExtendedStyle</c>.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Mask used to select the styles to be set.</para>
    /// <para><em>lParam</em></para>
    /// <para>Value that indicates the extended style. For more information on styles, see Tree-View Control Extended Styles.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>If this message succeeds, it returns <c>S_OK</c>. Otherwise, it returns an <c>HRESULT</c> error code.</para>
    /// </summary>
    /// <remarks>
    /// The extended styles for a tree-view control have nothing to do with the extended styles used with function <c>CreateWindowEx</c>
    /// or function <c>SetWindowLong</c>.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setextendedstyle
    TVM_SETEXTENDEDSTYLE = TV_FIRST + 44,

    /// <summary>
    /// Retrieves the extended style for a tree-view control. Send this message explicitly or by using the
    /// <c>TreeView_GetExtendedStyle</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the value of extended style.For more information on styles, see Tree-View Control Extended Styles.</para>
    /// </summary>
    /// <remarks>
    /// The extended styles for a tree-view control have nothing to do with the extended styles used with function <c>CreateWindowEx</c>
    /// or function <c>SetWindowLong</c>.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getextendedstyle
    TVM_GETEXTENDEDSTYLE = TV_FIRST + 45,

    /// <summary>
    /// Inserts a new item in a tree-view control. You can send this message explicitly or by using the <c>TreeView_InsertItem</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Pointer to a <c>TVINSERTSTRUCT</c> structure that specifies the attributes of the tree-view item.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the <c>HTREEITEM</c> handle to the new item if successful, or <c>NULL</c> otherwise.</para>
    /// </summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-insertitem
    TVM_INSERTITEM = TV_FIRST + 50,

    /// <summary>
    /// Sets information used to determine auto-scroll characteristics. You can send this message explicitly or by using the
    /// <c>TreeView_SetAutoScrollInfo</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Specifies pixels per second. The offset to scroll is divided by the wParam to determine the total duration of the auto-scroll.</para>
    /// <para><em>lParam</em></para>
    /// <para>
    /// Specifies the redraw time interval. Redraw at every elasped interval, until the item is scrolled into view. Given wParam, the
    /// location of the item is calculated and a repaint occurs. Set this value to create smooth scrolling.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns <c>TRUE</c>.</para>
    /// </summary>
    /// <remarks>
    /// Autoscroll information is used to scroll a nonvisible item into view. The control must have the <c>TVS_EX_AUTOHSCROLL</c>
    /// extended style. For information on extended styles, see Tree-View Control Extended Styles.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setautoscrollinfo
    TVM_SETAUTOSCROLLINFO = TV_FIRST + 59,

    /// <summary>
    /// <para>
    /// [Intended for internal use; not recommended for use in applications. This message may not be supported in future versions of Windows.]
    /// </para>
    /// <para>
    /// Sets the hot item for a tree-view control. You can send this message explicitly or by using the <c>TreeView_SetHot</c> macro.
    /// </para>
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Handle to the new hot item. If this value is <c>NULL</c>, the tree-view control will be set to have no hot item.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns <c>TRUE</c> if successful, or <c>FALSE</c> otherwise.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The hot item is the item that the mouse is hovering over. This message makes an item look like it is the hot item even if the
    /// mouse is not hovering over it.
    /// </para>
    /// <para>This message has no visible effect if the <c>TVS_TRACKSELECT</c> style is not set.</para>
    /// <para>If it succeeds, this message causes the hot item to be redrawn.</para>
    /// <para>This message is ignored if lParam is <c>NULL</c> and the tree-view control is tracking the mouse.</para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-sethot
    TVM_SETHOT = TV_FIRST + 58,

    /// <summary>
    /// Retrieves some or all of a tree-view item's attributes. You can send this message explicitly or by using the
    /// <c>TreeView_GetItem</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>
    /// Pointer to a <c>TVITEM</c> structure that specifies the information to retrieve and receives information about the item. With
    /// version 4.71 and later, you can use a <c>TVITEMEX</c> structure instead.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns <c>TRUE</c> if successful, or <c>FALSE</c> otherwise.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// When the message is sent, the <c>hItem</c> member of the <c>TVITEM</c> or <c>TVITEMEX</c> structure identifies the item to
    /// retrieve information about, and the <c>mask</c> member specifies the attributes to retrieve.
    /// </para>
    /// <para>
    /// If the TVIF_TEXT flag is set in the <c>mask</c> member of the <c>TVITEM</c> or <c>TVITEMEX</c> structure, the <c>pszText</c>
    /// member must point to a valid buffer and the <c>cchTextMax</c> member must be set to the number of characters in that buffer.
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getitem
    TVM_GETITEM = TV_FIRST + 62,

    /// <summary>
    /// The <c>TVM_SETITEM</c> message sets some or all of a tree-view item's attributes. You can send this message explicitly or by
    /// using the <c>TreeView_SetItem</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>
    /// Pointer to a <c>TVITEM</c> structure that contains the new item attributes. With version 4.71 and later, you can use a
    /// <c>TVITEMEX</c> structure instead.
    /// </para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns <c>TRUE</c> if successful, or <c>FALSE</c> otherwise.</para>
    /// </summary>
    /// <remarks>
    /// The <c>hItem</c> member of the <c>TVITEM</c> or <c>TVITEMEX</c> structure identifies the item, and the <c>mask</c> member
    /// specifies which attributes to set.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-setitem
    TVM_SETITEM = TV_FIRST + 63,

    /// <summary>
    /// Retrieves the incremental search string for a tree-view control. The tree-view control uses the incremental search string to
    /// select an item based on characters typed by the user. You can send this message explicitly or by using the
    /// <c>TreeView_GetISearchString</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Pointer to the buffer that receives the incremental search string.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the number of characters in the incremental search string.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>Security Warning:</c> Using this message incorrectly might compromise the security of your program. You must allocate a large
    /// enough buffer to hold the string. First call the message passing <c>NULL</c> in lParam. This returns the number of characters,
    /// excluding <c>NULL</c>, that are required. Then call the message a second time to retrieve the string. You should review Security
    /// Considerations: Microsoft Windows Controls before continuing.
    /// </para>
    /// <para>If the tree-view control is not in incremental search mode, the return value is zero.</para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getisearchstring
    TVM_GETISEARCHSTRING = TV_FIRST + 64,

    /// <summary>
    /// Begins in-place editing of the specified item's text, replacing the text of the item with a single-line edit control containing
    /// the text. This message implicitly selects and focuses the specified item. You can send this message explicitly or by using the
    /// <c>TreeView_EditLabel</c> macro.
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Handle to the item to edit.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns the handle to the edit control used to edit the item text if successful, or <c>NULL</c> otherwise.</para>
    /// </summary>
    /// <remarks>
    /// <para>This message sends a TVN_BEGINLABELEDIT notification code to the parent of the tree-view control.</para>
    /// <para>
    /// When the user completes or cancels editing, the edit control is destroyed and the handle is no longer valid. You can subclass the
    /// edit control, but do not destroy it.
    /// </para>
    /// <para>
    /// The control must have the focus before you send this message to the control. Focus can be set using the <c>SetFocus</c> function.
    /// </para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-editlabel
    TVM_EDITLABEL = TV_FIRST + 65,

    /// <summary>This message is not implemented.</summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getselectedcount
    TVM_GETSELECTEDCOUNT = TV_FIRST + 70,

    /// <summary>
    /// Shows the infotip for a specified item in a tree-view control. You can send this message explicitly or by using the
    /// <c>TreeView_ShowInfoTip</c> macro..
    /// <para><strong>Parameters</strong></para>
    /// <para><em>wParam</em></para>
    /// <para>Must be zero.</para>
    /// <para><em>lParam</em></para>
    /// <para>Handle to the item.</para>
    /// <para><strong>Returns</strong></para>
    /// <para>Returns zero.</para>
    /// </summary>
    /// <remarks>
    /// Most applications do not use this message. Infotips are shown automatically. For more information, see Using Tree-view Infotips
    /// in the About Tree-View Controls overview.
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-showinfotip
    TVM_SHOWINFOTIP = TV_FIRST + 71,

    /// <summary>This message is not implemented.</summary>
    // https://docs.microsoft.com/en-us/windows/win32/controls/tvm-getitempartrect
    TVM_GETITEMPARTRECT = TV_FIRST + 72,
}
