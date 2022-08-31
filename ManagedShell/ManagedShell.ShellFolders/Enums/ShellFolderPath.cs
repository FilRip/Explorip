﻿namespace ManagedShell.ShellFolders.Enums
{
    public class ShellFolderPath
    {
        private ShellFolderPath(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public static ShellFolderPath ComputerFolder => new("::{20D04FE0-3AEA-1069-A2D8-08002B30309D}");
        public static ShellFolderPath ControlPanelFolder => new("::{21EC2020-3AEA-1069-A2DD-08002B30309D}");
        public static ShellFolderPath RecycleBinFolder => new("::{645FF040-5081-101B-9F08-00AA002F954E}");
    }
}
