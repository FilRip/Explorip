using System;
using System.IO;
using System.Windows;

using ExploripSharedCopy.Controls;

using Securify.ShellLink;

namespace ExploripSharedCopy.Helpers
{
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

#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
        public static void CreateShortcut(string path, string name, Action<CreateShortcutWindow> actionOnWindow)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                CreateShortcutWindow win = new();
                actionOnWindow?.Invoke(win);
                if (win.ShowDialog() == true)
                {
                    Shortcut sc = Shortcut.CreateShortcut(win.Target);
                    sc.WriteToFile(Path.Combine(path, win.ShortcutName + ".lnk"));
                }
            });
        }
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé
    }
}
