using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Explorip.ComposantsWinForm.FilRipListView
{
    /// <summary>
    /// Colonne pour le contrôle FilRipListView
    /// </summary>
    [DefaultProperty("Text")]
    [DesignTimeVisible(false)]
    [ToolboxItem(false)]
    [TypeConverter(typeof(ColumnHeaderConverter))]
    public class FilRipColumnHeader : ColumnHeader
    {
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public FilRipColumnHeader() : base()
        {
        }

        /// <summary>
        /// Active/Desactive le changement de couleur, selon l'expression, pour cette colonne
        /// </summary>
        [Category("Couleur automatique"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool ActiveCouleurOk { get; set; }

        /// <summary>
        /// Expression à évaluer pour choisir la couleur
        /// </summary>
        [Category("Couleur automatique"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public string ExpressionOK { get; set; }

        /// <summary>
        /// Couleur si ExpressionOK est Vrai
        /// </summary>
        [Category("Couleur automatique"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurOk { get; set; }

        /// <summary>
        /// Couleur si ExpressionOK est faux
        /// </summary>
        [Category("Couleur automatique"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurSinon { get; set; }

        /// <summary>
        /// Couleur de l'arrière plan du titre de la colonne
        /// </summary>
        [Category("Entête"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurArrierePlan { get; set; }

        /// <summary>
        /// Couleur du texte du titre de la colonne
        /// </summary>
        [Category("Entête"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurTexte { get; set; }

        /// <summary>
        /// Active la redéfinition des couleurs pour l'entête
        /// </summary>
        [Category("Entête"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool ActiveCouleur { get; set; }
    }
}
