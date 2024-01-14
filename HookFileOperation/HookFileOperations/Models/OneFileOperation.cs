using System;
using System.IO;

namespace Explorip.HookFileOperations.Models;

[Serializable()]
public class OneFileOperation
{
    public OneFileOperation() { }

    public OneFileOperation(EFileOperation operation) : this()
    {
        FileOperation = operation;
    }

    public EFileOperation FileOperation { get; set; }

    public string Source { get; set; }

    public string Destination { get; set; }

    public string NewName { get; set; }

    public FileAttributes Attributes { get; set; }

    public object Properties { get; set; }

    public FilesOperations.Interfaces.EFileOperation OperationsFlags { get; set; } = FilesOperations.Interfaces.EFileOperation.None;

    public FilesOperations.Interfaces.IFileOperationProgressSink Callback { get; set; }

    public object ProgressDialog { get; set; }

    public string ProgressMessage { get; set; }

    public uint Cookie { get; set; }

    public bool ForceDeleteNoRecycled { get; set; }

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
#pragma warning disable S112 // General exceptions should never be thrown
                throw new Exception("Unknown IFileOperation command");
#pragma warning restore S112 // General exceptions should never be thrown
        }
    }
}
