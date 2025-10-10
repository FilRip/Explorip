using System.Windows.Media;

namespace CoolBytes.ScriptInterpreter.Helpers;

public static class ExtensionsColor
{
    public static SolidColorBrush ToSolidColorBrush(this System.Drawing.Color couleur)
    {
        return new SolidColorBrush(Color.FromArgb(couleur.A, couleur.R, couleur.G, couleur.B));
    }

    public static System.Drawing.Color ToSystemColor(this SolidColorBrush brush)
    {
        return System.Drawing.Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);
    }
}
