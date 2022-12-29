using System;
using System.ComponentModel.Design;

namespace Explorip.ComposantsWinForm.FilRipListView
{
    /// <summary>
    /// Classe pour l'éditeur de colonne en mode Design
    /// </summary>
    public class FilRipColumnsCollectionEditor : CollectionEditor
    {
        /// <summary>
        /// Constructeur de base
        /// </summary>
        /// <param name="type"></param>
        public FilRipColumnsCollectionEditor(Type type) : base(type)
        {
        }

        /// <summary>
        /// Retourne le type des colonnes
        /// </summary>
        protected override Type CreateCollectionItemType()
        {
            return typeof(FilRipColumnHeader);
        }
    }
}
