using System;
using System.Windows;
using System.Windows.Controls;

namespace ExploripCopy.Helpers;

public static class ExtensionsWpf
{
    public static void AdaptSize(ListView lv, GridLength[] columnsSize)
    {
        GridView gv = ((GridView)lv.View);
        double totalWidth = 0;
        for (int i = 0; i < gv.Columns.Count; i++)
            if (!columnsSize[i].IsStar)
                totalWidth += gv.Columns[i].Width;
        double remainingSize = lv.ActualWidth - totalWidth - SystemParameters.VerticalScrollBarWidth;
        remainingSize = Math.Max(remainingSize, 0);
        for (int i = 0; i < gv.Columns.Count; i++)
            if (columnsSize[i].IsStar)
                gv.Columns[i].Width = columnsSize[i].Value * remainingSize;
    }
}
