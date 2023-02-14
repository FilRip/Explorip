using System;
using System.IO;
using System.Windows.Forms;

namespace Explorip.FilesOperations
{
    public static class ContextMenuPaste
    {
#pragma warning disable S1104, S2223 // Fields should not have public accessibility
        public static string RepDestination;
#pragma warning restore S1104, S2223 // Fields should not have public accessibility

        public static void MenuColler(object sender, EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (!data.GetDataPresent(DataFormats.FileDrop))
                return;
            string[] files = (string[])
              data.GetData(DataFormats.FileDrop);
            MemoryStream stream = (MemoryStream)
              data.GetData("Preferred DropEffect", true);
            int flag = stream.ReadByte();
            if (flag != 2 && flag != 5)
                return;
            bool cut = (flag == 2);
            FileOperation fileOperation = new();
            // TODO : Coller des raccourcis
            // Voir ShellLink dans github
            foreach (string file in files)
            {
                try
                {
                    if (cut)
                        fileOperation.MoveItem(file, RepDestination, Path.GetFileName(file));
                    else
                        fileOperation.CopyItem(file, RepDestination, Path.GetFileName(file));
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Failed to perform the" +
                        " specified operation:\n\n" + ex.Message,
                        "File operation failed");
                }
            }
            fileOperation.PerformOperations();
        }

        public static bool CollerDispo()
        {
            IDataObject data = Clipboard.GetDataObject();
            return data.GetDataPresent(DataFormats.FileDrop);
        }
    }
}
