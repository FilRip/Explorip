﻿using ManagedShell.ShellFolders.Enums;

namespace ManagedShell.ShellFolders
{
    public class ShellMenuCommand
    {
        public MFT Flags { get; set; }
        public uint UID { get; set; }
        public string Label { get; set; }
        public bool FoldersOnly { get; set; }
    }
}
