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
        private string _texteOk = "";
        private Color _couleurOk = Color.Black;
        private Color _couleurSinon = Color.Red;
        private bool _activeCouleurOk = false;

        private Color _couleurEnteteFond = SystemColors.Control;
        private Color _couleurEnteteTexte = SystemColors.ControlText;
        private bool _activeCouleurEntete = false;

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
        public bool ActiveCouleurOk
        {
            get { return _activeCouleurOk; }
            set { _activeCouleurOk = value; }
        }

        /// <summary>
        /// Expression à évaluer pour choisir la couleur
        /// </summary>
        [Category("Couleur automatique"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public string ExpressionOK
        {
            get { return _texteOk; }
            set { _texteOk = value; }
        }

        /// <summary>
        /// Couleur si ExpressionOK est Vrai
        /// </summary>
        [Category("Couleur automatique"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurOk
        {
            get { return _couleurOk; }
            set { _couleurOk = value; }
        }

        /// <summary>
        /// Couleur si ExpressionOK est faux
        /// </summary>
        [Category("Couleur automatique"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurSinon
        {
            get { return _couleurSinon; }
            set { _couleurSinon = value; }
        }

        /// <summary>
        /// Couleur de l'arrière plan du titre de la colonne
        /// </summary>
        [Category("Entête"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurArrierePlan
        {
            get { return _couleurEnteteFond; }
            set { _couleurEnteteFond = value; }
        }

        /// <summary>
        /// Couleur du texte du titre de la colonne
        /// </summary>
        [Category("Entête"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurTexte
        {
            get { return _couleurEnteteTexte; }
            set { _couleurEnteteTexte = value; }
        }

        /// <summary>
        /// Active la redéfinition des couleurs pour l'entête
        /// </summary>
        [Category("Entête"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool ActiveCouleur
        {
            get { return _activeCouleurEntete; }
            set { _activeCouleurEntete = value; }
        }
    }
}
