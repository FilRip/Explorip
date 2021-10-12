using Filexplorip.Helpers;
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
        private readonly int _uwpHostProcess;

        public FormTaskBar()
        {
            InitializeComponent();
            //_precedenteListe = new List<Process>();
            _uwpHostProcess = Process.GetProcesses().Single(item => item.Id!=Process.GetCurrentProcess().Id &&
                                                            item.MainWindowHandle != IntPtr.Zero &&
                                                            !string.IsNullOrEmpty(item.MainModule.FileName) &&
                                                            item.MainModule.FileName.ToLower().Trim() == Environment.GetFolderPath(Environment.SpecialFolder.System).ToLower().Trim() + @"\applicationframehost.exe").Id;
        }

        private void TimerRefresh_Tick(object sender, EventArgs e)
        {
            try
            {
                timerRefresh.Enabled = false;
                lvTaches.Items.Clear();
                UnManagedVersion();
            }
            catch (Exception) { }
            finally
            {
                //timerRefresh.Enabled = true;
            }
        }

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
            WinAPI.Modeles.WINDOWPLACEMENT windowState = new WinAPI.Modeles.WINDOWPLACEMENT();
            windowState.length = System.Runtime.InteropServices.Marshal.SizeOf(typeof(WinAPI.Modeles.WINDOWPLACEMENT));
            WinAPI.Modeles.PROCESS_BASIC_INFORMATION pbi = new WinAPI.Modeles.PROCESS_BASIC_INFORMATION();

            Process processusParent;
            GetDesktopWindowHandlesAndTitles(out List<IntPtr> pointeurs, out List<string> titres);
            if (titres != null)
                for (int i = 0; i < titres.Count; i++)
                {
                    User32.GetWindowThreadProcessId(pointeurs[i], out uint processId);
                    process = Process.GetProcessById((int)processId);
                    User32.GetWindowPlacement(process.MainWindowHandle, ref windowState);
                    int status = Ntdll.NtQueryInformationProcess(process.Handle, Ntdll.PROCESSINFOCLASS.ProcessBasicInformation, ref pbi, System.Runtime.InteropServices.Marshal.SizeOf(typeof(WinAPI.Modeles.PROCESS_BASIC_INFORMATION)), out int returnSize);
                    if (status != 0)
                    {
                        processusParent = null;
                    }
                    else
                    {
                        processusParent = Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
                    }
                    if ((process.Id != _uwpHostProcess || (process.Responding)) && (process.Id != Process.GetCurrentProcess().Id) && (windowState.showCmd != WinAPI.Modeles.ShowWindowCommands.Hide))
                    {
                        lvTaches.Items.Add($"({processId}) = {process.ProcessName} | {titres[i]} | Responding={process.Responding}, Parent={processusParent?.ProcessName}");
                    }
                }
        }
    }
}
