using System;
using System.Reflection;

namespace CoolBytes.ScriptInterpreter.YAXLib;

/// <summary>
/// Provides extensions as a bridge for the differences 
/// between .Net Framework "Type" and .Net Core "TypeInfo".
/// </summary>
internal static class ReflectionBridgeExtensions
{
    /*
    MIT License

    Copyright (c) 2016 to 2099 Stef Heyenrath
    Sourcecode: https://github.com/StefH/ReflectionBridge

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    */

    /*
    Modified by axuno gGmbH (https://github.com/axuno and http://www.axuno.net) for YAXLib
    */
    public static Assembly GetAssembly(this Type type)
    {
        return type.Assembly;
    }

    public static bool IsAbstract(this Type type)
    {
        return type.IsAbstract;
    }

    public static bool IsEnum(this Type type)
    {
        return type.IsEnum;
    }

    public static bool IsClass(this Type type)
    {
        return type.IsClass;
    }

    public static bool IsPrimitive(this Type type)
    {
        return type.IsPrimitive;
    }

    public static bool IsGenericType(this Type type)
    {
        return type.IsGenericType;
    }

    public static bool IsGenericTypeDefinition(this Type type)
    {
        return type.IsGenericTypeDefinition;
    }

    public static bool IsInterface(this Type type)
    {
        return type.IsInterface;
    }

    public static Type BaseType(this Type type)
    {
        return type.BaseType;
    }

    public static bool IsValueType(this Type type)
    {
        return type.IsValueType;
    }

    public static PropertyInfo GetProperty(this Type type, string name, Type[] types)
    {
        return type.GetProperty(name, types);
    }

    public static object InvokeMethod<T>(this Type type, string methodName, object target, T value)
    {
        return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, [value]);
    }

    public static object InvokeMethod(this Type type, string methodName, object target, object[] arg)
    {
        return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, arg);
    }
}
