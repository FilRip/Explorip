using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Explorip.Helpers
{
    public class Icones
    {
        private static Icon GetFileIcon(string name, bool linkOverlay, bool repertoire, bool othersOverlay, bool petiteIcone)
        {
            SHFILEINFO shfi = new SHFILEINFO();
            Shell32.SHGFI flags = Shell32.SHGFI.ICON/* | Shell32.SHGFI.USEFILEATTRIBUTES*/;

            if (linkOverlay) flags |= Shell32.SHGFI.LINKOVERLAY;

            if (petiteIcone)
                flags |= Shell32.SHGFI.SMALLICON;
            else
                flags |= Shell32.SHGFI.LARGEICON;

            if (othersOverlay)
                flags |= Shell32.SHGFI.ADDOVERLAYS;

            Shell32.FILE_ATTRIBUTE attribut = Shell32.FILE_ATTRIBUTE.NORMAL;
            if (repertoire)
                attribut = Shell32.FILE_ATTRIBUTE.DIRECTORY;

            Shell32.SHGetFileInfo(name,
                attribut,
                ref shfi,
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                flags);
            
            Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            User32.DestroyIcon(shfi.hIcon); // Cleanup
            return icon;
        }

        private static BitmapSource GetBitmapIcon(string name, bool linkOverlay, bool repertoire, bool othersOverlay, bool petiteIcone)
        {
            SHFILEINFO shfi = new SHFILEINFO();
            Shell32.SHGFI flags = Shell32.SHGFI.ICON/* | Shell32.SHGFI.USEFILEATTRIBUTES*/;

            if (linkOverlay) flags |= Shell32.SHGFI.LINKOVERLAY;

            if (petiteIcone)
                flags |= Shell32.SHGFI.SMALLICON;
            else
                flags |= Shell32.SHGFI.LARGEICON;

            if (othersOverlay)
                flags |= Shell32.SHGFI.ADDOVERLAYS;

            Shell32.FILE_ATTRIBUTE attribut = Shell32.FILE_ATTRIBUTE.NORMAL;
            if (repertoire)
                attribut = Shell32.FILE_ATTRIBUTE.DIRECTORY;

            Shell32.SHGetFileInfo(name,
                attribut,
                ref shfi,
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                flags);

            return Imaging.CreateBitmapSourceFromHIcon(shfi.hIcon, new Int32Rect(0, 0, (petiteIcone ? 16 : 32), (petiteIcone ? 16 : 32)), BitmapSizeOptions.FromEmptyOptions());
        }

        public static Bitmap GetBitmapIcone(string name, bool linkOverlay, bool repertoire, bool othersOverlay, bool petiteIcone)
        {
            BitmapSource source = GetBitmapIcon(name, linkOverlay, repertoire, othersOverlay, petiteIcone);

            Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight, PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            bmp.MakeTransparent();
            return bmp;
        }

        public static Bitmap GetIcone(string name, bool linkOverlay, bool repertoire, bool othersOverlay, bool petiteIcone)
        {
            Icon icone = GetFileIcon(name, linkOverlay, repertoire, othersOverlay, petiteIcone);
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

        // TODO : Recupérer les icones des autres tailles
        // voir : https://stackoverflow.com/questions/28012229/what-is-the-maximum-size-of-an-icon-returned-from-shgetfileinfo
    }
}
