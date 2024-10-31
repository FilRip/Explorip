using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using Securify.ShellLink;

namespace ExploripComponents;

public partial class OneFile : OneFileSystem
{
    private readonly bool _isLink;

    public OneFile(string fullPath, OneDirectory parentDirectory) : base(fullPath, Path.GetFileName(fullPath), parentDirectory)
    {
        string filename = Path.GetFileName(fullPath);
        _isLink = Path.GetExtension(filename) == ".lnk";
        DisplayText = _isLink ? filename.Substring(0, filename.Length - 4) : filename;
    }

    public override void DoubleClickFile()
    {
        if (_isLink)
        {
            Shortcut shortcut = Shortcut.ReadFromFile(FullPath);
            if (Directory.Exists(shortcut.Target))
                ParentDirectory!.GetRootParent().MainViewModel!.BrowseTo(shortcut.Target);
            return;
        }

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
        if (e.Data.GetDataPresent(DataFormats.FileDrop) && ParentDirectory!.GetRootParent().MainViewModel!.NbMillisecondsStartDragging > Constants.DelayIgnoreDrag)
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

    public override void Rename()
    {
        File.Move(FullPath, Path.Combine(Path.GetDirectoryName(FullPath), NewName));
        base.Rename();
    }
}
