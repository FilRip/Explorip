using System.Drawing;
using System.Windows.Forms;

using Explorip.Helpers;

namespace Explorip.Themes
{
    public static class AutoTheme
    {
        private static Image _pbDisable, _pbEnable, _nbDisable, _nbEnable;

        public static void InitButtons()
        {
            _pbDisable = Properties.Resources.PreviousButton.ChangeCouleur(Color.White, Color.LightGray);
            _pbEnable = Properties.Resources.PreviousButton.ChangeCouleur(Color.White, WindowsSettings.GetWindowsAccentColor());

            _nbDisable = Properties.Resources.NextButton.ChangeCouleur(Color.White, Color.LightGray);
            _nbEnable = Properties.Resources.NextButton.ChangeCouleur(Color.White, WindowsSettings.GetWindowsAccentColor());
        }

        #region WinForm

        public static Image ButtonNextEnabled
        {
            get { return _nbEnable; }
        }

        public static Image ButtonNextDisabled
        {
            get { return _nbDisable; }
        }

        public static Image ButtonPreviousEnabled
        {
            get { return _pbEnable; }
        }

        public static Image ButtonPreviousDisabled
        {
            get { return _pbDisable; }
        }

        #endregion
    }
}
