using System;
using System.Windows;
using System.Windows.Threading;

namespace CoolBytes.ScriptInterpreter.Helpers;

public static class ExtensionsWpf
{
    public static void InvokeIfRequired(this FrameworkElement element, Action action, DispatcherPriority priorite = DispatcherPriority.Normal)
    {
        if (element.Dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            element.Dispatcher.Invoke(action, priorite);
        }
    }

    public static T InvokeIfRequired<T>(this FrameworkElement element, Func<T> action, DispatcherPriority priorite = DispatcherPriority.Normal)
    {
        if (element.Dispatcher.CheckAccess())
        {
            return action();
        }
        else
        {
            return element.Dispatcher.Invoke(action, priorite);
        }
    }

    public static void BeginInvokeIfRequired(this FrameworkElement element, Action action, DispatcherPriority priorite = DispatcherPriority.Normal)
    {
        if (element.Dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            element.Dispatcher.BeginInvoke(action, priorite);
        }
    }
}
