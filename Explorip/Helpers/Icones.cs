using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

using Explorip.Exceptions;
using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

namespace Explorip.Helpers
{
    public static class Icones
    {
        private static Dictionary<Shell32.SHIL, IImageList> _listeInterfaceIcones;

        private static void InitialiseListeInterfaceIcones()
        {
            if (_listeInterfaceIcones != null)
                _listeInterfaceIcones.Clear();
            else
                _listeInterfaceIcones = new Dictionary<Shell32.SHIL, IImageList>();

            Guid Id = typeof(IImageList).GUID;
            Shell32.SHGetImageList(Shell32.SHIL.LARGE, ref Id, out IImageList large);
            _listeInterfaceIcones.Add(Shell32.SHIL.LARGE, large);
            Shell32.SHGetImageList(Shell32.SHIL.SMALL, ref Id, out IImageList small);
            _listeInterfaceIcones.Add(Shell32.SHIL.SMALL, small);
            Shell32.SHGetImageList(Shell32.SHIL.EXTRALARGE, ref Id, out IImageList extraLarge);
            _listeInterfaceIcones.Add(Shell32.SHIL.EXTRALARGE, extraLarge);
            Shell32.SHGetImageList(Shell32.SHIL.JUMBO, ref Id, out IImageList jumbo);
            _listeInterfaceIcones.Add(Shell32.SHIL.JUMBO, jumbo);
        }

        public static int GetNumIcon(string name, bool linkOverlay, bool othersOverlay, out IntPtr pidl)
        {
            pidl = IntPtr.Zero;
            try
            {
                ShFileInfo shfi = new();
                Shell32.SHGFI flags = Shell32.SHGFI.SYSICONINDEX | Shell32.SHGFI.ICON;

                if ((linkOverlay) || (othersOverlay))
                {
                    flags |= Shell32.SHGFI.OVERLAYINDEX;
                }

                Shell32.FILE_ATTRIBUTE attribut = Shell32.FILE_ATTRIBUTE.NULL;

                pidl = Shell32.SHGetFileInfo(name, attribut, ref shfi, (uint)Marshal.SizeOf(shfi), flags);

                if (shfi.hIcon != IntPtr.Zero)
                    User32.DestroyIcon(shfi.hIcon);

                return shfi.iIcon;
            }
            catch (Exception) { /* Ignore errors */ }
            return 0;
        }

        public static Bitmap GetFileIcon(IntPtr pidl, int numIcone, Shell32.SHIL taille)
        {
            if (_listeInterfaceIcones == null)
                InitialiseListeInterfaceIcones();

            Bitmap retour = null;
            try
            {
                if (pidl != IntPtr.Zero)
                {
                    int erreur = 0;
                    IntPtr hIcon = IntPtr.Zero;
                    IImageList ppv = _listeInterfaceIcones[taille];
                    if (ppv != null)
                    {
                        erreur = ppv.GetIcon(numIcone & 0xFFFFFF, (int)ILD.TRANSPARENT, ref hIcon);
                        // Recup l'icone
                        if (erreur == (int)Commun.HRESULT.S_OK)
                        {
                            Icon icone = Icon.FromHandle(hIcon);
                            retour = new Bitmap(icone.Width, icone.Height);
                            Graphics graphics = Graphics.FromImage(retour);
                            graphics.DrawIcon(icone, 0, 0);
                            User32.DestroyIcon(hIcon);
                            Gdi32.DeleteObject(icone.Handle);
                            icone.Dispose();

                            // Recup l'overlay
                            int overlayIndex = 0;
                            if (ppv.GetOverlayImage(numIcone >> 24, ref overlayIndex) == (int)Commun.HRESULT.S_OK)
                            {
                                erreur = ppv.GetIcon(overlayIndex, (int)ILD.TRANSPARENT, ref hIcon);
                                if (erreur == (int)Commun.HRESULT.S_OK)
                                {
                                    Icon overlay = Icon.FromHandle(hIcon);
                                    graphics.DrawIcon(overlay, 0, 0);
                                    User32.DestroyIcon(hIcon);
                                    Gdi32.DeleteObject(overlay.Handle);
                                    overlay.Dispose();
                                }
                            }

                            graphics.Save();
                            graphics.Dispose();
                        }

                        if (retour == null)
                        {
                            string message = "Erreur GetIcon num=" + erreur.ToString("X");
                            message += $", {BetterWin32Errors.ErreurWindows.RetourneTexteErreur(erreur)}";

                            throw new ExploripException(message);
                        }
                    }
                    else
                    {
                        string message = "Erreur SHGetImageList introuvable pour taille " + taille.ToString("G");
                        throw new ExploripException(message);
                    }
                }
                else
                    throw new ArgumentNullException(nameof(pidl));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            }

            if (retour == null)
            {
                // TODO : Retourner une icone par défaut
            }

            return retour;
        }

        private static Icon GetFileIcon(IntPtr pidl, bool petiteIcone)
        {
            ShFileInfo shfi = new();

            Shell32.SHGFI flag = Shell32.SHGFI.PIDL | Shell32.SHGFI.DISPLAYNAME | Shell32.SHGFI.ICON;

            if (petiteIcone)
            {
                flag |= Shell32.SHGFI.SMALLICON;
            }
            else
            {
                flag |= Shell32.SHGFI.LARGEICON;
            }

            Shell32.SHGetFileInfo(pidl,
                0,
                ref shfi,
                (uint)Marshal.SizeOf(shfi),
                flag);

            Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            User32.DestroyIcon(shfi.hIcon); // Cleanup
            return icon;
        }

        public static Bitmap GetIcone(IntPtr pidl, bool petiteIcone)
        {
            Icon icone = GetFileIcon(pidl, petiteIcone);
            Bitmap image = icone.ToBitmap();
            icone.Dispose();
            image.MakeTransparent(Color.Black);
            return image;
        }
    }
}
