using System;
using System.Diagnostics;
using System.Linq;

using Explorip.WinAPI;

namespace Explorip.Helpers
{
    public static class GestionFenetres
    {
        /// <summary>
        /// Retourne le Handle de la fenêtre principale du processus en cours
        /// </summary>
        /// <returns></returns>
        public static IntPtr RetourneFenetrePrincipale()
        {
            return Process.GetCurrentProcess().MainWindowHandle;
        }

        /// <summary>
        /// Retourne le Handle de la fenêtre principal d'un autre processus
        /// </summary>
        /// <param name="IdProcessus">Id du processus</param>
        public static IntPtr RetourneFenetrePrincipale(int IdProcessus)
        {
            Process processus = Process.GetProcessById(IdProcessus);
            if (processus == null) throw new ExceptionGestionFenetres($"Aucun processus trouvé avec l'Id {IdProcessus}");
            return processus.MainWindowHandle;
        }

        /// <summary>
        /// Retourne le Handle de la fenêtre principal d'un autre processus
        /// </summary>
        /// <param name="nomProcessus">Nom du processus</param>
        public static IntPtr RetourneFenetrePrincipale(string nomProcessus)
        {
            Process[] processus = Process.GetProcessesByName(nomProcessus);
            if ((processus == null) || (processus.Length == 0)) throw new ExceptionGestionFenetres($"Aucun processus trouvé sous le nom {nomProcessus}");
            if (processus.Length > 1) throw new ExceptionGestionFenetres($"Plusieurs processus trouvé sous le nom {nomProcessus}");
            return processus[0].MainWindowHandle;
        }

        /// <summary>
        /// Retourne le Handle de la console d'un autre processus
        /// </summary>
        /// <param name="IdProcessus">Id du processus</param>
        public static IntPtr RetourneConsoleProcessus(int IdProcessus)
        {
            IntPtr retour = IntPtr.Zero;
            if (Process.GetProcessById(IdProcessus) == null) throw new ExceptionGestionFenetres($"Aucun processus trouvé avec l'Id {IdProcessus}");
            if (User32.AttachConsole(IdProcessus))
            {
                try
                {
                    retour = User32.GetConsoleWindow();
                }
                catch (Exception ex)
                {
                    throw new ExceptionGestionFenetres(ex.Message);
                }
                finally
                {
                    User32.FreeConsole();
                }
            }
            return retour;
        }

        /// <summary>
        /// Retourne le Handle de la console d'un autre processus
        /// Retourne une exception si il y a aucun ou plusieurs processus protant ce nom
        /// </summary>
        /// <param name="nomProcessus">Nom du processus</param>
        public static IntPtr RetourneConsoleProcessus(string nomProcessus)
        {
            if (string.IsNullOrWhiteSpace(nomProcessus)) throw new ArgumentNullException(nameof(nomProcessus));
            Process[] processus = Process.GetProcessesByName(nomProcessus);
            if ((processus == null) || (processus.Length == 0)) throw new ExceptionGestionFenetres($"Aucun processus trouvé sous le nom {nomProcessus}");
            if (processus.Length > 1) throw new ExceptionGestionFenetres($"Plusieurs processus trouvé sous le nom {nomProcessus}");
            return RetourneConsoleProcessus(processus[0].Id);
        }

        /// <summary>
        /// Exception levée si erreur dans une méthode de gestionFenetres
        /// </summary>
        public class ExceptionGestionFenetres : Exception
        {
            private readonly string _erreur;

            public ExceptionGestionFenetres(string erreur)
            {
                _erreur = erreur;
            }

            public override string Message
            {
                get
                {
                    return _erreur;
                }
            }
        }

        /// <summary>
        /// Retourne toutes les fenêtres ouvertes de l'application Windows Forms actuelle
        /// </summary>
        public static System.Windows.Forms.Form[] RetourneToutesLesFenetresWF()
        {
            try
            {
                return System.Windows.Forms.Application.OpenForms.OfType<System.Windows.Forms.Form>().ToArray();
            }
            catch (Exception ex)
            {
                throw new ExceptionGestionFenetres("Erreur pendant la tentative de lister les fenêtres de l'application. Est-ce une application Windows Forms ?" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// Retourne toutes les fenêtres ouvertes de l'application WPF actuelle
        /// </summary>
        public static System.Windows.Window[] RetourneToutesLesFenetresWPF()
        {
            try
            {
                return System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().ToArray();
            }
            catch (Exception ex)
            {
                throw new ExceptionGestionFenetres("Erreur pendant la tentative de lister les fenêtres de l'application. Est-ce une application WPF ?" + Environment.NewLine + ex.Message);
            }
        }
    }
}
