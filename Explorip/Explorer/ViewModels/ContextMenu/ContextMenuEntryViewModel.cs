using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls.ContextMenu;
using Explorip.Explorer.Helpers;

using ManagedShell.Interop;
using ManagedShell.ShellFolders.Interfaces;

using Microsoft.WindowsAPICodePack.Shell.Common;

namespace Explorip.Explorer.ViewModels;

public partial class ContextMenuEntryViewModel(ShellContextMenuEntry entry, PopUpExplorerContextMenuViewModel parent, ContextMenuEntryViewModel contextMenuEntry) : ObservableObject
{
    private readonly PopUpExplorerContextMenuViewModel _parent = parent;
    private readonly ContextMenuEntryViewModel _parentContextMenuEntry = contextMenuEntry;

    [ObservableProperty()]
    private ShellContextMenuEntry _entry = entry;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(HasSubItems))]
    private ObservableCollection<ContextMenuEntryViewModel> _subItems = [];
    [ObservableProperty()]
    private bool _isVisible = true;
    [ObservableProperty()]
    private bool _isEnabled = true;

    public SolidColorBrush Background
    {
        get { return _parent.Background; }
    }

    public SolidColorBrush Foreground
    {
        get { return _parent.Foreground; }
    }

    public double Dpi
    {
        get { return _parent.Dpi; }
    }

    public string Label
    {
        get { return Entry.Name.Replace("&", ""); }
    }

    public bool IconVisible
    {
        get { return Entry.Icon != null; }
    }

    public bool HasSubItems
    {
        get { return SubItems.Count > 0; }
    }

    public bool IsMouseOver { get; set; }

    [RelayCommand()]
    private void Execute()
    {
        if (HasSubItems)
            return;

        switch (Entry.Source)
        {
            case ETypeCommand.ShellVerb:
                string command, param;
                command = Entry.Command;
                param = _parent.ListSelected[0].ParsingName;
                int lastIndex;
                if (Entry.Command.StartsWith("\""))
                    lastIndex = command.IndexOf('\"', 1);
                else
                    lastIndex = command.IndexOf(' ');
                command = command.Substring(0, lastIndex + 1);
                if (Entry.Command.Length > lastIndex + 1)
                {
                    param = Entry.Command.Substring(lastIndex + 1);
                    for (int i = 1; i <= _parent.ListSelected.Length; i++)
                        param = param.Replace($"%{i}", _parent.ListSelected[i - 1].ParsingName).Trim();
                }
                Process.Start(command, param);
                break;
            case ETypeCommand.ContextMenuHandler:
                break;
            case ETypeCommand.CommandStore:
                break;
            /*case ETypeCommand.SendTo:
                if (Path.GetExtension(Entry.Command) == ".lnk")
                {
                    StringBuilder sb = new();
                    foreach (ShellObject so in _parent.ListSelected)
                    {
                        if (sb.Length > 0)
                            sb.Append(' ');
                        sb.Append('\"');
                        sb.Append(so.ParsingName);
                        sb.Append('\"');
                    }
                    Process.Start(Entry.Command, sb.ToString());
                }
                else if (Guid.TryParse(Entry.Command, out Guid guidSendTo))
                {
                    if (guidSendTo == Guid.Empty)
                    {
                        Guid.TryParse(Entry.Command, out guidSendTo);
                    }
                    if (guidSendTo != Guid.Empty)
                    {
                        try
                        {
                            Type t = Type.GetTypeFromCLSID(guidSendTo);
                            NativeMethods.IExplorerCommand exCommand = (NativeMethods.IExplorerCommand)Activator.CreateInstance(t);
                            exCommand.GetTitle(null, out IntPtr ptrTitle);
                            if (ptrTitle != IntPtr.Zero)
                            {
                                Entry.Name = Marshal.PtrToStringUni(ptrTitle);
                                Marshal.FreeCoTaskMem(ptrTitle);
                            }
                        }
                        catch (Exception) { /* Errors *//* }
                    }
                }
                break;*/
            case ETypeCommand.CreateShortcut:
                break;
            case ETypeCommand.Rename:
                break;
            case ETypeCommand.Share:
                break;
            case ETypeCommand.New:
                break;
        }
    }

    [RelayCommand()]
    private void MouseEnter()
    {
        IsMouseOver = true;
        if (HasSubItems && _parent.Popup == null)
        {
            _parent.Popup = new PopUpExplorerContextMenuSubItems()
            {
                DataContext = this,
                Owner = Window.GetWindow(_parent.ParentTab),
            };
            SubItems[0].IsMouseOver = true;
            _parent.Popup.Show();
        }
        else
        {
            if (!HasSubItems || _parent.Popup?.DataContext != this)
            {
                if (_parentContextMenuEntry != null && _parentContextMenuEntry.SubItems.Contains(this))
                    return;
                _parent.Popup?.Close();
                _parent.Popup = null;
            }
        }
    }

    [RelayCommand()]
    private void MouseLeave()
    {
        IsMouseOver = false;
        if (HasSubItems && _parent.Popup != null && !SubItems.Any(p => p.IsMouseOver))
        {
            if (_parentContextMenuEntry == null || !_parentContextMenuEntry.SubItems.Contains(this))
                return;
            _parent.Popup.Close();
            _parent.Popup = null;
        }
    }
}
