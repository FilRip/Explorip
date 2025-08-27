using System;
using System.Runtime.InteropServices;
using System.Security;

namespace HookTaskbarList.TaskbarList.Interfaces;

/// <summary>
/// Extends ITaskbarList3 by providing a method that allows the caller to control two property values for the tab thumbnail and peek feature.
/// </summary>
[SuppressUnmanagedCodeSecurity()]
[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
public interface ITaskbarList
{
    /// <summary>
    /// Initializes the taskbar list object. This method must be called before any other ITaskbarList methods can be called.
    /// </summary>
    void HrInit();

    /// <summary>Adds an item to the taskbar.</summary>
    /// <param name="hWnd">A handle to the window to be added to the taskbar.</param>
    void AddTab(IntPtr hWnd);

    /// <summary>Deletes an item from the taskbar.</summary>
    /// <param name="hWnd">A handle to the window to be deleted from the taskbar.</param>
    void DeleteTab(IntPtr hWnd);

    /// <summary>
    /// Activates an item on the taskbar. The window is not actually activated; the window's item on the taskbar is merely displayed
    /// as active.
    /// </summary>
    /// <param name="hWnd">A handle to the window on the taskbar to be displayed as active.</param>
    void ActivateTab(IntPtr hWnd);

    /// <summary>Marks a taskbar button as active but does not visually activate it.</summary>
    /// <param name="hWnd">A handle to the window to be marked as active.</param>
    void SetActiveAlt(IntPtr hWnd);

    /// <summary>Marks a window as full-screen.</summary>
    /// <param name="hWnd">The handle of the window to be marked.</param>
    /// <param name="fFullscreen">A Boolean value marking the desired full-screen status of the window.</param>
    void MarkFullscreenWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

    /// <summary>
    /// Displays or updates a progress bar hosted in a taskbar button to show the specific percentage completed of the full operation.
    /// </summary>
    /// <param name="hWnd">The handle of the window whose associated taskbar button is being used as a progress indicator.</param>
    /// <param name="ullCompleted">
    /// An application-defined value that indicates the proportion of the operation that has been completed at the time the method is called.
    /// </param>
    /// <param name="ullTotal">
    /// An application-defined value that specifies the value ullCompleted will have when the operation is complete.
    /// </param>
    void SetProgressValue(IntPtr hWnd, ulong ullCompleted, ulong ullTotal);

    /// <summary>Sets the type and state of the progress indicator displayed on a taskbar button.</summary>
    /// <param name="hWnd">
    /// The handle of the window in which the progress of an operation is being shown. This window's associated taskbar button will
    /// display the progress bar.
    /// </param>
    /// <param name="tbpFlags">
    /// Flags that control the current state of the progress button. Specify only one of the following flags; all states are mutually
    /// exclusive of all others.
    /// </param>
    void SetProgressState(IntPtr hWnd, ETaskbarProgress tbpFlags);

    /// <summary>
    /// Informs the taskbar that a new tab or document thumbnail has been provided for display in an application's taskbar group flyout.
    /// </summary>
    /// <param name="IntPtrTab">Handle of the tab or document window. This value is required and cannot be NULL.</param>
    /// <param name="IntPtrMDI">
    /// Handle of the application's main window. This value tells the taskbar which application's preview group to attach the new
    /// thumbnail to. This value is required and cannot be NULL.
    /// </param>
    /// <remarks>
    /// By itself, registering a tab thumbnail alone will not result in its being displayed. You must also call
    /// ITaskbarList3::SetTabOrder to instruct the group where to display it.
    /// </remarks>
    void RegisterTab(IntPtr IntPtrTab, IntPtr IntPtrMDI);

    /// <summary>Removes a thumbnail from an application's preview group when that tab or document is closed in the application.</summary>
    /// <param name="IntPtrTab">
    /// The handle of the tab window whose thumbnail is being removed. This is the same value with which the thumbnail was registered
    /// as part the group through ITaskbarList3::RegisterTab. This value is required and cannot be NULL.
    /// </param>
    /// <remarks>
    /// It is the responsibility of the calling application to free IntPtrTab through DestroyWindow. UnregisterTab must be called
    /// before the handle is freed.
    /// </remarks>
    void UnregisterTab(IntPtr IntPtrTab);

    /// <summary>
    /// Inserts a new thumbnail into a tabbed-document interface (TDI) or multiple-document interface (MDI) application's group
    /// flyout or moves an existing thumbnail to a new position in the application's group.
    /// </summary>
    /// <param name="IntPtrTab">
    /// The handle of the tab window whose thumbnail is being placed. This value is required, must already be registered through
    /// ITaskbarList3::RegisterTab, and cannot be NULL.
    /// </param>
    /// <param name="IntPtrInsertBefore">
    /// The handle of the tab window whose thumbnail that IntPtrTab is inserted to the left of. This handle must already be registered
    /// through ITaskbarList3::RegisterTab. If this value is NULL, the new thumbnail is added to the end of the list.
    /// </param>
    /// <remarks>This method must be called for the thumbnail to be shown in the group. Call it after you have called ITaskbarList3::RegisterTab.</remarks>
    void SetTabOrder(IntPtr IntPtrTab, [Optional()] IntPtr IntPtrInsertBefore);

    /// <summary>Informs the taskbar that a tab or document window has been made the active window.</summary>
    /// <param name="IntPtrTab">
    /// Handle of the active tab window. This handle must already be registered through ITaskbarList3::RegisterTab. This value can be
    /// NULL if no tab is active.
    /// </param>
    /// <param name="IntPtrMDI">
    /// Handle of the application's main window. This value tells the taskbar which group the thumbnail is a member of. This value is
    /// required and cannot be NULL.
    /// </param>
    /// <param name="dwReserved">Reserved; set to 0.</param>
    void SetTabActive([Optional()] IntPtr IntPtrTab, IntPtr IntPtrMDI, uint dwReserved = 0);

    /// <summary>
    /// Adds a thumbnail toolbar with a specified set of buttons to the thumbnail image of a window in a taskbar button flyout.
    /// </summary>
    /// <param name="IntPtr">
    /// The handle of the window whose thumbnail representation will receive the toolbar. This handle must belong to the calling process.
    /// </param>
    /// <param name="cButtons">
    /// The number of buttons defined in the array pointed to by pButton. The maximum number of buttons allowed is 7.
    /// </param>
    /// <param name="pButtons">
    /// A pointer to an array of THUMBBUTTON structures. Each THUMBBUTTON defines an individual button to be added to the toolbar.
    /// Buttons cannot be added or deleted later, so this must be the full defined set. Buttons also cannot be reordered, so their
    /// order in the array, which is the order in which they are displayed left to right, will be their permanent order.
    /// </param>
    void ThumbBarAddButtons(IntPtr hWndWindow, uint cButtons, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ThumbButton[] pButtons);

    /// <summary>
    /// Shows, enables, disables, or hides buttons in a thumbnail toolbar as required by the window's current state. A thumbnail
    /// toolbar is a toolbar embedded in a thumbnail image of a window in a taskbar button flyout.
    /// </summary>
    /// <param name="hWnd">The handle of the window whose thumbnail representation contains the toolbar.</param>
    /// <param name="cButtons">
    /// The number of buttons defined in the array pointed to by pButton. The maximum number of buttons allowed is 7. This array
    /// contains only structures that represent existing buttons that are being updated.
    /// </param>
    /// <param name="pButtons">
    /// A pointer to an array of THUMBBUTTON structures. Each THUMBBUTTON defines an individual button. If the button already exists
    /// (the iId value is already defined), then that existing button is updated with the information provided in the structure.
    /// </param>
    void ThumbBarUpdateButtons(IntPtr hWnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ThumbButton[] pButtons);

    /// <summary>
    /// Specifies an image list that contains button images for a toolbar embedded in a thumbnail image of a window in a taskbar
    /// button flyout.
    /// </summary>
    /// <param name="hWnd">
    /// The handle of the window whose thumbnail representation contains the toolbar to be updated. This handle must belong to the
    /// calling process.
    /// </param>
    /// <param name="himl">The handle of the image list that contains all button images to be used in the toolbar.</param>
    /// <remarks>
    /// Applications must provide these button images:
    /// <list type="bullet">
    /// <item>
    /// <term>The button in its default active state.</term>
    /// </item>
    /// <item>
    /// <term>Images suitable for use with high-dpi (dots per inch) displays.</term>
    /// </item>
    /// </list>
    /// <para>
    /// Images must be 32-bit and of dimensions GetSystemMetrics(SM_CXICON) x GetSystemMetrics(SM_CYICON). The toolbar itself
    /// provides visuals for a button's clicked, disabled, and hover states.
    /// </para>
    /// </remarks>
    void ThumbBarSetImageList(IntPtr hWnd, IntPtr himl);

    /// <summary>Applies an overlay to a taskbar button to indicate application status or a notification to the user.</summary>
    /// <param name="hWnd">
    /// The handle of the window whose associated taskbar button receives the overlay. This handle must belong to a calling process
    /// associated with the button's application and must be a valid IntPtr or the call is ignored.
    /// </param>
    /// <param name="hIcon">
    /// The handle of an icon to use as the overlay. This should be a small icon, measuring 16x16 pixels at 96 dpi. If an overlay
    /// icon is already applied to the taskbar button, that existing overlay is replaced.
    /// <para>
    /// This value can be NULL.How a NULL value is handled depends on whether the taskbar button represents a single window or a
    /// group of windows.
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <term>If the taskbar button represents a single window, the overlay icon is removed from the display.</term>
    /// </item>
    /// <item>
    /// <term>
    /// If the taskbar button represents a group of windows and a previous overlay is still available (received earlier than the
    /// current overlay, but not yet freed by a NULL value), then that previous overlay is displayed in place of the current overlay.
    /// </term>
    /// </item>
    /// </list>
    /// <para>
    /// It is the responsibility of the calling application to free hIcon when it is no longer needed.This can generally be done
    /// after you call SetOverlayIcon because the taskbar makes and uses its own copy of the icon.
    /// </para>
    /// </param>
    /// <param name="pszDescription">
    /// A pointer to a string that provides an alt text version of the information conveyed by the overlay, for accessibility purposes.
    /// </param>
    void SetOverlayIcon(IntPtr hWnd, [Optional()] IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string? pszDescription);

    /// <summary>
    /// Specifies or updates the text of the tooltip that is displayed when the mouse pointer rests on an individual preview
    /// thumbnail in a taskbar button flyout.
    /// </summary>
    /// <param name="hWnd">
    /// The handle to the window whose thumbnail displays the tooltip. This handle must belong to the calling process.
    /// </param>
    /// <param name="pszTip">
    /// The pointer to the text to be displayed in the tooltip. This value can be NULL, in which case the title of the window
    /// specified by IntPtr is used as the tooltip.
    /// </param>
    void SetThumbnailTooltip(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string? pszTip);

    /// <summary>Selects a portion of a window's client area to display as that window's thumbnail in the taskbar.</summary>
    /// <param name="hWnd">The handle to a window represented in the taskbar.</param>
    /// <param name="prcClip">
    /// A pointer to a RECT structure that specifies a selection within the window's client area, relative to the upper-left corner
    /// of that client area. To clear a clip that is already in place and return to the default display of the thumbnail, set this
    /// parameter to NULL.
    /// </param>
    void SetThumbnailClip(IntPtr hWnd, [Optional()] Rect? prcClip);

    /// <summary>
    /// Allows a tab to specify whether the main application frame window or the tab window should be used as a thumbnail or in the
    /// peek feature under certain circumstances.
    /// </summary>
    /// <param name="IntPtrTab">
    /// The handle of the tab window that is to have properties set. This handle must already be registered through RegisterTab.
    /// </param>
    /// <param name="stpFlags">
    /// One or more members of the STPFLAG enumeration that specify the displayed thumbnail and peek image source of the tab thumbnail.
    /// </param>
    void SetTabProperties(IntPtr IntPtrTab, EThumbTab stpFlags);
}
