using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WindowsDesktop.Interop
{
    internal class ComInterfaceAssembly
    {
        private readonly Dictionary<string, Type> _knownTypes = [];
        private readonly Assembly _compiledAssembly;

        public ComInterfaceAssembly(Assembly compiledAssembly)
        {
            _compiledAssembly = compiledAssembly;
        }

        internal Type GetType(string typeName)
        {
            if (!_knownTypes.TryGetValue(typeName, out Type type))
            {
                type = _knownTypes[typeName] = _compiledAssembly
                    .GetTypes()
                    .SingleOrDefault(x => x.Name.Split('.')[x.Name.Split('.').Length - 1] == typeName);
            }

            return type;
        }

        internal static object CreateInstance(Type type, Guid? guidService)
        {
            Type shellType = Type.GetTypeFromCLSID(ClSid.ImmersiveShell);
            IServiceProvider shell = (IServiceProvider)Activator.CreateInstance(shellType);

            shell.QueryService(guidService ?? type.GUID, type.GUID, out object ppvObject);

            return ppvObject;
        }

        internal (Type type, object instance) CreateInstance(string comInterfaceName, Guid? guidService)
        {
            Type type = GetType(comInterfaceName);
            object instance = CreateInstance(type, guidService);

            return (type, instance);
        }
    }
}
