using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

using Explorip.Exceptions;

using WpfScreenHelper;

namespace Explorip.Helpers
{
    public static class ScreenManager
    {
        public static int NumberOfScreen()
        {
            return Screen.AllScreens.Count();
        }

        public static Screen MainScreen()
        {
            return Screen.PrimaryScreen;
        }

        public static bool ScreenConnected
        {
            get { return Screen.AllScreens.Any(); }
        }

        private static void ThrowIfNoScreenConnected()
        {
            if (!Screen.AllScreens.Any())
                throw new ScreenManagerException(Constants.Localization.NO_MONITOR_CONNECTED);
        }

        public static Screen[] ListScreenExceptPrimary()
        {
            List<Screen> result = null;
            if (NumberOfScreen() > 1)
            {
                result = new List<Screen>();
                foreach (Screen ecran in Screen.AllScreens)
                {
                    if (!ecran.Primary)
                        result.Add(ecran);
                }
            }
            return result?.ToArray();
        }

        public static Screen GetLeftScreen()
        {
            ThrowIfNoScreenConnected();
            if (Screen.AllScreens.Count() == 1) return Screen.AllScreens.ElementAt(0);
            int posX = int.MaxValue;
            Screen last = null;
            foreach (Screen s in Screen.AllScreens)
                if (s.WorkingArea.Location.X < posX)
                {
                    last = s;
                    posX = (int)s.WorkingArea.Location.X;
                }
            return last;
        }

        public static Screen GetRightScreen()
        {
            ThrowIfNoScreenConnected();
            if (Screen.AllScreens.Count() == 1) return Screen.AllScreens.ElementAt(0);
            int posX = -1;
            Screen last = null;
            foreach (Screen s in Screen.AllScreens)
                if (s.WorkingArea.Location.X > posX)
                {
                    last = s;
                    posX = (int)s.WorkingArea.Location.X;
                }
            return last;
        }

        public static Screen GetCenterScreen()
        {
            ThrowIfNoScreenConnected();
            if (Screen.AllScreens.Count() == 1)
                return Screen.AllScreens.ElementAt(0);
            if (Screen.AllScreens.Count() == 2)
                return Screen.PrimaryScreen;
            Math.DivRem(Screen.AllScreens.Count(), 2, out int nbEvenScreen);
            if (nbEvenScreen == 0) return Screen.PrimaryScreen; // A voir si on retourne une exception ou l'écran principal quand il n'y a pas d'écran central (nombre paire d'écran)
            List<Screen> OrderedScreen;
            OrderedScreen = Screen.AllScreens.OrderBy(item => item.WorkingArea.Location.X).ToList();
            while (OrderedScreen.Count > 1)
            {
                OrderedScreen.RemoveAt(0);
                OrderedScreen.RemoveAt(OrderedScreen.Count - 1);
            }
            return OrderedScreen[0];
        }

        /// <summary>
        /// Retourne un écran, de base 0
        /// </summary>
        /// <param name="id">Numéro de l'écran</param>
        /// <returns></returns>
        public static Screen GetScreen(int id)
        {
            ThrowIfNoScreenConnected();
            if (id > Screen.AllScreens.Count() - 1)
                throw new ArgumentOutOfRangeException(nameof(id));
            return Screen.AllScreens.ElementAt(id);
        }

        public static Screen GetScreen(SCREEN_POSITION positionScreen)
        {
            return positionScreen switch
            {
                SCREEN_POSITION.LEFT => GetLeftScreen(),
                SCREEN_POSITION.CENTER => GetCenterScreen(),
                SCREEN_POSITION.RIGHT => GetRightScreen(),
                _ => throw new ScreenManagerException("Position de l'écran demandé inconnu"),
            };
        }

        public enum SCREEN_POSITION
        {
            LEFT = 1,
            CENTER = 2,
            RIGHT = 3
        }

        #region WPF

        public static void MoveWindowToScreen(Window window, int numScreen)
        {
            MoveWindowToScreen(window, numScreen, false);
        }

        public static void MoveWindowToScreen(Window window, int numScreen, bool pleinEcran)
        {
            if (numScreen > Screen.AllScreens.Count() - 1)
                throw new ArgumentOutOfRangeException(nameof(numScreen));
            MoveWindowToScreen(window, Screen.AllScreens.ElementAt(numScreen), pleinEcran);
        }

        public static void MoveWindowToScreen(Window fenetre, Screen screen)
        {
            MoveWindowToScreen(fenetre, screen, false);
        }

        public static void MoveWindowToScreen(Window fenetre, Screen screen, bool pleinEcran)
        {
            fenetre.Left = screen.WorkingArea.Location.X;
            fenetre.Top = screen.WorkingArea.Location.Y;
            if (fenetre.Width > screen.WorkingArea.Width) fenetre.Width = screen.WorkingArea.Width;
            if (fenetre.Height > screen.WorkingArea.Height) fenetre.Height = screen.WorkingArea.Height;
            if (pleinEcran) fenetre.WindowState = WindowState.Maximized;
        }

        public static void MoveWindowToScreen(Window window, SCREEN_POSITION screen)
        {
            MoveWindowToScreen(window, screen, false);
        }

        public static void MoveWindowToScreen(Window window, SCREEN_POSITION screen, bool fullScreen)
        {
            Screen s = GetScreen(screen);
            MoveWindowToScreen(window, s, fullScreen);
        }

        /// <summary>
        /// Retourne l'écran courant de l'application d'où provient l'appel
        /// </summary>
        public static Screen GetCurrentScreen()
        {
            Screen currentScreen = Application.Current.Dispatcher.Invoke(() =>
            {
                Window currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault();
                Screen ecran = Screen.FromHandle(new WindowInteropHelper(currentWindow).Handle);
                return ecran;
            });

            return currentScreen;
        }

        /// <summary>
        /// Affichage sur l'écran choisi dans le fichier de config puis centrage
        /// TODO : vérifier avec tous les écrans, s'inspirer de DeplacerSurEcran ?
        public static void CenterOnScreen(Window targetWindow, int numScreen = -1)
        {
            Screen screen;
            if (numScreen < 0)
                screen = GetCurrentScreen();
            else
                screen = GetScreen(numScreen);

            Rect r = screen.WorkingArea;
            targetWindow.Top = r.Top + r.Height / 2 - targetWindow.ActualHeight / 2;
            targetWindow.Left = r.Left + r.Width / 2 - targetWindow.ActualWidth / 2;
        }

        #endregion
    }
}
