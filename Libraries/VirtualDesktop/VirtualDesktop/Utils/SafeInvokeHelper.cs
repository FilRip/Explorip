using System;
using System.Runtime.InteropServices;

using VirtualDesktop.Interop;

namespace VirtualDesktop.Utils;

internal static class SafeInvokeHelper
{
    internal static T? SafeInvoke<T>(Func<T> action, params HResult[] hResult)
    {
        try
        {
            return action();
        }
        catch (COMException ex) when (ex.Match(hResult is { Length: 0 } ? [HResult.TYPE_E_ELEMENTNOTFOUND,] : hResult))
        {
            return default;
        }
    }

    internal static bool SafeInvoke(Action action, params HResult[] hResult)
    {
        try
        {
            action();
            return true;
        }
        catch (COMException ex) when (ex.Match(hResult is { Length: 0 } ? [HResult.TYPE_E_ELEMENTNOTFOUND,] : hResult))
        {
            return false;
        }
    }

}
