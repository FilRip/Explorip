using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WindowsDesktop.Interop
{
    internal class ComInterfaceAssembly
    {
        private readonly Dictionary<string, Type> _knownTypes = new();
        private readonly Assembly _compiledAssembly;

        public ComInterfaceAssembly(Assembly compiledAssembly)
        {
            _compiledAssembly = compiledAssembly;
        }

        internal Type GetType(string typeName)
        {
            if (!_knownTypes.TryGetValue(typeName, out var type))
            {
                type = _knownTypes[typeName] = _compiledAssembly
                    .GetTypes()
                    .SingleOrDefault(x => x.Name.Split('.').Last() == typeName);
            }

            return type;
        }

        internal object CreateInstance(Type type, Guid? guidService)
        {
            Type shellType = Type.GetTypeFromCLSID(CLSID.ImmersiveShell);
            IServiceProvider shell = (IServiceProvider)Activator.CreateInstance(shellType);

            shell.QueryService(guidService ?? type.GUID, type.GUID, out var ppvObject);

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
