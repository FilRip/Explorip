using System;
using System.Linq;
using System.Windows;

namespace ExploripSharedCopy.Helpers
{
    internal static class ExtensionsWpf
    {
        internal static Window GetCurrentWindow(this Application application)
        {
            Window ret = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ret = null;
                foreach (Window win in application.Windows)
                    if (win.IsActive)
                    {
                        ret = win;
                        break;
                    }
                ret ??= Application.Current.MainWindow;
            }));
            return ret;
        }

        internal static T GetCurrentWindow<T>(this Application application) where T : Window
        {
            T ret = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ret = null;
                foreach (T win in application.Windows.OfType<T>())
                    if (win.IsActive)
                    {
                        ret = win;
                        break;
                    }
                ret ??= application.Windows.OfType<T>().FirstOrDefault();
            }));
            return ret;
        }
    }
}
