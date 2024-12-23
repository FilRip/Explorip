using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using Securify.ShellLink;

namespace ExploripComponents.Models;

public partial class OneFile : OneFileSystem
{
    private readonly bool _isLink;
    private readonly DateTime? _dtDeleted;

    public OneFile(string fullPath, OneDirectory parentDirectory) : base(fullPath, Path.GetFileName(fullPath), parentDirectory)
    {
        string filename = Path.GetFileName(fullPath);
        _isLink = Path.GetExtension(filename) == ".lnk";
        DisplayText = _isLink ? filename.Substring(0, filename.Length - 4) : filename;
    }

    public OneFile(string fullPath, DateTime DtDeleted, DateTime DtLastWrite, ulong size, OneDirectory parentDirectory) : base(fullPath, Path.GetFileName(fullPath), parentDirectory)
    {
        _fromRecycledBin = true;
        _lastSize = size;
        _lastWriteTime = DtLastWrite;
        _dtDeleted = DtDeleted;
    }

    public OneFile(string display, OneDirectory networkRoot, IntPtr pidl, IntPtr relativePidl) : this(display, networkRoot)
    {
        _isNetworkResource = true;
        _pidl = pidl;
        _pidlRelative = relativePidl;
    }

    public DateTime? DtDeleted
    {
        get { return _dtDeleted; }
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

    public override void EditMode(bool activate)
    {
        if (ParentDirectory == null || _fromRecycledBin)
            return;

        if (activate)
        {
            if (RenameMode)
                return;
            NewName = Path.GetFileName(FullPath);
            if (_isLink)
                NewName = NewName.Substring(0, NewName.Length - 4);
            RenameMode = true;
            ParentDirectory.GetRootParent().MainViewModel!.SetCurrentlyRenaming(this);
        }
        else
        {
            ParentDirectory.GetRootParent().MainViewModel!.SetCurrentlyRenaming(null);
            RenameMode = false;
        }
    }

    public override void Rename()
    {
        string file = NewName;
        if (_isLink)
            file += ".lnk";
        if (file != Path.GetFileName(FullPath))
            File.Move(FullPath, Path.Combine(Path.GetDirectoryName(FullPath), file));
        base.Rename();
    }
}
