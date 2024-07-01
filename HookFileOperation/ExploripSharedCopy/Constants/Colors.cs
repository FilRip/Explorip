using System.Windows.Media;

using ExploripSharedCopy.Helpers;

namespace ExploripSharedCopy.Constants;

public static class Colors
{
    public static Color AccentColor { get; private set; }
    public static Color BackgroundColor { get; private set; }
    public static Color ForegroundColor { get; private set; }
    public static SolidColorBrush AccentColorBrush { get; private set; }
    public static SolidColorBrush BackgroundColorBrush { get; private set; }
    public static SolidColorBrush ForegroundColorBrush { get; private set; }
    public static SolidColorBrush TransparentColorBrush { get; private set; }
    public static SolidColorBrush SelectedBackgroundShellObject { get; private set; }

    public static void LoadTheme()
    {
        AccentColor = WindowsSettings.GetWindowsAccentColor();
        AccentColorBrush = new SolidColorBrush(Color.FromArgb(255, AccentColor.R, AccentColor.G, AccentColor.B));
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            BackgroundColor = Color.FromArgb(255, 0, 0, 0);
            ForegroundColor = Color.FromArgb(255, 255, 255, 255);
        }
        else
        {
            BackgroundColor = Color.FromArgb(255, 255, 255, 255);
            ForegroundColor = Color.FromArgb(255, 0, 0, 0);
        }
        BackgroundColorBrush = new SolidColorBrush(Color.FromArgb(255, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
        ForegroundColorBrush = new SolidColorBrush(Color.FromArgb(255, ForegroundColor.R, ForegroundColor.G, ForegroundColor.B));
        TransparentColorBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        SelectedBackgroundShellObject = new SolidColorBrush(Color.FromArgb(128, System.Drawing.Color.DarkGray.R, System.Drawing.Color.DarkGray.G, System.Drawing.Color.DarkGray.B));
    }
}
