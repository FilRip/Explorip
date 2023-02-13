/*
 * This code is provided under the Code Project Open Licence (CPOL)
 * See http://www.codeproject.com/info/cpol10.aspx for details
*/

using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace System.Drawing
{
    internal sealed class ThemedColors
    {
        #region Variables and Constants

        private const string NormalColor = "NormalColor";
        private const string HomeStead = "HomeStead";
        private const string Metallic = "Metallic";
        private const string NoTheme = "NoTheme";

        private static readonly Color[] _toolBorder = new Color[] { Color.FromArgb(127, 157, 185), Color.FromArgb(164, 185, 127), Color.FromArgb(165, 172, 178), Color.FromArgb(132, 130, 132) };
        #endregion

        #region Properties

        public static ColorScheme CurrentThemeIndex
        {
            get { return ThemedColors.GetCurrentThemeIndex(); }
        }

        public static Color ToolBorder
        {
            get { return ThemedColors._toolBorder[(int)ThemedColors.CurrentThemeIndex]; }
        }

        #endregion

        #region Constructors

        private ThemedColors() { }

        #endregion

        private static ColorScheme GetCurrentThemeIndex()
        {
            ColorScheme theme = ColorScheme.NoTheme;

            if (VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser && Application.RenderWithVisualStyles)
            {
                theme = VisualStyleInformation.ColorScheme switch
                {
                    NormalColor => ColorScheme.NormalColor,
                    HomeStead => ColorScheme.HomeStead,
                    Metallic => ColorScheme.Metallic,
                    _ => ColorScheme.NoTheme,
                };
            }

            return theme;
        }

        public enum ColorScheme
        {
            NormalColor = 0,
            HomeStead = 1,
            Metallic = 2,
            NoTheme = 3
        }
    }
}
