using Explorip.Helpers;
using Explorip.WinAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Explorip.Forms
{
    // TODO : Utiliser ManagedShell pour la barre des taches
    // Et peut etre : https://github.com/dbarros/WindowsAPICodePack
    // Voir https://github.com/dremin/RetroBar
    // https://stackoverflow.com/questions/24081665/windows-api-code-pack-where-is-it/25048686#25048686
    // Si utilise WinForm, voir :
    // http://www.codeproject.com/KB/cs/floatingform.aspx
    // http://www.codeproject.com/KB/miscctrl/magicdocking.aspx?target=dockable|controls
    // https://sourceforge.net/projects/dockpanelsuite/
    // https://www.codeproject.com/Articles/17729/Add-Docking-and-Floating-Support-Easily-and-Quickl
    public partial class FormTaskBar : Form
    {
        public FormTaskBar()
        {
            InitializeComponent();
            lvTaches.SmallImageList = new ImageList();
            lvTaches.LargeImageList = new ImageList();
        }

        private void TimerRefresh_Tick(object sender, EventArgs e)
        {
            try
            {
                timerRefresh.Enabled = false;
                lvTaches.Items.Clear();
                lvTaches.SmallImageList.Images.Clear();
                lvTaches.LargeImageList.Images.Clear();
                ManagedVersion();
            }
            catch (Exception) { }
            finally
            {
                //timerRefresh.Enabled = true;
            }
        }

        private void ManagedVersion()
        {
            foreach (Process p in Process.GetProcesses())
            {
                if ((p.MainWindowHandle != IntPtr.Zero) && (!string.IsNullOrWhiteSpace(p.MainWindowTitle)) && (p.Responding) && (User32.IsWindowVisible(p.MainWindowHandle)))
                {
                    lvTaches.Items.Add($"({p.Id}) {p.ProcessName} | {p.MainWindowTitle} | NomModule={p.MainModule.ModuleName}");
                    if (!lvTaches.SmallImageList.Images.ContainsKey(p.MainModule.FileName))
                    {
                        lvTaches.SmallImageList.Images.Add(p.MainModule.FileName, Icones.GetIcone(p.MainModule.FileName, false, false, false, true));
                    }
                    if (!lvTaches.LargeImageList.Images.ContainsKey(p.MainModule.FileName))
                    {
                        lvTaches.LargeImageList.Images.Add(p.MainModule.FileName, Icones.GetIcone(p.MainModule.FileName, false, false, false, false));
                    }
                    lvTaches.Items[lvTaches.Items.Count - 1].ImageKey = p.MainModule.FileName;
                }
            }
        }

        private void VerrouillerLaBarreDesTachesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                VerrouillerLaBarreDesTachesToolStripMenuItem.Checked = !VerrouillerLaBarreDesTachesToolStripMenuItem.Checked;
                if (VerrouillerLaBarreDesTachesToolStripMenuItem.Checked)
                    FormBorderStyle = FormBorderStyle.FixedToolWindow;
                else
                    FormBorderStyle = FormBorderStyle.SizableToolWindow;
            }
            catch (Exception) { }
        }

        private void QuitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^{ESC}");
        }
    }
}
