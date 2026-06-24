using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls;
using Explorip.Explorer.Windows;

using ExploripConfig.Configuration;

using ManagedShell.Interop;

using Microsoft.WindowsAPICodePack.Shell.Common;

namespace Explorip.Explorer.ViewModels;

public partial class WpfExplorerBrowserViewModel : ObservableObject
{
    private WpfExplorerBrowser _window;

    public WpfExplorerBrowserViewModel() : base()
    {
        _middleFileOperationFontSize = ConfigManager.ExplorerMiddleFileOperationFontSize;
    }

    public void SetWindow(WpfExplorerBrowser window)
    {
        _window = window;
    }

    [ObservableProperty()]
    private bool _windowMaximized;

    [ObservableProperty()]
    private bool _selectionLeft;

    [ObservableProperty()]
    private bool _selectionRight;

    [ObservableProperty()]
    private double _middleFileOperationFontSize;

    [ObservableProperty()]
    private bool _showMiddleButtons;

    private static void CopyBetweenTab(TabExplorerBrowser tabSource, TabExplorerBrowser tabDestination, bool move = false)
    {
        ShellObject[] listeItems = [.. tabSource.CurrentTabExplorer.ExplorerBrowser.ExplorerBrowserControl.SelectedItems.OfType<ShellObject>()];
        string destination = tabDestination.CurrentTabExplorer.ExplorerBrowser.NavigationLog.CurrentLocation.GetDisplayName(DisplayNameType.FileSystemPath);
        Task.Run(() =>
        {
            FilesOperations.FileOperation fileOperation = new(NativeMethods.GetDesktopWindow());
            if (listeItems?.Length > 0)
            {
                string fichier;
                foreach (ShellObject item in listeItems)
                {
                    fichier = item.GetDisplayName(DisplayNameType.FileSystemPath);
                    if (move)
                        fileOperation.MoveItem(fichier, destination, Path.GetFileName(fichier));
                    else
                        fileOperation.CopyItem(fichier, destination, Path.GetFileName(fichier));
                }
                fileOperation.PerformOperations();
                fileOperation.Dispose();
            }
        });
    }

    private static void DeleteSelectTab(TabExplorerBrowser tab)
    {
        ShellObject[] listeItems = [.. tab.CurrentTabExplorer.ExplorerBrowser.ExplorerBrowserControl.SelectedItems.OfType<ShellObject>()];
        Task.Run(() =>
        {
            FilesOperations.FileOperation fileOperation = new(NativeMethods.GetDesktopWindow());
            if (listeItems?.Length > 0)
            {
                foreach (ShellObject file in listeItems)
                {
                    fileOperation.DeleteItem(file.GetDisplayName(DisplayNameType.FileSystemPath));
                }
                fileOperation.PerformOperations();
                fileOperation.Dispose();
            }
        });
    }


    [RelayCommand()]
    private void CopyLeft()
    {
        CopyBetweenTab(_window.LeftTab, _window.RightTab);
    }

    [RelayCommand()]
    private void CopyRight()
    {
        CopyBetweenTab(_window.RightTab, _window.LeftTab);
    }

    [RelayCommand()]
    private void MoveLeft()
    {
        CopyBetweenTab(_window.LeftTab, _window.RightTab, true);
    }

    [RelayCommand()]
    private void MoveRight()
    {
        CopyBetweenTab(_window.RightTab, _window.LeftTab, true);
    }

    [RelayCommand()]
    private void DeleteLeft()
    {
        DeleteSelectTab(_window.LeftTab);
    }

    [RelayCommand()]
    private void DeleteRight()
    {
        DeleteSelectTab(_window.RightTab);
    }
}
