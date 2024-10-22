using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Enums;
using ManagedShell.ShellFolders.Interfaces;

using static ManagedShell.Interop.NativeMethods;

namespace ExploripComponents;

public class DragDropHelper : IDropSource
{
    private static DragDropHelper? _instance;
    public static DragDropHelper GetInstance()
    {
        if (_instance == null)
        {
            _instance = new();
            _instance.GetIDropTargetHelper();
        }
        return _instance;
    }

    private bool _dragDropActivate;

    private IntPtr _pidlParent;
    private IShellFolder? _shellFolder;
    private IDropTarget? _dropTarget;
    private IntPtr _dropTargetHelperPtr;
    private IDropTargetHelper? _dropTargetHelper;
    private IntPtr _ptrData;

    public const DragDropEffects effetDragDrop = DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;

    private DragDropHelper()
    {
        ListFs = [];
        ListPtrFs = [];
    }

    public bool DragDropActivate
    {
        get { return _dragDropActivate; }
        set
        {
            if (!value && _dragDropActivate)
                FreeMemory();
            _dragDropActivate = value;
        }
    }

    public List<string> ListFs { get; private set; }

    public IntPtr[] ListPtrFs { get; private set; }

    private void CreateDataPtr()
    {
        ListPtrFs = new IntPtr[ListFs.Count];
        int position = 0;
        List<ShellItem> listeShellItems = [];
        foreach (string item in ListFs)
        {
            ShellItem si = new(item);
            ListPtrFs[position++] = si.RelativePidl;
            listeShellItems.Add(si);
        }

        _ptrData = GetIDataObject([.. listeShellItems]);
    }

    public bool ItemDrag(List<string> listFs, string fullPath)
    {
        try
        {
            if (listFs == null || listFs.Count == 0)
                return false;

            FreeMemory();

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                // Si on a pas de dossier, on prend la racine de toute chose : le bureau
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
                GetIDropTarget();
                CreateDataPtr();
            }

            _dragDropActivate = true;
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

    private void GetIDropTarget()
    {
        DirectoryInfo dirInfo = new(Path.GetDirectoryName(ListFs[0]));
        ShellItem item = new(dirInfo.FullName);
        Guid guid = typeof(IDropTarget).GUID;
        if (_shellFolder != null)
        {
            int erreur = GetShellFolder(dirInfo.Parent).GetUIObjectOf(
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

    public static IShellFolder GetShellFolder(DirectoryInfo directoryInfo)
    {
        // TODO : Rewrite, check https://www.codeproject.com/Articles/39224/Rewrite-DirectoryInfo-using-IShellFolder
        IShellFolder sfd = ShellContextMenu.GetDesktopFolder();
        uint pchEaten = 0;
        SFGAO pdwAttributes = 0;
        sfd.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, directoryInfo.FullName, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
        Guid guid = typeof(IShellFolder).GUID;
        sfd.BindToObject(pPIDL, IntPtr.Zero, ref guid, out IntPtr pUnknownParentFolder);
        return (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnknownParentFolder, typeof(IShellFolder));
    }

    public void DragDrop(object sender, DragEventArgs e, System.Windows.Point mousePos)
    {
        MK keys = MK.RBUTTON;
        if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
            keys |= MK.CONTROL;
        if (e.KeyStates.HasFlag(DragDropKeyStates.AltKey))
            keys |= MK.ALT;
        if (e.KeyStates.HasFlag(DragDropKeyStates.ShiftKey))
            keys |= MK.SHIFT;
        DragDropEffects effets = effetDragDrop;

        if (ListFs.Count == 0)
            return;

        if (_ptrData != IntPtr.Zero)
        {
            ManagedShell.Interop.NativeMethods.Point nativePoint = new((int)mousePos.X, (int)mousePos.Y);

            _dropTarget!.DragEnter(_ptrData, keys, nativePoint, ref effets);
            _dropTargetHelper!.DragEnter((PresentationSource.FromVisual((System.Windows.Controls.Control)sender) as HwndSource)!.Handle, _ptrData, nativePoint, effetDragDrop);

            _dropTarget.DragOver(keys, nativePoint, ref effets);
            _dropTargetHelper.DragOver(nativePoint, effetDragDrop);

            _dropTarget.DragDrop(_ptrData, keys, nativePoint, ref effets);
            _dropTargetHelper.Drop(_ptrData, nativePoint, effets);
        }
        _dragDropActivate = false;
    }

    #region Interface IDropSource

    int IDropSource.QueryContinueDrag(bool fEscapePressed, MK grfKeyState)
    {
        if (fEscapePressed)
        {
            _dragDropActivate = false;
            return DRAGDROP_S_CANCEL;
        }
        else
        {
            if ((grfKeyState & MK.RBUTTON) == 0)
                return DRAGDROP_S_DROP;
            else
                return (int)HResult.SUCCESS;
        }
    }

    int IDropSource.GiveFeedback(DragDropEffects dwEffect)
    {
        return DRAGDROP_S_USEDEFAULTCURSORS;
    }

    #endregion

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
        if (_shellFolder!.GetUIObjectOf(
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

    private void GetIDropTargetHelper()
    {
        Guid guidDropTarget = typeof(IDropTargetHelper).GUID, guidDropHelper = CLSID_DragDropHelper;

        if (Interop.CoCreateInstance(
                ref guidDropHelper,
                IntPtr.Zero,
                CLSCTX.INPROC_SERVER,
                ref guidDropTarget,
                out _dropTargetHelperPtr) == (int)HResult.SUCCESS)
        {
            _dropTargetHelper =
                (IDropTargetHelper)Marshal.GetTypedObjectForIUnknown(_dropTargetHelperPtr, typeof(IDropTargetHelper));
        }
        else
        {
            _dropTargetHelper = null;
            _dropTargetHelperPtr = IntPtr.Zero;
        }
    }
}
