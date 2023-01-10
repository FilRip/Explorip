using System.ComponentModel;
using System.Windows.Forms;

namespace Explorip.ComposantsWinForm.FilRipListView
{
    /// <summary>
    /// Collection gérant la liste des colonnes pour le contrôle FilRipListView
    /// </summary>
    [ListBindable(false)]
    public class FilRipColumnHeaderCollection : ListView.ColumnHeaderCollection
    {
        /// <summary>
        /// Constructeur de base
        /// </summary>
        /// <param name="owner">Objet FilRipListView pour lequel dépend cete liste de colonnes</param>
        public FilRipColumnHeaderCollection(ListView owner) : base(owner)
        {
        }

        /// <summary>
        /// Ajouter une colonne
        /// </summary>
        /// <param name="value">Colonne à ajouter</param>
        /// <returns></returns>
        public int Add(FilRipColumnHeader value)
        {
            return base.Add(value);
        }

        /// <summary>
        /// Supprimer une colonne
        /// </summary>
        /// <param name="column">Colonne à supprimer</param>
        public void Remove(FilRipColumnHeader column)
        {
            base.Remove(column);
        }

        /// <summary>
        /// Ajouter une liste de colonnes
        /// </summary>
        /// <param name="values">Liste de colonnes à ajouter</param>
        public void AddRange(FilRipColumnHeader[] values)
        {
            base.AddRange(values);
        }
    }
}
