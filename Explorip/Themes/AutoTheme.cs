﻿using System.Drawing;
using System.Windows.Forms;

using Explorip.ComposantsWinForm;
using Explorip.ComposantsWinForm.FilRipListView;
using Explorip.Helpers;

namespace Explorip.Themes
{
    public static class AutoTheme
    {
        private static Image _pbDisable, _pbEnable, _nbDisable, _nbEnable;
        private static System.Windows.Media.Imaging.BitmapSource _pbWpfDisable, _pbWpfEnable, _nbWpfDisable, _nbWpfEnable;

        public static void InitButtons()
        {
            _pbDisable = Properties.Resources.PreviousButton.ChangeCouleur(Color.White, Color.LightGray);
            _pbEnable = Properties.Resources.PreviousButton.ChangeCouleur(Color.White, WindowsSettings.GetWindowsAccentColor());

            _nbDisable = Properties.Resources.NextButton.ChangeCouleur(Color.White, Color.LightGray);
            _nbEnable = Properties.Resources.NextButton.ChangeCouleur(Color.White, WindowsSettings.GetWindowsAccentColor());

            _pbWpfDisable = new Bitmap(_pbDisable).ToBitmapSource();
            _pbWpfEnable = new Bitmap(_pbEnable).ToBitmapSource();

            _nbWpfDisable = new Bitmap(_nbDisable).ToBitmapSource();
            _nbWpfEnable = new Bitmap(_nbEnable).ToBitmapSource();
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

        public static void AppliqueThemeWindows(Form form)
        {
            bool sombre = WindowsSettings.IsWindowsApplicationInDarkMode();
            ChangeThemeRecursif(form, sombre);
        }

        public static void ChangeThemeRecursif(Control control, bool sombre)
        {
            if (sombre)
            {
                if (control.BackColor != Color.Transparent)
                    control.BackColor = Color.Black;
                if (control is Form)
                {
                    control.ForeColor = Color.White;
                    control.BackColor = Color.Black;
                }
                else if (control is Button)
                {
                    if (control.BackColor != Color.Transparent)
                        control.BackColor = Color.LightGray;
                    if (control.ForeColor != Color.Transparent)
                        control.ForeColor = Color.Black;
                }
                else if (control is FilRipListView lv)
                {
                    control.ForeColor = Color.White;
                    lv.CouleurAlternee1 = Color.Black;
                    lv.CouleurAlternee2 = Color.FromArgb(0x30, 0x30, 0x30);
                    foreach (FilRipColumnHeader col in lv.Columns)
                    {
                        col.ActiveCouleur = true;
                        col.CouleurArrierePlan = Color.Black;
                        col.CouleurTexte = Color.White;
                        if (col.ActiveCouleurOk)
                        {
                            if (col.CouleurOk == Color.Black)
                                col.CouleurOk = Color.White;
                            if (col.CouleurSinon == Color.Black)
                                col.CouleurSinon = Color.White;
                        }
                    }
                }
                else if (control is ButtonBase or TabPage or GroupBox or Label)
                {
                    if (control.BackColor != Color.Transparent && control is not LinkLabel)
                    {
                        control.BackColor = Color.Black;
                        control.ForeColor = Color.White;
                    }
                }
                else if (control is TabExplorerBrowser tabControl)
                {
                    tabControl.DisplayStyleProvider.TextColorSelected = Color.White;
                    tabControl.DisplayStyleProvider.TextColor = Color.White;
                }
                else if (control is not ProgressBar && control.ForeColor != Color.Transparent)
                {
                    control.ForeColor = Color.White;
                }
            }
            else
            {
                if (control.BackColor != Color.Transparent)
                    control.BackColor = SystemColors.Window;
                if (control.ForeColor != Color.Transparent)
                    control.ForeColor = SystemColors.WindowText;
                if (control is Form)
                {
                    control.ForeColor = SystemColors.WindowText;
                    control.BackColor = SystemColors.Window;
                }
                else if (control is FilRipListView lv)
                {
                    control.ForeColor = Color.Black;
                    lv.CouleurAlternee1 = Color.White;
                    lv.CouleurAlternee2 = Color.FromArgb(0xEF, 0xEF, 0xEF);
                    foreach (FilRipColumnHeader col in lv.Columns)
                    {
                        col.ActiveCouleur = false;
                        col.CouleurArrierePlan = SystemColors.Control;
                        col.CouleurTexte = SystemColors.ControlText;
                        if (col.ActiveCouleurOk)
                        {
                            if (col.CouleurOk == Color.White)
                                col.CouleurOk = Color.Black;
                            if (col.CouleurSinon == Color.White)
                                col.CouleurSinon = Color.Black;
                        }
                    }
                }
                else if (control is ButtonBase or TabPage or GroupBox or Label)
                {
                    if (control.BackColor != Color.Transparent && control is not LinkLabel)
                    {
                        control.ForeColor = SystemColors.ControlText;
                        control.BackColor = SystemColors.Control;
                    }
                }
                else if (control is ProgressBar)
                {
                    control.BackColor = SystemColors.Control;
                    control.ForeColor = Color.Red;
                }
                else if (control is TabExplorerBrowser tabControl)
                {
                    tabControl.DisplayStyleProvider.TextColorSelected = Color.Black;
                    tabControl.DisplayStyleProvider.TextColor = Color.Black;
                }
            }

            if (control.ContextMenuStrip != null)
            {
                ChangeThemeMenu(control.ContextMenuStrip.Items, sombre);
            }

            if (control.HasChildren)
            {
                foreach (Control sousControle in control.Controls)
                    ChangeThemeRecursif(sousControle, sombre);
            }
        }

        public static void ChangeThemeMenu(ToolStripItemCollection sousMenu, bool sombre)
        {
            foreach (ToolStripItem item in sousMenu)
            {
                if (sombre)
                {
                    item.BackColor = Color.FromArgb(60, 60, 60);
                    item.ForeColor = Color.White;
                }
                else
                {
                    item.BackColor = Color.White;
                    item.ForeColor = Color.Black;
                }

                if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
                {
                    ChangeThemeMenu(menuItem.DropDownItems, sombre);
                }
            }
        }

        #endregion

        #region WPF

        public static System.Windows.Media.ImageSource PreviousButtonDisabled
        {
            get { return _pbWpfDisable; }
        }

        public static System.Windows.Media.ImageSource PreviousButtonEnabled
        {
            get { return _pbWpfEnable; }
        }

        public static System.Windows.Media.ImageSource NextButtonDisabled
        {
            get { return _nbWpfDisable; }
        }

        public static System.Windows.Media.ImageSource NextButtonEnabled
        {
            get { return _nbWpfEnable; }
        }

        #endregion
    }
}
