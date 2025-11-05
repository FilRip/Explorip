using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

using ManagedShell.Common.Logging;

namespace Explorip.Helpers;

public static class DragGhostAdorner
{
    private static MyAdornerWindow _ghostAdorner;

    public static void StartDragGhost(UIElement source, MouseEventArgs e)
    {
        if (source == null)
            return;
        Point offset = e.GetPosition(source);
        _ghostAdorner = new();
        _ghostAdorner.SetImage(ExtensionsWpf.CreateImageFromWpfControl(source), offset, null, 0.75);
        _ghostAdorner.Show();
        ShellLogger.Debug("Start Ghost Adorner");
    }

    public static void UpdateDragGhost()
    {
        _ghostAdorner?.UpdatePosition();
    }

    public static void StopDragGhost()
    {
        if (_ghostAdorner != null)
        {
            ShellLogger.Debug("Stop Ghost Adorner");
            _ghostAdorner.Close();
            _ghostAdorner = null;
        }
    }
}
