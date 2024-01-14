using System.Windows.Media;

using ExploripSharedCopy.Helpers;

namespace Explorip.Constants;

public static class Colors
{
    public static System.Drawing.Color AccentColor { get; private set; }
    public static System.Drawing.Color BackgroundColor { get; private set; }
    public static System.Drawing.Color ForegroundColor { get; private set; }
    public static SolidColorBrush AccentColorBrush { get; private set; }
    public static SolidColorBrush BackgroundColorBrush { get; private set; }
    public static SolidColorBrush ForegroundColorBrush { get; private set; }

    public static void LoadTheme()
    {
        AccentColor = WindowsSettings.GetWindowsAccentColor();
        AccentColorBrush = new SolidColorBrush(Color.FromArgb(255, AccentColor.R, AccentColor.G, AccentColor.B));
        if (WindowsSettings.IsWindowsApplicationInDarkMode())
        {
            BackgroundColor = System.Drawing.Color.Black;
            ForegroundColor = System.Drawing.Color.White;
        }
        else
        {
            BackgroundColor = System.Drawing.Color.White;
            ForegroundColor = System.Drawing.Color.Black;
        }
        BackgroundColorBrush = new SolidColorBrush(Color.FromArgb(255, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
        ForegroundColorBrush = new SolidColorBrush(Color.FromArgb(255, ForegroundColor.R, ForegroundColor.G, ForegroundColor.B));
    }
}
