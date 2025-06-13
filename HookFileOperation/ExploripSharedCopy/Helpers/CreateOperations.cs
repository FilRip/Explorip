using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

using ExploripSharedCopy.Controls;

namespace ExploripSharedCopy.Helpers;

public static class CreateOperations
{
    public static void CreateFolder(string path, string name, Action<InputBoxWindow> actionOnWindow)
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            InputBoxWindow input = new()
            {
                CheckValidPathName = true,
                Question = path,
                UserEdit = name,
            };
            actionOnWindow?.Invoke(input);
            if (input.ShowDialog() == true)
            {
                Directory.CreateDirectory(Path.Combine(path, input.UserEdit));
            }
        });
    }

    public static void CreateShortcut(string path, string newName)
    {
        int iteration = 1;
        string name = (string.IsNullOrWhiteSpace(newName) ? Constants.Localization.NEW_SHORTCUT_NAME + ".lnk" : newName);
        while (File.Exists(Path.Combine(path, name)))
        {
            iteration++;
            name = Constants.Localization.NEW_SHORTCUT_NAME + $" ({iteration}).lnk";
        }
        File.CreateText(Path.Combine(path, name)).Close();
        Process.Start("rundll32.exe", $"appwiz.cpl,NewLinkHere {Path.Combine(path, name)}");
    }
}
