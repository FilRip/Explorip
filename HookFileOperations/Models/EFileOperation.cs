using System;

namespace Explorip.HookFileOperations.Models
{
    [Serializable()]
    public enum EFileOperation
    {
        None = 0,
        Copy = 1,
        Move = 2,
        Delete = 3,
        Rename = 4,
        Create = 5,
        ApplyProperties = 6,
        SetProperties = 7,
        Advice = 8,
        Unadvice = 9,
        ProgressDialog = 10,
        ChangeOperationFlags = 11,
        ProgressMessage = 12,
    }
}
