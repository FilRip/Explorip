using Filexplorip.WinAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Filexplorip.Forms
{
    public partial class FormTaskBar : Form
    {
        //private readonly List<Process> _precedenteListe;
        private readonly int _uwpHostProcess;

        public FormTaskBar()
        {
            InitializeComponent();
            //_precedenteListe = new List<Process>();
            _uwpHostProcess = Process.GetProcesses().Single(item => item.Id!=Process.GetCurrentProcess().Id && item.MainWindowHandle != IntPtr.Zero && !string.IsNullOrEmpty(item.MainModule.FileName) && item.MainModule.FileName.ToLower().Trim() == Environment.GetFolderPath(Environment.SpecialFolder.System).ToLower().Trim() + @"\applicationframehost.exe").Id;
        }

        private void TimerRefresh_Tick(object sender, EventArgs e)
        {
            try
            {
                timerRefresh.Enabled = false;
                // Managed version
                /*List<Process> nouvelleListe = new List<Process>();
                foreach (Process processus in Process.GetProcesses().Where(item => item.Responding && item.Id != Process.GetCurrentProcess().Id && !string.IsNullOrWhiteSpace(item.MainWindowTitle) && (item.MainModule.FileName.ToLower() != Environment.GetFolderPath(Environment.SpecialFolder.System).ToLower() + @"\applicationframehost.exe")).ToList())
                {
                    nouvelleListe.Add(processus);
                }

                if (ListeDifferentes(nouvelleListe))
                {
                    _precedenteListe = nouvelleListe;
                    lvTaches.Items.Clear();
                    lvTaches.LargeImageList = new ImageList();
                    foreach (Process processus in _precedenteListe)
                    {
                        lvTaches.LargeImageList.Images.Add(Helpers.Icones.GetFileIcon(processus.MainModule.FileName, false, false));
                        lvTaches.Items.Add(processus.MainWindowTitle);
                        lvTaches.Items[lvTaches.Items.Count - 1].ImageIndex = lvTaches.Items.Count - 1;
                    }
                }*/
                UnManagedVersion();
            }
            catch (Exception) { }
            finally
            {
                //timerRefresh.Enabled = true;
            }
        }

        /*private bool ListeDifferentes(List<Process> nouvelleListe)
        {
            if (nouvelleListe.Count != _precedenteListe.Count)
                return true;
            foreach (Process processus in nouvelleListe)
            {
                if (_precedenteListe.Any(item => item.Id == processus.Id))
                {
                    if (_precedenteListe.Single(item => item.Id == processus.Id).MainWindowTitle != processus.MainWindowTitle)
                        return true;
                }
                else
                    return true;
            }
            return false;
        }*/


        // Save window titles and handles in these lists.
        private static List<IntPtr> WindowHandles;
        private static List<string> WindowTitles;

        // Return a list of the desktop windows' handles and titles.
        public static void GetDesktopWindowHandlesAndTitles(
            out List<IntPtr> handles, out List<string> titles)
        {
            WindowHandles = new List<IntPtr>();
            WindowTitles = new List<string>();

            if (!User32.EnumDesktopWindows(IntPtr.Zero, FilterCallback,
                IntPtr.Zero))
            {
                handles = null;
                titles = null;
            }
            else
            {
                handles = WindowHandles;
                titles = WindowTitles;
            }
        }

        // We use this function to filter windows.
        // This version selects visible windows that have titles.
        private static bool FilterCallback(IntPtr hWnd, int lParam)
        {
            // Get the window's title.
            StringBuilder sb_title = new StringBuilder(1024);
            _ = User32.GetWindowText(hWnd, sb_title, sb_title.Capacity);
            string title = sb_title.ToString();

            // If the window is visible and has a title, save it.
            if (User32.IsWindowVisible(hWnd) &&
                !string.IsNullOrEmpty(title) && hWnd != User32.GetShellWindow())
            {
                WindowHandles.Add(hWnd);
                WindowTitles.Add(title);
            }

            // Return true to indicate that we
            // should continue enumerating windows.
            return true;
        }

        private void UnManagedVersion()
        {
            Process process;

            GetDesktopWindowHandlesAndTitles(out List<IntPtr> pointeurs, out List<string> titres);
            if (titres != null)
                for (int i = 0; i < titres.Count; i++)
                {
                    User32.GetWindowThreadProcessId(pointeurs[i], out uint processId);
                    process = Process.GetProcessById((int)processId);
                    if ((process.Id != _uwpHostProcess) || (process.Responding))
                        lvTaches.Items.Add($"({processId}) = {titres[i]}");
                }
        }
    }
}
