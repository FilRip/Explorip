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
                    lvTaches.SmallImageList.Images.Add(Icones.GetIcone(p.MainModule.FileName, false, false, false, false));
                    lvTaches.Items[lvTaches.Items.Count - 1].ImageIndex = lvTaches.SmallImageList.Images.Count - 1;
                }
            }
        }
    }
}
