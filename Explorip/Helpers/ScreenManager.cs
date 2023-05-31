using System;
using System.Collections.Generic;
using System.Drawing;
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
            List<Screen> retour = null;
            if (NumberOfScreen() > 1)
            {
                retour = new List<Screen>();
                foreach (Screen ecran in Screen.AllScreens)
                {
                    if (!ecran.Primary)
                        retour.Add(ecran);
                }
            }
            return retour?.ToArray();
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
            Math.DivRem(Screen.AllScreens.Count(), 2, out int nbEcranPair);
            if (nbEcranPair == 0) return Screen.PrimaryScreen; // A voir si on retourne une exception ou l'écran principal quand il n'y a pas d'écran central (nombre paire d'écran)
            List<Screen> EcranTries;
            EcranTries = Screen.AllScreens.OrderBy(item => item.WorkingArea.Location.X).ToList();
            while (EcranTries.Count > 1)
            {
                EcranTries.RemoveAt(0);
                EcranTries.RemoveAt(EcranTries.Count - 1);
            }
            return EcranTries[0];
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

        public static Screen GetScreen(SCREEN_POSITION posEcran)
        {
            return posEcran switch
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

        public static void MoveWindowToScreen(Window fenetre, int numEcran)
        {
            MoveWindowToScreen(fenetre, numEcran, false);
        }

        public static void MoveWindowToScreen(Window fenetre, int numEcran, bool pleinEcran)
        {
            if (numEcran > Screen.AllScreens.Count() - 1)
                throw new ArgumentOutOfRangeException(nameof(numEcran));
            MoveWindowToScreen(fenetre, Screen.AllScreens.ElementAt(numEcran), pleinEcran);
        }

        public static void MoveWindowToScreen(Window fenetre, Screen ecran)
        {
            MoveWindowToScreen(fenetre, ecran, false);
        }

        public static void MoveWindowToScreen(Window fenetre, Screen ecran, bool pleinEcran)
        {
            fenetre.Left = ecran.WorkingArea.Location.X;
            fenetre.Top = ecran.WorkingArea.Location.Y;
            if (fenetre.Width > ecran.WorkingArea.Width) fenetre.Width = ecran.WorkingArea.Width;
            if (fenetre.Height > ecran.WorkingArea.Height) fenetre.Height = ecran.WorkingArea.Height;
            if (pleinEcran) fenetre.WindowState = WindowState.Maximized;
        }

        public static void MoveWindowToScreen(Window fenetre, SCREEN_POSITION ecran)
        {
            MoveWindowToScreen(fenetre, ecran, false);
        }

        public static void MoveWindowToScreen(Window fenetre, SCREEN_POSITION ecran, bool pleinEcran)
        {
            Screen s = GetScreen(ecran);
            MoveWindowToScreen(fenetre, s, pleinEcran);
        }

        /// <summary>
        /// Retourne l'écran courant de l'application d'où provient l'appel
        /// </summary>
        public static Screen GetCurrentScreen()
        {
            var ecranCourant = Application.Current.Dispatcher.Invoke(() =>
            {
                Window fenetreCourante = Application.Current.Windows.OfType<Window>().FirstOrDefault();
                Screen ecran = Screen.FromHandle(new WindowInteropHelper(fenetreCourante).Handle);
                return ecran;
            });

            return ecranCourant;
        }

        /// <summary>
        /// Affichage sur l'écran choisi dans le fichier de config puis centrage
        /// TODO : vérifier avec tous les écrans, s'inspirer de DeplacerSurEcran ?
        public static void CenterOnScreen(Window targetWindow, int numeroEcran = -1)
        {
            Screen ecran;
            if (numeroEcran < 0)
                ecran = GetCurrentScreen();
            else
                ecran = GetScreen(numeroEcran);

            Rect r = ecran.WorkingArea;
            targetWindow.Top = r.Top + r.Height / 2 - targetWindow.ActualHeight / 2;
            targetWindow.Left = r.Left + r.Width / 2 - targetWindow.ActualWidth / 2;
        }

        #endregion
    }
}
