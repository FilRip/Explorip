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
        private ToolStripMenuItem _menuColonnes;
        private readonly FilRipColumnHeaderCollection columnHeaderCollection;
        private const string titreSousMenuColonne = "Colonnes affichées";

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public FilRipListView()
        {
            ColumnClick += new ColumnClickEventHandler(MonListView_ColumnClick);
#pragma warning disable S125
            //ListViewItemSorter = new FilRipListViewComparateur();
#pragma warning restore S125
            MouseDown += ConstruireMenuColonnes;
            OwnerDraw = true;
            DrawItem += MonListView_DrawItem;
            DrawSubItem += MonListView_DrawSubItem;
            DrawColumnHeader += MonListView_DrawColumnHeader;
            columnHeaderCollection = new FilRipColumnHeaderCollection(this);
        }

        /// <summary>
        /// Change le handler appelé lors du rafraichissement des entête de colonnes
        /// Si null, on remet par défaut
        /// </summary>
        /// <param name="nouvelHandler">Nouvel handler pointant sur une méthode pour dessiner les entetes de colonnes, si null, on remet par défaut</param>
        public void DefinirHandlerOfDrawColumnHeader(DrawListViewColumnHeaderEventHandler nouvelHandler)
        {
            DrawColumnHeader -= MonListView_DrawColumnHeader;
            if (nouvelHandler == null)
                DrawColumnHeader += MonListView_DrawColumnHeader;
            else
                DrawColumnHeader += nouvelHandler;
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
                    if (ActiverCouleurAlternee)
                    {
                        e.SubItem.BackColor = e.ItemIndex % 2 == 0 ? CouleurAlternee1 : CouleurAlternee2;
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
        public bool AddMenuShowColumns { get; set; }

        /// <summary>
        /// Active ou non l'alternative de couleur de fond une ligne sur deux
        /// </summary>
        [Category("Couleur alternée"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool ActiverCouleurAlternee { get; set; }

        /// <summary>
        /// Couleur de fond pour les lignes impaires
        /// </summary>
        [Category("Couleur alternée"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurAlternee1 { get; set; } = Color.White;

        /// <summary>
        /// Couleur de fond pour les lignes paires
        /// </summary>
        [Category("Couleur alternée"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color CouleurAlternee2 { get; set; } = Color.LightGray;

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
            if (!AddMenuShowColumns || Columns.Count == 0) return;

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
                            BackColor = BackColor,
                            ForeColor = ForeColor
                        };
                        col.Click += ClickOnColumnMenu;
                        _menuColonnes.DropDownItems.Add(col);
                    }
            }
        }

        private void ClickOnColumnMenu(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menu)
            {
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
                Sort();
            }
            else
                ((FilRipListViewComparateur)ListViewItemSorter).NumColonne = e.Column;
        }
    }
}
