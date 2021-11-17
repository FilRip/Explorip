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
        private static bool GetMenuItem(int IdOrPositionMenu, IntPtr pointeurMenu, bool usePosition, out string libelle, out uint IdCmd, out Bitmap icone, out MFS etat)
        {
            icone = null;
            libelle = null;
            IdCmd = 0;
            etat = MFS.ENABLED;
            try
            {
                MENUITEMINFO sortie = new MENUITEMINFO
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                    dwTypeData = new string('\0', 256),
                    fMask = MIIM.STRING | MIIM.STATE | MIIM.ID | MIIM.BITMAP,
                    fType = MFT.STRING | MFT.DISABLED | MFT.GRAYED
                };
                sortie.cch = sortie.dwTypeData.Length - 1;
                if (User32.GetMenuItemInfo(pointeurMenu, (uint)IdOrPositionMenu, usePosition, ref sortie))
                {
                    libelle = sortie.dwTypeData;
                    if (sortie.hbmpItem != IntPtr.Zero)
                    {
                        icone = Image.FromHbitmap(sortie.hbmpItem);
                        icone.MakeTransparent();
                    }
                    etat = sortie.fState;
                    IdCmd = sortie.wID;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            return false;           
        }

        public static void CopierVersCms(ContextMenuStrip cms, ToolStripMenuItem sousMenu, IntPtr pointeurMenu, EventHandler ClickMenu, bool background)
        {
            if (pointeurMenu != IntPtr.Zero)
            {
                int nbMenu = User32.GetMenuItemCount(pointeurMenu);
                if (nbMenu > 0)
                {
                    int IdMenu;
                    string libelle;
                    uint IdCmd;
                    Bitmap icone;
                    MFS etat;
                    ToolStripItem menuAAjouter;
                    for (int i = 0; i < nbMenu; i++)
                    {
                        IdMenu = User32.GetMenuItemID(pointeurMenu, i);
                        if (IdMenu > 0)
                        {
                            GetMenuItem(IdMenu, pointeurMenu, false, out libelle, out IdCmd, out icone, out etat);//)
                            if (string.IsNullOrWhiteSpace(libelle))
                            {
                                if (cms.Items[cms.Items.Count - 1].GetType() == typeof(ToolStripSeparator))
                                    continue;
                                menuAAjouter = new ToolStripSeparator();
                            }
                            else
                            {
                                menuAAjouter = new ToolStripMenuItem()
                                {
                                    Text = libelle,
                                    Image = icone,
                                    Tag = IdCmd,
                                    Enabled = (etat == MFS.ENABLED),
                                };
                                menuAAjouter.Click += ClickMenu;
                                if (background)
                                {
                                    if (libelle.Trim().ToLower().Replace("&", "") == Constantes.Localisation.GetInstance().LIBELLE_COLLER.Trim().ToLower() ||
                                        libelle.Trim().ToLower().Replace("&", "") == Constantes.Localisation.GetInstance().LIBELLE_COLLER_RACCOURCI.Trim().ToLower())
                                    {
                                        if (FilesOperations.ContextMenuPaste.CollerDispo())
                                        {
                                            menuAAjouter.Enabled = true;
                                            menuAAjouter.Click -= ClickMenu;
                                            menuAAjouter.Click += FilesOperations.ContextMenuPaste.MenuColler;
                                        }
                                    }
                                }
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
                            GetMenuItem(i, pointeurMenu, true, out libelle, out IdCmd, out icone, out etat);
                            if (string.IsNullOrWhiteSpace(libelle))
                            {
                                if (cms.Items[cms.Items.Count - 1].GetType() != typeof(ToolStripSeparator))
                                    cms.Items.Add(new ToolStripSeparator());
                            }
                            else
                            {
                                cms.Items.Add(new ToolStripMenuItem()
                                {
                                    Text = libelle,
                                    Image = icone,
                                    Tag = IdCmd,
                                    Enabled = (etat == MFS.ENABLED),
                                });

                                CopierVersCms(cms, (ToolStripMenuItem)cms.Items[cms.Items.Count - 1], IdSousMenu, ClickMenu, background);
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
