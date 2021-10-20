using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Explorip.WinAPI;
using Explorip.WinAPI.Modeles;

namespace Explorip.Helpers
{
    public static class PopUpMenuExtensions
    {
        private static string GetMenuItemText(uint IdMenu, IntPtr pointeurMenu)
        {
            string retour = "";
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    dwTypeData = new string('\0', 256),
                    fMask = MIIM.STRING,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                sortie.cch = sortie.dwTypeData.Length - 1;
                if (User32.GetMenuItemInfo(pointeurMenu, IdMenu, false, ref sortie))
                {
                    retour = sortie.dwTypeData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private static string GetMenuItemText(int position, IntPtr pointeurMenu)
        {
            string retour = "";
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    dwTypeData = new string('\0', 256),
                    fMask = MIIM.STRING,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                sortie.cch = sortie.dwTypeData.Length - 1;
                if (User32.GetMenuItemInfo(pointeurMenu, (uint)position, true, ref sortie))
                {
                    retour = sortie.dwTypeData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private static Image GetMenuItemIcone(uint IdMenu, IntPtr pointeurMenu)
        {
            Bitmap retour = null;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    fMask = MIIM.BITMAP,
                };
                if (User32.GetMenuItemInfo(pointeurMenu, IdMenu, false, ref sortie))
                {
                    retour = Image.FromHbitmap(sortie.hbmpItem);
                    retour.MakeTransparent(Color.Black);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private static Image GetMenuItemIcone(int position, IntPtr pointeurMenu)
        {
            Bitmap retour = null;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    fMask = MIIM.BITMAP,
                };
                if (User32.GetMenuItemInfo(pointeurMenu, (uint)position, true, ref sortie))
                {
                    retour = Image.FromHbitmap(sortie.hbmpItem);
                    retour.MakeTransparent(Color.Black);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private static uint GetMenuItemID(int position, IntPtr pointeurMenu)
        {
            uint retour = 0;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    fMask = MIIM.ID,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                if (User32.GetMenuItemInfo(pointeurMenu, (uint)position, true, ref sortie))
                {
                    retour = sortie.wID;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private static uint GetMenuItemID(uint IdMenu, IntPtr pointeurMenu)
        {
            uint retour = 0;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    fMask = MIIM.ID,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                if (User32.GetMenuItemInfo(pointeurMenu, IdMenu, false, ref sortie))
                {
                    retour = sortie.wID;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private static MFS GetMenuItemState(uint IdMenu, IntPtr pointeurMenu)
        {
            MFS retour = MFS.DEFAULT;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    fMask = MIIM.STATE,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                if (User32.GetMenuItemInfo(pointeurMenu, IdMenu, false, ref sortie))
                {
                    retour = sortie.fState;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        private static MFS GetMenuItemState(int position, IntPtr pointeurMenu)
        {
            MFS retour = MFS.DEFAULT;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    fMask = MIIM.STATE,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                if (User32.GetMenuItemInfo(pointeurMenu, (uint)position, true, ref sortie))
                {
                    retour = sortie.fState;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return retour;
        }

        public static void CopierVersCms(ContextMenuStrip cms, ToolStripMenuItem sousMenu, IntPtr pointeurMenu, EventHandler ClickMenu)
        {
            if (pointeurMenu != IntPtr.Zero)
            {
                int nbMenu = User32.GetMenuItemCount(pointeurMenu);
                if (nbMenu > 0)
                {
                    int IdMenu;
                    string libelle;
                    ToolStripItem menuAAjouter;
                    for (int i = 0; i < nbMenu; i++)
                    {
                        IdMenu = User32.GetMenuItemID(pointeurMenu, i);
                        if (IdMenu > 0)
                        {
                            libelle = GetMenuItemText((uint)IdMenu, pointeurMenu).Trim();
                            if (string.IsNullOrWhiteSpace(libelle))
                            {
                                if (cms.Items[cms.Items.Count - 1].GetType() == typeof(ToolStripSeparator))
                                    continue;
                                menuAAjouter = new ToolStripSeparator();
                            }
                            else
                            {
                                MFS etat = GetMenuItemState((uint)IdMenu, pointeurMenu);
                                menuAAjouter = new ToolStripMenuItem()
                                {
                                    Text = libelle,
                                    Image = GetMenuItemIcone((uint)IdMenu, pointeurMenu),
                                    Tag = GetMenuItemID((uint)IdMenu, pointeurMenu),
                                    Enabled = (etat == MFS.ENABLED),
                                };
                                menuAAjouter.Click += ClickMenu;
                            }
                            if (sousMenu == null)
                            {
                                cms.Items.Add(menuAAjouter);
                            }
                            else
                            {
                                if ((menuAAjouter.GetType() == typeof(ToolStripSeparator)) && (nbMenu == 1))
                                    cms.Items.Add(menuAAjouter);
                                else
                                {
                                    if ((sousMenu.DropDownItems.Count > 0) && (sousMenu.DropDownItems[sousMenu.DropDownItems.Count - 1].GetType() == typeof(ToolStripSeparator)) && (menuAAjouter.GetType() == typeof(ToolStripSeparator)))
                                        continue;
                                    sousMenu.DropDownItems.Add(menuAAjouter);
                                }
                            }
                        }
                        else if (IdMenu < 0)
                        {
                            IntPtr IdSousMenu = User32.GetSubMenu(pointeurMenu, i);
                            libelle = GetMenuItemText(i, pointeurMenu);
                            if (string.IsNullOrWhiteSpace(libelle))
                            {
                                if (cms.Items[cms.Items.Count - 1].GetType() != typeof(ToolStripSeparator))
                                    cms.Items.Add(new ToolStripSeparator());
                            }
                            else
                            {
                                MFS etat = GetMenuItemState(i, pointeurMenu);
                                cms.Items.Add(new ToolStripMenuItem()
                                {
                                    Text = libelle,
                                    Image = GetMenuItemIcone(i, pointeurMenu),
                                    Tag = GetMenuItemID(i, pointeurMenu),
                                    Enabled = (etat == MFS.ENABLED),
                                });

                                CopierVersCms(cms, (ToolStripMenuItem)cms.Items[cms.Items.Count - 1], IdSousMenu, ClickMenu);
                            }
                        }
                    }
                    if ((cms != null) && (cms.Items.Count > 0))
                    {
                        if (cms.Items[cms.Items.Count - 1].GetType() == typeof(ToolStripSeparator))
                            cms.Items.RemoveAt(cms.Items.Count - 1);
                    }
                    if ((sousMenu != null) && (sousMenu.DropDownItems != null) && (sousMenu.DropDownItems.Count > 0))
                    {
                        if (sousMenu.DropDownItems[sousMenu.DropDownItems.Count - 1].GetType() == typeof(ToolStripSeparator))
                            sousMenu.DropDownItems.RemoveAt(sousMenu.DropDownItems.Count - 1);
                    }
                }
            }
        }

        public static void EtendreSousMenuPopUp(IntPtr pointeurMenu, IContextMenu2 COMMenu)
        {
            if (pointeurMenu == IntPtr.Zero)
                return;
            int nbMenu = User32.GetMenuItemCount(pointeurMenu);
            if (nbMenu > 0)
            {
                int IdMenu;
                for (int i = 0; i < nbMenu; i++)
                {
                    IdMenu = User32.GetMenuItemID(pointeurMenu, i);
                    if (IdMenu < 0)
                    {
                        IntPtr IdSousMenu = User32.GetSubMenu(pointeurMenu, i);
                        COMMenu.HandleMenuMsg((int)Commun.WM.INITMENUPOPUP, IdSousMenu, (IntPtr)i);
                    }
                }
            }
        }
    }
}
