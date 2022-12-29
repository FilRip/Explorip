using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FilRip.Utils.Extensions
{
    /// <summary>
    /// Classe contenant des méthodes d'extensions pour les applications WinForms
    /// </summary>
    public static class ExtensionsWinForm
    {
        /// <summary>
        /// Retourne le formulaire actuellement actif de l'application
        /// </summary>
        public static Form ActiveForm
        {
            get
            {
                Form retour = Form.ActiveForm;
                if (retour == null)
                {
#pragma warning disable S3011 // Oui, c'est normal, ce champs est de type internal/friend
                    PropertyInfo propriete = typeof(Form).GetProperty("Active", BindingFlags.Instance | BindingFlags.NonPublic);
#pragma warning restore S3011
                    retour = Application.OpenForms.ToList<Form>()?.FirstOrDefault(f => (bool)propriete.GetValue(f));
                }
                return retour;
            }
        }

        /// <summary>
        /// Retourne si oui ou non cette fenêtre est active
        /// </summary>
        /// <param name="formulaire">Fenêtre à tester si active ou non</param>
        public static bool IsActive(this Form formulaire)
        {
            if (formulaire == null)
                throw new ArgumentNullException(nameof(formulaire));
#pragma warning disable S3011 // Oui, c'est normal, ce champs est de type internal/friend
            PropertyInfo propriete = typeof(Form).GetProperty("Active", BindingFlags.Instance | BindingFlags.NonPublic);
#pragma warning restore S3011
            return (bool)propriete.GetValue(formulaire);
        }

        /// <summary>
        /// Retourne la fenêtre parente de ce contrôle
        /// </summary>
        /// <param name="control">Contrôle pour lequel on veut la fenêtre</param>
        public static Form GetParentForm(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            Control element = control;
            while (element is not Form && element.Parent != null)
                element = element.Parent;
            return element is Form form ? form : null;
        }

        /// <summary>
        /// Retourne le parent le plus proche d'un contrôle, d'un type précis
        /// </summary>
        /// <typeparam name="T">Type à rechercher comme plus proche parent</typeparam>
        /// <param name="control">Contrôle pour lequel on veut le plus proche parent</param>
        public static T GetParentOfType<T>(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            Control element = control;
            while (element is not T && element.Parent != null)
                element = element.Parent;
            return element is T ret ? ret : default;
        }
    }
}
