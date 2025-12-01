using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Interop;

using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Structs;

namespace ManagedShell.ShellFolders.Interfaces;

[ComImport(), Guid("000214E2-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellBrowser : IOleWindow
{
    /// <summary>
    /// Retrieves a handle to one of the windows participating in in-place activation (frame, document, parent, or in-place object window).
    /// </summary>
    /// <param name="pIntPtr">A pointer to a variable that receives the window handle.</param>
    /// <returns>
    /// This method returns S_OK on success. Other possible return values include the following.
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <description>E_FAIL</description>
    /// <description>The object is windowless.</description>
    /// </item>
    /// <item>
    /// <description>E_OUTOFMEMORY</description>
    /// <description>There is insufficient memory available for this operation.</description>
    /// </item>
    /// <item>
    /// <description>E_UNEXPECTED</description>
    /// <description>An unexpected error has occurred.</description>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// Five types of windows comprise the windows hierarchy. When a object is active in place, it has access to some or all of
    /// these windows.
    /// </para>
    /// <list type="table">
    /// <listheader>
    /// <term>Window</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <description>Frame</description>
    /// <description>The outermost main window where the container application's main menu resides.</description>
    /// </item>
    /// <item>
    /// <description>Document</description>
    /// <description>The window that displays the compound document containing the embedded object to the user.</description>
    /// </item>
    /// <item>
    /// <description>Pane</description>
    /// <description>
    /// The subwindow of the document window that contains the object's view. Applicable only for applications with split-pane windows.
    /// </description>
    /// </item>
    /// <item>
    /// <description>Parent</description>
    /// <description>
    /// The container window that contains that object's view. The object application installs its window as a child of this window.
    /// </description>
    /// </item>
    /// <item>
    /// <description>In-place</description>
    /// <description>
    /// The window containing the active in-place object. The object application creates this window and installs it as a child of
    /// its hatch window, which is a child of the container's parent window.
    /// </description>
    /// </item>
    /// </list>
    /// <para>
    /// Each type of window has a different role in the in-place activation architecture. However, it is not necessary to employ a
    /// separate physical window for each type. Many container applications use the same window for their frame, document, pane, and
    /// parent windows.
    /// </para>
    /// </remarks>
    [PreserveSig()]
    new int GetWindow(out IntPtr pIntPtr);

    /// <summary>Determines whether context-sensitive help mode should be entered during an in-place activation session.</summary>
    /// <param name="fEnterMode">
    /// <see langword="true"/> if help mode should be entered; <see langword="false"/> if it should be exited.
    /// </param>
    /// <returns>
    /// <para>
    /// This method returns S_OK if the help mode was entered or exited successfully, depending on the value passed in <paramref
    /// name="fEnterMode"/>. Other possible return values include the following. <br/>
    /// </para>
    /// <list type="table">
    /// <listheader>
    /// <term>Return code</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <description>E_INVALIDARG</description>
    /// <description>The specified <paramref name="fEnterMode"/> value is not valid.</description>
    /// </item>
    /// <item>
    /// <description>E_OUTOFMEMORY</description>
    /// <description>There is insufficient memory available for this operation.</description>
    /// </item>
    /// <item>
    /// <description>E_UNEXPECTED</description>
    /// <description>An unexpected error has occurred.</description>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>Applications can invoke context-sensitive help when the user:</para>
    /// <list type="bullet">
    /// <item>presses SHIFT+F1, then clicks a topic</item>
    /// <item>presses F1 when a menu item is selected</item>
    /// </list>
    /// <para>
    /// When SHIFT+F1 is pressed, either the frame or active object can receive the keystrokes. If the container's frame receives
    /// the keystrokes, it calls its containing document's IOleWindow::ContextSensitiveHelp method with <paramref
    /// name="fEnterMode"/> set to <see langword="true"/>. This propagates the help state to all of its in-place objects so they can
    /// correctly handle the mouse click or WM_COMMAND.
    /// </para>
    /// <para>
    /// If an active object receives the SHIFT+F1 keystrokes, it calls the container's IOleWindow::ContextSensitiveHelp method with
    /// <paramref name="fEnterMode"/> set to <see langword="true"/>, which then recursively calls each of its in-place sites until
    /// there are no more to be notified. The container then calls its document's or frame's IOleWindow::ContextSensitiveHelp method
    /// with <paramref name="fEnterMode"/> set to <see langword="true"/>.
    /// </para>
    /// <para>When in context-sensitive help mode, an object that receives the mouse click can either:</para>
    /// <list type="bullet">
    /// <item>Ignore the click if it does not support context-sensitive help.</item>
    /// <item>
    /// Tell all the other objects to exit context-sensitive help mode with ContextSensitiveHelp set to FALSE and then provide help
    /// for that context.
    /// </item>
    /// </list>
    /// <para>
    /// An object in context-sensitive help mode that receives a WM_COMMAND should tell all the other in-place objects to exit
    /// context-sensitive help mode and then provide help for the command.
    /// </para>
    /// <para>
    /// If a container application is to support context-sensitive help on menu items, it must either provide its own message filter
    /// so that it can intercept the F1 key or ask the OLE library to add a message filter by calling OleSetMenuDescriptor, passing
    /// valid, non-NULL values for the lpFrame and lpActiveObj parameters.
    /// </para>
    /// </remarks>
    [PreserveSig()]
    new int ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool)] bool fEnterMode);

    /// <summary>
    /// Allows the container to insert its menu groups into the composite menu that is displayed when an extended namespace is being
    /// viewed or used.
    /// </summary>
    /// <param name="IntPtrShared">A handle to an empty menu.</param>
    /// <param name="lpMenuWidths">
    /// The address of an OLEMENUGROUPWIDTHS array of six LONG values. The container fills in elements 0, 2, and 4 to reflect the
    /// number of menu elements it provided in the File, View, and Window menu groups.
    /// </param>
    [PreserveSig()]
    int InsertMenusSB(IntPtr IntPtrShared, ref OleMenuGroupWidths lpMenuWidths);

    /// <summary>Installs the composite menu in the view window.</summary>
    /// <param name="IntPtrShared">
    /// A handle to the composite menu constructed by calls to IShellBrowser::InsertMenusSB and the InsertMenu function.
    /// </param>
    /// <param name="holemenuRes"></param>
    /// <param name="IntPtrActiveObject">The view's window handle.</param>
    [PreserveSig()]
    int SetMenuSB(IntPtr IntPtrShared, IntPtr holemenuRes, IntPtr IntPtrActiveObject);

    /// <summary>
    /// Permits the container to remove any of its menu elements from the in-place composite menu and to free all associated resources.
    /// </summary>
    /// <param name="IntPtrShared">
    /// A handle to the in-place composite menu that was constructed by calls to IShellBrowser::InsertMenusSB and the InsertMenu function.
    /// </param>
    [PreserveSig()]
    int RemoveMenusSB(IntPtr IntPtrShared);

    /// <summary>Sets and displays status text about the in-place object in the container's frame-window status bar.</summary>
    /// <param name="pszStatusText">A pointer to a null-terminated character string that contains the message to display.</param>
    [PreserveSig()]
    int SetStatusTextSB([MarshalAs(UnmanagedType.LPWStr)] string pszStatusText);

    /// <summary>Tells Windows Explorer to enable or disable its modeless dialog boxes.</summary>
    /// <param name="fEnable">
    /// Specifies whether the modeless dialog boxes are to be enabled or disabled. If this parameter is nonzero, modeless dialog
    /// boxes are enabled. If this parameter is zero, modeless dialog boxes are disabled.
    /// </param>
    [PreserveSig()]
    int EnableModelessSB([MarshalAs(UnmanagedType.Bool)] bool fEnable);

    /// <summary>Translates accelerator keystrokes intended for the browser's frame while the view is active.</summary>
    /// <param name="pmsg">The address of an MSG structure containing the keystroke message.</param>
    /// <param name="wID">
    /// The command identifier value corresponding to the keystroke in the container-provided accelerator table. Containers should
    /// use this value instead of translating again.
    /// </param>
    [PreserveSig()]
    int TranslateAcceleratorSB(ref MSG pmsg, ushort wID);

    /// <summary>Informs Windows Explorer to browse to another folder.</summary>
    /// <param name="pidl">
    /// The address of an ITEMIDLIST (item identifier list) structure that specifies an object's location. This value is dependent
    /// on the flag or flags set in the wFlags parameter.
    /// </param>
    /// <param name="wFlags">Flags specifying the folder to be browsed.</param>
    [PreserveSig()]
    int BrowseObject(IntPtr pidl, SBSP wFlags);

    /// <summary>Gets an IStream interface that can be used for storage of view-specific state information.</summary>
    /// <param name="grfMode">Read/write access of the IStream interface.</param>
    /// <param name="ppStrm">The address that receives the IStream interface pointer.</param>
    [PreserveSig()]
    int GetViewStateStream(STGM grfMode, [MarshalAs(UnmanagedType.Interface)] out IStream ppStrm);

    /// <summary>Gets the window handle to a browser control.</summary>
    /// <param name="id">
    /// <para>Type: <c>UINT</c></para>
    /// <para>The control handle that is being requested. This parameter can be one of the following values:</para>
    /// <para>FCW_TOOLBAR</para>
    /// <para>Retrieves the window handle to the browser's toolbar.</para>
    /// <para>FCW_STATUS</para>
    /// <para>Retrieves the window handle to the browser's status bar.</para>
    /// <para>FCW_TREE</para>
    /// <para>Retrieves the window handle to the browser's tree view.</para>
    /// <para>FCW_PROGRESS</para>
    /// <para>Retrieves the window handle to the browser's progress bar.</para>
    /// </param>
    /// <param name="pIntPtr">
    /// <para>Type: <c>IntPtr*</c></para>
    /// <para>The address of the window handle to the Windows Explorer control.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <c>int</c></para>
    /// <para>Returns S_OK if successful, or a COM-defined error value otherwise.</para>
    /// </returns>
    /// <remarks>
    /// <para>
    /// <c>GetControlWindow</c> is used so views can directly manipulate the browser's controls. <c>FCW_TREE</c> should be used only to
    /// determine if the tree is present.
    /// </para>
    /// <para>Notes to Calling Applications</para>
    /// <para>
    /// <c>GetControlWindow</c> is used to manipulate and test the state of the control windows. Do not send messages directly to these
    /// controls; instead, use IShellBrowser::SendControlMsg. Be prepared for this method to return <c>NULL</c>. Later versions of
    /// Windows Explorer may not include a toolbar, status bar, or tree window.
    /// </para>
    /// <para>Notes to Implementers</para>
    /// <para><c>GetControlWindow</c> returns the window handle to these controls if they exist in your implementation.</para>
    /// <para>See also IShellBrowser</para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellbrowser-getcontrolwindow
    [PreserveSig()]
    int GetControlWindow(FrameControlWindows id, out IntPtr pIntPtr);

    /// <summary>Sends control messages to either the toolbar or the status bar in a Windows Explorer window.</summary>
    /// <param name="id">
    /// <para>Type: <c>UINT</c></para>
    /// <para>An identifier for either a toolbar (<c>FCW_TOOLBAR</c>) or for a status bar window (<c>FCW_STATUS</c>).</para>
    /// </param>
    /// <param name="uMsg">
    /// <para>Type: <c>UINT</c></para>
    /// <para>The message to be sent to the control.</para>
    /// </param>
    /// <param name="wParam">
    /// <para>Type: <c>WPARAM</c></para>
    /// <para>The value depends on the message specified in the uMsg parameter.</para>
    /// </param>
    /// <param name="lParam">
    /// <para>Type: <c>LPARAM</c></para>
    /// <para>The value depends on the message specified in the uMsg parameter.</para>
    /// </param>
    /// <param name="pret">
    /// <para>Type: <c>LRESULT*</c></para>
    /// <para>The address of the return value of the SendMessage function.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <c>int</c></para>
    /// <para>Returns <c>S_OK</c> if successful, or a COM-defined error value otherwise.</para>
    /// </returns>
    /// <remarks>
    /// <para>Refer to the Common Controls documentation for more information on the messages that can be sent to the toolbar or status bar control.</para>
    /// <para>Notes to Calling Applications</para>
    /// <para>Use of this call requires diligent attention, because leaving either the status bar or toolbar in an inappropriate state will affect the performance of Windows Explorer.</para>
    /// <para>Notes to Implementers</para>
    /// <para>If your Windows Explorer does not have these controls, you can return <c>E_NOTIMPL</c>.</para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellbrowser-sendcontrolmsg
    [PreserveSig()]
    int SendControlMsg(FrameControlWindows id, uint uMsg, IntPtr wParam, IntPtr lParam, out IntPtr pret);

    /// <summary>Retrieves the currently active (displayed) Shell view object.</summary>
    /// <param name="ppshv">
    /// <para>Type: <c>IShellView**</c></para>
    /// <para>The address of the pointer to the currently active Shell view object.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <c>int</c></para>
    /// <para>Returns S_OK if successful, or a COM-defined error value otherwise.</para>
    /// </returns>
    /// <remarks>
    /// <para>Notes to Calling Applications</para>
    /// <para>Because the IShellBrowser interface can host several Shell views simultaneously, this method provides an easy way to determine the active Shell view object.</para>
    /// </remarks>
    // https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellbrowser-queryactiveshellview
    [PreserveSig()]
    int QueryActiveShellView(out IShellView ppshv);

    /// <summary>Called by the Shell view when the view window or one of its child windows gets the focus or becomes active.</summary>
    /// <param name="ppshv">Address of the view object's IShellView pointer.</param>
    [PreserveSig()]
    int OnViewWindowActive(IShellView ppshv);

    /// <summary>
    /// <note type="note">This method has no effect on Windows Vista or later operating systems.</note> Adds toolbar items to
    /// Windows Explorer's toolbar.
    /// </summary>
    /// <param name="lpButtons">The address of an array of TBBUTTON structures.</param>
    /// <param name="nButtons">The number of TBBUTTON structures in the lpButtons array.</param>
    /// <param name="uFlags">Flags specifying where the toolbar buttons should go.</param>
    [PreserveSig()]
    int SetToolbarItems([Optional, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] TbButton[] lpButtons, uint nButtons, FCT uFlags);
}
