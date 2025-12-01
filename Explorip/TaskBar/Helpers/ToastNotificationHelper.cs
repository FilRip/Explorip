using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Media;

using ManagedShell.Common.Logging;

using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace Explorip.TaskBar.Helpers;

public static class ToastHelper
{
    private const string MyAppUserModelId = "CoolBytes.Explorip";

    /// <summary>
    /// Send new toast notifications windows
    /// </summary>
    public static void Show(string title, string message, ImageSource icon)
    {
        if (string.IsNullOrWhiteSpace(title))
            title = MyAppUserModelId;

        Thread thread = new(() =>
        {
            XmlDocument template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText01);
            //((XmlElement)template.GetElementsByTagName("image")[0]).SetAttribute("src", icon.);
            template.GetElementsByTagName("text")[0].AppendChild(template.CreateTextNode(message));
            ToastNotification toast = new(template);
            toast.Activated += Toast_Activated;
            ToastNotificationManager.CreateToastNotifier(title).Show(toast);
        });
        thread.Start();
    }

    private static void Toast_Activated(ToastNotification sender, object args)
    {
        ShellLogger.Debug($"Click on Toast Notification {sender}");
    }
}
