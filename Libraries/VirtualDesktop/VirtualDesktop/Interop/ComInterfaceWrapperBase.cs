using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WindowsDesktop.Interop
{
    public abstract class ComInterfaceWrapperBase
    {
        private readonly Dictionary<string, MethodInfo> _methods = [];

        private protected ComInterfaceAssembly ComInterfaceAssembly { get; }

        public Type ComInterfaceType { get; }

        public object ComObject { get; }

        public uint ComVersion { get; }

        private protected ComInterfaceWrapperBase(ComInterfaceAssembly assembly, string comInterfaceName = null, uint latestVersion = 1, Guid? service = null)
        {
            string comInterfaceName2 = comInterfaceName ?? GetType().GetComInterfaceNameIfWrapper();
            for (uint version = latestVersion; version >= 1; version--)
            {
                Type type = assembly.GetType(version != 1 ? $"{comInterfaceName2}{version}" : comInterfaceName2);
                if (type != null)
                {
                    object instance = ComInterfaceAssembly.CreateInstance(type, service);
                    ComInterfaceAssembly = assembly;
                    ComInterfaceType = type;
                    ComObject = instance;
                    ComVersion = version;
                    return;
                }
            }

            throw new InvalidOperationException($"{comInterfaceName2} or later version is not found.");
        }

        private protected ComInterfaceWrapperBase(ComInterfaceAssembly assembly, object comObject, string comInterfaceName = null, uint latestVersion = 1)
        {
            string comInterfaceName2 = comInterfaceName ?? GetType().GetComInterfaceNameIfWrapper();
            for (uint version = latestVersion; version >= 1; version--)
            {
                Type type = assembly.GetType(version != 1 ? $"{comInterfaceName2}{version}" : comInterfaceName2);
                if (type != null)
                {
                    ComInterfaceAssembly = assembly;
                    ComInterfaceType = type;
                    ComObject = comObject;
                    ComVersion = version;
                    return;
                }
            }

            throw new InvalidOperationException($"{comInterfaceName2} or later version is not found.");
        }

        protected static object[] Args(params object[] args)
            => args;

        protected void Invoke(object[] parameters = null, [CallerMemberName()] string methodName = "")
            => this.Invoke<object>(parameters, methodName);

        protected T Invoke<T>(object[] parameters = null, [CallerMemberName()] string methodName = "")
        {
            if (!_methods.TryGetValue(methodName, out MethodInfo methodInfo))
            {
                _methods[methodName] = methodInfo = ComInterfaceType.GetMethod(methodName);

                if (methodInfo == null)
                {
                    throw new NotSupportedException($"{methodName} is not supported.");
                }
            }

            try
            {
                return (T)methodInfo.Invoke(ComObject, parameters);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }
    }
}
