using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.Interop;
using Microsoft.WindowsAPICodePack.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell.Common;
using Microsoft.WindowsAPICodePack.Shell.Exceptions;
using Microsoft.WindowsAPICodePack.Shell.Interop.Common;
using Microsoft.WindowsAPICodePack.Shell.Interop.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell.Resources;

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem;

/// <summary>
/// Creates a readonly collection of IProperty objects.
/// </summary>
public class ShellPropertyCollection : ReadOnlyCollection<IShellProperty>, IDisposable
{
    #region Internal Constructor

    /// <summary>
    /// Creates a new Property collection given an IPropertyStore object
    /// </summary>
    /// <param name="nativePropertyStore">IPropertyStore</param>
    internal ShellPropertyCollection(IPropertyStore nativePropertyStore)
        : base([])
    {
        NativePropertyStore = nativePropertyStore;
        AddProperties(nativePropertyStore);
    }

    #endregion

    #region Public Constructor

    /// <summary>
    /// Creates a new Property collection given an IShellItem2 native interface
    /// </summary>
    /// <param name="parent">Parent ShellObject</param>
    public ShellPropertyCollection(ShellObject parent)
        : base([])
    {
        ParentShellObject = parent;
        IPropertyStore nativePropertyStore = null;
        try
        {
            nativePropertyStore = CreateDefaultPropertyStore(ParentShellObject);
            AddProperties(nativePropertyStore);
        }
        catch
        {
            parent?.Dispose();
            throw;
        }
        finally
        {
            if (nativePropertyStore != null)
            {
                Marshal.ReleaseComObject(nativePropertyStore);
            }
        }
    }

    /// <summary>
    /// Creates a new <c>ShellPropertyCollection</c> object with the specified file or folder path.
    /// </summary>
    /// <param name="path">The path to the file or folder.</param>        
    public ShellPropertyCollection(string path) : this(ShellObjectFactory.Create(path)) { }

    #endregion

    #region Private Methods

    private ShellObject ParentShellObject { get; set; }

    private IPropertyStore NativePropertyStore { get; set; }

    private void AddProperties(IPropertyStore nativePropertyStore)
    {

        // Populate the property collection
        nativePropertyStore.GetCount(out uint propertyCount);
        for (uint i = 0; i < propertyCount; i++)
        {
            nativePropertyStore.GetAt(i, out PropertyKey propKey);

            if (ParentShellObject != null)
            {
                Items.Add(ParentShellObject.Properties.CreateTypedProperty(propKey));
            }
            else
            {
                Items.Add(CreateTypedProperty(propKey, NativePropertyStore));
            }
        }
    }

    internal static IPropertyStore CreateDefaultPropertyStore(ShellObject shellObj)
    {

        Guid guid = new(ShellIidGuid.IPropertyStore);
        int hr = shellObj.NativeShellItem2.GetPropertyStore(
               ShellNativeMethods.GetPropertyStoreOptions.BestEffort,
               ref guid,
               out IPropertyStore nativePropertyStore);

        // throw on failure 
        if (nativePropertyStore == null || !CoreErrorHelper.Succeeded(hr))
        {
            throw new ShellException(hr);
        }

        return nativePropertyStore;
    }




    #endregion

    #region Collection Public Methods

    /// <summary>
    /// Checks if a property with the given canonical name is available.
    /// </summary>
    /// <param name="canonicalName">The canonical name of the property.</param>
    /// <returns><B>True</B> if available, <B>false</B> otherwise.</returns>
    public bool Contains(string canonicalName)
    {
        if (string.IsNullOrEmpty(canonicalName))
        {
            throw new ArgumentException(LocalizedMessages.PropertyCollectionNullCanonicalName, "canonicalName");
        }

        return Items.Any(p => p.CanonicalName == canonicalName);

    }

    /// <summary>
    /// Checks if a property with the given property key is available.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns><B>True</B> if available, <B>false</B> otherwise.</returns>
    public bool Contains(PropertyKey key)
    {
        return Items.Any(p => p.PropertyKey == key);
    }

    /// <summary>
    /// Gets the property associated with the supplied canonical name string.
    /// The canonical name property is case-sensitive.
    /// 
    /// </summary>
    /// <param name="canonicalName">The canonical name.</param>
    /// <returns>The property associated with the canonical name, if found.</returns>
    /// <exception cref="IndexOutOfRangeException">Throws IndexOutOfRangeException 
    /// if no matching property is found.</exception>
    public IShellProperty this[string canonicalName]
    {
        get
        {
            if (string.IsNullOrEmpty(canonicalName))
            {
                throw new ArgumentException(LocalizedMessages.PropertyCollectionNullCanonicalName, "canonicalName");
            }

            IShellProperty prop = Items.FirstOrDefault(p => p.CanonicalName == canonicalName) ?? throw new WindowsApiCodePackException(LocalizedMessages.PropertyCollectionCanonicalInvalidIndex);
            return prop;
        }
    }

    /// <summary>
    /// Gets a property associated with the supplied property key.
    /// 
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The property associated with the property key, if found.</returns>
    /// <exception cref="IndexOutOfRangeException">Throws IndexOutOfRangeException 
    /// if no matching property is found.</exception>
    public IShellProperty this[PropertyKey key]
    {
        get
        {
            IShellProperty prop = Items.FirstOrDefault(p => p.PropertyKey == key);
            if (prop != null) return prop;

#pragma warning disable S112
            throw new IndexOutOfRangeException(LocalizedMessages.PropertyCollectionInvalidIndex);
#pragma warning restore S112
        }
    }

    #endregion

    // TODO - ShellProperties.cs also has a similar class that is used for creating
    // a ShellObject specific IShellProperty. These 2 methods should be combined or moved to a 
    // common location.
    internal static IShellProperty CreateTypedProperty(PropertyKey propKey, IPropertyStore NativePropertyStore)
    {
        return ShellPropertyFactory.CreateShellProperty(propKey, NativePropertyStore);
    }

    #region IDisposable Members

    /// <summary>
    /// Release the native and managed objects
    /// </summary>
    /// <param name="disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (NativePropertyStore != null)
        {
            Marshal.ReleaseComObject(NativePropertyStore);
            NativePropertyStore = null;
        }
    }

    /// <summary>
    /// Release the native objects.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Implement the finalizer.
    /// </summary>
    ~ShellPropertyCollection()
    {
        Dispose(false);
    }

    #endregion
}
