using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

using ManagedShell.ShellFolders;
using ManagedShell.ShellFolders.Enums;

namespace Explorip.Helpers
{
    public class DragDropHelper : IDropSource
    {
        //private readonly Form _formInterception;

        private static DragDropHelper _instance;
        public static DragDropHelper GetInstance()
        {
            if (_instance == null)
            {
                _instance = new();
                _instance.GetIDropTargetHelper();
            }
            return _instance;
        }

        private bool _dragDropEnCours;

        private IntPtr _pidlParent;
        private IShellFolder _shellFolder;
        private WinAPI.Modeles.IDropTarget _dropTarget;
        private IntPtr _dropTargetHelperPtr;
        private IDropTargetHelper _dropTargetHelper;
        private IntPtr _pointeurData;

        public const DragDropEffects effetDragDrop = DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;

        private DragDropHelper()
        {
            /*_formInterception = new Form()
            {
                ShowInTaskbar = false,
                Text = "",
                Visible = false
            };*/
            ListeFichiersDossiers = new List<FileSystemInfo>();
        }

        public MouseButtons StartButton { get; set; }

        public bool DragDropEnCours
        {
            get { return _dragDropEnCours; }
            set
            {
                if (!value && _dragDropEnCours)
                    LibererMemoire();
                _dragDropEnCours = value;
            }
        }

        public DirectoryInfo RepertoireDemarrage { get; set; }

        public List<FileSystemInfo> ListeFichiersDossiers { get; private set; }

        public IntPtr[] ListePointeursFichiersDossiers { get; private set; }

        private void CreerPointeurData()
        {
            ListePointeursFichiersDossiers = new IntPtr[ListeFichiersDossiers.Count];
            int position = 0;
            List<ShellItem> listeShellItems = new();
            foreach (FileSystemInfo item in ListeFichiersDossiers)
            {
                ShellItem si = new(item.FullName);
                ListePointeursFichiersDossiers[position++] = si.RelativePidl;
                listeShellItems.Add(si);
            }

            _pointeurData = GetIDataObject(listeShellItems.ToArray());
        }

        public bool ItemDrag(List<FileSystemInfo> listeFichiersDossiers, MouseButtons boutonsSouris, DirectoryInfo repertoireCourant)
        {
            try
            {
                StartButton = boutonsSouris;

                RepertoireDemarrage = repertoireCourant;

                if ((listeFichiersDossiers == null) || (listeFichiersDossiers.Count == 0))
                    return false;

                LibererMemoire();

                if (repertoireCourant == null)
                {
                    // Si on a pas de dossier, on prend la racine de toute chose : le bureau
                    _shellFolder = ExtensionsDossiers.GetDesktopFolder();
                }
                else
                {
                    _shellFolder = repertoireCourant.GetShellFolder();
                }

                if (_shellFolder != null)
                {
                    ListeFichiersDossiers = listeFichiersDossiers;
                    GetIDropTarget();
                    CreerPointeurData();
                    Ole32.DoDragDrop(_pointeurData, this, effetDragDrop, out DragDropEffects effets);
                }

                _dragDropEnCours = true;
                return true;
            }
            catch (Exception) { /* Ignore errors */ }
            return false;
        }

        private void LibererMemoire()
        {
            ListeFichiersDossiers.Clear();
            if (ListePointeursFichiersDossiers != null)
                foreach (IntPtr pointeur in ListePointeursFichiersDossiers)
                    Marshal.FreeCoTaskMem(pointeur);
            ListePointeursFichiersDossiers = new IntPtr[0];
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
            DirectoryInfo dirInfo = new(Path.GetDirectoryName(ListeFichiersDossiers[0].FullName));
            ShellItem item = new(dirInfo.FullName);
            Guid guid = typeof(WinAPI.Modeles.IDropTarget).GUID;
            if (_shellFolder != null)
            {
                int erreur = dirInfo.Parent.GetShellFolder().GetUIObjectOf(
                    IntPtr.Zero,
                    1,
                    new IntPtr[] { item.RelativePidl },
                    ref guid,
                    IntPtr.Zero,
                    out IntPtr dropTargetPtr);
                if (erreur == (int)Commun.HRESULT.S_OK)
                {
                    _dropTarget = (WinAPI.Modeles.IDropTarget)Marshal.GetTypedObjectForIUnknown(dropTargetPtr, typeof(WinAPI.Modeles.IDropTarget));
                }
            }
        }

        public void DragDrop(object sender, DragEventArgs e)
        {
            MK touches = MK.RBUTTON;
            if (e.KeyState.EtatBit(4))
                touches |= MK.CONTROL;
            if (e.KeyState.EtatBit(6))
                touches |= MK.ALT;
            DragDropEffects effets = effetDragDrop;

            if (ListeFichiersDossiers == null || ListeFichiersDossiers.Count == 0)
            {
                return;
            }
            if (_pointeurData != IntPtr.Zero)
            {
                Console.WriteLine("Menu Start");
                if (_dropTarget != null)
                    _dropTarget.DragDrop(_pointeurData, touches, new NativePoint(e.X, e.Y), ref effets);
                _dropTargetHelper.Drop(_pointeurData, new NativePoint(e.X, e.Y), effets);
                Console.WriteLine("Menu end");
            }
            _dragDropEnCours = false;
        }

        #region Interface IDropSource

        int IDropSource.QueryContinueDrag(bool fEscapePressed, MK grfKeyState)
        {
            if (fEscapePressed)
            {
                _dragDropEnCours = false;
                return Commun.DRAGDROP_S_CANCEL;
            }
            else
            {
                if ((StartButton & MouseButtons.Left) != 0 && (grfKeyState & MK.LBUTTON) == 0)
                    return Commun.DRAGDROP_S_DROP;
                else if ((StartButton & MouseButtons.Right) != 0 && (grfKeyState & MK.RBUTTON) == 0)
                    return Commun.DRAGDROP_S_DROP;
                else
                    return (int)Commun.HRESULT.S_OK;
            }
        }

        int IDropSource.GiveFeedback(DragDropEffects dwEffect)
        {
            return Commun.DRAGDROP_S_USEDEFAULTCURSORS;
        }

        #endregion

        /// <summary>
        /// This method will use the GetUIObjectOf method of IShellFolder to obtain the IDataObject of a
        /// ShellItem. 
        /// </summary>
        /// <param name="item">The item for which to obtain the IDataObject</param>
        /// <param name="dataObjectPtr">A pointer to the returned IDataObject</param>
        /// <returns>the IDataObject the ShellItem</returns>
        public IntPtr GetIDataObject(ShellItem[] items)
        {
            IntPtr[] pidls = new IntPtr[items.Length];
            for (int i = 0; i < items.Length; i++)
                pidls[i] = items[i].RelativePidl;

            Guid guid = new("{0000010e-0000-0000-C000-000000000046}");
            if (_shellFolder.GetUIObjectOf(
                    IntPtr.Zero,
                    (uint)pidls.Length,
                    pidls,
                    ref guid,
                    IntPtr.Zero,
                    out IntPtr dataObjectPtr) == (int)Commun.HRESULT.S_OK)
            {
                return dataObjectPtr;
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        public void DragEnter(object sender, DragEventArgs e)
        {
            MK touches = MK.RBUTTON;
            if (e.KeyState.EtatBit(4))
                touches |= MK.CONTROL;
            if (e.KeyState.EtatBit(6))
                touches |= MK.ALT;
            DragDropEffects effets = effetDragDrop;
            if (_dropTarget != null)
                _dropTarget.DragEnter(_pointeurData, touches, new NativePoint(e.X, e.Y), ref effets);
            _dropTargetHelper.DragEnter(((Control)sender).Handle, _pointeurData, new NativePoint(e.X, e.Y), effetDragDrop);
        }

        public void DragOver(object sender, DragEventArgs e)
        {
            MK touches = MK.RBUTTON;
            if (e.KeyState.EtatBit(4))
                touches |= MK.CONTROL;
            if (e.KeyState.EtatBit(6))
                touches |= MK.ALT;
            DragDropEffects effets = effetDragDrop;
            if (_dropTarget != null)
                _dropTarget.DragOver(touches, new NativePoint(e.X, e.Y), ref effets);
            _dropTargetHelper.DragOver(new NativePoint(e.X, e.Y), effetDragDrop);
        }

        public void DragLeave(object sender, EventArgs e)
        {
            _dropTargetHelper.DragLeave();
            _dropTarget.DragLeave();
            LibererMemoire();
            _dragDropEnCours = false;
        }

        private void GetIDropTargetHelper()
        {
            Guid guidDropTarget = typeof(IDropTargetHelper).GUID, guidDropHelper = Shell32.CLSID_DragDropHelper;

            if (Ole32.CoCreateInstance(
                    ref guidDropHelper,
                    IntPtr.Zero,
                    CLSCTX.INPROC_SERVER,
                    ref guidDropTarget,
                    out _dropTargetHelperPtr) == (int)Commun.HRESULT.S_OK)
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
}
