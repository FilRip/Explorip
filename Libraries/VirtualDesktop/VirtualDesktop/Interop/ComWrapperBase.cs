using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace VirtualDesktop.Interop;

public abstract class ComWrapperBase<TInterface>
{
    private readonly Dictionary<string, MethodInfo> _methods = [];

    private protected ComInterfaceAssembly ComInterfaceAssembly { get; }

    public TInterface Interface
        => (TInterface)(object)this;

    public Type ComInterfaceType { get; }

    public object ComObject { get; }

    private protected ComWrapperBase(ComInterfaceAssembly assembly)
    {
        (Type? type, object? instance) = assembly.CreateInstance(typeof(TInterface).Name);

        this.ComInterfaceAssembly = assembly;
        this.ComInterfaceType = type;
        this.ComObject = instance;
    }

    private protected ComWrapperBase(ComInterfaceAssembly assembly, Guid clsid)
    {
        (Type? type, object? instance) = assembly.CreateInstance(typeof(TInterface).Name, clsid);

        this.ComInterfaceAssembly = assembly;
        this.ComInterfaceType = type;
        this.ComObject = instance;
    }

    private protected ComWrapperBase(ComInterfaceAssembly assembly, object comObject)
    {
        this.ComInterfaceAssembly = assembly;
        this.ComInterfaceType = assembly.GetType(typeof(TInterface).Name);
        this.ComObject = comObject;
    }

    protected static object?[] Args(params object?[] args)
        => args;

    protected void InvokeMethod(object?[]? parameters = null, [CallerMemberName] string methodName = "")
        => this.InvokeMethod<object>(parameters, methodName);

    protected T? InvokeMethod<T>(object?[]? parameters = null, [CallerMemberName] string methodName = "")
    {
        if (this._methods.TryGetValue(methodName, out MethodInfo? methodInfo)
            || (methodInfo = this.ComInterfaceType.GetMethod(methodName)) != null)
        {
            this._methods[methodName] = methodInfo;
        }
        else throw new NotSupportedException($"Method '{methodName}' is not supported in COM interface '{typeof(TInterface).Name}'.");

        try
        {
            return (T?)methodInfo.Invoke(this.ComObject, parameters);
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }
}
