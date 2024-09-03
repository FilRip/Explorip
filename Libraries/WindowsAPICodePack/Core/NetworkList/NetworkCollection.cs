using System.Collections;
using System.Collections.Generic;

using Microsoft.WindowsAPICodePack.Interop.NetworkList;

namespace Microsoft.WindowsAPICodePack.NetworkList;

/// <summary>
/// An enumerable collection of <see cref="Network"/> objects.
/// </summary>
public class NetworkCollection : IEnumerable<Network>
{
    #region Private Fields

    readonly IEnumerable networkEnumerable;

    #endregion // Private Fields

    internal NetworkCollection(IEnumerable networkEnumerable)
    {
        this.networkEnumerable = networkEnumerable;
    }

    #region IEnumerable<Network> Members

    /// <summary>
    /// Returns the strongly typed enumerator for this collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/>  object.</returns>
    public IEnumerator<Network> GetEnumerator()
    {
        foreach (INetwork network in networkEnumerable)
        {
            yield return new Network(network);
        }
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Returns the enumerator for this collection.
    /// </summary>
    ///<returns>An <see cref="IEnumerator"/> object.</returns> 
    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (INetwork network in networkEnumerable)
        {
            yield return new Network(network);
        }
    }

    #endregion
}