using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

using System.Drawing;

namespace Explorip.Helpers
{
    internal class Icones
    {
        /// <summary>
        /// Gets the Shell Icon for the given file...
        /// </summary>
        /// <param name="name">Path to file.</param>
        /// <param name="linkOverlay">Link Overlay</param>
        /// <returns>Icon</returns>
        public static Icon GetFileIcon(string name, bool linkOverlay, bool repertoire, bool othersOverlay, bool petiteIcone)
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
    }
}
