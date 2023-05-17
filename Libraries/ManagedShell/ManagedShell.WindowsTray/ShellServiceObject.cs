using System;

using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;

using static ManagedShell.Interop.NativeMethods;

namespace ManagedShell.WindowsTray
{
    public class ShellServiceObject : IDisposable
    {
        const string CGID_SHELLSERVICEOBJECT = "000214D2-0000-0000-C000-000000000046";
        private IOleCommandTarget sysTrayObject;

        public void Start()
        {
            if (EnvironmentHelper.IsAppRunningAsShell)
            {
                try
                {
                    sysTrayObject = (IOleCommandTarget)new SysTrayObject();
                    Guid sso = new(CGID_SHELLSERVICEOBJECT);
                    sysTrayObject.Exec(ref sso, OLECMDID_NEW, OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero);
                }
                catch
                {
                    ShellLogger.Debug("ShellServiceObject: Unable to start");
                }
            }
        }

        private bool _isDisposed;
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (sysTrayObject != null)
                {
                    try
                    {
                        Guid sso = new(CGID_SHELLSERVICEOBJECT);
                        sysTrayObject.Exec(ref sso, OLECMDID_SAVE, OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero);
                    }
                    catch
                    {
                        ShellLogger.Debug("ShellServiceObject: Unable to stop");
                    }
                }
                _isDisposed = true;
            }
        }
    }
}
