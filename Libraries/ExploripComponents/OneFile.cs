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
        base.Drop(sender, e);
    }
}
