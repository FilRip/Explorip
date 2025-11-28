using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

using Explorip.HookFileOperations.FilesOperations;

namespace Explorip.HookFileOperations.Models;

[Serializable()]
public class OneFileOperation : INotifyPropertyChanged
{
    private string _display;

    public event PropertyChangedEventHandler PropertyChanged;

    public OneFileOperation() { }

    public OneFileOperation(EFileOperation operation) : this()
    {
        FileOperation = operation;
    }

    public EFileOperation FileOperation { get; set; }

    public string Source { get; set; }

    public string DisplaySource
    {
        get { return string.IsNullOrWhiteSpace(_display) ? Source : _display; }
    }

    public void SetDisplaySource(string display)
    {
        _display = display;
        OnPropertyChanged(nameof(DisplaySource));
    }

    public string Destination { get; set; }

    public string NewName { get; set; }

    public FileAttributes Attributes { get; set; }

    public object Properties { get; set; }

    public FilesOperations.EFileOperation OperationsFlags { get; set; } = FilesOperations.EFileOperation.None;

    public FilesOperations.Interfaces.IFileOperationProgressSink Callback { get; set; }

    public object ProgressDialog { get; set; }

    public string ProgressMessage { get; set; }

    public uint Cookie { get; set; }

    public bool ForceDeleteNoRecycled { get; set; }

    public bool ResetChoice { get; set; }

    public long CurrentOffset { get; set; }

    public void WriteOperation(FileOperation currentFileOperation)
    {
        switch (FileOperation)
        {
            case EFileOperation.Copy:
                currentFileOperation.CopyItem(Source, Destination, NewName);
                break;
            case EFileOperation.Move:
                currentFileOperation.MoveItem(Source, Destination, NewName);
                break;
            case EFileOperation.Delete:
                currentFileOperation.DeleteItem(Source);
                break;
            case EFileOperation.Rename:
                currentFileOperation.RenameItem(Source, NewName);
                break;
            case EFileOperation.Create:
                currentFileOperation.NewItem(Destination, NewName, Attributes);
                break;
            case EFileOperation.ApplyProperties:
                currentFileOperation.ApplyPropertiesToItem(Source);
                break;
            case EFileOperation.ChangeOperationFlags:
                currentFileOperation.ChangeOperationFlags(OperationsFlags);
                break;
            case EFileOperation.Advice:
                currentFileOperation.Advice(Callback);
                break;
            case EFileOperation.Unadvice:
                currentFileOperation.Unadvice(Cookie);
                break;
            case EFileOperation.SetProperties:
                currentFileOperation.SetProperties(Properties);
                break;
            case EFileOperation.ProgressDialog:
                currentFileOperation.SetProgressDialog(ProgressDialog);
                break;
            case EFileOperation.ProgressMessage:
                currentFileOperation.SetProgressMessage(ProgressMessage);
                break;
            default:
#pragma warning disable IDE0079
#pragma warning disable S112 // General exceptions should never be thrown
                throw new Exception("Unknown IFileOperation command");
#pragma warning restore S112 // General exceptions should never be thrown
#pragma warning restore IDE0079
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
