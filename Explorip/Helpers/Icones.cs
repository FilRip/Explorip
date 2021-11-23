using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

using System;
using System.Drawing;

namespace Explorip.Helpers
{
    public class Icones
    {
        private static readonly object _lockGetIcone = new object();

        public static Icon GetFileIcon(string name, bool linkOverlay, bool othersOverlay, Shell32.SHIL taille)
        {
            Icon retour = null;
            lock (_lockGetIcone)
            {
                Bitmap bitmap = null;
                try
                {
                    SHFILEINFO shfi = new SHFILEINFO();
                    Shell32.SHGFI flags = Shell32.SHGFI.SYSICONINDEX | Shell32.SHGFI.ICON;

                    if ((linkOverlay) || (othersOverlay))
                    {
                        flags |= Shell32.SHGFI.OVERLAYINDEX;
                    }

                    Shell32.FILE_ATTRIBUTE attribut = Shell32.FILE_ATTRIBUTE.NULL;

                    Shell32.SHGetFileInfo(name,
                        attribut,
                        ref shfi,
                        (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                        flags);

                    Guid Id = typeof(IImageList).GUID;
                    IntPtr hIcon = IntPtr.Zero;
                    Shell32.SHGetImageList(taille, ref Id, out IImageList ppv);

                    // Recup l'icone
                    if (ppv.GetIcon(shfi.iIcon & 0xFFFFFF, (int)ILD.TRANSPARENT, ref hIcon) == (int)Commun.HRESULT.S_OK)
                    {
                        Image icone = Icon.FromHandle(hIcon).ToBitmap();
                        bitmap = new Bitmap(icone.Width, icone.Height);
                        Graphics graphics = Graphics.FromImage(bitmap);
                        graphics.DrawImage(icone, new Point(0, 0));
                        User32.DestroyIcon(hIcon);

                        // Recup l'overlay
                        int overlayIndex = 0;
                        if (ppv.GetOverlayImage(shfi.iIcon >> 24, ref overlayIndex) == (int)Commun.HRESULT.S_OK)
                        {
                            ppv.GetIcon(overlayIndex, (int)ILD.TRANSPARENT, ref hIcon);
                            Image overlay = Icon.FromHandle(hIcon).ToBitmap();
                            graphics.DrawImage(overlay, new Point(0, 0));
                            User32.DestroyIcon(hIcon);
                        }

                        // Retourne l'icone construite
                        graphics.Save();
                    }
                    if (bitmap != null)
                    {
                        retour = Icon.FromHandle(bitmap.GetHicon());
                        Gdi32.DeleteObject(bitmap.GetHbitmap());
                        bitmap.Dispose();
                    }
                }
                catch (Exception) { }
            }
            if (retour == null)
            {
                // TODO : Retourner une icone basique
                Console.WriteLine("Pas d'icone pour " + name);
            }
            return retour;
        }

        private static Icon GetFileIcon(IntPtr pidl, bool petiteIcone)
        {
            SHFILEINFO shfi = new SHFILEINFO();

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
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
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
