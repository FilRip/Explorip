using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ExploripShellExt
{
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("89E45E24-C007-418F-9AB8-576BE44D7EC6")]
    [ComVisible(true)]
    public class ExploripCopyHook : ICopyHookW
    {
        public uint CopyCallback(IntPtr hwnd, FILEOP fileOperation, uint flags, [MarshalAs(UnmanagedType.LPWStr)] string srcFile, uint srcAttribs, [MarshalAs(UnmanagedType.LPWStr)] string destFile, uint destAttribs)
        {
            NativeWindow owner = new();
            owner.AssignHandle(hwnd);
            try
            {
                DialogResult result = DialogResult.Yes;

                /*if (Process.GetProcessesByName("Explorip")?.Length > 0)
                {
                    return (uint)DialogResult.No;
                }
                else
                    return (uint)DialogResult.Yes;*/

                // If the file name contains "Test" and it is being renamed...
                if (srcFile.Contains("Test") && fileOperation == FILEOP.Rename)
                {
                    result = MessageBox.Show(owner,
                        String.Format("Are you sure to rename the folder {0} as {1} ?",
                        srcFile, destFile), "CSShellExtCopyHookHandler",
                        MessageBoxButtons.YesNoCancel);
                }

                Debug.Assert(result == DialogResult.Yes || result == DialogResult.No ||
                    result == DialogResult.Cancel);
                return (uint)result;
            }
            finally
            {
                owner.ReleaseHandle();
            }
        }

        #region Shell Extension Registration

        [ComRegisterFunction()]
        public static void Register(Type t)
        {
            try
            {
                ShellExtReg.RegisterShellExtCopyHookHandler(
                    "ExploripShellExt", t.GUID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Log the error
                throw;  // Re-throw the exception
            }
        }

        [ComUnregisterFunction()]
        public static void Unregister(Type t)
        {
            try
            {
                ShellExtReg.UnregisterShellExtCopyHookHandler(
                    "ExploripShellExt");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Log the error
                throw;  // Re-throw the exception
            }
        }

        #endregion
    }
}
