using System;
using System.Collections.Generic;
using System.Reflection;

namespace VirtualDesktop.Interop;

[AttributeUsage(AttributeTargets.Interface)]
internal class ComInterfaceAttribute : Attribute
{
    public string? InterfaceName { get; }

    public ComInterfaceAttribute()
    {
    }

    public ComInterfaceAttribute(string interfaceName)
    {
        this.InterfaceName = interfaceName;
    }
}

internal static class ComInterfaceAttributeExtensions
{
    /// <summary>
    /// Gets COM interface name if specific type has '<see cref="ComInterfaceAttribute"/>' attribute.
    /// </summary>
    public static string? GetComInterfaceNameIfWrapper(this Type type)
    {
        ComInterfaceAttribute attr = type.GetCustomAttribute<ComInterfaceAttribute>();
        if (attr == null) return null;

        return attr.InterfaceName ?? type.Name;
    }
}
