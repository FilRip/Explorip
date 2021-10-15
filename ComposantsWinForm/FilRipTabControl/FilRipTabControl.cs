using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Explorip.ComposantsWinForm.FilRipTabControl
{
    /// <summary>
    /// Composant de gestion d'onglet avec une gestion avancée des couleurs
    /// </summary>
    [ToolboxBitmap(typeof(TabControl))]
    public class FilRipTabControl : TabControl
    {
        #region Champs

        private Bitmap _BackImage;
        private Bitmap _BackBuffer;
        private Graphics _BackBufferGraphics;
        private Bitmap _TabBuffer;
        private Graphics _TabBufferGraphics;
        private readonly Color[] _toolBorder;

        private Point _Padding;
        private bool _HotTrack;

        private int _Radius = 1;
        private int _Overlap;
        private bool _FocusTrack;
        private float _Opacity = 1;

        private Color _BorderColorSelected = Color.Empty;
        private Color _BorderColor = Color.Empty;
        private Color _BorderColorHot = Color.Empty;

        private Color _FocusColor = Color.Empty;

        private Color _TextColor = Color.Empty;
        private Color _TextColorSelected = Color.Empty;
        private Color _TextColorDisabled = Color.Empty;

        private Color _backgroundColorDebut = Color.FromArgb(207, 207, 207);
        private Color _backgroundColorFin = Color.FromArgb(242, 242, 242);
        private Color _backgroundColorSelectedDebut = SystemColors.ControlLight;
        private Color _backgroundColorSelectedFin = SystemColors.Window;
        private Color _backgroundColorHotDebut = Color.FromArgb(234, 246, 253);
        private Color _backgroundColorHotFin = Color.FromArgb(167, 217, 245);
        private FontStyle _styleText = FontStyle.Regular;
        private FontStyle _styleTextSelected = FontStyle.Regular;
        private int _lastActiveIndex = -1;

        #endregion

        #region	Constructeur/Destructeur

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public FilRipTabControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw, true);

            _BackBuffer = new Bitmap(Width, Height);
            _BackBufferGraphics = Graphics.FromImage(_BackBuffer);
            _TabBuffer = new Bitmap(Width, Height);
            _TabBufferGraphics = Graphics.FromImage(_TabBuffer);

            _BorderColor = Color.Empty;
            _BorderColorSelected = Color.Empty;
            _FocusColor = Color.Orange;
            _HotTrack = true;
            //	Must set after the _Overlap as this is used in the calculations of the actual padding
            _Padding = new Point(6, 3);
            _FocusTrack = true;
            _Radius = 2;
            _toolBorder = new Color[] { Color.FromArgb(127, 157, 185), Color.FromArgb(164, 185, 127), Color.FromArgb(165, 172, 178), Color.FromArgb(132, 130, 132) };
        }

        /// <summary>
        /// Pour gérer les onglets qui vont de droite à gauche
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (RightToLeftLayout)
                    cp.ExStyle = cp.ExStyle | 0x400000 | 0x100000;
                return cp;
            }
        }

        /// <summary>
        /// Destructeur de base
        /// </summary>
        /// <param name="disposing">Faut-il détruire les objets managés ?</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_BackImage != null)
                {
                    _BackImage.Dispose();
                }
                if (_BackBufferGraphics != null)
                {
                    _BackBufferGraphics.Dispose();
                }
                if (_BackBuffer != null)
                {
                    _BackBuffer.Dispose();
                }
                if (_TabBufferGraphics != null)
                {
                    _TabBufferGraphics.Dispose();
                }
                if (_TabBuffer != null)
                {
                    _TabBuffer.Dispose();
                }
            }
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Ajoute une marge dans le titre de l'onglet. Obsolete : utiliser Radius
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new Point Padding
        {
            get { return _Padding; }
            set
            {
                _Padding = value;
                if (value.X + _Radius / 2 < 1)
                {
                    base.Padding = new Point(0, value.Y);
                }
                else
                {
                    base.Padding = new Point(value.X + (_Radius / 2) - 1, value.Y);
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Permet de gérer les marges pour les titres des onglets tout en gardant la gestion des couleurs
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public int Radius
        {
            get { return _Radius; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("The radius must be greater than 1", "value");
                }
                _Radius = value;
                //	Adjust padding
                Padding = _Padding;
            }
        }

        /// <summary>
        /// Permet de mettre l'onglet actif par dessus les onglets qui le juxte 
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public int Overlap
        {
            get { return _Overlap; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("The tabs cannot have a negative overlap", "value");
                }
                _Overlap = value;
            }
        }

        /// <summary>
        /// Active une barre de couleur au dessus du titre de l'onglet actif
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool FocusTrack
        {
            get { return _FocusTrack; }
            set
            {
                _FocusTrack = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Active la mise en surbrillance des onglets que la souris survole
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new bool HotTrack
        {
            get { return _HotTrack; }
            set
            {
                _HotTrack = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Opacité de la barre des onglets
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public float Opacity
        {
            get { return _Opacity; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("The opacity must be between 0 and 1", "value");
                }
                if (value > 1)
                {
                    throw new ArgumentException("The opacity must be between 0 and 1", "value");
                }
                _Opacity = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de la bordure de l'onglet sélectionné
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color BorderColorSelected
        {
            get
            {
                if (_BorderColorSelected.IsEmpty)
                {
                    return _toolBorder[0];
                }
                else
                {
                    return _BorderColorSelected;
                }
            }
            set
            {
                if (value.Equals(_toolBorder[0]))
                {
                    _BorderColorSelected = Color.Empty;
                }
                else
                {
                    _BorderColorSelected = value;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de la barre de couleur au dessus du titre de l'onglet survolé par la souris
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color BorderColorHot
        {
            get
            {
                if (_BorderColorHot.IsEmpty)
                {
                    return SystemColors.ControlDark;
                }
                else
                {
                    return _BorderColorHot;
                }
            }
            set
            {
                if (value.Equals(SystemColors.ControlDark))
                {
                    _BorderColorHot = Color.Empty;
                }
                else
                {
                    _BorderColorHot = value;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de la bordure des onglets non sélectionnés
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color BorderColor
        {
            get
            {
                if (_BorderColor.IsEmpty)
                {
                    return SystemColors.ControlDark;
                }
                else
                {
                    return _BorderColor;
                }
            }
            set
            {
                if (value.Equals(SystemColors.ControlDark))
                {
                    _BorderColor = Color.Empty;
                }
                else
                {
                    _BorderColor = value;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur du titre des onglets non sélectionnés
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color TextColor
        {
            get
            {
                if (_TextColor.IsEmpty)
                {
                    return SystemColors.ControlText;
                }
                else
                {
                    return _TextColor;
                }
            }
            set
            {
                if (value.Equals(SystemColors.ControlText))
                {
                    _TextColor = Color.Empty;
                }
                else
                {
                    _TextColor = value;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur du titre de l'onglet sélectionné
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color TextColorSelected
        {
            get
            {
                if (_TextColorSelected.IsEmpty)
                {
                    return SystemColors.ControlText;
                }
                else
                {
                    return _TextColorSelected;
                }
            }
            set
            {
                if (value.Equals(SystemColors.ControlText))
                {
                    _TextColorSelected = Color.Empty;
                }
                else
                {
                    _TextColorSelected = value;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur du titre des onglets désactivés (en lecture seule)
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color TextColorDisabled
        {
            get
            {
                if (_TextColor.IsEmpty)
                {
                    return SystemColors.ControlDark;
                }
                else
                {
                    return _TextColorDisabled;
                }
            }
            set
            {
                if (value.Equals(SystemColors.ControlDark))
                {
                    _TextColorDisabled = Color.Empty;
                }
                else
                {
                    _TextColorDisabled = value;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de la barre au dessus du titre de l'onglet actif
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color FocusColor
        {
            get { return _FocusColor; }
            set
            {
                _FocusColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Active la possibilité d'avoir plusieurs lignes dans la barre des onglets
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new bool Multiline
        {
            get
            {
                return base.Multiline;
            }
            set
            {
                base.Multiline = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Active (ou non) la lecture de droite à gauche des onglets
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override bool RightToLeftLayout
        {
            get
            {
                return base.RightToLeftLayout;
            }
            set
            {
                base.RightToLeftLayout = value;
                UpdateStyles();
            }
        }

        /// <summary>
        /// Alignement du titre des onglets dans leur cadres.
        /// </summary>
        [Category("Appearance")]
        public new TabAlignment Alignment
        {
            get
            {
                return base.Alignment;
            }
            set
            {
                base.Alignment = value;
                switch (value)
                {
                    case TabAlignment.Top:
                    case TabAlignment.Bottom:
                        Multiline = false;
                        break;
                    case TabAlignment.Left:
                    case TabAlignment.Right:
                        Multiline = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Apparence des boutons (obsolete)
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new TabAppearance Appearance
        {
            get
            {
                return base.Appearance;
            }
            set
            {
                base.Appearance = TabAppearance.Normal;
            }
        }

        /// <summary>
        /// Obtient la zone d'affichage du contrôle
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Rectangle DisplayRectangle
        {
            get
            {
                int itemHeight = ((Alignment == TabAlignment.Bottom || Alignment == TabAlignment.Top) ? ItemSize.Height : ItemSize.Width);

                //	Special processing to hide tabs
                int tabStripHeight = 5 + itemHeight * RowCount;

                Rectangle rect = new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);
                switch (Alignment)
                {
                    case TabAlignment.Top:
                        rect = new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);
                        break;
                    case TabAlignment.Bottom:
                        rect = new Rectangle(4, 4, Width - 8, Height - tabStripHeight - 4);
                        break;
                    case TabAlignment.Left:
                        rect = new Rectangle(tabStripHeight, 4, Width - tabStripHeight - 4, Height - 8);
                        break;
                    case TabAlignment.Right:
                        rect = new Rectangle(4, 4, Width - tabStripHeight - 4, Height - 8);
                        break;
                }
                return rect;
            }
        }

        /// <summary>
        /// Couleur de fond inférieur du bouton des onglets
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(Color), "FromArgb(207, 207, 207)")]
        public Color BackgroundColorDebut
        {
            get
            {
                return _backgroundColorDebut;
            }
            set
            {
                _backgroundColorDebut = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de fond inférieur du bouton de l'onglet survolé à la souris
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color BackgroundColorHotDebut
        {
            get
            {
                return _backgroundColorHotDebut;
            }
            set
            {
                _backgroundColorHotDebut = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de fond inférieur du bouton de l'onglet actif
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color BackgroundColorSelectedDebut
        {
            get
            {
                return _backgroundColorSelectedDebut;
            }
            set
            {
                _backgroundColorSelectedDebut = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de fond supérieur du bouton des onglets
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color BackgroundColorFin
        {
            get
            {
                return _backgroundColorFin;
            }
            set
            {
                _backgroundColorFin = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de fond supérieur du bouton de l'onglet survolé à la souris
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color BackgroundColorHotFin
        {
            get
            {
                return _backgroundColorHotFin;
            }
            set
            {
                _backgroundColorHotFin = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Couleur de fond supérieur du bouton de l'onglet actif
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color BackgroundColorSelectedFin
        {
            get
            {
                return _backgroundColorSelectedFin;
            }
            set
            {
                _backgroundColorSelectedFin = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Style du titre des onglets
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public FontStyle StyleText
        {
            get
            {
                return _styleText;
            }
            set
            {
                _styleText = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Style du titre de l'onglet actif
        /// </summary>
        [Category(nameof(FilRipTabControl)), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public FontStyle StyleTextSelected
        {
            get
            {
                return _styleTextSelected;
            }
            set
            {
                _styleTextSelected = value;
                Invalidate();
            }
        }

        #endregion

        #region	Evenements

        /// <summary>
        /// Evenement à la création du contrôle
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            OnFontChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Evenement reçu lors du changement de la police de caractère
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnFontChanged(EventArgs e)
        {
            IntPtr hFont = Font.ToHfont();

            Message msg = new Message
            {
                Msg = 0x30,
                WParam = hFont,
                LParam = (IntPtr)(-1),
                HWnd = Handle
            };
            WndProc(ref msg);

            msg = new Message
            {
                Msg = 0x1d,
                WParam = IntPtr.Zero,
                LParam = IntPtr.Zero,
                HWnd = Handle
            };
            WndProc(ref msg);

            UpdateStyles();

            if (Visible)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Evenement reçu lors du changement de la taille du contrôle
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnResize(EventArgs e)
        {
            //	Recreate the buffer for manual double buffering
            if (Width > 0 && Height > 0)
            {
                if (_BackImage != null)
                {
                    _BackImage.Dispose();
                    _BackImage = null;
                }
                if (_BackBufferGraphics != null)
                {
                    _BackBufferGraphics.Dispose();
                }
                if (_BackBuffer != null)
                {
                    _BackBuffer.Dispose();
                }

                _BackBuffer = new Bitmap(Width, Height);
                _BackBufferGraphics = Graphics.FromImage(_BackBuffer);

                if (_TabBufferGraphics != null)
                {
                    _TabBufferGraphics.Dispose();
                }
                if (_TabBuffer != null)
                {
                    _TabBuffer.Dispose();
                }

                _TabBuffer = new Bitmap(Width, Height);
                _TabBufferGraphics = Graphics.FromImage(_TabBuffer);

                if (_BackImage != null)
                {
                    _BackImage.Dispose();
                    _BackImage = null;
                }

            }
            base.OnResize(e);
        }

        /// <summary>
        /// Evenement reçu lorsque le parent de ce contrôle (le conteneur) change sa couleur de fond
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnParentBackColorChanged(EventArgs e)
        {
            if (_BackImage != null)
            {
                _BackImage.Dispose();
                _BackImage = null;
            }
            base.OnParentBackColorChanged(e);
        }

        /// <summary>
        /// Evenement reçu lorsque le parent de ce contrôle (le conteneur) change son image de fond
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnParentBackgroundImageChanged(EventArgs e)
        {
            if (_BackImage != null)
            {
                _BackImage.Dispose();
                _BackImage = null;
            }
            base.OnParentBackgroundImageChanged(e);
        }

        /// <summary>
        /// Evenement reçu lorsque le parent de ce contrôle (le conteneur) change sa taille
        /// </summary>
        /// <param name="sender">Parent (conteneur) de ce contrôle</param>
        /// <param name="e">Argument de l'evenement</param>
        private void OnParentResize(object sender, EventArgs e)
        {
            if (Visible)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Evenement reçu lorsque le parent de ce contrôle (le conteneur) change, passe à un autre conteneur
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent != null)
            {
                Parent.Resize += OnParentResize;
            }
        }

        /// <summary>
        /// Evenement reçu quand on déplace ce contrôle
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnMove(EventArgs e)
        {
            if (Width > 0 && Height > 0)
            {
                if (_BackImage != null)
                {
                    _BackImage.Dispose();
                    _BackImage = null;
                }
            }
            base.OnMove(e);
            Invalidate();
        }

        /// <summary>
        /// Evenement reçu quand le pointeur de la souris sort de ce contrôle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            _lastActiveIndex = -1;
            base.OnMouseLeave(e);
            Invalidate();
        }

        /// <summary>
        /// Evenement reçu lorsque le pointeur de la souris change de position dans ce contrôle
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_lastActiveIndex != ActiveIndex)
            {
                _lastActiveIndex = ActiveIndex;
                Invalidate();
            }
        }

        private bool _forceSelectParClick = false;
        /// <summary>
        /// Evenement reçu quand on demande à sélectionner un onglet
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnSelecting(TabControlCancelEventArgs e)
        {
            if (SizeMode == TabSizeMode.Fixed)
            {
                if ((!_forceSelectParClick) && (e.TabPageIndex < TabCount - 1))
                {
                    e.Cancel = true;
                }
                else
                {
                    _forceSelectParClick = false;
                }
            }
            else
            {
                base.OnSelecting(e);
            }
        }

        /// <summary>
        /// Evenement reçu quand l'utilisateur clique sur un des boutons des onglets
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (SizeMode == TabSizeMode.Fixed)
            {
                if ((_lastActiveIndex >= 0) && (_lastActiveIndex != SelectedIndex))
                {
                    _forceSelectParClick = true;
                    SelectedIndex = _lastActiveIndex;
                    Focus();
                    Invalidate();
                }
            }
            else
            {
                base.OnMouseClick(e);
            }
        }

        [Browsable(false)]
        private int ActiveIndex
        {
            get
            {
                if (TabCount > 0)
                {
                    for (int i = 0; i < TabCount; i++)
                    {
                        Rectangle rect = GetTabRect(i);
                        if (rect.Contains(MousePosition))
                        {
                            return i;
                        }
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// Evenement reçu quand on ajoute un onglet à ce contrôle
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (Visible)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Evenement reçu quand on supprime un onglet de ce contrôle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            if (Visible)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Gestion des touches raccourcis des onglets
        /// </summary>
        /// <param name="charCode">Caractère servant de touche raccourcis pour sélectionner l'onglet</param>
        /// <returns></returns>
        protected override bool ProcessMnemonic(char charCode)
        {
            foreach (TabPage page in TabPages)
            {
                if (IsMnemonic(charCode, page.Text))
                {
                    SelectedTab = page;
                    return true;
                }
            }
            return base.ProcessMnemonic(charCode);
        }

        #endregion

        #region	Dessine Tab

        /// <summary>
        /// Evenement déclenché à chaque fois que l'on redessine ce contrôle
        /// </summary>
        /// <param name="e">Argument de l'evenement</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //	We must always paint the entire area of the tab control
            if (e.ClipRectangle.Equals(ClientRectangle))
            {
                CustomPaint(e.Graphics);
            }
            else
            {
                //	it is less intensive to just reinvoke the paint with the whole surface available to draw on.
                Invalidate();
            }
        }

        private void CustomPaint(Graphics screenGraphics)
        {
            if (Width > 0 && Height > 0)
            {
                if (_BackImage == null)
                {
                    //	Cached Background Image
                    _BackImage = new Bitmap(Width, Height);
                    Graphics backGraphics = Graphics.FromImage(_BackImage);
                    backGraphics.Clear(Color.Transparent);
                    PaintTransparentBackground(backGraphics, ClientRectangle);
                }

                _BackBufferGraphics.Clear(Color.Transparent);
                _BackBufferGraphics.DrawImageUnscaled(_BackImage, 0, 0);

                _TabBufferGraphics.Clear(Color.Transparent);

                if (TabCount > 0)
                {
                    //	When top or bottom and scrollable we need to clip the sides from painting the tabs.
                    //	Left and right are always multiline.
                    if ((Alignment == TabAlignment.Bottom || Alignment == TabAlignment.Top) && !Multiline)
                    {
                        _TabBufferGraphics.Clip = new Region(new RectangleF(ClientRectangle.X + 3, ClientRectangle.Y, ClientRectangle.Width - 6, ClientRectangle.Height));
                    }

                    //	Draw each tabpage from right to left.  We do it this way to handle
                    //	the overlap correctly.
                    if (Multiline)
                    {
                        for (int row = 0; row < RowCount; row++)
                        {
                            for (int index = TabCount - 1; index >= 0; index--)
                            {
                                if (index != SelectedIndex && (RowCount == 1 || GetTabRow(index) == row))
                                {
                                    DrawTabPage(index, _TabBufferGraphics);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int index = TabCount - 1; index >= 0; index--)
                        {
                            if (index != SelectedIndex)
                            {
                                DrawTabPage(index, _TabBufferGraphics);
                            }
                        }
                    }

                    //	The selected tab must be drawn last so it appears on top.
                    if (SelectedIndex > -1)
                    {
                        DrawTabPage(SelectedIndex, _TabBufferGraphics);
                    }
                }
                _TabBufferGraphics.Flush();

                //	Paint the tabs on top of the background

                // Create a new color matrix and set the alpha value to 0.5
                ColorMatrix alphaMatrix = new ColorMatrix();
                alphaMatrix.Matrix00 = alphaMatrix.Matrix11 = alphaMatrix.Matrix22 = alphaMatrix.Matrix44 = 1;
                alphaMatrix.Matrix33 = Opacity;

                // Create a new image attribute object and set the color matrix to
                // the one just created
                using (ImageAttributes alphaAttributes = new ImageAttributes())
                {
                    alphaAttributes.SetColorMatrix(alphaMatrix);

                    // Draw the original image with the image attributes specified
                    _BackBufferGraphics.DrawImage(_TabBuffer,
                                                  new Rectangle(0, 0, _TabBuffer.Width, _TabBuffer.Height),
                                                  0, 0, _TabBuffer.Width, _TabBuffer.Height, GraphicsUnit.Pixel,
                                                  alphaAttributes);
                }

                _BackBufferGraphics.Flush();

                if (RightToLeftLayout)
                {
                    screenGraphics.DrawImageUnscaled(_BackBuffer, -1, 0);
                }
                else
                {
                    screenGraphics.DrawImageUnscaled(_BackBuffer, 0, 0);
                }
            }
        }

        private void PaintTab(int index, Graphics graphics)
        {
            using (GraphicsPath tabpath = GetTabBorder(index))
            {
                using (Brush fillBrush = GetTabBackgroundBrush(index))
                {
                    //	Paint the background
                    graphics.FillPath(fillBrush, tabpath);

                    //	Paint a focus indication
                    DrawTabFocusIndicator(tabpath, index, graphics);
                }
            }
        }

        private void PaintTransparentBackground(Graphics graphics, Rectangle clipRect)
        {
            if ((Parent != null))
            {

                //	Set the cliprect to be relative to the parent
                clipRect.Offset(Location);

                //	Save the current state before we do anything.
                GraphicsState state = graphics.Save();

                //	Set the graphicsobject to be relative to the parent
                graphics.TranslateTransform((float)-Location.X, (float)-Location.Y);
                graphics.SmoothingMode = SmoothingMode.HighSpeed;

                //	Paint the parent
                PaintEventArgs e = new PaintEventArgs(graphics, clipRect);
                try
                {
                    InvokePaintBackground(Parent, e);
                    InvokePaint(Parent, e);
                }
                finally
                {
                    //	Restore the graphics state and the clipRect to their original locations
                    graphics.Restore(state);
                    clipRect.Offset(-Location.X, -Location.Y);
                }
            }
        }

        private Brush GetPageBackgroundBrush(int index)
        {
            //	Capture the colours dependant on selection state of the tab
            Color light = Color.FromArgb(242, 242, 242);
            if (Alignment == TabAlignment.Top)
            {
                light = Color.FromArgb(207, 207, 207);
            }

            if (SelectedIndex == index)
            {
                light = SystemColors.Window;
            }
            else if (!TabPages[index].Enabled)
            {
                light = Color.FromArgb(207, 207, 207);
            }
            else if (_HotTrack && index == ActiveIndex)
            {
                //	Enable hot tracking
                light = Color.FromArgb(234, 246, 253);
            }

            return new SolidBrush(light);
        }

        private void DrawTabPage(int index, Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighSpeed;

            //	Get TabPageBorder
            using (GraphicsPath tabPageBorderPath = GetTabPageBorder(index))
            {

                //	Paint the background
                using (Brush fillBrush = GetPageBackgroundBrush(index))
                {
                    graphics.FillPath(fillBrush, tabPageBorderPath);
                }

                //	Paint the tab
                PaintTab(index, graphics);

                //	Draw the text
                DrawTabText(index, graphics);

                //	Paint the border
                DrawTabBorder(tabPageBorderPath, index, graphics);
            }
        }

        private void DrawTabFocusIndicator(GraphicsPath tabpath, int index, Graphics graphics)
        {
            if (_FocusTrack && index == SelectedIndex)
            {
                Brush focusBrush = null;
                RectangleF pathRect = tabpath.GetBounds();
                Rectangle focusRect = Rectangle.Empty;
                switch (Alignment)
                {
                    case TabAlignment.Top:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, (int)pathRect.Width, 4);
                        focusBrush = new LinearGradientBrush(focusRect, _FocusColor, SystemColors.Window, LinearGradientMode.Vertical);
                        break;
                    case TabAlignment.Bottom:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Bottom - 4, (int)pathRect.Width, 4);
                        focusBrush = new LinearGradientBrush(focusRect, SystemColors.ControlLight, _FocusColor, LinearGradientMode.Vertical);
                        break;
                    case TabAlignment.Left:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, 4, (int)pathRect.Height);
                        focusBrush = new LinearGradientBrush(focusRect, _FocusColor, SystemColors.ControlLight, LinearGradientMode.Horizontal);
                        break;
                    case TabAlignment.Right:
                        focusRect = new Rectangle((int)pathRect.Right - 4, (int)pathRect.Y, 4, (int)pathRect.Height);
                        focusBrush = new LinearGradientBrush(focusRect, SystemColors.ControlLight, _FocusColor, LinearGradientMode.Horizontal);
                        break;
                }

                //	Ensure the focus stip does not go outside the tab
                Region focusRegion = new Region(focusRect);
                focusRegion.Intersect(tabpath);
                graphics.FillRegion(focusBrush, focusRegion);
                focusRegion.Dispose();
                focusBrush.Dispose();
            }
        }

        private void DrawTabBorder(GraphicsPath path, int index, Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            Color borderColor;
            if (index == SelectedIndex)
            {
                borderColor = BorderColorSelected;
            }
            else if (HotTrack && index == ActiveIndex)
            {
                borderColor = BorderColorHot;
            }
            else
            {
                borderColor = BorderColor;
            }

            using (Pen borderPen = new Pen(borderColor))
            {
                if (index == 0)
                {
                    path.AddLine(new PointF(Margin.Left, Margin.Top), new PointF(Margin.Left, Margin.Top + GetTabRect(0).Height));
                }
                graphics.DrawPath(borderPen, path);
            }
        }

        private void DrawTabText(int index, Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            Rectangle tabBounds = GetTabTextRect(index);
            Font police;

            if (SelectedIndex == index)
            {
                police = new Font(Font, _styleTextSelected);
                using (Brush textBrush = new SolidBrush(TextColorSelected))
                {
                    graphics.DrawString(TabPages[index].Text, police, textBrush, tabBounds, GetStringFormat());
                }
            }
            else
            {
                police = new Font(Font, _styleText);
                if (TabPages[index].Enabled)
                {
                    using (Brush textBrush = new SolidBrush(TextColor))
                    {
                        graphics.DrawString(TabPages[index].Text, police, textBrush, tabBounds, GetStringFormat());
                    }
                }
                else
                {
                    using (Brush textBrush = new SolidBrush(TextColorDisabled))
                    {
                        graphics.DrawString(TabPages[index].Text, police, textBrush, tabBounds, GetStringFormat());
                    }
                }
            }
        }

        #endregion

        #region String formatting

        private StringFormat GetStringFormat()
        {
            StringFormat format = null;

            //	Rotate Text by 90 degrees for left and right tabs
            switch (Alignment)
            {
                case TabAlignment.Top:
                case TabAlignment.Bottom:
                    format = new StringFormat();
                    break;
                case TabAlignment.Left:
                case TabAlignment.Right:
                    format = new StringFormat(StringFormatFlags.DirectionVertical);
                    break;
            }
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            if (FindForm() != null && FindForm().KeyPreview)
            {
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
            }
            else
            {
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
            }
            if (RightToLeft == RightToLeft.Yes)
            {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }
            return format;
        }

        #endregion

        #region Dessine fond & images des Tab

        /// <summary>
        /// Taille par défaut des boutons pour les onglets.
        /// Attention : lecture seule quand SizeMode est à Fixed
        /// </summary>
        public new Size ItemSize
        {
            get
            {
                Size tailleMax = base.ItemSize;
                if ((TabCount > 0) && (SizeMode == TabSizeMode.Fixed))
                {
                    tailleMax = new Size();
                    for (int i = 0; i < TabCount; i++)
                    {
                        tailleMax.Width += GetTabRect(i).Width;
                        tailleMax.Height += GetTabRect(i).Height;
                    }
                    tailleMax.Width /= TabCount;
                    tailleMax.Height /= TabCount;
                    if (tailleMax != base.ItemSize)
                    {
                        base.ItemSize = tailleMax;
                    }
                }
                return tailleMax;
            }
            set
            {
                Size tailleMax = value;
                if ((TabCount > 0) && (SizeMode == TabSizeMode.Fixed))
                {
                    tailleMax = new Size();
                    for (int i = 0; i < TabCount; i++)
                    {
                        tailleMax.Width += GetTabRect(i).Width;
                        tailleMax.Height += GetTabRect(i).Height;
                    }
                    tailleMax.Width /= TabCount;
                    tailleMax.Height /= TabCount;
                }
                if (base.ItemSize != tailleMax)
                {
                    base.ItemSize = tailleMax;
                }
            }
        }

        /// <summary>
        /// Retourne les coordonnées du bouton de l'onglet dont le numéro est donné en paramètre
        /// </summary>
        /// <param name="index">Numéro (de base zéro) de l'onglet</param>
        public new Rectangle GetTabRect(int index)
        {
            Rectangle rect = base.GetTabRect(index);

            if (SizeMode == TabSizeMode.Fixed)
            {
                Size taille = TextRenderer.MeasureText(TabPages[index].Text, new Font(Font, FontStyle.Bold));
                rect.Width = taille.Width + 2;
                if (index > 0)
                {
                    Rectangle rectPrecedent = GetTabRect(index - 1);
                    rect.X = rectPrecedent.X + rectPrecedent.Width;
                }
            }

            return rect;
        }

        private Rectangle GetTabRectStyle(int index)
        {
            if (index < 0)
            {
                return new Rectangle();
            }

            Rectangle tabBounds = GetTabRect(index);

            if (RightToLeftLayout)
            {
                tabBounds.X = Width - tabBounds.Right;
            }
            bool firstTabinRow = IsFirstTabInRow(index);

            //	Expand to overlap the tabpage
            switch (Alignment)
            {
                case TabAlignment.Top:
                    tabBounds.Height += 2;
                    break;
                case TabAlignment.Bottom:
                    tabBounds.Height += 2;
                    tabBounds.Y -= 2;
                    break;
                case TabAlignment.Left:
                    tabBounds.Width += 2;
                    break;
                case TabAlignment.Right:
                    tabBounds.X -= 2;
                    tabBounds.Width += 2;
                    break;
            }

            //	Greate Overlap unless first tab in the row to align with tabpage
            if ((!firstTabinRow || RightToLeftLayout) && _Overlap > 0)
            {
                if (Alignment == TabAlignment.Bottom || Alignment == TabAlignment.Top)
                {
                    tabBounds.X -= _Overlap;
                    tabBounds.Width += _Overlap;
                }
                else
                {
                    tabBounds.Y -= _Overlap;
                    tabBounds.Height += _Overlap;
                }
            }

            //	Adjust first tab in the row to align with tabpage
            EnsureFirstTabIsInView(ref tabBounds, index);

            firstTabinRow = IsFirstTabInRow(index);

            //	Make non-SelectedTabs smaller and selected tab bigger
            switch (Alignment)
            {
                case TabAlignment.Top:
                    if (index == SelectedIndex)
                    {
                        if (tabBounds.Y > 0)
                        {
                            tabBounds.Y -= 1;
                            tabBounds.Height += 1;
                        }

                        if (firstTabinRow)
                        {
                            tabBounds.Width += 1;
                        }
                        else
                        {
                            tabBounds.X -= 1;
                            tabBounds.Width += 2;
                        }
                    }
                    else
                    {
                        tabBounds.Y += 1;
                        tabBounds.Height -= 1;
                    }
                    break;
                case TabAlignment.Bottom:
                    if (index == SelectedIndex)
                    {
                        if (tabBounds.Bottom < Bottom)
                        {
                            tabBounds.Height += 1;
                        }
                        if (firstTabinRow)
                        {
                            tabBounds.Width += 1;
                        }
                        else
                        {
                            tabBounds.X -= 1;
                            tabBounds.Width += 2;
                        }
                    }
                    else
                    {
                        tabBounds.Height -= 1;
                    }
                    break;
                case TabAlignment.Left:
                    if (index == SelectedIndex)
                    {
                        if (tabBounds.X > 0)
                        {
                            tabBounds.X -= 1;
                            tabBounds.Width += 1;
                        }

                        if (firstTabinRow)
                        {
                            tabBounds.Height += 1;
                        }
                        else
                        {
                            tabBounds.Y -= 1;
                            tabBounds.Height += 2;
                        }
                    }
                    else
                    {
                        tabBounds.X += 1;
                        tabBounds.Width -= 1;
                    }
                    break;
                case TabAlignment.Right:
                    if (index == SelectedIndex)
                    {
                        if (tabBounds.Right < Right)
                        {
                            tabBounds.Width += 1;
                        }
                        if (firstTabinRow)
                        {
                            tabBounds.Height += 1;
                        }
                        else
                        {
                            tabBounds.Y -= 1;
                            tabBounds.Height += 2;
                        }
                    }
                    else
                    {
                        tabBounds.Width -= 1;
                    }
                    break;
            }

            //	Adjust first tab in the row to align with tabpage
            EnsureFirstTabIsInView(ref tabBounds, index);

            return tabBounds;
        }

        private void EnsureFirstTabIsInView(ref Rectangle tabBounds, int index)
        {
            //	Adjust first tab in the row to align with tabpage
            //	Make sure we only reposition visible tabs, as we may have scrolled out of view.

            bool firstTabinRow = IsFirstTabInRow(index);

            if (firstTabinRow)
            {
                if (Alignment == TabAlignment.Bottom || Alignment == TabAlignment.Top)
                {
                    if (RightToLeftLayout)
                    {
                        if (tabBounds.Left < Right)
                        {
                            int tabPageRight = GetPageBounds(index).Right;
                            if (tabBounds.Right > tabPageRight)
                            {
                                tabBounds.Width -= (tabBounds.Right - tabPageRight);
                            }
                        }
                    }
                    else
                    {
                        if (tabBounds.Right > 0)
                        {
                            int tabPageX = GetPageBounds(index).X;
                            if (tabBounds.X < tabPageX)
                            {
                                tabBounds.Width -= (tabPageX - tabBounds.X);
                                tabBounds.X = tabPageX;
                            }
                        }
                    }
                }
                else
                {
                    if (RightToLeftLayout)
                    {
                        if (tabBounds.Top < Bottom)
                        {
                            int tabPageBottom = GetPageBounds(index).Bottom;
                            if (tabBounds.Bottom > tabPageBottom)
                            {
                                tabBounds.Height -= (tabBounds.Bottom - tabPageBottom);
                            }
                        }
                    }
                    else
                    {
                        if (tabBounds.Bottom > 0)
                        {
                            int tabPageY = GetPageBounds(index).Location.Y;
                            if (tabBounds.Y < tabPageY)
                            {
                                tabBounds.Height -= (tabPageY - tabBounds.Y);
                                tabBounds.Y = tabPageY;
                            }
                        }
                    }
                }
            }
        }

        private Brush GetTabBackgroundBrush(int index)
        {
            LinearGradientBrush fillBrush = null;

            //	Capture the colours dependant on selection state of the tab
            Color dark = _backgroundColorDebut;
            Color light = _backgroundColorFin;

            if (SelectedIndex == index)
            {
                dark = _backgroundColorSelectedDebut;
                light = _backgroundColorSelectedFin;
            }
            else if (!TabPages[index].Enabled)
            {
                light = dark;
            }
            else if (_HotTrack && index == ActiveIndex)
            {
                //	Enable hot tracking
                light = _backgroundColorHotDebut;
                dark = _backgroundColorHotFin;
            }

            //	Get the correctly aligned gradient
            Rectangle tabBounds = GetTabRectStyle(index);
            tabBounds.Inflate(3, 3);
            tabBounds.X -= 1;
            tabBounds.Y -= 1;
            switch (Alignment)
            {
                case TabAlignment.Top:
                    if (SelectedIndex == index)
                    {
                        dark = light;
                    }
                    fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical);
                    break;
                case TabAlignment.Bottom:
                    fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical);
                    break;
                case TabAlignment.Left:
                    fillBrush = new LinearGradientBrush(tabBounds, dark, light, LinearGradientMode.Horizontal);
                    break;
                case TabAlignment.Right:
                    fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Horizontal);
                    break;
            }

            //	Add the blend
            fillBrush.Blend = GetBackgroundBlend();

            return fillBrush;
        }

        private Blend GetBackgroundBlend()
        {
            float[] relativeIntensities = new float[] { 0f, 0.7f, 1f };
            float[] relativePositions = new float[] { 0f, 0.6f, 1f };

            //	Glass look to top aligned tabs
            if (Alignment == TabAlignment.Top)
            {
                relativeIntensities = new float[] { 0f, 0.5f, 1f, 1f };
                relativePositions = new float[] { 0f, 0.5f, 0.51f, 1f };
            }

            Blend blend = new Blend
            {
                Factors = relativeIntensities,
                Positions = relativePositions
            };

            return blend;
        }

        private GraphicsPath GetTabBorder(int index)
        {

            GraphicsPath path = new GraphicsPath();
            Rectangle tabBounds = GetTabRectStyle(index);

            AddTabBorder(path, tabBounds);

            path.CloseFigure();
            return path;
        }

        private GraphicsPath GetTabPageBorder(int index)
        {

            GraphicsPath path = new GraphicsPath();
            Rectangle pageBounds = GetPageBounds(index);
            Rectangle tabBounds = GetTabRect(index);

            AddTabBorder(path, tabBounds);
            AddPageBorder(path, pageBounds, tabBounds);

            path.CloseFigure();
            return path;
        }

        private Rectangle GetPageBounds(int index)
        {
            Rectangle pageBounds = TabPages[index].Bounds;
            pageBounds.Width += 1;
            pageBounds.Height += 1;
            pageBounds.X -= 1;
            pageBounds.Y -= 1;

            if (pageBounds.Bottom > Height - 4)
            {
                pageBounds.Height -= (pageBounds.Bottom - Height + 4);
            }

            return pageBounds;
        }

        private Rectangle GetTabTextRect(int index)
        {
            Rectangle textRect = new Rectangle();
            using (GraphicsPath path = GetTabBorder(index))
            {
                RectangleF tabBounds = path.GetBounds();

                textRect = new Rectangle((int)tabBounds.X, (int)tabBounds.Y, (int)tabBounds.Width, (int)tabBounds.Height);

                //	Make it shorter or thinner to fit the height or width because of the padding added to the tab for painting
                switch (Alignment)
                {
                    case TabAlignment.Top:
                        textRect.Y += 4;
                        textRect.Height -= 6;
                        break;
                    case TabAlignment.Bottom:
                        textRect.Y += 2;
                        textRect.Height -= 6;
                        break;
                    case TabAlignment.Left:
                        textRect.X += 4;
                        textRect.Width -= 6;
                        break;
                    case TabAlignment.Right:
                        textRect.X += 2;
                        textRect.Width -= 6;
                        break;
                }

                //	Ensure it fits inside the path at the centre line
                if (Alignment == TabAlignment.Bottom || Alignment == TabAlignment.Top)
                {
                    while (!path.IsVisible(textRect.Right, textRect.Y) && textRect.Width > 0)
                    {
                        textRect.Width -= 1;
                    }
                    while (!path.IsVisible(textRect.X, textRect.Y) && textRect.Width > 0)
                    {
                        textRect.X += 1;
                        textRect.Width -= 1;
                    }
                }
                else
                {
                    while (!path.IsVisible(textRect.X, textRect.Bottom) && textRect.Height > 0)
                    {
                        textRect.Height -= 1;
                    }
                    while (!path.IsVisible(textRect.X, textRect.Y) && textRect.Height > 0)
                    {
                        textRect.Y += 1;
                        textRect.Height -= 1;
                    }
                }
            }

            return textRect;
        }

        private int GetTabRow(int index)
        {
            //	All calculations will use this rect as the base point
            //	because the itemsize does not return the correct width.
            Rectangle rect = GetTabRect(index);

            int row = -1;

            switch (Alignment)
            {
                case TabAlignment.Top:
                    row = (rect.Y - 2) / rect.Height;
                    break;
                case TabAlignment.Bottom:
                    row = ((Height - rect.Y - 2) / rect.Height) - 1;
                    break;
                case TabAlignment.Left:
                    row = (rect.X - 2) / rect.Width;
                    break;
                case TabAlignment.Right:
                    row = ((Width - rect.X - 2) / rect.Width) - 1;
                    break;
            }
            return row;
        }

        private bool IsFirstTabInRow(int index)
        {
            if (index < 0)
            {
                return false;
            }
            bool firstTabinRow = (index == 0);
            if (!firstTabinRow)
            {
                if (Alignment == TabAlignment.Bottom || Alignment == TabAlignment.Top)
                {
                    if (GetTabRect(index).X == 2)
                    {
                        firstTabinRow = true;
                    }
                }
                else
                {
                    if (GetTabRect(index).Y == 2)
                    {
                        firstTabinRow = true;
                    }
                }
            }
            return firstTabinRow;
        }

        private void AddTabBorder(GraphicsPath path, Rectangle tabBounds)
        {
            switch (Alignment)
            {
                case TabAlignment.Top:
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    break;
                case TabAlignment.Bottom:
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    break;
                case TabAlignment.Left:
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    break;
                case TabAlignment.Right:
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    break;
            }
        }

        private void AddPageBorder(GraphicsPath path, Rectangle pageBounds, Rectangle tabBounds)
        {
            switch (Alignment)
            {
                case TabAlignment.Top:
                    path.AddLine(tabBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, tabBounds.X, pageBounds.Y);
                    break;
                case TabAlignment.Bottom:
                    path.AddLine(tabBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, tabBounds.Right, pageBounds.Bottom);
                    break;
                case TabAlignment.Left:
                    path.AddLine(pageBounds.X, tabBounds.Y, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, tabBounds.Bottom);
                    break;
                case TabAlignment.Right:
                    path.AddLine(pageBounds.Right, tabBounds.Bottom, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, tabBounds.Y);
                    break;
            }
        }

        /// <summary>
        /// Retourne la position du pointeur de la souris par rapport aux coordonnées de ce contrôle
        /// </summary>
        public new Point MousePosition
        {
            get
            {
                Point loc = PointToClient(Control.MousePosition);
                if (RightToLeftLayout)
                {
                    loc.X = (Width - loc.X);
                }
                return loc;
            }
        }

        #endregion
    }
}
