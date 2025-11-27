using System.Threading;

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Explorip.TaskBar.Helpers;

public static class ToastHelper
{
    private const string AppUserModelId = "CoolBytes.Explorip";

    /// <summary>
    /// Send new toast notifications windows
    /// </summary>
    public static void Show(string title, string message)
    {
        Thread thread = new(() =>
        {
            string xml = $"<toast><visual><binding template=\"ToastText01\"><text id=\"1\">{message}</text></binding></visual></toast>";
            XmlDocument toastCompiled = new();
            toastCompiled.LoadXml(xml);
            ToastNotification toast = new(toastCompiled);
            ToastNotificationManager.CreateToastNotifier(AppUserModelId).Show(toast);
        });
        thread.Start();
    }
}
