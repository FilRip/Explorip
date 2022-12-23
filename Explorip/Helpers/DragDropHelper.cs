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
                _dragDropEnCours = value;
                if (!value)
                    LibererMemoire();
            }
        }

        public DirectoryInfo RepertoireDemarrage { get; set; }

        public List<FileSystemInfo> ListeFichiersDossiers { get; private set; }

        public IntPtr[] ListePointeursFichiersDossiers { get; private set; }

        private void CreerPointeurData()
        {
            ListePointeursFichiersDossiers = new IntPtr[ListeFichiersDossiers.Count];
            int position = 0;
            foreach (FileSystemInfo item in ListeFichiersDossiers)
            {
                ListePointeursFichiersDossiers[position++] = new ShellItem(item.FullName).RelativePidl;
            }

            Guid guid = new("{0000010e-0000-0000-C000-000000000046}");
            _shellFolder.GetUIObjectOf(
                IntPtr.Zero,
                (uint)ListePointeursFichiersDossiers.Length,
                ListePointeursFichiersDossiers,
                ref guid,
                IntPtr.Zero,
                out _pointeurData);
        }

        public bool ItemDrag(List<FileSystemInfo> listeFichiersDossiers, MouseButtons boutonsSouris, DirectoryInfo repertoireCourant)
        {
            try
            {
                StartButton = boutonsSouris;
                int erreur;

                RepertoireDemarrage = repertoireCourant;

                if ((listeFichiersDossiers == null) || (listeFichiersDossiers.Count == 0))
                    return false;

                LibererMemoire();
                // TODO : Penser a libérer les espaces mémoires/pointeurs réservés par cette méthode

                if (repertoireCourant == null)
                {
                    erreur = Shell32.SHGetDesktopFolder(out _pidlParent);
                    if (erreur == (int)Commun.HRESULT.S_OK)
                        return false;
                    _shellFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(_pidlParent, typeof(IShellFolder));
                }
                else
                {
                    Guid guidSH = typeof(IShellFolder).GUID;
                    _pidlParent = Shell32.ILCreateFromPath(repertoireCourant.FullName);
                    Shell32.SHBindToParent(_pidlParent, ref guidSH, out IntPtr pidlInterface, IntPtr.Zero);
                    _shellFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pidlInterface, typeof(IShellFolder));
                }
                if (_pidlParent != IntPtr.Zero)
                {
                    ListeFichiersDossiers = listeFichiersDossiers;
                    GetIDropTarget();
                    CreerPointeurData();
                    Console.WriteLine($"DemarreDragDrop dans {repertoireCourant.FullName} parent de {listeFichiersDossiers[0]}");
                    Ole32.DoDragDrop(_pointeurData, this, effetDragDrop, out DragDropEffects effets);
                }

                _dragDropEnCours = true;
                return true;
            }
            catch (Exception) { }
            return false;
        }

        private void GetParent()
        {
            Guid guid = typeof(IShellFolder).GUID;
            IShellFolder oDesktopFolder;
            Shell32.SHGetDesktopFolder(out IntPtr pUnkownDesktopFolder);
            oDesktopFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnkownDesktopFolder, typeof(IShellFolder));
            uint pchEaten = 0;
            WinAPI.Modeles.SFGAO pdwAttributes = 0;
            oDesktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, RepertoireDemarrage.FullName, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
            Console.WriteLine("Parent défini sur " + RepertoireDemarrage.FullName);
            oDesktopFolder.BindToObject(pPIDL, IntPtr.Zero, ref guid, out object pUnknownParentFolder);
            _shellFolder = (IShellFolder)pUnknownParentFolder;
        }

        private void LibererMemoire()
        {
            // TODO : Libérer les mémoires/pointeurs aliés
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
        }

        private void GetIDropTarget()
        {
            GetParent();
            ShellItem item = new(Path.GetDirectoryName(ListeFichiersDossiers[0].FullName));
            Console.WriteLine("Item défini sur = " + item.ToString());
            int erreur;
            Guid guid = typeof(WinAPI.Modeles.IDropTarget).GUID;

            erreur = _shellFolder.GetUIObjectOf(
                IntPtr.Zero,
                1,
                new IntPtr[] { item.RelativePidl },
                ref guid,
                IntPtr.Zero,
                out IntPtr dropTargetPtr);

            if (erreur == (int)Commun.HRESULT.S_OK)
            {
                Console.WriteLine("GetUIObjectOf Created with " + item.ToString());
                _dropTarget = (WinAPI.Modeles.IDropTarget)Marshal.GetTypedObjectForIUnknown(dropTargetPtr, typeof(WinAPI.Modeles.IDropTarget));
            }
            else
                throw new Exceptions.ExploripException($"DropHandler, err {erreur}={BetterWin32Errors.ErreurWindows.RetourneTexteErreur(erreur)}");
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
                Console.WriteLine("Liste fichier/dossier vide");
                return;
            }
            //_pointeurData = GetIDataObject(new ShellItem[] { new ShellItem(ListeFichiersDossiers[0].FullName) });
            if (_pointeurData != IntPtr.Zero)
            {
                Console.WriteLine("Pointeur data sur " + ListeFichiersDossiers[0].FullName);
                Console.WriteLine("OpenPopUp DragDrop");
                /*dropTarget.DragEnter(_pointeurData, touches, new POINT(Cursor.Position.X, Cursor.Position.Y), ref effets);
                dropTarget.DragOver(touches, new POINT(Cursor.Position.X, Cursor.Position.Y), ref effets);*/
                int erreur = _dropTarget.DragDrop(_pointeurData, touches, new POINT(e.X, e.Y), ref effets);
                _dropTargetHelper.Drop(_pointeurData, new POINT(e.X, e.Y), effets);
                Console.WriteLine("Retour de la popUp : " + erreur.ToString());
            }
            else
                Console.WriteLine("GetIDataObject retourne null");
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
        public IntPtr GetIDataObject(ShellItem[] items, IShellFolder parent = null)
        {
            IntPtr[] pidls = new IntPtr[items.Length];
            for (int i = 0; i < items.Length; i++)
                pidls[i] = items[i].RelativePidl;

            if (parent != null)
                _shellFolder = parent;

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
            _dropTarget.DragEnter(_pointeurData, touches, new POINT(e.X, e.Y), ref effets);
            _dropTargetHelper.DragEnter(((Control)sender).Handle, _pointeurData, new POINT(e.X, e.Y), effetDragDrop);
        }

        public void DragOver(object sender, DragEventArgs e)
        {
            MK touches = MK.RBUTTON;
            if (e.KeyState.EtatBit(4))
                touches |= MK.CONTROL;
            if (e.KeyState.EtatBit(6))
                touches |= MK.ALT;
            DragDropEffects effets = effetDragDrop;
            _dropTarget.DragOver(touches, new POINT(e.X, e.Y), ref effets);
            _dropTargetHelper.DragOver(new POINT(e.X, e.Y), effetDragDrop);
        }

        public void DragLeave(object sender, EventArgs e)
        {
            _dropTargetHelper.DragLeave();
        }

        public bool GetIDropTargetHelper()
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

                return true;
            }
            else
            {
                _dropTargetHelper = null;
                _dropTargetHelperPtr = IntPtr.Zero;
                return false;
            }
        }
    }
}
