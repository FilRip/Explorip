namespace WindowsDesktop.Interop
{
    [ComInterfaceWrapper]
    internal class VirtualDesktopPinnedApps(ComInterfaceAssembly assembly) : ComInterfaceWrapperBase(assembly, service: ClSid.VirtualDesktopPinnedApps)
    {
        public bool IsViewPinned(ApplicationView applicationView)
        {
            return this.Invoke<bool>(Args(applicationView.ComObject));
        }

        public void PinView(ApplicationView applicationView)
        {
            this.Invoke(Args(applicationView.ComObject));
        }

        public void UnpinView(ApplicationView applicationView)
        {
            this.Invoke(Args(applicationView.ComObject));
        }

        public bool IsAppIdPinned(string appId)
        {
            return this.Invoke<bool>(Args(appId));
        }

        public void PinAppID(string appId)
        {
            this.Invoke(Args(appId));
        }

        public void UnpinAppID(string appId)
        {
            this.Invoke(Args(appId));
        }
    }
}
