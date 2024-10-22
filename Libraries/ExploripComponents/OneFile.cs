using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ExploripComponents;

public partial class OneFile(string fullPath, OneDirectory parentDirectory) : OneFileSystem(fullPath, Path.GetFileName(fullPath), parentDirectory)
{
    public override void DoubleClickFile()
    {
        Process process = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = FullPath,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(FullPath),
            },
        };
        Task.Run(process.Start);
    }

    public override void Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string file = ((DataObject)e.Data).GetFileDropList()[0];
            if (file != FullPath)
            {
                Process process = new()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = FullPath,
                        UseShellExecute = true,
                        WorkingDirectory = Path.GetDirectoryName(FullPath),
                        Arguments = file,
                    },
                };
                Task.Run(process.Start);
            }
        }
        base.Drop(sender, e);
    }
}
