using System;
using System.IO;

namespace Explorip.HookFileOperations.Models
{
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
            }
        }
    }
}
