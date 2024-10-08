﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.Shell.Common;
using Microsoft.WindowsAPICodePack.Shell.Interop.Common;
using Microsoft.WindowsAPICodePack.Shell.Interop.ExplorerBrowser;
using Microsoft.WindowsAPICodePack.Shell.Resources;

namespace Microsoft.WindowsAPICodePack.Shell.ExplorerBrowser;

/// <summary>
/// This class is a wrapper around the Windows Explorer Browser control.
/// </summary>
public sealed class ExplorerBrowser :
    UserControl,
    Interop.ExplorerBrowser.IServiceProvider,
    IExplorerPaneVisibility,
    IExplorerBrowserEvents,
    ICommDlgBrowser3,
    IMessageFilter
{
    #region properties
    /// <summary>
    /// Options that control how the ExplorerBrowser navigates
    /// </summary>
    public ExplorerBrowserNavigationOptions NavigationOptions { get; private set; }

    /// <summary>
    /// Options that control how the content of the ExplorerBorwser looks
    /// </summary>
    public ExplorerBrowserContentOptions ContentOptions { get; private set; }

    private IShellItemArray shellItemsArray;
    private ShellObjectCollection itemsCollection;
    /// <summary>
    /// The set of ShellObjects in the Explorer Browser
    /// </summary>
    public ShellObjectCollection Items
    {
        get
        {
            if (shellItemsArray != null)
            {
                Marshal.ReleaseComObject(shellItemsArray);
            }

            if (itemsCollection != null)
            {
                itemsCollection.Dispose();
                itemsCollection = null;
            }

            shellItemsArray = GetItemsArray();
            itemsCollection = new ShellObjectCollection(shellItemsArray, true);

            return itemsCollection;
        }
    }

    private IShellItemArray selectedShellItemsArray;
    private ShellObjectCollection selectedItemsCollection;
    /// <summary>
    /// The set of selected ShellObjects in the Explorer Browser
    /// </summary>
    public ShellObjectCollection SelectedItems
    {
        get
        {
            if (selectedShellItemsArray != null)
            {
                Marshal.ReleaseComObject(selectedShellItemsArray);
            }

            if (selectedItemsCollection != null)
            {
                selectedItemsCollection.Dispose();
                selectedItemsCollection = null;
            }

            selectedShellItemsArray = GetSelectedItemsArray();
            selectedItemsCollection = new ShellObjectCollection(selectedShellItemsArray, true);

            return selectedItemsCollection;
        }
    }

    /// <summary>
    /// Contains the navigation history of the ExplorerBrowser
    /// </summary>
    public ExplorerBrowserNavigationLog NavigationLog { get; private set; }

    /// <summary>
    /// The name of the property bag used to persist changes to the ExplorerBrowser's view state.
    /// </summary>
    public string PropertyBagName
    {
        get { return propertyBagName; }
        set
        {
            propertyBagName = value;
            explorerBrowserControl?.SetPropertyBag(propertyBagName);
        }
    }

    #endregion

    #region operations

    /// <summary>
    /// Clears the Explorer Browser of existing content, fills it with
    /// content from the specified container, and adds a new point to the Travel Log.
    /// </summary>
    /// <param name="shellObject">The shell container to navigate to.</param>
    /// <exception cref="COMException">Will throw if navigation fails for any other reason.</exception>
    public void Navigate(ShellObject shellObject)
    {
        if (shellObject == null)
        {
            throw new ArgumentNullException(nameof(shellObject));
        }

        if (explorerBrowserControl == null)
        {
            antecreationNavigationTarget = shellObject;
        }
        else
        {
            HResult hr = explorerBrowserControl.BrowseToObject(shellObject.NativeShellItem, 0);
            if (hr != HResult.Ok)
            {
                if ((hr == HResult.ResourceInUse || hr == HResult.Canceled) && NavigationFailed != null)
                {
                    NavigationFailedEventArgs args = new()
                    {
                        FailedLocation = shellObject
                    };
                    NavigationFailed(this, args);
                }
                else
                {
                    throw new CommonControlException(LocalizedMessages.ExplorerBrowserBrowseToObjectFailed, hr);
                }
            }
        }
    }

    /// <summary>
    /// Navigates within the navigation log. This does not change the set of 
    /// locations in the navigation log.
    /// </summary>
    /// <param name="direction">Forward of Backward</param>
    /// <returns>True if the navigation succeeded, false if it failed for any reason.</returns>
    public bool NavigateLogLocation(NavigationLogDirection direction)
    {
        return NavigationLog.NavigateLog(direction);
    }

    /// <summary>
    /// Navigate within the navigation log. This does not change the set of 
    /// locations in the navigation log.
    /// </summary>
    /// <param name="navigationLogIndex">An index into the navigation logs Locations collection.</param>
    /// <returns>True if the navigation succeeded, false if it failed for any reason.</returns>
    public bool NavigateLogLocation(int navigationLogIndex)
    {
        return NavigationLog.NavigateLog(navigationLogIndex);
    }

    #endregion

    #region events

    /// <summary>
    /// Fires when the SelectedItems collection changes. 
    /// </summary>
    public event EventHandler SelectionChanged;

    /// <summary>
    /// Fires when the Items colection changes. 
    /// </summary>
    public event EventHandler ItemsChanged;

    /// <summary>
    /// Fires when a navigation has been initiated, but is not yet complete.
    /// </summary>
#pragma warning disable S3264 // Events should be invoked
    public event EventHandler<NavigationPendingEventArgs> NavigationPending;
#pragma warning restore S3264 // Events should be invoked

    /// <summary>
    /// Fires when a navigation has been 'completed': no NavigationPending listener 
    /// has cancelled, and the ExplorerBorwser has created a new view. The view 
    /// will be populated with new items asynchronously, and ItemsChanged will be 
    /// fired to reflect this some time later.
    /// </summary>
    public event EventHandler<NavigationCompleteEventArgs> NavigationComplete;

    /// <summary>
    /// Fires when either a NavigationPending listener cancels the navigation, or
    /// if the operating system determines that navigation is not possible.
    /// </summary>
    public event EventHandler<NavigationFailedEventArgs> NavigationFailed;

    /// <summary>
    /// Fires when the ExplorerBorwser view has finished enumerating files.
    /// </summary>
    public event EventHandler ViewEnumerationComplete;

    /// <summary>
    /// Fires when the item selected in the view has changed (i.e., a rename ).
    /// This is not the same as SelectionChanged.
    /// </summary>
    public event EventHandler ViewSelectedItemChanged;

    #endregion

    #region implementation

    #region construction

    internal ExplorerBrowserClass explorerBrowserControl;

    // for the IExplorerBrowserEvents Advise call
    internal uint eventsCookie;

    // name of the property bag that contains the view state options of the browser
    string propertyBagName = typeof(ExplorerBrowser).FullName;

    /// <summary>
    /// Initializes the ExplorerBorwser WinForms wrapper.
    /// </summary>
    public ExplorerBrowser()
        : base()
    {
        NavigationOptions = new ExplorerBrowserNavigationOptions(this);
        ContentOptions = new ExplorerBrowserContentOptions(this);
        NavigationLog = new ExplorerBrowserNavigationLog(this);
        PreviewKeyDown += ExplorerBrowser_PreviewKeyDown;
    }

    private void ExplorerBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
            e.IsInputKey = true;
    }

    internal IExplorerBrowser ExplorerBrowserInterface
    {
        get { return explorerBrowserControl; }
    }

    #endregion

    #region message handlers

    /// <summary>
    /// Displays a placeholder for the explorer browser in design mode
    /// </summary>
    /// <param name="e">Contains information about the paint event.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
        if (DesignMode && e != null)
        {
            using (LinearGradientBrush linGrBrush = new(
                ClientRectangle,
                Color.Aqua,
                Color.CadetBlue,
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(linGrBrush, ClientRectangle);
            }

            using Font font = new("Garamond", 30);
            using StringFormat sf = new();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(
                "ExplorerBrowserControl",
                font,
                Brushes.White,
                ClientRectangle,
                sf);
        }

        base.OnPaint(e);
    }

    ShellObject antecreationNavigationTarget;
    ExplorerBrowserViewEvents viewEvents;

    /// <summary>
    /// Creates and initializes the native ExplorerBrowser control
    /// </summary>
    protected override void OnCreateControl()
    {
        base.OnCreateControl();

        if (!DesignMode)
        {
            explorerBrowserControl = new ExplorerBrowserClass();

            // hooks up IExplorerPaneVisibility and ICommDlgBrowser event notifications
            ExplorerBrowserNativeMethods.IUnknown_SetSite(explorerBrowserControl, this);

            // hooks up IExplorerBrowserEvents event notification
            explorerBrowserControl.Advise(
                Marshal.GetComInterfaceForObject(this, typeof(IExplorerBrowserEvents)),
                out eventsCookie);

            // sets up ExplorerBrowser view connection point events
            viewEvents = new ExplorerBrowserViewEvents(this);

            NativeRect rect = new()
            {
                Top = ClientRectangle.Top,
                Left = ClientRectangle.Left,
                Right = ClientRectangle.Right,
                Bottom = ClientRectangle.Bottom
            };

            explorerBrowserControl.Initialize(Handle, ref rect, null);

            // Force an initial show frames so that IExplorerPaneVisibility works the first time it is set.
            // This also enables the control panel to be browsed to. If it is not set, then navigating to 
            // the control panel succeeds, but no items are visible in the view.
            explorerBrowserControl.SetOptions(ExplorerBrowserOptions.ShowFrames);

            explorerBrowserControl.SetPropertyBag(propertyBagName);

            if (antecreationNavigationTarget != null)
            {
                BeginInvoke(new MethodInvoker(
                delegate
                {
                    Navigate(antecreationNavigationTarget);
                    antecreationNavigationTarget = null;
                }));
            }
        }

        Application.AddMessageFilter(this);
    }

    /// <summary>
    /// Sizes the native control to match the WinForms control wrapper.
    /// </summary>
    /// <param name="e">Contains information about the size changed event.</param>
    protected override void OnSizeChanged(EventArgs e)
    {
        if (explorerBrowserControl != null)
        {
            NativeRect rect = new()
            {
                Top = ClientRectangle.Top,
                Left = ClientRectangle.Left,
                Right = ClientRectangle.Right,
                Bottom = ClientRectangle.Bottom
            };

            IntPtr ptr = IntPtr.Zero;
            explorerBrowserControl.SetRect(ref ptr, rect);
        }

        base.OnSizeChanged(e);
    }

    /// <summary>
    /// Cleans up the explorer browser events+object when the window is being taken down.
    /// </summary>
    /// <param name="e">An EventArgs that contains event data.</param>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (explorerBrowserControl != null)
        {
            // unhook events
            viewEvents.DisconnectFromView();
            explorerBrowserControl.Unadvise(eventsCookie);
            ExplorerBrowserNativeMethods.IUnknown_SetSite(explorerBrowserControl, null);

            // destroy the explorer browser control
            explorerBrowserControl.Destroy();

            // release com reference to it
            Marshal.ReleaseComObject(explorerBrowserControl);
            explorerBrowserControl = null;
        }

        base.OnHandleDestroyed(e);
    }
    #endregion

    #region object interfaces

    #region IServiceProvider

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guidService">calling service</param>
    /// <param name="riid">requested interface guid</param>
    /// <param name="ppvObject">caller-allocated memory for interface pointer</param>
    /// <returns></returns>
#pragma warning disable S125, S1871 // Sections of code should not be commented out
    HResult Interop.ExplorerBrowser.IServiceProvider.QueryService(
        ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
    {
        HResult hr;

        if (guidService.CompareTo(new Guid(ExplorerBrowserIidGuid.IExplorerPaneVisibility)) == 0)
        {
            // Responding to this SID allows us to control the visibility of the 
            // explorer browser panes
            ppvObject =
                Marshal.GetComInterfaceForObject(this, typeof(IExplorerPaneVisibility));
            hr = HResult.Ok;
        }
        else if (guidService.CompareTo(new Guid(ExplorerBrowserIidGuid.ICommDlgBrowser)) == 0)
        {
            if (riid.CompareTo(new Guid(ExplorerBrowserIidGuid.ICommDlgBrowser)) == 0)
            {
                ppvObject = Marshal.GetComInterfaceForObject(this, typeof(ICommDlgBrowser3));
                hr = HResult.Ok;
            }
            // The below lines are commented out to decline requests for the ICommDlgBrowser2 interface.
            // This interface is incorrectly marshaled back to unmanaged, and causes an exception.
            // There is a bug for this, I have not figured the underlying cause.
            // Remove this comment and uncomment the following code to enable the ICommDlgBrowser2 interface
            //else if (riid.CompareTo(new Guid(ExplorerBrowserIIDGuid.ICommDlgBrowser2)) == 0)
            //{
            //    ppvObject = Marshal.GetComInterfaceForObject(this, typeof(ICommDlgBrowser3));
            //    hr = HResult.Ok;                    
            //}
            else if (riid.CompareTo(new Guid(ExplorerBrowserIidGuid.ICommDlgBrowser3)) == 0)
            {
                ppvObject = Marshal.GetComInterfaceForObject(this, typeof(ICommDlgBrowser3));
                hr = HResult.Ok;
            }
            else
            {
                ppvObject = IntPtr.Zero;
                hr = HResult.NoInterface;
            }
        }
        else
        {
            IntPtr nullObj = IntPtr.Zero;
            ppvObject = nullObj;
            hr = HResult.NoInterface;
        }

        return hr;
    }
#pragma warning restore S125, S1871 // Sections of code should not be commented out
    #endregion

    #region IExplorerPaneVisibility

    /// <summary>
    /// Controls the visibility of the explorer borwser panes
    /// </summary>
    /// <param name="explorerPane">a guid identifying the pane</param>
    /// <param name="peps">the pane state desired</param>
    /// <returns></returns>
    HResult IExplorerPaneVisibility.GetPaneState(ref Guid explorerPane, out ExplorerPaneState peps)
    {
        peps = explorerPane.ToString() switch
        {
            ExplorerBrowserViewPanes.AdvancedQuery => VisibilityToPaneState(NavigationOptions.PaneVisibility.AdvancedQuery),
            ExplorerBrowserViewPanes.Commands => VisibilityToPaneState(NavigationOptions.PaneVisibility.Commands),
            ExplorerBrowserViewPanes.CommandsOrganize => VisibilityToPaneState(NavigationOptions.PaneVisibility.CommandsOrganize),
            ExplorerBrowserViewPanes.CommandsView => VisibilityToPaneState(NavigationOptions.PaneVisibility.CommandsView),
            ExplorerBrowserViewPanes.Details => VisibilityToPaneState(NavigationOptions.PaneVisibility.Details),
            ExplorerBrowserViewPanes.Navigation => VisibilityToPaneState(NavigationOptions.PaneVisibility.Navigation),
            ExplorerBrowserViewPanes.Preview => VisibilityToPaneState(NavigationOptions.PaneVisibility.Preview),
            ExplorerBrowserViewPanes.Query => VisibilityToPaneState(NavigationOptions.PaneVisibility.Query),
            _ => VisibilityToPaneState(PaneVisibilityState.Show),
        };
        return HResult.Ok;
    }

    private static ExplorerPaneState VisibilityToPaneState(PaneVisibilityState visibility)
    {
        return visibility switch
        {
            PaneVisibilityState.DoNotCare => ExplorerPaneState.DoNotCare,
            PaneVisibilityState.Hide => ExplorerPaneState.DefaultOff | ExplorerPaneState.Force,
            PaneVisibilityState.Show => ExplorerPaneState.DefaultOn | ExplorerPaneState.Force,
            _ => throw new ArgumentException("unexpected PaneVisibilityState"),
        };
    }

    #endregion

    #region IExplorerBrowserEvents

    HResult IExplorerBrowserEvents.OnNavigationPending(IntPtr pidlFolder)
    {
        bool canceled = false;

        if (NavigationPending != null)
        {
            NavigationPendingEventArgs args = new()
            {
                // For some special items (like network machines), ShellObject.FromIDList
                // might return null
                PendingLocation = ShellObjectFactory.Create(pidlFolder)
            };

            if (args.PendingLocation != null)
            {
                foreach (Delegate del in NavigationPending.GetInvocationList())
                {
                    del.DynamicInvoke(this, args);
                    if (args.Cancel)
                    {
                        canceled = true;
                    }
                }
            }
        }

        return canceled ? HResult.Canceled : HResult.Ok;
    }

    HResult IExplorerBrowserEvents.OnViewCreated(object psv)
    {
        viewEvents.ConnectToView((IShellView)psv);

        return HResult.Ok;
    }

    HResult IExplorerBrowserEvents.OnNavigationComplete(IntPtr pidlFolder)
    {
        // view mode may change 
        ContentOptions.folderSettings.ViewMode = GetCurrentViewMode();

        if (NavigationComplete != null)
        {
            NavigationCompleteEventArgs args = new()
            {
                NewLocation = ShellObjectFactory.Create(pidlFolder)
            };
            NavigationComplete(this, args);
        }
        return HResult.Ok;
    }

    HResult IExplorerBrowserEvents.OnNavigationFailed(IntPtr pidlFolder)
    {
        if (NavigationFailed != null)
        {
            NavigationFailedEventArgs args = new()
            {
                FailedLocation = ShellObjectFactory.Create(pidlFolder)
            };
            NavigationFailed(this, args);
        }
        return HResult.Ok;
    }
    #endregion

    #region ICommDlgBrowser

    HResult ICommDlgBrowser3.OnDefaultCommand(IntPtr ppshv)
    {
        return HResult.False;
    }

    HResult ICommDlgBrowser3.OnStateChange(IntPtr ppshv, CommDlgBrowserStateChange uChange)
    {
        if (uChange == CommDlgBrowserStateChange.SelectionChange)
        {
            FireSelectionChanged();
        }

        return HResult.Ok;
    }

    HResult ICommDlgBrowser3.IncludeObject(IntPtr ppshv, IntPtr pidl)
    {
        // items in the view have changed, so the collections need updating
        FireContentChanged();

        return HResult.Ok;
    }

    #endregion

    #region ICommDlgBrowser2 Members

    // The below methods can be called into, but marshalling the response causes an exception to be
    // thrown from unmanaged code.  At this time, I decline calls requesting the ICommDlgBrowser2
    // interface.  This is logged as a bug, but moved to less of a priority, as it only affects being
    // able to change the default action text for remapping the default action.

    HResult ICommDlgBrowser3.GetDefaultMenuText(IShellView shellView, [MarshalAs(UnmanagedType.LPWStr)] string buffer, int bufferMaxLength)
    {
        return HResult.False;
        //OK if new
        //False if default
        //other if error
    }

#pragma warning disable S125 // Sections of code should not be commented out
    HResult ICommDlgBrowser3.GetViewFlags(ref CommDlgBrowser2View pdwFlags)
    {
        //var flags = CommDlgBrowser2ViewFlags.NoSelectVerb;
        //Marshal.WriteInt32(pdwFlags, 0);
        pdwFlags = CommDlgBrowser2View.ShowAllFiles;
        return HResult.Ok;
    }
#pragma warning restore S125 // Sections of code should not be commented out

    HResult ICommDlgBrowser3.Notify(IntPtr pshv, CommDlgBrowserNotifyType notifyType)
    {
        return HResult.Ok;
    }

    #endregion

    #region ICommDlgBrowser3 Members

    HResult ICommDlgBrowser3.GetCurrentFilter(StringBuilder pszFileSpec, int cchFileSpec)
    {
        // If the method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        return HResult.Ok;
    }

    HResult ICommDlgBrowser3.OnColumnClicked(IShellView ppshv, int iColumn)
    {
        // If the method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        return HResult.Ok;
    }

    HResult ICommDlgBrowser3.OnPreViewCreated(IShellView ppshv)
    {
        // If the method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code
        return HResult.Ok;
    }

    #endregion

    #region IMessageFilter Members

    bool IMessageFilter.PreFilterMessage(ref Message m)
    {
        HResult hr = HResult.False;
        if (explorerBrowserControl != null)
        {
            // translate keyboard input
            hr = ((IInputObject)explorerBrowserControl).TranslateAcceleratorIO(ref m);
        }
        return hr == HResult.Ok;
    }

    #endregion

    #endregion

    #region utilities

    /// <summary>
    /// Returns the current view mode of the browser
    /// </summary>
    /// <returns></returns>
    internal FolderViewMode GetCurrentViewMode()
    {
        IFolderView2 ifv2 = GetFolderView2();
        uint viewMode = 0;
        if (ifv2 != null)
        {
            try
            {
                HResult hr = ifv2.GetCurrentViewMode(out viewMode);
                if (hr != HResult.Ok) { throw new ShellException(hr); }
            }
            finally
            {
                Marshal.ReleaseComObject(ifv2);
            }
        }
        return (FolderViewMode)viewMode;
    }

    /// <summary>
    /// Gets the IFolderView2 interface from the explorer browser.
    /// </summary>
    /// <returns></returns>
    internal IFolderView2 GetFolderView2()
    {
        Guid iid = new(ExplorerBrowserIidGuid.IFolderView2);
        if (explorerBrowserControl != null)
        {
            HResult hr = explorerBrowserControl.GetCurrentView(ref iid, out IntPtr view);
            switch (hr)
            {
                case HResult.Ok:
                    break;

                case HResult.NoInterface:
                case HResult.Fail:
#if LOG_KNOWN_COM_ERRORS
                    Debugger.Log( 2, "ExplorerBrowser", "Unable to obtain view. Error=" + e.ToString( ) );
#endif
                    return null;

                default:
                    throw new CommonControlException(LocalizedMessages.ExplorerBrowserFailedToGetView, hr);
            }

            return (IFolderView2)Marshal.GetObjectForIUnknown(view);
        }
        return null;
    }

    /// <summary>
    /// Gets the selected items in the explorer browser as an IShellItemArray
    /// </summary>
    /// <returns></returns>
    internal IShellItemArray GetSelectedItemsArray()
    {
        IShellItemArray iArray = null;
        IFolderView2 iFV2 = GetFolderView2();
        if (iFV2 != null)
        {
            try
            {
                Guid iidShellItemArray = new(ShellIidGuid.IShellItemArray);
                HResult hr = iFV2.Items((uint)ShellViewGetItemObject.Selection, ref iidShellItemArray, out object oArray);
                iArray = oArray as IShellItemArray;
                if (hr != HResult.Ok &&
                    hr != HResult.ElementNotFound &&
                    hr != HResult.Fail)
                {
                    throw new CommonControlException(LocalizedMessages.ExplorerBrowserUnexpectedError, hr);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(iFV2);
            }
        }

        return iArray;
    }

    internal int GetItemsCount()
    {
        int itemsCount = 0;

        IFolderView2 iFV2 = GetFolderView2();
        if (iFV2 != null)
        {
            try
            {
                HResult hr = iFV2.ItemCount((uint)ShellViewGetItemObject.AllView, out itemsCount);

                if (hr != HResult.Ok &&
                    hr != HResult.ElementNotFound &&
                    hr != HResult.Fail)
                {
                    throw new CommonControlException(LocalizedMessages.ExplorerBrowserItemCount, hr);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(iFV2);
            }
        }

        return itemsCount;
    }

    internal int GetSelectedItemsCount()
    {
        int itemsCount = 0;

        IFolderView2 iFV2 = GetFolderView2();
        if (iFV2 != null)
        {
            try
            {
                HResult hr = iFV2.ItemCount((uint)ShellViewGetItemObject.Selection, out itemsCount);

                if (hr != HResult.Ok &&
                    hr != HResult.ElementNotFound &&
                    hr != HResult.Fail)
                {
                    throw new CommonControlException(LocalizedMessages.ExplorerBrowserSelectedItemCount, hr);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(iFV2);
            }
        }

        return itemsCount;
    }

    /// <summary>
    /// Gets the items in the ExplorerBrowser as an IShellItemArray
    /// </summary>
    /// <returns></returns>
    internal IShellItemArray GetItemsArray()
    {
        IShellItemArray iArray = null;
        IFolderView2 iFV2 = GetFolderView2();
        if (iFV2 != null)
        {
            try
            {
                Guid iidShellItemArray = new(ShellIidGuid.IShellItemArray);
                HResult hr = iFV2.Items((uint)ShellViewGetItemObject.AllView, ref iidShellItemArray, out object oArray);
                if (hr != HResult.Ok &&
                    hr != HResult.Fail &&
                    hr != HResult.ElementNotFound &&
                    hr != HResult.InvalidArguments)
                {
                    throw new CommonControlException(LocalizedMessages.ExplorerBrowserViewItems, hr);
                }

                iArray = oArray as IShellItemArray;
            }
            finally
            {
                Marshal.ReleaseComObject(iFV2);
            }
        }
        return iArray;
    }

    #endregion

    #region view event forwarding

    internal void FireSelectionChanged()
    {
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void FireContentChanged()
    {
        ItemsChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void FireContentEnumerationComplete()
    {
        ViewEnumerationComplete?.Invoke(this, EventArgs.Empty);
    }

    internal void FireSelectedItemChanged()
    {
        ViewSelectedItemChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #endregion
}
