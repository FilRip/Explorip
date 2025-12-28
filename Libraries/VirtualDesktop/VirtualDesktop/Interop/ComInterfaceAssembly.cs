using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace VirtualDesktop.Interop;

internal class ComInterfaceAssembly(Assembly compiledAssembly)
{
    private readonly Dictionary<string, Type> _knownTypes = [];
    private readonly Assembly _compiledAssembly = compiledAssembly;

    public DirectoryInfo AssemblyLocation
        => new(this._compiledAssembly.Location);

    internal Type GetType(string typeName)
    {
        if (!this._knownTypes.TryGetValue(typeName, out Type? type))
        {
            type = this._knownTypes[typeName] = this._compiledAssembly
                .GetTypes()
                .Single(x => x.Name.Split('.')[x.Name.Split('.').Length - 1] == typeName);
        }

        return type;
    }

    internal (Type type, object instance) CreateInstance(string comInterfaceName)
    {
        Type type = this.GetType(comInterfaceName);
        object instance = CreateInstance(type, null);

        return (type, instance);
    }

    internal (Type type, object instance) CreateInstance(string comInterfaceName, Guid clsid)
    {
        Type type = this.GetType(comInterfaceName);
        object instance = CreateInstance(type, clsid);

        return (type, instance);
    }

    private static object CreateInstance(Type type, Guid? guidService)
    {
        Type shellType = Type.GetTypeFromCLSID(Clsid.ImmersiveShell)
            ?? throw new VirtualDesktopException($"Type of ImmersiveShell ('{Clsid.ImmersiveShell}') is not found.");
        IServiceProvider shell = Activator.CreateInstance(shellType) as IServiceProvider
            ?? throw new VirtualDesktopException("Failed to create an instance of ImmersiveShell.");

        return shell.QueryService(guidService ?? type.GUID, type.GUID);
    }
}
