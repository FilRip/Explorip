using System;

namespace Explorip.WinAPI.Modeles
{
    public enum HookType
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    public class HookEventArgs : EventArgs
    {
        public int HookCode { get; set; }	// Hook code
        public IntPtr WParam { get; set; }	// WPARAM argument
        public IntPtr LParam { get; set; }	// LPARAM argument
    }

    public class LocalWindowsHook
    {
        // ************************************************************************
        // Filter function delegate
        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
        // ************************************************************************

        // ************************************************************************
        // Internal properties
        protected IntPtr m_hhook = IntPtr.Zero;
        protected HookProc m_filterFunc = null;
        protected HookType m_hookType;
        // ************************************************************************

        // ************************************************************************
        // Event delegate
        public delegate void HookEventHandler(object sender, HookEventArgs e);
        // ************************************************************************

        // ************************************************************************
        // Event: HookInvoked 
        public event HookEventHandler HookInvoked;
        protected void OnHookInvoked(HookEventArgs e)
        {
            HookInvoked?.Invoke(this, e);
        }
        // ************************************************************************

        // ************************************************************************
        // Class constructor(s)
        public LocalWindowsHook(HookType hook)
        {
            m_hookType = hook;
            m_filterFunc = new HookProc(CoreHookProc);
        }
        public LocalWindowsHook(HookType hook, HookProc func)
        {
            m_hookType = hook;
            m_filterFunc = func;
        }
        // ************************************************************************

        // ************************************************************************
        // Default filter function
        protected int CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return User32.CallNextHookEx(m_hhook, code, wParam, lParam);

            // Let clients determine what to do
            HookEventArgs e = new()
            {
                HookCode = code,
                WParam = wParam,
                LParam = lParam
            };
            OnHookInvoked(e);

            // Yield to the next hook in the chain
            return User32.CallNextHookEx(m_hhook, code, wParam, lParam);
        }
        // ************************************************************************

        // ************************************************************************
        // Install the hook
        public void Install()
        {
            m_hhook = User32.SetWindowsHookEx(
                m_hookType,
                m_filterFunc,
                IntPtr.Zero,
                System.Threading.Thread.CurrentThread.ManagedThreadId);
        }
        // ************************************************************************

        // ************************************************************************
        // Uninstall the hook
        public void Uninstall()
        {
            User32.UnhookWindowsHookEx(m_hhook);
        }
        // ************************************************************************
    }
}
