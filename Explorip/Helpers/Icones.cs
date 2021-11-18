using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

using System;
using System.Drawing;

namespace Explorip.Helpers
{
    public class Icones
    {
        private static Icon GetFileIcon(string name, bool linkOverlay, bool othersOverlay, Shell32.SHIL taille)
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

            Bitmap bitmap;

            // Recup l'icone
            ppv.GetIcon(shfi.iIcon & 0xFFFFFF, (int)ILD.TRANSPARENT, ref hIcon);
            Image icone = Icon.FromHandle(hIcon).ToBitmap();
            bitmap = new Bitmap(icone.Width, icone.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawImage(icone, new Point(0, 0));

            // Recup l'overlay
            int overlayIndex = 0;
            if (ppv.GetOverlayImage(shfi.iIcon >> 24, ref overlayIndex) == (int)Commun.HRESULT.S_OK)
            {
                ppv.GetIcon(overlayIndex, (int)ILD.TRANSPARENT, ref hIcon);
                Image overlay = Icon.FromHandle(hIcon).ToBitmap();
                graphics.DrawImage(overlay, new Point(0, 0));
            }

            // Retourne l'icone construite
            graphics.Save();

            return Icon.FromHandle(bitmap.GetHicon());
        }

        public static Bitmap GetIcone(string name, bool linkOverlay, bool othersOverlay, bool petiteIcone)
        {
            Icon icone = GetFileIcon(name, linkOverlay, othersOverlay, petiteIcone ? Shell32.SHIL.SMALL : Shell32.SHIL.LARGE);
            Bitmap image = icone.ToBitmap();
            icone.Dispose();
            image.MakeTransparent();
            return image;
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
