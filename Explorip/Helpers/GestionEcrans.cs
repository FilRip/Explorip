using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Explorip.Exceptions;

namespace Explorip.Helpers
{
    public static class GestionEcrans
    {
        public static int NbEcran()
        {
            return Screen.AllScreens.Length;
        }

        public static Screen EcranPrincipal()
        {
            return Screen.PrimaryScreen;
        }

        public static bool EcranConnecte
        {
            get { return (Screen.AllScreens.Length > 0); }
        }

        private static void VerifSiEcranPresent()
        {
            if (Screen.AllScreens.Length == 0) throw new GestionEcransException("Aucun écran connecté/détecté");
        }

        public static Screen[] ListeAutresEcrans()
        {
            List<Screen> retour = null;
            if (NbEcran() > 1)
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

        /// <summary>
        /// Retourne un écran, de base 0
        /// </summary>
        /// <param name="id">Numéro de l'écran</param>
        /// <returns></returns>
        public static Screen Ecran(int id)
        {
            VerifSiEcranPresent();
            if (id > Screen.AllScreens.Length - 1) throw new GestionEcransException("Ce numéro d'écran n'existe pas");
            return Screen.AllScreens[id];
        }

        public static Screen EcranGauche()
        {
            VerifSiEcranPresent();
            if (Screen.AllScreens.Length == 1) return Screen.AllScreens[0];
            int posX = int.MaxValue;
            Screen last = null;
            foreach (Screen s in Screen.AllScreens)
                if (s.WorkingArea.Location.X < posX)
                {
                    last = s;
                    posX = s.WorkingArea.Location.X;
                }
            return last;
        }

        public static Screen EcranDroite()
        {
            VerifSiEcranPresent();
            if (Screen.AllScreens.Length == 1) return Screen.AllScreens[0];
            int posX = -1;
            Screen last = null;
            foreach (Screen s in Screen.AllScreens)
                if (s.WorkingArea.Location.X > posX)
                {
                    last = s;
                    posX = s.WorkingArea.Location.X;
                }
            return last;
        }

        public static Screen EcranCentre()
        {
            VerifSiEcranPresent();
            if (Screen.AllScreens.Length == 1)
                return Screen.AllScreens[0];
            if (Screen.AllScreens.Length == 2)
                return Screen.PrimaryScreen;
            Math.DivRem(Screen.AllScreens.Length, 2, out int nbEcranPair);
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

        public static Screen RetourneEcran(POSITION_ECRAN posEcran)
        {
            return posEcran switch
            {
                POSITION_ECRAN.GAUCHE => EcranGauche(),
                POSITION_ECRAN.CENTRE => EcranCentre(),
                POSITION_ECRAN.DROITE => EcranDroite(),
                _ => throw new GestionEcransException("Position de l'écran demandé inconnu"),
            };
        }

        public enum POSITION_ECRAN
        {
            GAUCHE = 1,
            CENTRE = 2,
            DROITE = 3
        }

        #region Formulaire

        public static void DeplaceFenetreSurEcran(Form form, int numEcran)
        {
            DeplaceFenetreSurEcran(form, numEcran, false);
        }

        public static void DeplaceFenetreSurEcran(Form form, int numEcran, bool pleinEcran)
        {
            if (numEcran > Screen.AllScreens.Length - 1) throw new GestionEcransException("Ce numéro d'écran n'existe pas");
            DeplaceFenetreSurEcran(form, Screen.AllScreens[numEcran], pleinEcran);
        }

        public static void DeplaceFenetreSurEcran(Form form, Screen ecran)
        {
            DeplaceFenetreSurEcran(form, ecran, false);
        }

        public static void DeplaceFenetreSurEcran(Form form, Screen ecran, bool pleinEcran)
        {
            form.Left = ecran.WorkingArea.Location.X;
            form.Top = ecran.WorkingArea.Location.Y;
            if (form.Width > ecran.WorkingArea.Width) form.Width = ecran.WorkingArea.Width;
            if (form.Height > ecran.WorkingArea.Height) form.Height = ecran.WorkingArea.Height;
            if (pleinEcran) form.WindowState = FormWindowState.Maximized;
        }

        public static void DeplaceFenetreSurEcran(Form form, POSITION_ECRAN ecran)
        {
            DeplaceFenetreSurEcran(form, ecran, false);
        }

        public static void DeplaceFenetreSurEcran(Form form, POSITION_ECRAN ecran, bool pleinEcran)
        {
            Screen s = RetourneEcran(ecran);
            DeplaceFenetreSurEcran(form, s, pleinEcran);
        }

        #endregion

        #region WPF

        public static void DeplaceFenetreSurEcran(System.Windows.Window fenetre, int numEcran)
        {
            DeplaceFenetreSurEcran(fenetre, numEcran, false);
        }

        public static void DeplaceFenetreSurEcran(System.Windows.Window fenetre, int numEcran, bool pleinEcran)
        {
            if (numEcran > Screen.AllScreens.Length - 1) throw new GestionEcransException("Ce numéro d'écran n'existe pas");
            DeplaceFenetreSurEcran(fenetre, Screen.AllScreens[numEcran], pleinEcran);
        }

        public static void DeplaceFenetreSurEcran(System.Windows.Window fenetre, Screen ecran)
        {
            DeplaceFenetreSurEcran(fenetre, ecran, false);
        }

        public static void DeplaceFenetreSurEcran(System.Windows.Window fenetre, Screen ecran, bool pleinEcran)
        {
            fenetre.Left = ecran.WorkingArea.Location.X;
            fenetre.Top = ecran.WorkingArea.Location.Y;
            if (fenetre.Width > ecran.WorkingArea.Width) fenetre.Width = ecran.WorkingArea.Width;
            if (fenetre.Height > ecran.WorkingArea.Height) fenetre.Height = ecran.WorkingArea.Height;
            if (pleinEcran) fenetre.WindowState = System.Windows.WindowState.Maximized;
        }

        public static void DeplaceFenetreSurEcran(System.Windows.Window fenetre, POSITION_ECRAN ecran)
        {
            DeplaceFenetreSurEcran(fenetre, ecran, false);
        }

        public static void DeplaceFenetreSurEcran(System.Windows.Window fenetre, POSITION_ECRAN ecran, bool pleinEcran)
        {
            Screen s = RetourneEcran(ecran);
            DeplaceFenetreSurEcran(fenetre, s, pleinEcran);
        }

        #endregion
    }
}
