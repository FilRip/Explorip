using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;

using static ManagedShell.Interop.NativeMethods;

namespace ExploripComponents;

public class DragDropHelper
{
    private static DragDropHelper? _instance;
    public static DragDropHelper GetInstance()
    {
        _instance ??= new();
        return _instance;
    }

    private IntPtr _pidlParent;
    private IShellFolder? _shellFolder;
    private IDropTarget? _dropTarget;
    private IntPtr _ptrData;

    private const DragDropEffects dragDropEffects = DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;

    private DragDropHelper()
    {
        ListFs = [];
        ListPtrFs = [];
    }

    public List<string> ListFs { get; private set; }

    public IntPtr[] ListPtrFs { get; private set; }

    private void CreateDataPtr()
    {
        ListPtrFs = new IntPtr[ListFs.Count];
        int position = 0;
        List<ShellItem> listShellItems = [];
        foreach (string item in ListFs)
        {
            ShellItem si = new(item);
            ListPtrFs[position++] = si.RelativePidl;
            listShellItems.Add(si);
        }

        _ptrData = GetIDataObject([.. listShellItems]);
    }

    private bool ItemDrag(List<string> listFs, string fullPath)
    {
        try
        {
            if (listFs == null || listFs.Count == 0)
                return false;

            FreeMemory();

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                // If there is no folder, we take the parent of everything : the desktop
                _shellFolder = ShellContextMenu.GetDesktopFolder();
            }
            else
            {
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                ShellContextMenu.GetDesktopFolder().ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fullPath, ref pchEaten, out nint pPIDL, ref pdwAttributes);
                Guid guid = typeof(IShellFolder).GUID;
                ShellContextMenu.GetDesktopFolder().BindToObject(pPIDL, IntPtr.Zero, ref guid, out nint pUnknownParentFolder);
                _shellFolder = (IShellFolder)Marshal.GetObjectForIUnknown(pUnknownParentFolder);
            }

            if (_shellFolder != null)
            {
                ListFs = listFs;
                GetIDropTarget(fullPath);
                CreateDataPtr();
            }

            return true;
        }
        catch (Exception) { /* Ignore errors */ }
        return false;
    }

    private void FreeMemory()
    {
        ListFs.Clear();
        if (ListPtrFs != null)
            foreach (IntPtr pointeur in ListPtrFs)
                Marshal.FreeCoTaskMem(pointeur);
        ListPtrFs = [];
        if (_shellFolder != null)
        {
            Marshal.ReleaseComObject(_shellFolder);
            _shellFolder = null;
        }
        if (_pidlParent != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(_pidlParent);
            _pidlParent = IntPtr.Zero;
        }
        if (_dropTarget != null)
        {
            Marshal.ReleaseComObject(_dropTarget);
            _dropTarget = null;
        }
    }

    private void GetIDropTarget(string path)
    {
        DirectoryInfo dirInfo = new(path);
        ShellItem item = new(dirInfo.FullName);
        Guid guid = typeof(IDropTarget).GUID;
        if (_shellFolder != null)
        {
            int erreur = GetShellFolder(dirInfo.Parent?.FullName).GetUIObjectOf(
                IntPtr.Zero,
                1,
                [item.RelativePidl],
                ref guid,
                IntPtr.Zero,
                out IntPtr dropTargetPtr);
            if (erreur == (int)HResult.SUCCESS)
                _dropTarget = (IDropTarget)Marshal.GetTypedObjectForIUnknown(dropTargetPtr, typeof(IDropTarget));
        }
    }

    public static IShellFolder GetShellFolder(string? path)
    {
        Guid guid = typeof(IShellFolder).GUID;
        IShellFolder sfd = ShellContextMenu.GetDesktopFolder();
        if (string.IsNullOrWhiteSpace(path))
        {
            IntPtr pidlMyComputer = IntPtr.Zero;
            SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL.CSIDL_DRIVES, ref pidlMyComputer);
            sfd.BindToObject(pidlMyComputer, IntPtr.Zero, guid, out IntPtr ptrMyComputer);
            IShellFolder myComputerFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(ptrMyComputer, typeof(IShellFolder));
            return myComputerFolder;
        }
        uint pchEaten = 0;
        SFGAO pdwAttributes = 0;
        sfd.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, path, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
        sfd.BindToObject(pPIDL, IntPtr.Zero, ref guid, out IntPtr pUnknownParentFolder);
        return (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnknownParentFolder, typeof(IShellFolder));
    }

    public void DragDrop(DragDropKeyStates e, System.Windows.Point mousePos, string fullPath, List<string> listFs)
    {
        try
        {
            if (!ItemDrag(listFs, fullPath))
                return;

            MK keys = MK.RBUTTON;
            if (e.HasFlag(DragDropKeyStates.ControlKey))
                keys |= MK.CONTROL;
            if (e.HasFlag(DragDropKeyStates.AltKey))
                keys |= MK.ALT;
            if (e.HasFlag(DragDropKeyStates.ShiftKey))
                keys |= MK.SHIFT;

            if (ListFs.Count == 0)
                return;

            if (_ptrData != IntPtr.Zero && _dropTarget != null)
            {
                PointInt nativePoint = new((int)mousePos.X, (int)mousePos.Y);

                DragDropEffects effects = dragDropEffects;
                _dropTarget.DragEnter(_ptrData, keys, nativePoint, ref effects);
                effects = dragDropEffects;
                _dropTarget.DragDrop(_ptrData, keys, nativePoint, ref effects);
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }

    /// <summary>
    /// This method will use the GetUIObjectOf method of IShellFolder to obtain the IDataObject of a
    /// ShellItem. 
    /// </summary>
    /// <param name="item">The item for which to obtain the IDataObject</param>
    /// <param name="dataObjectPtr">A ManagedShell.Interop.NativeMethods.Pointer to the returned IDataObject</param>
    /// <returns>the IDataObject the ShellItem</returns>
    public IntPtr GetIDataObject(ShellItem[] items)
    {
        IntPtr[] pidls = new IntPtr[items.Length];
        for (int i = 0; i < items.Length; i++)
            pidls[i] = items[i].RelativePidl;

        Guid guid = new("{0000010e-0000-0000-C000-000000000046}");
        if (GetShellFolder(Path.GetDirectoryName(ListFs[0]))!.GetUIObjectOf(
                IntPtr.Zero,
                (uint)pidls.Length,
                pidls,
                ref guid,
                IntPtr.Zero,
                out IntPtr dataObjectPtr) == (int)HResult.SUCCESS)
        {
            return dataObjectPtr;
        }
        else
        {
            return IntPtr.Zero;
        }
    }
}
