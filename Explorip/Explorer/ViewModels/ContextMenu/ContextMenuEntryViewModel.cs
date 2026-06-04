using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Explorip.Explorer.Controls.ContextMenu;
using Explorip.Explorer.Helpers.ContextMenu;
using Explorip.Explorer.Windows;

using ManagedShell.Interop;
using ManagedShell.ShellFolders.Enums;

using ManagedShell.ShellFolders.Structs;

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

    public PopUpExplorerContextMenuSubItems Popup { get; set; }

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
                _parent.Close();
                break;
            case ETypeCommand.ContextMenuHandler:
                System.Drawing.Point mousePos = new();
                NativeMethods.GetCursorPos(ref mousePos);
                CmInvokeCommandInfoEx invoke = new()
                {
                    lpVerbW = new IntPtr(Entry.NumCmd),
                    lpVerb = new IntPtr(Entry.NumCmd),
                    lpDirectoryW = _parent.ParentFolder.ParsingName,
                    lpDirectory = _parent.ParentFolder.ParsingName,
                    fMask = ContextMenuInfoCommands.UNICODE | ContextMenuInfoCommands.PTINVOKE |
                        (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ? ContextMenuInfoCommands.CONTROL_DOWN : 0) |
                        (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? ContextMenuInfoCommands.SHIFT_DOWN : 0),
                    ptInvoke = new NativeMethods.Point((long)mousePos.X, (long)mousePos.Y),
                    nShow = NativeMethods.WindowShowStyle.ShowNormal,
                    hwnd = ((WpfExplorerBrowser)Window.GetWindow(_parent.ParentTab)).WindowHandle,
                };
                Entry.ContextMenu?.InvokeCommand(ref invoke);
                _parent.Close();
                break;
            case ETypeCommand.CommandStore:
                break;
            case ETypeCommand.SendTo:
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
                        catch (Exception) { /* Errors, certainly not a IExplorerCommand */ }
                    }
                }
                _parent.Close();
                break;
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
        if (HasSubItems && ((_parentContextMenuEntry == null && _parent.Popup != null && _parent.Popup.DataContext != this) || (_parentContextMenuEntry != null && _parentContextMenuEntry.Popup != null && _parentContextMenuEntry.Popup.DataContext != this)))
        {
            if (_parentContextMenuEntry == null)
            {
                _parent.Popup?.Close();
                _parent.Popup = null;
            }
            else
            {
                _parentContextMenuEntry.Popup?.Close();
                _parentContextMenuEntry.Popup = null;
            }
        }
        if (HasSubItems && ((_parentContextMenuEntry == null && _parent.Popup == null) || (_parentContextMenuEntry != null && _parentContextMenuEntry.Popup == null)))
        {
            if (_parentContextMenuEntry == null)
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
                _parentContextMenuEntry.Popup = new PopUpExplorerContextMenuSubItems()
                {
                    DataContext = this,
                    Owner = _parent.Popup,
                };
                SubItems[0].IsMouseOver = true;
                _parentContextMenuEntry.Popup.Show();
            }
        }
        else
        {
            if (!HasSubItems || ((_parentContextMenuEntry == null && _parent.Popup?.DataContext != this) || (_parentContextMenuEntry != null && _parentContextMenuEntry.Popup.DataContext != this)))
            {
                if (_parentContextMenuEntry == null)
                {
                    _parent.Popup?.Close();
                    _parent.Popup = null;
                }
                else
                {
                    _parentContextMenuEntry.Popup?.Close();
                    _parentContextMenuEntry.Popup = null;
                }
            }
        }
    }

    [RelayCommand()]
    private void MouseLeave()
    {
        IsMouseOver = false;
        if (HasSubItems && ((_parentContextMenuEntry == null && _parent.Popup != null) || (_parentContextMenuEntry != null && _parentContextMenuEntry.Popup != null)) && !SubItems.Any(p => p.IsMouseOver))
        {
            if ((_parentContextMenuEntry == null))
            {
                _parent.Popup.Close();
                _parent.Popup = null;
            }
            else
            {
                _parentContextMenuEntry.Popup.Close();
                _parentContextMenuEntry.Popup = null;
            }
        }
    }
}
