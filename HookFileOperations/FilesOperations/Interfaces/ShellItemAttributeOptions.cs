namespace Explorip.HookFileOperations.FilesOperations.Interfaces
{
    public enum ShellItemAttributeOptions
    {
        // if multiple items and the attirbutes together.
        And = 0x00000001,
        // if multiple items or the attributes together.
        Or = 0x00000002,
        // Call GetAttributes directly on the 
        // ShellFolder for multiple attributes.
        AppCompat = 0x00000003,

        // A mask for SIATTRIBFLAGS_AND, SIATTRIBFLAGS_OR, and SIATTRIBFLAGS_APPCOMPAT. Callers normally do not use this value.
        Mask = 0x00000003,

        // Windows 7 and later. Examine all items in the array to compute the attributes. 
        // Note that this can result in poor performance over large arrays and therefore it 
        // should be used only when needed. Cases in which you pass this flag should be extremely rare.
        AllItems = 0x00004000
    }
}
