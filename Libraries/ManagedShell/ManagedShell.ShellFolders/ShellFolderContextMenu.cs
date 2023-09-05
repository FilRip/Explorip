using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.Interop;
using ManagedShell.ShellFolders.Interfaces;

namespace ManagedShell.ShellFolders
{
    public class ShellFolderContextMenu : ShellContextMenu
    {
        public delegate void FolderItemSelectAction(uint uid, string path);
        private readonly FolderItemSelectAction folderItemSelected;

        public ShellFolderContextMenu(ShellFolder folder, FolderItemSelectAction folderItemSelected, ShellMenuCommandBuilder builder)
        {
            if (folder == null)
            {
                return;
            }

            lock (IconHelper.ComLock)
            {
                x = Cursor.Position.X;
                y = Cursor.Position.Y;

                this.folderItemSelected = folderItemSelected;

                SetupContextMenu(folder, builder);
            }
        }

        private void ConfigureMenuItems(ShellFolder folder, IntPtr contextMenu, ShellMenuCommandBuilder builder)
        {
            int numAdded = 0;
            ShellNewMenus.Clear();

            foreach (ShellMenuCommand command in builder.Commands)
            {
                if (command is ShellNewMenuCommand shellNewCommand)
                {
                    shellNewCommand.AddSubMenu(folder, numAdded, ref contextMenu);
                    ShellNewMenus.Add(shellNewCommand);
                }
                else
                {
                    Interop.AppendMenu(contextMenu, command.Flags, command.UID, command.Label);
                }

                numAdded++;
            }
        }

        private void SetupContextMenu(ShellFolder folder, ShellMenuCommandBuilder builder)
        {
            try
            {
                nativeMenuPtr = Interop.CreatePopupMenu();

                ConfigureMenuItems(folder, nativeMenuPtr, builder);

                ShowMenu(folder, nativeMenuPtr);
            }
            catch (Exception e)
            {
                ShellLogger.Error($"ShellContextMenu: Error building folder context menu: {e.Message}");
            }
            finally
            {
                FreeResources();

                foreach (ShellNewMenuCommand subMenu in ShellNewMenus)
                {
                    subMenu.FreeResources();
                }
            }
        }

        private void ShowMenu(ShellFolder folder, IntPtr contextMenu)
        {
            CreateHandle(new CreateParams());

            if (EnvironmentHelper.IsWindows10DarkModeSupported)
            {
                NativeMethods.AllowDarkModeForWindow(Handle, true);
            }

            uint selected = Interop.TrackPopupMenuEx(
                contextMenu,
                NativeMethods.TPM.RETURNCMD,
                x,
                y,
                Handle,
                IntPtr.Zero);

            if (selected >= Interop.CMD_FIRST)
            {
                if (selected <= Interop.CMD_LAST)
                {
                    // custom commands are greater than CMD_LAST, so this must be a sub menu item
                    foreach (IContextMenu subMenu in ShellNewMenus.Select(item => item.iContextMenu))
                    {
                        if (subMenu != null)
                        {
                            InvokeCommand(
                                subMenu,
                                selected - Interop.CMD_FIRST,
                                new Point(x, y));
                        }
                    }
                }

                folderItemSelected?.Invoke(selected, folder.Path);
            }

            DestroyHandle();
        }
    }
}
