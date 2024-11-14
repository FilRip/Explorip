using System;
using System.Runtime.InteropServices;

namespace ManagedShell.ShellFolders.Interfaces;

[ComImport, Guid("00000114-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IOleWindow
{
    /// <summary>
    /// Retrieves a handle to one of the windows participating in in-place activation (frame, document, parent, or in-place object window).
    /// </summary>
    /// <param name="phwnd">A pointer to a variable that receives the window handle.</param>
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
    int GetWindow(out IntPtr phwnd);

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
    int ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool)] bool fEnterMode);
}
