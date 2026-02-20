using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Media;

using Explorip.Helpers;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Explorip.TaskBar.Helpers;

public static class ToastHelper
{
    private const string MyAppUserModelId = "Explorip";

    /// <summary>
    /// Send new toast notifications windows
    /// </summary>
    public static bool Show(string appUserModelId, string title, string message, int processId)
    {
        if (string.IsNullOrWhiteSpace(appUserModelId))
            appUserModelId = MyAppUserModelId;
        bool handled = false;
        if (processId <= 0)
            processId = Process.GetCurrentProcess().Id;
        string filepath = Process.GetProcessById(processId).MainModule.FileName;
        IntPtr hicon = IconHelper.GetIconByFilename(filepath, ManagedShell.Common.Enums.IconSize.ExtraLarge, out _);
        ImageSource icon = IconManager.Convert(hicon);
        string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");
        icon?.SaveImageSource(tempFile, 32, 32);
        Uri uriIcon = new(tempFile);

        Thread thread = new(() =>
        {
            try
            {
                // https://learn.microsoft.com/en-us/windows/apps/develop/notifications/app-notifications/
                XmlDocument template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText01);
                XmlElement imageTag = ((XmlElement)template.GetElementsByTagName("image")[0]);
                imageTag.SetAttribute("src", uriIcon.ToString());
                template.GetElementsByTagName("text")[0].AppendChild(template.CreateTextNode(message));
                ToastNotification toast = new(template);
                toast.Activated += Toast_Activated;
                toast.Dismissed += Toast_Dismissed;
                ToastNotificationManager.CreateToastNotifier(appUserModelId).Show(toast);
                handled = true;
            }
#pragma warning disable IDE0079
#pragma warning disable S2486
            catch (Exception)
            {
#if DEBUG
                if (Debugger.IsAttached)
                    Debugger.Break();
#endif
            }
#pragma warning restore S2486
#pragma warning restore IDE0079
        });
        thread.Start();
        thread.Join();
        return handled;
    }

    private static void Toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
    {
        ShellLogger.Debug($"Dismiss Toast Notification {sender}");
    }

    private static void Toast_Activated(ToastNotification sender, object args)
    {
        ShellLogger.Debug($"Activated Toast Notification {sender}");
    }
}
