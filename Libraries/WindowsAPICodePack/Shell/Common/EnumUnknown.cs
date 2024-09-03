using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.Shell.Interop.Common;

namespace Microsoft.WindowsAPICodePack.Shell.Common;

internal class EnumUnknownClass : IEnumUnknown
{
    readonly List<ICondition> conditionList = [];
    int current = -1;

    internal EnumUnknownClass(ICondition[] conditions)
    {
        conditionList.AddRange(conditions);
    }

    #region IEnumUnknown Members

    public HResult Next(uint requestedNumber, ref IntPtr buffer, ref uint fetchedNumber)
    {
        current++;

        if (current < conditionList.Count)
        {
            buffer = Marshal.GetIUnknownForObject(conditionList[current]);
            fetchedNumber = 1;
            return HResult.Ok;
        }

        return HResult.False;
    }

    public HResult Skip(uint number)
    {
        int temp = current + (int)number;

        if (temp > conditionList.Count - 1)
        {
            return HResult.False;
        }

        current = temp;
        return HResult.Ok;
    }

    public HResult Reset()
    {
        current = -1;
        return HResult.Ok;
    }

    public HResult Clone(out IEnumUnknown result)
    {
        result = new EnumUnknownClass(conditionList.ToArray());
        return HResult.Ok;
    }

    #endregion
}