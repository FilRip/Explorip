﻿//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

using Microsoft.WindowsAPICodePack.Shell;

namespace Microsoft.WindowsAPICodePack.Controls.WindowsPresentationFoundation;

/// <summary>
/// Interaction logic for ExplorerBrowser.xaml
/// </summary>
public partial class ExplorerBrowser : UserControl, IDisposable
{
    /// <summary>
    /// The underlying WinForms control
    /// </summary>
    public WindowsForms.ExplorerBrowser ExplorerBrowserControl { get; set; }

    private readonly ObservableCollection<ShellObject> selectedItems;
    private readonly ObservableCollection<ShellObject> items;
    private readonly ObservableCollection<ShellObject> navigationLog;
    private readonly DispatcherTimer dtCLRUpdater = new();

    private ShellObject initialNavigationTarget;
    private ExplorerBrowserViewMode? initialViewMode;

    private readonly AutoResetEvent itemsChanged = new(false);
    private readonly AutoResetEvent selectionChanged = new(false);
    private int selectionChangeWaitCount;

    /// <summary>
    /// Hosts the ExplorerBrowser WinForms wrapper in this control
    /// </summary>
    public ExplorerBrowser()
    {
        InitializeComponent();

        // the ExplorerBrowser WinForms control
        ExplorerBrowserControl = new WindowsForms.ExplorerBrowser();

        // back the dependency collection properties with instances
        SelectedItems = selectedItems = [];
        Items = items = [];
        NavigationLog = navigationLog = [];

        // hook up events for collection synchronization
        ExplorerBrowserControl.ItemsChanged += new EventHandler(ItemsChanged);
        ExplorerBrowserControl.SelectionChanged += new EventHandler(SelectionChanged);
        ExplorerBrowserControl.ViewEnumerationComplete += new EventHandler(ExplorerBrowserControl_ViewEnumerationComplete);
        ExplorerBrowserControl.ViewSelectedItemChanged += new EventHandler(ExplorerBrowserControl_ViewSelectedItemChanged);
        ExplorerBrowserControl.NavigationLog.NavigationLogChanged += new EventHandler<NavigationLogEventArgs>(NavigationLogChanged);

        // host the control           
        WindowsFormsHost host = new()
        {
            Focusable = true,
        };
        try
        {
            host.Child = ExplorerBrowserControl;
            this.AddChild(host);
        }
        catch
        {
            host.Dispose();
            throw;
        }

        Loaded += new RoutedEventHandler(ExplorerBrowser_Loaded);
    }

    private void ExplorerBrowserControl_ViewSelectedItemChanged(object sender, EventArgs e)
    {
        // Empty method ?!
    }

    private void ExplorerBrowserControl_ViewEnumerationComplete(object sender, EventArgs e)
    {
        itemsChanged.Set();
        selectionChanged.Set();
    }

    /// <summary>
    /// To avoid the 'Dispatcher processing has been suspended' InvalidOperationException on Win7,
    /// the ExplorerBorwser native control is initialized after this control is fully loaded.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ExplorerBrowser_Loaded(object sender, RoutedEventArgs e)
    {
        // setup timer to update dependency properties from CLR properties of WinForms ExplorerBrowser object
        dtCLRUpdater.Tick += new EventHandler(UpdateDependencyPropertiesFromCLRPRoperties);
        dtCLRUpdater.Interval = new TimeSpan(100 * 10000); // 100ms
        dtCLRUpdater.Start();

        if (initialNavigationTarget != null)
        {
            ExplorerBrowserControl.Navigate(initialNavigationTarget);
            initialNavigationTarget = null;
        }

        if (initialViewMode != null)
        {
            ExplorerBrowserControl.ContentOptions.ViewMode = (ExplorerBrowserViewMode)initialViewMode;
            initialViewMode = null;
        }
    }

    /// <summary>
    /// Map changes to the CLR flags to the dependency properties
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateDependencyPropertiesFromCLRPRoperties(object sender, EventArgs e)
    {
        AlignLeft = ExplorerBrowserControl.ContentOptions.AlignLeft;
        AutoArrange = ExplorerBrowserControl.ContentOptions.AutoArrange;
        CheckSelect = ExplorerBrowserControl.ContentOptions.CheckSelect;
        ExtendedTiles = ExplorerBrowserControl.ContentOptions.ExtendedTiles;
        FullRowSelect = ExplorerBrowserControl.ContentOptions.FullRowSelect;
        HideFileNames = ExplorerBrowserControl.ContentOptions.HideFileNames;
        NoBrowserViewState = ExplorerBrowserControl.ContentOptions.NoBrowserViewState;
        NoColumnHeader = ExplorerBrowserControl.ContentOptions.NoColumnHeader;
        NoHeaderInAllViews = ExplorerBrowserControl.ContentOptions.NoHeaderInAllViews;
        NoIcons = ExplorerBrowserControl.ContentOptions.NoIcons;
        NoSubfolders = ExplorerBrowserControl.ContentOptions.NoSubfolders;
        SingleClickActivate = ExplorerBrowserControl.ContentOptions.SingleClickActivate;
        SingleSelection = ExplorerBrowserControl.ContentOptions.SingleSelection;
        ThumbnailSize = ExplorerBrowserControl.ContentOptions.ThumbnailSize;
        ViewMode = ExplorerBrowserControl.ContentOptions.ViewMode;
        AlwaysNavigate = ExplorerBrowserControl.NavigationOptions.AlwaysNavigate;
        NavigateOnce = ExplorerBrowserControl.NavigationOptions.NavigateOnce;
        AdvancedQueryPane = ExplorerBrowserControl.NavigationOptions.PaneVisibility.AdvancedQuery;
        CommandsPane = ExplorerBrowserControl.NavigationOptions.PaneVisibility.Commands;
        CommandsOrganizePane = ExplorerBrowserControl.NavigationOptions.PaneVisibility.CommandsOrganize;
        CommandsViewPane = ExplorerBrowserControl.NavigationOptions.PaneVisibility.CommandsView;
        DetailsPane = ExplorerBrowserControl.NavigationOptions.PaneVisibility.Details;
        NavigationPane = ExplorerBrowserControl.NavigationOptions.PaneVisibility.Navigation;
        PreviewPane = ExplorerBrowserControl.NavigationOptions.PaneVisibility.Preview;
        QueryPane = ExplorerBrowserControl.NavigationOptions.PaneVisibility.Query;
        NavigationLogIndex = ExplorerBrowserControl.NavigationLog.CurrentLocationIndex;

        if (disposedValue)
            return;

        if (itemsChanged.WaitOne(1, false))
        {
            items.Clear();
            foreach (ShellObject obj in ExplorerBrowserControl.Items.OfType<ShellObject>())
            {
                items.Add(obj);
            }
        }

        if (selectionChanged.WaitOne(1, false))
        {
            selectionChangeWaitCount = 4;
        }
        else if (selectionChangeWaitCount > 0)
        {
            selectionChangeWaitCount--;

            if (selectionChangeWaitCount == 0)
            {
                selectedItems.Clear();
                foreach (ShellObject obj in ExplorerBrowserControl.SelectedItems.OfType<ShellObject>())
                {
                    selectedItems.Add(obj);
                }
            }
        }
    }

    /// <summary>
    /// Synchronize NavigationLog collection to dependency collection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void NavigationLogChanged(object sender, NavigationLogEventArgs args)
    {
        navigationLog.Clear();
        foreach (ShellObject obj in ExplorerBrowserControl.NavigationLog.Locations)
        {
            navigationLog.Add(obj);
        }
    }

    /// <summary>
    /// Synchronize SelectedItems collection to dependency collection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectionChanged(object sender, EventArgs e)
    {
        selectionChanged.Set();
    }

    // Synchronize ItemsCollection to dependency collection
    private void ItemsChanged(object sender, EventArgs e)
    {
        try
        {
            itemsChanged.Set();
        }
        catch (Exception) { /* ignore errors */ }
    }

    /// <summary>
    /// The items in the ExplorerBrowser window
    /// </summary>
    public ObservableCollection<ShellObject> Items
    {
        get
        {
            return (ObservableCollection<ShellObject>)GetValue(ItemsProperty);
        }
        set
        {
            SetValue(ItemsPropertyKey, value);
        }
    }

    private static readonly DependencyPropertyKey ItemsPropertyKey =
                DependencyProperty.RegisterReadOnly(
                    "Items", typeof(ObservableCollection<ShellObject>),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(null));

    /// <summary>
    /// The items in the ExplorerBrowser window
    /// </summary>
    public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

    /// <summary>
    /// The selected items in the ExplorerBrowser window
    /// </summary>
    public ObservableCollection<ShellObject> SelectedItems
    {
        get
        {
            return (ObservableCollection<ShellObject>)GetValue(SelectedItemsProperty);
        }
        internal set
        {
            SetValue(SelectedItemsPropertyKey, value);
        }
    }

    private static readonly DependencyPropertyKey SelectedItemsPropertyKey =
                DependencyProperty.RegisterReadOnly(
                    "SelectedItems", typeof(ObservableCollection<ShellObject>),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(null));

    /// <summary>
    /// The selected items in the ExplorerBrowser window
    /// </summary>
    public ObservableCollection<ShellObject> NavigationLog
    {
        get
        {
            return (ObservableCollection<ShellObject>)GetValue(NavigationLogProperty);
        }
        internal set
        {
            SetValue(NavigationLogPropertyKey, value);
        }
    }

    private static readonly DependencyPropertyKey NavigationLogPropertyKey =
                DependencyProperty.RegisterReadOnly(
                    "NavigationLog", typeof(ObservableCollection<ShellObject>),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(null));

    /// <summary>
    /// The NavigationLog
    /// </summary>
    public static readonly DependencyProperty NavigationLogProperty = NavigationLogPropertyKey.DependencyProperty;

    /// <summary>
    /// The selected items in the ExplorerBrowser window
    /// </summary>
    public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;


    /// <summary>
    /// The location the explorer browser is navigating to
    /// </summary>
    public ShellObject NavigationTarget
    {
        get
        {
            return (ShellObject)GetValue(NavigationTargetProperty);
        }
        set
        {
            SetValue(NavigationTargetProperty, value);
        }
    }

    /// <summary>
    /// The DependencyProperty for the NavigationTarget property
    /// </summary>
    public static readonly DependencyProperty NavigationTargetProperty =
                DependencyProperty.Register(
                    "NavigationTarget", typeof(ShellObject),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(null, NavigationTargetChanged));

    private static void NavigationTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;

        if (instance.ExplorerBrowserControl.explorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.Navigate((ShellObject)e.NewValue);
        }
        else
        {
            instance.initialNavigationTarget = (ShellObject)e.NewValue;
        }
    }

    /// <summary>
    /// The view should be left-aligned. 
    /// </summary>
    public bool AlignLeft
    {
        get { return (bool)GetValue(AlignLeftProperty); }
        set { SetValue(AlignLeftProperty, value); }
    }

    internal readonly static DependencyProperty AlignLeftProperty =
                DependencyProperty.Register(
                    "AlignLeft", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnAlignLeftChanged));

    private static void OnAlignLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
            instance.ExplorerBrowserControl.ContentOptions.AlignLeft = (bool)e.NewValue;
    }


    /// <summary>
    /// Automatically arrange the elements in the view. 
    /// </summary>
    public bool AutoArrange
    {
        get { return (bool)GetValue(AutoArrangeProperty); }
        set { SetValue(AutoArrangeProperty, value); }
    }

    internal readonly static DependencyProperty AutoArrangeProperty =
                DependencyProperty.Register(
                    "AutoArrange", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnAutoArrangeChanged));

    private static void OnAutoArrangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
            instance.ExplorerBrowserControl.ContentOptions.AutoArrange = (bool)e.NewValue;
    }

    /// <summary>
    /// Turns on check mode for the view
    /// </summary>
    public bool CheckSelect
    {
        get { return (bool)GetValue(CheckSelectProperty); }
        set { SetValue(CheckSelectProperty, value); }
    }

    internal readonly static DependencyProperty CheckSelectProperty =
                DependencyProperty.Register(
                    "CheckSelect", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnCheckSelectChanged));

    private static void OnCheckSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.ContentOptions.CheckSelect = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// When the view is in "tile view mode" the layout of a single item should be extended to the width of the view.
    /// </summary>
    public bool ExtendedTiles
    {
        get { return (bool)GetValue(ExtendedTilesProperty); }
        set { SetValue(ExtendedTilesProperty, value); }
    }

    internal readonly static DependencyProperty ExtendedTilesProperty =
                DependencyProperty.Register(
                    "ExtendedTiles", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnExtendedTilesChanged));

    private static void OnExtendedTilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
            instance.ExplorerBrowserControl.ContentOptions.ExtendedTiles = (bool)e.NewValue;
    }

    /// <summary>
    /// When an item is selected, the item and all its sub-items are highlighted.
    /// </summary>
    public bool FullRowSelect
    {
        get { return (bool)GetValue(FullRowSelectProperty); }
        set { SetValue(FullRowSelectProperty, value); }
    }

    internal readonly static DependencyProperty FullRowSelectProperty =
                DependencyProperty.Register(
                    "FullRowSelect", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnFullRowSelectChanged));

    private static void OnFullRowSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.ContentOptions.FullRowSelect = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// The view should not display file names
    /// </summary>
    public bool HideFileNames
    {
        get { return (bool)GetValue(HideFileNamesProperty); }
        set { SetValue(HideFileNamesProperty, value); }
    }

    internal readonly static DependencyProperty HideFileNamesProperty =
                DependencyProperty.Register(
                    "HideFileNames", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnHideFileNamesChanged));

    private static void OnHideFileNamesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.ContentOptions.HideFileNames = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// The view should not save view state in the browser.
    /// </summary>
    public bool NoBrowserViewState
    {
        get { return (bool)GetValue(NoBrowserViewStateProperty); }
        set { SetValue(NoBrowserViewStateProperty, value); }
    }

    internal readonly static DependencyProperty NoBrowserViewStateProperty =
                DependencyProperty.Register(
                    "NoBrowserViewState", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnNoBrowserViewStateChanged));

    private static void OnNoBrowserViewStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.ContentOptions.NoBrowserViewState = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// Do not display a column header in the view in any view mode.
    /// </summary>
    public bool NoColumnHeader
    {
        get { return (bool)GetValue(NoColumnHeaderProperty); }
        set { SetValue(NoColumnHeaderProperty, value); }
    }

    internal readonly static DependencyProperty NoColumnHeaderProperty =
                DependencyProperty.Register(
                    "NoColumnHeader", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnNoColumnHeaderChanged));

    private static void OnNoColumnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
            instance.ExplorerBrowserControl.ContentOptions.NoColumnHeader = (bool)e.NewValue;
    }

    /// <summary>
    /// Only show the column header in details view mode.
    /// </summary>
    public bool NoHeaderInAllViews
    {
        get { return (bool)GetValue(NoHeaderInAllViewsProperty); }
        set { SetValue(NoHeaderInAllViewsProperty, value); }
    }

    internal readonly static DependencyProperty NoHeaderInAllViewsProperty =
                DependencyProperty.Register(
                    "NoHeaderInAllViews", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnNoHeaderInAllViewsChanged));

    private static void OnNoHeaderInAllViewsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.ContentOptions.NoHeaderInAllViews = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// The view should not display icons. 
    /// </summary>
    public bool NoIcons
    {
        get { return (bool)GetValue(NoIconsProperty); }
        set { SetValue(NoIconsProperty, value); }
    }

    internal readonly static DependencyProperty NoIconsProperty =
                DependencyProperty.Register(
                    "NoIcons", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnNoIconsChanged));

    private static void OnNoIconsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.ContentOptions.NoIcons = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// Do not show subfolders. 
    /// </summary>
    public bool NoSubfolders
    {
        get { return (bool)GetValue(NoSubfoldersProperty); }
        set { SetValue(NoSubfoldersProperty, value); }
    }

    internal readonly static DependencyProperty NoSubfoldersProperty =
                DependencyProperty.Register(
                    "NoSubfolders", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnNoSubfoldersChanged));

    private static void OnNoSubfoldersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.ContentOptions.NoSubfolders = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// Navigate with a single click
    /// </summary>
    public bool SingleClickActivate
    {
        get { return (bool)GetValue(SingleClickActivateProperty); }
        set { SetValue(SingleClickActivateProperty, value); }
    }

    internal readonly static DependencyProperty SingleClickActivateProperty =
                DependencyProperty.Register(
                    "SingleClickActivate", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnSingleClickActivateChanged));

    private static void OnSingleClickActivateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.ContentOptions.SingleClickActivate = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// Do not allow more than a single item to be selected.
    /// </summary>
    public bool SingleSelection
    {
        get { return (bool)GetValue(SingleSelectionProperty); }
        set { SetValue(SingleSelectionProperty, value); }
    }

    internal readonly static DependencyProperty SingleSelectionProperty =
                DependencyProperty.Register(
                    "SingleSelection", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnSingleSelectionChanged));

    private static void OnSingleSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.ContentOptions.SingleSelection = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// The size of the thumbnails in the explorer browser
    /// </summary>
    public int ThumbnailSize
    {
        get { return (int)GetValue(ThumbnailSizeProperty); }
        set { SetValue(ThumbnailSizeProperty, value); }
    }

    internal readonly static DependencyProperty ThumbnailSizeProperty =
                DependencyProperty.Register(
                    "ThumbnailSize", typeof(int),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(32, OnThumbnailSizeChanged));

    private static void OnThumbnailSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.ContentOptions.ThumbnailSize = (int)e.NewValue;
        }
    }



    /// <summary>
    /// The various view modes of the explorer browser control
    /// </summary>
    public ExplorerBrowserViewMode ViewMode
    {
        get { return (ExplorerBrowserViewMode)GetValue(ViewModeProperty); }
        set { SetValue(ViewModeProperty, value); }
    }

    internal readonly static DependencyProperty ViewModeProperty =
                DependencyProperty.Register(
                    "ViewMode", typeof(ExplorerBrowserViewMode),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(ExplorerBrowserViewMode.Auto, OnViewModeChanged));

    private static void OnViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;

        if (instance.ExplorerBrowserControl != null)
        {
            if (instance.ExplorerBrowserControl.explorerBrowserControl == null)
            {
                instance.initialViewMode = (ExplorerBrowserViewMode)e.NewValue;
            }
            else
            {
                instance.ExplorerBrowserControl.ContentOptions.ViewMode = (ExplorerBrowserViewMode)e.NewValue;
            }
        }
    }


    /// <summary>
    /// Always navigate, even if you are attempting to navigate to the current folder.
    /// </summary>
    public bool AlwaysNavigate
    {
        get { return (bool)GetValue(AlwaysNavigateProperty); }
        set { SetValue(AlwaysNavigateProperty, value); }
    }

    internal readonly static DependencyProperty AlwaysNavigateProperty =
                DependencyProperty.Register(
                    "AlwaysNavigate", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnAlwaysNavigateChanged));

    private static void OnAlwaysNavigateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.NavigationOptions.AlwaysNavigate = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// Do not navigate further than the initial navigation.
    /// </summary>
    public bool NavigateOnce
    {
        get { return (bool)GetValue(NavigateOnceProperty); }
        set { SetValue(NavigateOnceProperty, value); }
    }

    internal readonly static DependencyProperty NavigateOnceProperty =
                DependencyProperty.Register(
                    "NavigateOnce", typeof(bool),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(false, OnNavigateOnceChanged));

    private static void OnNavigateOnceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.NavigationOptions.NavigateOnce = (bool)e.NewValue;
        }
    }

    /// <summary>
    /// Show/Hide the AdvancedQuery pane on subsequent navigation
    /// </summary>
    public PaneVisibilityState AdvancedQueryPane
    {
        get { return (PaneVisibilityState)GetValue(AdvancedQueryPaneProperty); }
        set { SetValue(AdvancedQueryPaneProperty, value); }
    }

    internal readonly static DependencyProperty AdvancedQueryPaneProperty =
                DependencyProperty.Register(
                    "AdvancedQueryPane", typeof(PaneVisibilityState),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(PaneVisibilityState.DoNotCare, OnAdvancedQueryPaneChanged));

    private static void OnAdvancedQueryPaneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.NavigationOptions.PaneVisibility.AdvancedQuery = (PaneVisibilityState)e.NewValue;
        }
    }

    /// <summary>
    /// Show/Hide the Commands pane on subsequent navigation
    /// </summary>
    public PaneVisibilityState CommandsPane
    {
        get { return (PaneVisibilityState)GetValue(CommandsPaneProperty); }
        set { SetValue(CommandsPaneProperty, value); }
    }

    internal readonly static DependencyProperty CommandsPaneProperty =
                DependencyProperty.Register(
                    "CommandsPane", typeof(PaneVisibilityState),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(PaneVisibilityState.DoNotCare, OnCommandsPaneChanged));

    private static void OnCommandsPaneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.NavigationOptions.PaneVisibility.Commands =
                (PaneVisibilityState)e.NewValue;
        }
    }

    /// <summary>
    /// Show/Hide the Organize menu in the Commands pane on subsequent navigation
    /// </summary>
    public PaneVisibilityState CommandsOrganizePane
    {
        get { return (PaneVisibilityState)GetValue(CommandsOrganizePaneProperty); }
        set { SetValue(CommandsOrganizePaneProperty, value); }
    }

    internal readonly static DependencyProperty CommandsOrganizePaneProperty =
                DependencyProperty.Register(
                    "CommandsOrganizePane", typeof(PaneVisibilityState),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(PaneVisibilityState.DoNotCare, OnCommandsOrganizePaneChanged));

    private static void OnCommandsOrganizePaneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.NavigationOptions.PaneVisibility.CommandsOrganize =
                (PaneVisibilityState)e.NewValue;
        }
    }

    /// <summary>
    /// Show/Hide the View menu in the Commands pane on subsequent navigation
    /// </summary>
    public PaneVisibilityState CommandsViewPane
    {
        get { return (PaneVisibilityState)GetValue(CommandsViewPaneProperty); }
        set { SetValue(CommandsViewPaneProperty, value); }
    }

    internal readonly static DependencyProperty CommandsViewPaneProperty =
                DependencyProperty.Register(
                    "CommandsViewPane", typeof(PaneVisibilityState),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(PaneVisibilityState.DoNotCare, OnCommandsViewPaneChanged));

    private static void OnCommandsViewPaneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.NavigationOptions.PaneVisibility.CommandsView = (PaneVisibilityState)e.NewValue;
        }
    }

    /// <summary>
    /// Show/Hide the Details pane on subsequent navigation
    /// </summary>
    public PaneVisibilityState DetailsPane
    {
        get { return (PaneVisibilityState)GetValue(DetailsPaneProperty); }
        set { SetValue(DetailsPaneProperty, value); }
    }

    internal readonly static DependencyProperty DetailsPaneProperty =
                DependencyProperty.Register(
                    "DetailsPane", typeof(PaneVisibilityState),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(PaneVisibilityState.DoNotCare, OnDetailsPaneChanged));

    private static void OnDetailsPaneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.NavigationOptions.PaneVisibility.Details = (PaneVisibilityState)e.NewValue;
        }
    }

    /// <summary>
    /// Show/Hide the Navigation pane on subsequent navigation
    /// </summary>
    public PaneVisibilityState NavigationPane
    {
        get { return (PaneVisibilityState)GetValue(NavigationPaneProperty); }
        set { SetValue(NavigationPaneProperty, value); }
    }

    internal readonly static DependencyProperty NavigationPaneProperty =
                DependencyProperty.Register(
                    "NavigationPane", typeof(PaneVisibilityState),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(PaneVisibilityState.DoNotCare, OnNavigationPaneChanged));

    private static void OnNavigationPaneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.NavigationOptions.PaneVisibility.Navigation = (PaneVisibilityState)e.NewValue;
        }
    }

    /// <summary>
    /// Show/Hide the Preview pane on subsequent navigation
    /// </summary>
    public PaneVisibilityState PreviewPane
    {
        get { return (PaneVisibilityState)GetValue(PreviewPaneProperty); }
        set { SetValue(PreviewPaneProperty, value); }
    }

    internal readonly static DependencyProperty PreviewPaneProperty =
                DependencyProperty.Register(
                    "PreviewPane", typeof(PaneVisibilityState),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(PaneVisibilityState.DoNotCare, OnPreviewPaneChanged));

    private static void OnPreviewPaneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.NavigationOptions.PaneVisibility.Preview = (PaneVisibilityState)e.NewValue;
        }
    }

    /// <summary>
    /// Show/Hide the Query pane on subsequent navigation
    /// </summary>
    public PaneVisibilityState QueryPane
    {
        get { return (PaneVisibilityState)GetValue(QueryPaneProperty); }
        set { SetValue(QueryPaneProperty, value); }
    }

    internal readonly static DependencyProperty QueryPaneProperty =
                DependencyProperty.Register(
                    "QueryPane", typeof(PaneVisibilityState),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(PaneVisibilityState.DoNotCare, OnQueryPaneChanged));

    private static void OnQueryPaneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        if (instance.ExplorerBrowserControl != null)
        {
            instance.ExplorerBrowserControl.NavigationOptions.PaneVisibility.Query = (PaneVisibilityState)e.NewValue;
        }
    }

    public void Navigate(ShellObject destination)
    {
        ExplorerBrowserControl.Navigate(destination);
    }

    public void SetFocus()
    {
        ExplorerBrowserControl.Focus();
    }

    public void NavigateLogLocation(NavigationLogDirection direction)
    {
        ExplorerBrowserControl.NavigateLogLocation(direction);
    }

    /// <summary>
    /// Navigation log index
    /// </summary>
    public int NavigationLogIndex
    {
        get { return (int)GetValue(NavigationLogIndexProperty); }
        set { SetValue(NavigationLogIndexProperty, value); }
    }

    internal readonly static DependencyProperty NavigationLogIndexProperty =
                DependencyProperty.Register(
                    "NavigationLogIndex", typeof(int),
                    typeof(ExplorerBrowser),
                    new PropertyMetadata(0, OnNavigationLogIndexChanged));

    private static void OnNavigationLogIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExplorerBrowser instance = d as ExplorerBrowser;
        instance.ExplorerBrowserControl?.NavigationLog.NavigateLog((int)e.NewValue);
    }


    #region IDisposable Members

    private bool disposedValue;

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    /// <summary>
    /// Disposes the class
    /// </summary>        
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the browser.
    /// </summary>
    /// <param name="disposed"></param>
    protected virtual void Dispose(bool disposed)
    {
        if (disposed)
        {
            itemsChanged?.Close();
            selectionChanged?.Close();
            if (dtCLRUpdater != null && dtCLRUpdater.IsEnabled)
                dtCLRUpdater.Stop();
            disposedValue = true;
        }
    }

    #endregion
}
