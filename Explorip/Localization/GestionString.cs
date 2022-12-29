using System;
using System.Text;

using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

namespace Explorip.Localization
{
    public static class GestionString
    {
        /// <summary>
        ///   Searches for a text resource in a Windows library.
        ///   Sometimes, using the existing Windows resources, you can
        ///   make your code language independent and you don't have to
        ///   care about translation problems.
        /// </summary>
        /// <example>
        ///   btnCancel.Text = GestionString.Load("user32.dll", 801, "Cancel");
        ///   btnYes.Text = GestionString.Load("user32.dll", 805, "Yes");
        /// </example>
        /// <param name="libraryName">Name of the windows library, with extension ; like user32.dll or shell32.dll</param>
        /// <param name="Ident">Id of the string resource.</param>
        /// <param name="DefaultText">Return this text, if the resource string could not be found.</param>
        /// <returns>Desired string if the resource was found, otherwise the DefaultText</returns>
        public static string Load(string libraryName, uint Ident, string DefaultText)
        {
            IntPtr libraryHandle = Kernel32.GetModuleHandle(libraryName);
            if (libraryHandle != IntPtr.Zero)
            {
                StringBuilder sb = new(1024);
                int size = User32.LoadString(libraryHandle, Ident, sb, 1024);
                if (size > 0)
                    return sb.ToString();
                else
                    return DefaultText;
            }
            else
            {
                return DefaultText;
            }
        }

        /// <summary>
        /// Return the localized name of special folder (windows) like in C:\User<br/>
        /// For example : downloads, my documents, ...
        /// </summary>
        /// <param name="specialFolder">The special folder what you want to localize</param>
        public static string NomTraduit(this Environment.SpecialFolder specialFolder)
        {
            string chemin = Environment.GetFolderPath(specialFolder);
            try
            {
                if (specialFolder == Environment.SpecialFolder.MyComputer)
                {
                    IntPtr Pidl = IntPtr.Zero;
                    Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, Shell32.CSIDL.DRIVES, ref Pidl);
                    SHFILEINFO info = new();
                    if (Shell32.SHGetFileInfo(Pidl, Shell32.FILE_ATTRIBUTE.NULL, ref info, (uint)System.Runtime.InteropServices.Marshal.SizeOf(info), Shell32.SHGFI.TYPENAME | Shell32.SHGFI.PIDL | Shell32.SHGFI.DISPLAYNAME) != IntPtr.Zero)
                    {
                        return info.szDisplayName;
                    }
                    else
                        throw new Exceptions.ExploripException();
                }
                else
                {
                    SHFILEINFO info = new();
                    if (Shell32.SHGetFileInfo(chemin, Shell32.FILE_ATTRIBUTE.NORMAL, ref info, (uint)System.Runtime.InteropServices.Marshal.SizeOf(info), Shell32.SHGFI.DISPLAYNAME) != IntPtr.Zero)
                    {
                        return info.szDisplayName;
                    }
                    else
                        throw new Exceptions.ExploripException();
                }
            }
            catch (Exception)
            {
                return System.IO.Path.GetFileName(chemin);
            }
        }
    }
}
