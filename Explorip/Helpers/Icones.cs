using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

using System;
using System.Drawing;

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

        public static Bitmap GetIcone(string name, bool linkOverlay, bool repertoire, bool othersOverlay, bool petiteIcone)
        {
            Icon icone = GetFileIcon(name, linkOverlay, repertoire, othersOverlay, petiteIcone);
            Bitmap image = icone.ToBitmap();
            icone.Dispose();
            image.MakeTransparent(Color.Black);
            return image;
        }

        private static Icon GetFileIcon(IntPtr pidl)
        {
            SHFILEINFO shfi = new SHFILEINFO();

            Shell32.SHGetFileInfo(pidl,
                0,
                ref shfi,
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                (Shell32.SHGFI.PIDL | Shell32.SHGFI.DISPLAYNAME | Shell32.SHGFI.ICON | Shell32.SHGFI.SMALLICON));

            Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            User32.DestroyIcon(shfi.hIcon); // Cleanup
            return icon;
        }

        public static Bitmap GetIcone(IntPtr pidl)
        {
            Icon icone = GetFileIcon(pidl);
            Bitmap image = icone.ToBitmap();
            icone.Dispose();
            image.MakeTransparent(Color.Black);
            return image;
        }

        // TODO : Recupérer les icones des autres tailles
        // voir : https://stackoverflow.com/questions/28012229/what-is-the-maximum-size-of-an-icon-returned-from-shgetfileinfo
    }
}
