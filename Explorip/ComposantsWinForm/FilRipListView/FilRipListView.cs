using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Explorip.ComposantsWinForm.FilRipListView
{
    /// <summary>
    /// Classe héritant de ListView permettant de changer dynamiquement les couleurs des colonnes
    /// <para>Et pouvoir choisir les colonnes à afficher/cacher par menu contextuel sur ce contrôle</para>
    /// </summary>
    public class FilRipListView : ListView
    {
        private bool _addMenuShowColumns = true;
        private ToolStripMenuItem _menuColonnes;
        private readonly FilRipColumnHeaderCollection columnHeaderCollection;
        private const string titreSousMenuColonne = "Colonnes affichées";
        private bool _alternerCouleur = true;
        private Color _couleurAlternee1 = Color.White;
        private Color _couleurAlternee2 = Color.LightGray;

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public FilRipListView()
        {
            this.ColumnClick += new ColumnClickEventHandler(this.MonListView_ColumnClick);
            //ListViewItemSorter = new FilRipListViewComparateur();
            this.MouseDown += ConstruireMenuColonnes;
            OwnerDraw = true;
            this.DrawItem += MonListView_DrawItem;
            this.DrawSubItem += MonListView_DrawSubItem;
            this.DrawColumnHeader += MonListView_DrawColumnHeader;
            columnHeaderCollection = new FilRipColumnHeaderCollection(this);
        }

        /// <summary>
        /// Change le handler appelé lors du rafraichissement des entête de colonnes
        /// Si null, on remet par défaut
        /// </summary>
        /// <param name="nouvelHandler">Nouvel handler pointant sur une méthode pour dessiner les entetes de colonnes, si null, on remet par défaut</param>
        public void DefinirHandlerOfDrawColumnHeader(DrawListViewColumnHeaderEventHandler nouvelHandler)
        {
            this.DrawColumnHeader -= MonListView_DrawColumnHeader;
            if (nouvelHandler == null)
                this.DrawColumnHeader += MonListView_DrawColumnHeader;
            else
                this.DrawColumnHeader += nouvelHandler;
        }

        private void MonListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            try
            {
                FilRipColumnHeader enTete = ((FilRipColumnHeader)e.Header);
                if ((View == View.Details) && (enTete.ActiveCouleur))
                {
                    e.Graphics.FillRectangle(new SolidBrush(enTete.CouleurArrierePlan), e.Bounds);
                    Rectangle monBounds = e.Bounds;
                    monBounds.Width -= 1;
                    monBounds.Height -= 1;
                    e.Graphics.DrawRectangle(new Pen(enTete.CouleurTexte), e.Bounds);
                    TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font, e.Bounds, enTete.CouleurTexte, TextFormatFlags.VerticalCenter);
                }
                else
                    e.DrawDefault = true;
            }
            catch (Exception) { e.DrawDefault = true; }
        }

        private void MonListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            try
            {
                FilRipColumnHeader enTete = ((FilRipColumnHeader)e.Header);
                if (View == View.Details)
                {
                    if (enTete.ActiveCouleurOk)
                    {
                        if ((!string.IsNullOrWhiteSpace(enTete.ExpressionOK)) && (e.SubItem.Text == enTete.ExpressionOK))
                            e.SubItem.ForeColor = enTete.CouleurOk;
                        else
                            e.SubItem.ForeColor = enTete.CouleurSinon;
                    }
                    if (_alternerCouleur)
                    {
                        e.SubItem.BackColor = e.ItemIndex % 2 == 0 ? _couleurAlternee1 : _couleurAlternee2;
                    }
                }
                e.DrawDefault = true;
            }
            catch { e.DrawDefault = true; }
        }

        private void MonListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if ((!OwnerDraw) || (View != View.Details))
                e.DrawDefault = true;
        }

        /// <summary>
        /// Ajoute (ou non) le menu contextuel à ce contrôle permettant de choisir les colonnes à afficher/cacher
        /// </summary>
        [Category("Menu colonnes"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool AddMenuShowColumns
        {
            get { return _addMenuShowColumns; }
            set { _addMenuShowColumns = value; }
            
        }

        /// <summary>
        /// Active ou non l'alternative de couleur de fond une ligne sur deux
        /// </summary>
        [Category("Couleur alternée"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool ActiverCouleurAlternee
        {
            get { return _alternerCouleur; }
            set { _alternerCouleur = value; }
        }

        /// <summary>
        /// Couleur de fond pour les lignes impaires
        /// </summary>
        [Category("Couleur alternée"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurAlternee1
        {
            get { return _couleurAlternee1; }
            set { _couleurAlternee1 = value; }
        }

        /// <summary>
        /// Couleur de fond pour les lignes paires
        /// </summary>
        [Category("Couleur alternée"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurAlternee2
        {
            get { return _couleurAlternee2; }
            set { _couleurAlternee2 = value; }
        }

        /// <summary>
        /// Liste des colonnes
        /// </summary>
        [Editor(typeof(FilRipColumnsCollectionEditor), typeof(System.Drawing.Design.UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(true)]
        public new FilRipColumnHeaderCollection Columns { get { return columnHeaderCollection; } }

        /// <summary>
        /// Force le rafraichissement du menu contextuel de ce contrôle, par exemple si une colonne a été ajoutée (ou supprimée) dynamiquement APRES la génération du contrôle
        /// </summary>
        public void ForceRafraichirMenuColonne()
        {
            _menuColonnes.Dispose();
            _menuColonnes = null;
        }

        private void ConstruireMenuColonnes(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (!_addMenuShowColumns || Columns.Count == 0) return;

            if (ContextMenuStrip == null)
                ContextMenuStrip = new ContextMenuStrip();

            if ((ContextMenuStrip.Items != null) && (ContextMenuStrip.Items.Count > 0))
                foreach (ToolStripMenuItem item in ContextMenuStrip.Items.OfType<ToolStripMenuItem>())
                    if ((item.Text == titreSousMenuColonne) && (item.Tag != this))
                    {
                        item.Dispose();
                        _menuColonnes = null;
                        break;
                    }

            if ((_menuColonnes == null) || (_menuColonnes.Tag != this))
            {
                _menuColonnes = (ToolStripMenuItem)ContextMenuStrip.Items.Add(titreSousMenuColonne);
                _menuColonnes.BackColor = ContextMenuStrip.BackColor;
                _menuColonnes.ForeColor = ContextMenuStrip.ForeColor;
                _menuColonnes.Tag = this;

                ToolStripMenuItem col;

                if ((Columns != null) && (Columns.Count > 0))
                    foreach (ColumnHeader ch in Columns)
                    {
                        col = new ToolStripMenuItem(ch.Text)
                        {
                            CheckOnClick = true,
                            Checked = (ch.Width != 0),
                            Tag = ch,
                            BackColor = this.BackColor,
                            ForeColor = this.ForeColor
                        };
                        col.Click += ClickOnColumnMenu;
                        _menuColonnes.DropDownItems.Add(col);
                    }
            }
        }

        private void ClickOnColumnMenu(object sender, EventArgs e)
        {
            if ((sender != null) && (sender.GetType() == typeof(ToolStripMenuItem)))
            {
                ToolStripMenuItem menu = (ToolStripMenuItem)sender;
                if (menu.Checked)
                    ((ColumnHeader)menu.Tag).AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                else
                    ((ColumnHeader)menu.Tag).Width = 0;
            }
        }

        private void MonListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (((FilRipListViewComparateur)ListViewItemSorter).NumColonne == e.Column)
            {
                ((FilRipListViewComparateur)ListViewItemSorter).ChangeOrdre();
                this.Sort();
            }
            else
                ((FilRipListViewComparateur)ListViewItemSorter).NumColonne = e.Column;
        }
    }
}
