using System.Collections;
using System.Windows.Forms;

namespace Explorip.ComposantsWinForm.FilRipListView
{
    /// <summary>
    /// Classe permettant de gérer le tri par colonne (subitems) d'une ListView
    /// </summary>
    public class FilRipListViewComparateur : IComparer
    {
        private int _numColonne;
        private SortOrder _triActuel;

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public FilRipListViewComparateur()
        {
        }

        /// <summary>
        /// Numéro de la colonne sur laquelle le tri se fait actuellement
        /// </summary>
        public int NumColonne
        {
            get { return _numColonne; }
            set { _numColonne = value; }
        }

        /// <summary>
        /// Change l'ordre du tri (ascendant, descendant)
        /// </summary>
        public void ChangeOrdre()
        {
            if (_triActuel == SortOrder.Ascending)
                _triActuel = SortOrder.Descending;
            else
                _triActuel = SortOrder.Ascending;
        }

        /// <summary>
        /// Méthode implémentant l'interface IComparer
        /// Permet le tri sur la colonne choisie
        /// </summary>
        /// <param name="x">Ligne à comparer</param>
        /// <param name="y">Ligne à comparer</param>
        public int Compare(object x, object y)
        {
            string itemX, itemY;
            try
            {
                itemX = ((ListViewItem)x).SubItems[_numColonne].Text;
            }
            catch { itemX = ""; }
            try
            {
                itemY = ((ListViewItem)y).SubItems[_numColonne].Text;
            }
            catch { itemY = ""; }
            if (_triActuel == SortOrder.Ascending)
                return string.Compare(itemX, itemY);
            else
                return string.Compare(itemY, itemX);
        }
    }
}
