using Filexplorip.WinAPI;
using System.Drawing;

namespace Filexplorip.Helpers
{
    internal class Icones
    {
        /// <summary>
        /// Gets the Shell Icon for the given file...
        /// </summary>
        /// <param name="name">Path to file.</param>
        /// <param name="linkOverlay">Link Overlay</param>
        /// <returns>Icon</returns>
        public static Icon GetFileIcon(string name, bool linkOverlay, bool repertoire, bool othersOverlay)
        {
            Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
            Shell32.SHGFI flags = Shell32.SHGFI.ICON | Shell32.SHGFI.USEFILEATTRIBUTES;

            if (linkOverlay) flags |= Shell32.SHGFI.LINKOVERLAY;
            flags |= Shell32.SHGFI.SMALLICON;
            if (othersOverlay)
                flags |= Shell32.SHGFI.ADDOVERLAYS;

            uint attribut = Shell32.FILE_ATTRIBUTE_NORMAL;
            if (repertoire)
                attribut = Shell32.FILE_ATTRIBUTE_DIRECTORY;

            Shell32.SHGetFileInfo(name,
                attribut,
                ref shfi,
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                flags);


            // Copy (clone) the returned icon to a new object, thus allowing us 
            // to call DestroyIcon immediately
            Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            WinAPI.User32.DestroyIcon(shfi.hIcon); // Cleanup
            return icon;
        }
    }
}
