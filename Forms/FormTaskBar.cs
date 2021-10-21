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
        }

        private void TimerRefresh_Tick(object sender, EventArgs e)
        {
            try
            {
                timerRefresh.Enabled = false;
                lvTaches.Items.Clear();
                lvTaches.SmallImageList.Images.Clear();
                ManagedVersion();
            }
            catch (Exception) { }
            finally
            {
                timerRefresh.Enabled = true;
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
                        lvTaches.SmallImageList.Images.Add(p.MainModule.FileName, Icones.GetIcone(p.MainModule.FileName, false, false, false, true));
                    lvTaches.Items[lvTaches.Items.Count - 1].ImageKey = p.MainModule.FileName;
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams; // Retrieve the normal parameters.
                cp.Style = 0x40000000 | 0x4000000; // WS_CHILD | WS_CLIPSIBLINGS
                cp.ExStyle &= 0x00080000; // WS_EX_LAYERED
                //cp.Parent = GetDesktopWindow(); // Make "GetDesktopWindow()" from your own namespace.
                return cp;
            }
        }
    }
}
