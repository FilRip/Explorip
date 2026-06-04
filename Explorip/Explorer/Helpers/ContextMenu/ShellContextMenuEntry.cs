using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using Explorip.Helpers;

using ExploripConfig.Configuration;

using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.ShellFolders.Interfaces;

using Microsoft.WindowsAPICodePack.Shell.Common;

namespace Explorip.Explorer.Helpers.ContextMenu
{
    public partial class ShellContextMenuEntry : ObservableObject, ICloneable, IDisposable
    {
        private bool disposedValue;
        private IntPtr _menuHandle;

        [ObservableProperty()]
        private ETypeCommand _source;
        [ObservableProperty()]
        private string _keyPath;
        [ObservableProperty()]
        private string _name;
        [ObservableProperty()]
        private string _explorerCommandHandler;
        [ObservableProperty()]
        private string _command;
        [ObservableProperty()]
        private ImageSource _icon;
        [ObservableProperty()]
        private string _toolTip;
        [ObservableProperty()]
        private ObservableCollection<ShellContextMenuEntry> _subitems = [];

        public IContextMenu ContextMenu { get; set; }
        public uint NumCmd { get; set; }

        public object Clone()
        {
            return new ShellContextMenuEntry()
            {
                Source = Source,
                KeyPath = KeyPath,
                Name = Name,
                ExplorerCommandHandler = ExplorerCommandHandler,
                Command = Command,
                Icon = Icon,
                ContextMenu = ContextMenu,
            };
        }

        public void ExpandIt(ref List<ShellContextMenuEntry> list, ShellFolder parentFolder, ShellObject[] selectedItems)
        {
            bool toRemove = false;
            switch (Source)
            {
                case ETypeCommand.ContextMenuHandler:
                    if (Guid.TryParse(Command, out Guid guid))
                    {
                        IContextMenu exCommand = null;
                        IntPtr hMenu = IntPtr.Zero;
                        IntPtr ptrData = IntPtr.Zero;

                        if (guid == Guid.Parse("{7BA4C740-9E81-11CF-99D3-00AA004AE837}"))
                        {
                            Source = ETypeCommand.SendTo;
                            Name = Constants.Localization.SEND_TO;
                            Subitems = [.. ExtensionsContextMenu.ExpandSendTo(true)];
                            break;
                        }

                        try
                        {
                            exCommand = Activator.CreateInstance(Type.GetTypeFromCLSID(guid)) as IContextMenu;
                            //IContextMenu2 exCommand2 = Activator.CreateInstance(Type.GetTypeFromCLSID(guid)) as IContextMenu2;

                            if (exCommand != null)
                            {
                                ContextMenu = exCommand;
                                List<IntPtr> listPidlRelative = [];
                                foreach (ShellObject so in selectedItems)
                                    listPidlRelative.Add(NativeMethods.ILFindLastID(so.PIDL));
                                Guid guidIDataObject = new("{0000010e-0000-0000-C000-000000000046}");
                                NativeMethods.SHCreateDataObject(parentFolder.PIDL, (uint)selectedItems.Length, [.. listPidlRelative], IntPtr.Zero, guidIDataObject, out ptrData);
                                if (ptrData != IntPtr.Zero)
                                {
                                    ((IShellExtInit)exCommand).Initialize(IntPtr.Zero, ptrData, 0);
                                    hMenu = NativeMethods.CreatePopupMenu();
                                    exCommand.QueryContextMenu(hMenu, 0, 1, int.MaxValue, ManagedShell.ShellFolders.Enums.ContextMenuStates.NORMAL);
                                    _menuHandle = hMenu;
                                    int count = NativeMethods.GetMenuItemCount(hMenu);
                                    if (count > 0)
                                    {
                                        // Auto expand sub menu
                                        /*if (exCommand2 != null)
                                        {
                                            for (int i = 0; i < count; i++)
                                            {
                                                int idMenu = NativeMethods.GetMenuItemID(hMenu, i);
                                                if (idMenu < 0)
                                                {
                                                    IntPtr idSubMenu = NativeMethods.GetSubMenu(hMenu, i);
                                                    exCommand2.HandleMenuMsg((int)NativeMethods.WM.INITMENUPOPUP, idSubMenu, new IntPtr(i));
                                                }
                                            }
                                        }*/
                                        int index = 0;
                                        if (count > 1)
                                            index = list.IndexOf(this);
                                        for (uint i = 0; i < count; i++)
                                        {
                                            NativeMethods.MenuItemInfo menuItemInfo = new()
                                            {
                                                fMask = NativeMethods.MenuItemIntegrateMembers.STATE | NativeMethods.MenuItemIntegrateMembers.STRING | NativeMethods.MenuItemIntegrateMembers.ID | NativeMethods.MenuItemIntegrateMembers.SUBMENU | NativeMethods.MenuItemIntegrateMembers.BITMAP,
                                                dwTypeData = new string((char)0, 256),
                                                cch = 255,
                                                fType = NativeMethods.MenuItemTypes.DISABLED | NativeMethods.MenuItemTypes.GRAYED | NativeMethods.MenuItemTypes.STRING,
                                            };
                                            if (NativeMethods.GetMenuItemInfo(hMenu, i, true, ref menuItemInfo))
                                            {
                                                if (string.IsNullOrWhiteSpace(menuItemInfo.dwTypeData.Trim()))
                                                {
                                                    if (i == 0)
                                                        toRemove = true;
                                                }
                                                else if (i == 0)
                                                {
                                                    Name = menuItemInfo.dwTypeData.Trim();
                                                    NumCmd = menuItemInfo.wID;
                                                    Icon = ExtensionsContextMenu.GetDefaultIcon(Command);
                                                    Icon?.Freeze();
                                                    if (Icon == null && menuItemInfo.hbmpItem != IntPtr.Zero)
                                                        Icon = ((BitmapSource)IconImageConverter.GetImageFromHBitmap(menuItemInfo.hbmpItem, false)).MakeTransparentColor(ConfigManager.ExplorerContextMenuBackground?.Color ?? ExploripSharedCopy.Constants.Colors.BackgroundColor);
                                                    if (menuItemInfo.hSubMenu != IntPtr.Zero)
                                                        ExpandContextMenuSubItems(menuItemInfo.hSubMenu, this);
                                                }
                                                else
                                                {
                                                    index++;
                                                    ShellContextMenuEntry newEntry = (ShellContextMenuEntry)Clone();
                                                    newEntry.Name = menuItemInfo.dwTypeData.Trim();
                                                    newEntry.NumCmd = menuItemInfo.wID;
                                                    newEntry.Icon = ExtensionsContextMenu.GetDefaultIcon(Command);
                                                    newEntry.Icon?.Freeze();
                                                    if (newEntry.Icon == null && menuItemInfo.hbmpItem != IntPtr.Zero)
                                                        newEntry.Icon = ((BitmapSource)IconImageConverter.GetImageFromHBitmap(menuItemInfo.hbmpItem, false)).MakeTransparentColor(ConfigManager.ExplorerContextMenuBackground?.Color ?? ExploripSharedCopy.Constants.Colors.BackgroundColor);
                                                    if (menuItemInfo.hSubMenu != IntPtr.Zero)
                                                        ExpandContextMenuSubItems(menuItemInfo.hSubMenu, newEntry);
                                                    list.Insert(index, newEntry);
                                                }
                                            }
                                            else
                                                toRemove = true;
                                        }
                                    }
                                    else
                                        toRemove = true;
                                }
                            }
                        }
                        catch (Exception) { /* Errors */ }
                        finally
                        {
                            if (toRemove)
                                list.Remove(this);
                            if (ptrData != IntPtr.Zero)
                                Marshal.FreeCoTaskMem(ptrData);
                        }
                    }
                    break;
            }
        }

        private void ExpandContextMenuSubItems(IntPtr hMenu, ShellContextMenuEntry parent)
        {
            int count = NativeMethods.GetMenuItemCount(hMenu);
            if (count > 0)
            {
                for (uint i = 0; i < count; i++)
                {
                    NativeMethods.MenuItemInfo menuItemInfo = new()
                    {
                        fMask = NativeMethods.MenuItemIntegrateMembers.STATE | NativeMethods.MenuItemIntegrateMembers.STRING | NativeMethods.MenuItemIntegrateMembers.ID | NativeMethods.MenuItemIntegrateMembers.SUBMENU | NativeMethods.MenuItemIntegrateMembers.BITMAP,
                        dwTypeData = new string((char)0, 256),
                        cch = 255,
                        fType = NativeMethods.MenuItemTypes.DISABLED | NativeMethods.MenuItemTypes.GRAYED | NativeMethods.MenuItemTypes.STRING,
                    };
                    if (NativeMethods.GetMenuItemInfo(hMenu, i, true, ref menuItemInfo))
                    {
                        if (string.IsNullOrWhiteSpace(menuItemInfo.dwTypeData.Trim()))
                            continue;
                        ShellContextMenuEntry newEntry;
                        if (i == 0)
                            newEntry = (ShellContextMenuEntry)parent?.Clone();
                        else
                            newEntry = (ShellContextMenuEntry)Clone();
                        newEntry.Name = menuItemInfo.dwTypeData.Trim();
                        newEntry.NumCmd = menuItemInfo.wID;
                        newEntry.Icon = ExtensionsContextMenu.GetDefaultIcon(Command);
                        newEntry.Icon?.Freeze();
                        if (newEntry.Icon == null && menuItemInfo.hbmpItem != IntPtr.Zero)
                            newEntry.Icon = ((BitmapSource)IconImageConverter.GetImageFromHBitmap(menuItemInfo.hbmpItem, false)).MakeTransparentColor(ConfigManager.ExplorerContextMenuBackground?.Color ?? ExploripSharedCopy.Constants.Colors.BackgroundColor);
                        parent.Subitems.Add(newEntry);
                        if (menuItemInfo.hSubMenu != IntPtr.Zero)
                            ExpandContextMenuSubItems(menuItemInfo.hSubMenu, newEntry);
                    }
                }
            }
        }

        public bool IsDisposed
        {
            get { return disposedValue; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Subitems?.Count > 0)
                        for (int i = Subitems.Count - 1; i >= 0; i--)
                            Subitems[i].Dispose();
                    try
                    {
                        if (ContextMenu != null)
                        {
                            Marshal.ReleaseComObject(ContextMenu);
                            ContextMenu = null;
                        }
                        if (_menuHandle != IntPtr.Zero)
                        {
                            NativeMethods.DestroyMenu(_menuHandle);
                            _menuHandle = IntPtr.Zero;
                        }
                    }
                    catch (Exception) { /* Ignore errors, COM object certainly already released */ }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
