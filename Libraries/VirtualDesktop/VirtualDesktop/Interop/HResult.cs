using System;
using System.Linq;

namespace VirtualDesktop.Interop;

internal enum HResult : uint
{
    TYPE_E_OUTOFBOUNDS = 0x80028CA1,
    TYPE_E_ELEMENTNOTFOUND = 0x8002802B,
    REGDB_E_CLASSNOTREG = 0x80040154,
    RPC_S_SERVER_UNAVAILABLE = 0x800706BA,
}

internal static class HResultExtensions
{
    public static bool Match(this Exception ex, params HResult[] hResult)
    {
        try
        {
            return hResult
                .Cast<uint>()
                .Any(x => (uint)ex.HResult == x);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
