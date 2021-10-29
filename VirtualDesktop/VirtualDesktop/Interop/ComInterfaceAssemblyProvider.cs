using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using WindowsDesktop.Properties;

using Microsoft.CSharp;

namespace WindowsDesktop.Interop
{
    internal class ComInterfaceAssemblyProvider
    {
        private const string _placeholderGuid = "00000000-0000-0000-0000-000000000000";
        private const string _assemblyName = "VirtualDesktop.{0}.generated.dll";

        private static readonly Regex _assemblyRegex = new Regex(@"VirtualDesktop\.(?<build>\d{5}?)(\.\w*|)\.dll");
        private static readonly string _defaultAssemblyDirectoryPath = Path.Combine(ProductInfo.LocalAppData.FullName, "assemblies");
        private static readonly Version _requireVersion = new Version("1.0");
        private static readonly int[] _interfaceVersions = new[] { 10240, 20231, 21313, 21359, 22449 };

        private readonly string _assemblyDirectoryPath;

        public ComInterfaceAssemblyProvider(string assemblyDirectoryPath)
        {
            _assemblyDirectoryPath = assemblyDirectoryPath ?? _defaultAssemblyDirectoryPath;
        }

        public Assembly GetAssembly()
        {
            Assembly assembly = GetExistingAssembly();
            if (assembly != null) return assembly;

            return CreateAssembly();
        }

        private Assembly GetExistingAssembly()
        {
            string[] searchTargets = new[]
            {
                _assemblyDirectoryPath,
                Environment.CurrentDirectory,
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                _defaultAssemblyDirectoryPath,
            };

            foreach (string searchPath in searchTargets)
            {
                DirectoryInfo dir = new DirectoryInfo(searchPath);
                if (!dir.Exists) continue;

                foreach (FileInfo file in dir.GetFiles())
                {
                    if (int.TryParse(_assemblyRegex.Match(file.Name).Groups["build"]?.ToString(), out var build)
                        && build == ProductInfo.OSBuild)
                    {
                        AssemblyName name = AssemblyName.GetAssemblyName(file.FullName);
                        if (name.Version >= _requireVersion)
                        {
                            System.Diagnostics.Debug.WriteLine($"Assembly found: {file.FullName}");
#if !DEBUG
                            return Assembly.LoadFile(file.FullName);
#endif
                        }
                    }
                }
            }

            return null;
        }

        private Assembly CreateAssembly()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            int interfaceVersion = _interfaceVersions
                .Reverse()
                .First(build => build <= ProductInfo.OSBuild);
            string[] interfaceNames = executingAssembly
                .GetTypes()
                .SelectMany(x => x.GetComInterfaceNamesIfWrapper(interfaceVersion))
                .Where(x => x != null)
                .ToArray();
            Dictionary<string, Guid> iids = IID.GetIIDs(interfaceNames);
            List<string> compileTargets = new List<string>();
            {
                string assemblyInfo = executingAssembly.GetManifestResourceNames().Single(x => x.Contains("AssemblyInfo"));
                Stream stream = executingAssembly.GetManifestResourceStream(assemblyInfo);
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string sourceCode = reader.ReadToEnd()
                            .Replace("{VERSION}", ProductInfo.OSBuild.ToString())
                            .Replace("{BUILD}", interfaceVersion.ToString());
                        compileTargets.Add(sourceCode);
                    }
                }
            }

            foreach (string name in executingAssembly.GetManifestResourceNames())
            {
                string[] texts = Path.GetFileNameWithoutExtension(name)?.Split('.');
                string typeName = texts.LastOrDefault();
                if (typeName == null) continue;

                if (int.TryParse(string.Concat(texts[texts.Length - 2].Skip(1)), out int build) && build != interfaceVersion)
                {
                    continue;
                }

                string interfaceName = interfaceNames.FirstOrDefault(x => typeName == x);
                if ((interfaceName == null) || (!iids.ContainsKey(interfaceName)))
                {
                    continue;
                }

                Stream stream = executingAssembly.GetManifestResourceStream(name);
                if (stream == null) continue;

                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string sourceCode = reader.ReadToEnd().Replace(_placeholderGuid, iids[interfaceName].ToString());
                    compileTargets.Add(sourceCode);
                }
            }

            return Compile(compileTargets.ToArray());
        }

        private Assembly Compile(IEnumerable<string> sources)
        {
            var dir = new DirectoryInfo(this._assemblyDirectoryPath);

            if (!dir.Exists)
            {
                dir.Create();
            }

            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                string path = Path.Combine(dir.FullName, string.Format(_assemblyName, ProductInfo.OSBuild));
                CompilerParameters cp = new CompilerParameters()
                {
                    OutputAssembly = path,
                    GenerateExecutable = false,
                    GenerateInMemory = false,
                };
                cp.ReferencedAssemblies.Add("System.dll");
                cp.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

                CompilerResults result = provider.CompileAssemblyFromSource(cp, sources.ToArray());
                if (result.Errors.Count > 0)
                {
                    string message = $"Failed to compile COM interfaces assembly.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.OfType<CompilerError>().Select(x => $"  {x}"))}";

                    throw new Exception(message);
                }

                System.Diagnostics.Debug.WriteLine($"Assembly compiled: {path}");
                return result.CompiledAssembly;
            }
        }
    }
}
