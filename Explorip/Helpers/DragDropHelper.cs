using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

using ManagedShell.ShellFolders;

namespace Explorip.Helpers
{
    public class DragDropHelper : IDropSource
    {
        //private readonly Form _formInterception;

        private static DragDropHelper _instance;
        public static DragDropHelper GetInstance()
        {
            if (_instance == null)
                _instance = new();
            return _instance;
        }

        private MouseButtons _startButton;
        private bool _dragDropEnCours;
        private DirectoryInfo _repStart;

        private readonly List<FileSystemInfo> _listeFichiersDossiers;
        private IntPtr[] _listePointeursFichiersDossiers;

        private IntPtr _pidlParent;
        private IShellFolder _shellFolder;

        public const DragDropEffects effetDragDrop = DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;

        private DragDropHelper()
        {
            /*_formInterception = new Form()
            {
                ShowInTaskbar = false,
                Text = "",
                Visible = false
            };*/
            _listeFichiersDossiers = new List<FileSystemInfo>();
        }

        public MouseButtons StartButton
        {
            get { return _startButton; }
            set { _startButton = value; }
        }

        public bool DragDropEnCours
        {
            get { return _dragDropEnCours; }
            set
            {
                _dragDropEnCours = value;
                /*if (!value)
                    LibererMemoire();*/
            }
        }

        public DirectoryInfo RepertoireDemarrage
        {
            get { return _repStart; }
            set { _repStart = value; }
        }

        public List<FileSystemInfo> ListeFichiersDossiers
        {
            get { return _listeFichiersDossiers; }
        }

        public IntPtr[] ListePointeursFichiersDossiers
        {
            get { return _listePointeursFichiersDossiers; }
            set { _listePointeursFichiersDossiers = value; }
        }

        public bool DemarreDrag(List<FileSystemInfo> listeFichiersDossiers, MouseButtons boutonsSouris, DirectoryInfo repertoireCourant)
        {
            try
            {
                _startButton = boutonsSouris;
                int erreur;

                _repStart = repertoireCourant;

                if ((listeFichiersDossiers == null) || (listeFichiersDossiers.Count == 0))
                    return false;

                LibererMemoire();
                // TODO : Penser a libérer les espaces mémoires/pointeurs réservés par cette méthode

                if (repertoireCourant == null)
                {
                    erreur = WinAPI.Shell32.SHGetDesktopFolder(out _pidlParent);
                    if (erreur == (int)WinAPI.Commun.HRESULT.S_OK)
                        return false;
                    _shellFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(_pidlParent, typeof(IShellFolder));
                }
                else
                {
                    Guid guidSH = typeof(IShellFolder).GUID;
                    _pidlParent = WinAPI.Shell32.ILCreateFromPath(repertoireCourant.FullName);
                    WinAPI.Shell32.SHBindToParent(_pidlParent, ref guidSH, out IntPtr pidlInterface, IntPtr.Zero);
                    _shellFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pidlInterface, typeof(IShellFolder));
                }
                if (_pidlParent != IntPtr.Zero)
                {
                    _listePointeursFichiersDossiers = new IntPtr[listeFichiersDossiers.Count];
                    int position = 0;
                    foreach (FileSystemInfo item in listeFichiersDossiers)
                    {
                        _listePointeursFichiersDossiers[position++] = WinAPI.Shell32.ILCreateFromPath(item.FullName);
                        _listeFichiersDossiers.Add(item);
                    }

                    Guid guid = new("{0000010e-0000-0000-C000-000000000046}");
                    erreur = _shellFolder.GetUIObjectOf(
                        IntPtr.Zero,
                        (uint)listeFichiersDossiers.Count,
                        _listePointeursFichiersDossiers,
                        ref guid,
                        IntPtr.Zero,
                        out IntPtr pointeurData);
                    if (erreur == (int)WinAPI.Commun.HRESULT.S_OK)
                    {
                        Console.WriteLine($"DoDragDrop dans {repertoireCourant.FullName} parent de {listeFichiersDossiers[0]}");
                        WinAPI.Ole32.DoDragDrop(pointeurData, this, effetDragDrop, out DragDropEffects effets);
                    }
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
            SFGAO pdwAttributes = 0;
            oDesktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, _repStart.GetParent().FullName, ref pchEaten, out IntPtr pPIDL, ref pdwAttributes);
            Console.WriteLine("Parent défini sur " + _repStart.GetParent());
            oDesktopFolder.BindToObject(pPIDL, IntPtr.Zero, ref guid, out object pUnknownParentFolder);
            _shellFolder = (IShellFolder)pUnknownParentFolder;
        }

        private void LibererMemoire()
        {
            // TODO : Libérer les mémoires/pointeurs aliés
            _listeFichiersDossiers.Clear();
            if (_listePointeursFichiersDossiers != null)
                foreach (IntPtr pointeur in _listePointeursFichiersDossiers)
                    Marshal.FreeCoTaskMem(pointeur);
            _listePointeursFichiersDossiers = new IntPtr[0];
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

        public void DragDrop(object sender, DragEventArgs e)
        {
            // TODO : Menu contextuel shell drag'n drop

            GetParent();
            ShellItem item = new(_repStart.FullName);
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
                MK touches = MK.RBUTTON;
                if (e.KeyState.EtatBit(4))
                    touches |= MK.CONTROL;
                if (e.KeyState.EtatBit(6))
                    touches |= MK.ALT;
                DragDropEffects effets = effetDragDrop;
                WinAPI.Modeles.IDropTarget dropTarget = (WinAPI.Modeles.IDropTarget)Marshal.GetTypedObjectForIUnknown(dropTargetPtr, typeof(WinAPI.Modeles.IDropTarget));

                if (_listeFichiersDossiers == null || _listeFichiersDossiers.Count == 0)
                {
                    Console.WriteLine("Liste fichier/dossier vide");
                    return;
                }
                IntPtr pointeurData = GetIDataObject(new ShellItem[] { new ShellItem(_listeFichiersDossiers[0].FullName) });
                if (pointeurData != IntPtr.Zero)
                {
                    Console.WriteLine("Pointeur data sur " + _listeFichiersDossiers[0].FullName);
                    Console.WriteLine("OpenPopUp DragDrop");
                    erreur = dropTarget.DragDrop(pointeurData, touches, new POINT(Cursor.Position.X, Cursor.Position.Y), ref effets);
                    Console.WriteLine("Retour de la popUp : " + erreur.ToString());
                    ShellNewMenuCommand entreeMenu = new();
                    entreeMenu.AddSubMenu(new ShellFolder(_repStart.FullName, IntPtr.Zero), 0, ref _pidlParent);
                    ShellMenuCommandBuilder commande = new();
                    commande.AddCommand(entreeMenu);
                    ShellItemContextMenu monMenu = new(new ShellItem[] { item }, new ShellFolder(_repStart.FullName, IntPtr.Zero), IntPtr.Zero, null, true, commande, commande);
                }
                else
                    Console.WriteLine("GetIDataObject retourne null");
            }
            else
                throw new Exceptions.ExploripException($"DropHandler, err {erreur}={BetterWin32Errors.ErreurWindows.RetourneTexteErreur(erreur)}");
        }

        #region Interface IDropSource

        int IDropSource.QueryContinueDrag(bool fEscapePressed, MK grfKeyState)
        {
            if (fEscapePressed)
            {
                _dragDropEnCours = false;
                return WinAPI.Commun.DRAGDROP_S_CANCEL;
            }
            else
            {
                if ((_startButton & MouseButtons.Left) != 0 && (grfKeyState & MK.LBUTTON) == 0)
                    return Commun.DRAGDROP_S_DROP;
                else if ((_startButton & MouseButtons.Right) != 0 && (grfKeyState & MK.RBUTTON) == 0)
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
    }
}
