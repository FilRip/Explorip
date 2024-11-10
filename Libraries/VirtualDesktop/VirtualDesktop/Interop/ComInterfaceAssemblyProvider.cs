using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CSharp;

using WindowsDesktop.Exceptions;
using WindowsDesktop.Properties;

namespace WindowsDesktop.Interop
{
    internal class ComInterfaceAssemblyProvider
    {
        private const string _placeholderGuid = "00000000-0000-0000-0000-000000000000";
        private const string _assemblyName = "VirtualDesktop.{0}.generated.dll";

        private static readonly Regex _assemblyRegex = new(@"VirtualDesktop\.(?<build>\d{5}?)(\.\w*|)\.dll");
        private static readonly string _defaultAssemblyDirectoryPath = Path.Combine(ProductInfo.LocalAppData.FullName, "assemblies");
        private static readonly Version _requireVersion = new("1.0");
        private static readonly int[] _interfaceVersions = [10240, 20231, 21313, 21359, 22449, 22631, 26100];

        private readonly string _assemblyDirectoryPath;

        public ComInterfaceAssemblyProvider(string assemblyDirectoryPath)
        {
            _assemblyDirectoryPath = assemblyDirectoryPath ?? _defaultAssemblyDirectoryPath;
        }

        public Assembly GetAssembly()
        {
            if (!Debugger.IsAttached)
            {
                Assembly assembly = GetExistingAssembly();
                if (assembly != null)
                    return assembly;
            }

            return CreateAssembly();
        }

        private Assembly GetExistingAssembly()
        {
            string[] searchTargets =
            [
                _assemblyDirectoryPath,
                Environment.CurrentDirectory,
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                _defaultAssemblyDirectoryPath,
            ];

            foreach (string searchPath in searchTargets)
            {
                DirectoryInfo dir = new(searchPath);
                if (!dir.Exists)
                    continue;

                foreach (FileInfo file in dir.GetFiles())
                {
                    if (int.TryParse(_assemblyRegex.Match(file.Name).Groups["build"]?.ToString(), out int build)
                        && build == ProductInfo.OSBuild)
                    {
                        AssemblyName name = AssemblyName.GetAssemblyName(file.FullName);
                        if (name.Version >= _requireVersion)
                        {
                            Debug.WriteLine($"Assembly found: {file.FullName}");
#pragma warning disable S3885 // "Assembly.Load" should be used
                            return Assembly.LoadFile(file.FullName);
#pragma warning restore S3885 // "Assembly.Load" should be used
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
            Dictionary<string, Guid> iids = IiD.GetIIDs(interfaceNames);
            List<string> compileTargets = [];

            string assemblyInfo = executingAssembly.GetManifestResourceNames().Single(x => x.Contains("AssemblyInfo"));
            Stream sr = executingAssembly.GetManifestResourceStream(assemblyInfo);
            if (sr != null)
            {
                using StreamReader reader = new(sr, Encoding.UTF8);
                string sourceCode = reader.ReadToEnd()
                    .Replace("{VERSION}", ProductInfo.OSBuild.ToString())
                    .Replace("{BUILD}", interfaceVersion.ToString());
                compileTargets.Add(sourceCode);
            }

            foreach (string name in executingAssembly.GetManifestResourceNames())
            {
                string[] texts = Path.GetFileNameWithoutExtension(name)?.Split('.');
                string typeName = texts?.LastOrDefault();
                if (typeName == null)
                    continue;

                if (texts != null && int.TryParse(string.Concat(texts[texts.Length - 2].Skip(1)), out int build) && build != interfaceVersion)
                {
                    continue;
                }

                string interfaceName = Array.Find(interfaceNames, x => typeName == x);
                if ((interfaceName == null) || (!iids.ContainsKey(interfaceName)))
                {
                    continue;
                }

                Stream stream = executingAssembly.GetManifestResourceStream(name);
                if (stream == null)
                    continue;

                using StreamReader reader = new(stream, Encoding.UTF8);
                string sourceCode = reader.ReadToEnd().Replace(_placeholderGuid, iids[interfaceName].ToString());
                compileTargets.Add(sourceCode);
            }

            return Compile(compileTargets.ToArray());
        }

        private Assembly Compile(IEnumerable<string> sources)
        {
            DirectoryInfo dir = new(this._assemblyDirectoryPath);

            if (!dir.Exists)
            {
                dir.Create();
            }

            using CSharpCodeProvider provider = new();
            string path = Path.Combine(dir.FullName, string.Format(_assemblyName, ProductInfo.OSBuild));
            CompilerParameters cp = new()
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

                throw new VirtualDesktopException(message);
            }

            Debug.WriteLine($"Assembly compiled: {path}");
            return result.CompiledAssembly;
        }
    }
}
