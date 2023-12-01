using System.Collections.Generic;

using ManagedShell.ShellFolders.Enums;

namespace ManagedShell.ShellFolders
{
    public class ShellMenuCommandBuilder
    {
        internal readonly List<ShellMenuCommand> Commands = [];
        public uint DefaultItemUID { get; set; }

        public void AddCommand(ShellMenuCommand command)
        {
            Commands.Add(command);
        }

        public void AddSeparator()
        {
            Commands.Add(new ShellMenuCommand { Flags = MFT.SEPARATOR, Label = string.Empty, UID = 0 });
        }

        public void AddShellNewMenu()
        {
            Commands.Add(new ShellNewMenuCommand());
        }
    }
}
