using System;
using System.Collections.Generic;
using System.Linq;

namespace Monitorian;

public static class Extensions
{
    public static IEnumerable<TSource> Clip<TSource>(this IEnumerable<TSource> source, TSource start, TSource end) where TSource : IComparable
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        // Remove the elements before the start and after the end while keeping the elements
        // between intact.
        return source
            .SkipWhile(x => x.CompareTo(start) < 0)
            .Reverse()
            .SkipWhile(x => x.CompareTo(end) > 0)
            .Reverse();
    }
}
