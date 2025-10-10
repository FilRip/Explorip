using System;
using System.Linq;
using System.Reflection;

namespace CoolBytes.ScriptInterpreter.Helpers;

public static class Invocations
{
    public static bool InvokeMethod(string methodName, object objectSource, params object[] parameters)
    {
        return InvokeMethod(methodName, objectSource, out _, parameters);
    }

    public static bool InvokeMethod(string methodName, object objectSource, out object ret, params object[] parameters)
    {
        ret = null;
        try
        {
            if (objectSource == null)
                return false;

            Type objectType;
            objectType = objectSource.GetType();
            MethodInfo method;
#pragma warning disable S3011
            method = objectType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static);
#pragma warning restore S3011

            if (method == null)
                return false;

            if (parameters.Count() != method.GetParameters().Count())
                Array.Resize(ref parameters, method.GetParameters().Count() - parameters.Count());

            if (method.ReturnType == typeof(void))
                method.Invoke(objectSource, parameters);
            else
                ret = method.Invoke(objectSource, parameters);

            return true;
        }
        catch (Exception) { /* Ignore all errors, return false */ }
        return false;
    }
}
