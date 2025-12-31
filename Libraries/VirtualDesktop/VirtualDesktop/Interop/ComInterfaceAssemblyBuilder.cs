using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

using VirtualDesktop.Utils;

namespace VirtualDesktop.Interop;

internal class ComInterfaceAssemblyBuilder(VirtualDesktopCompilerConfiguration config)
{
    private const string _assemblyName = "VirtualDesktop.{0}.generated.dll";
    private const string _placeholderOsBuild = "{OS_BUILD}";
    private const string _placeholderAssemblyVersion = "{ASSEMBLY_VERSION}";
    private const string _placeholderInterfaceId = "00000000-0000-0000-0000-000000000000";

    // Now using assembly version even though regenerating our DLL won't strictly be needed for every new version this is the safest option
    // Otherwise people will surely forget to increment a specific version here
    private static readonly Version? _requireVersion = Assembly.GetExecutingAssembly().GetName().Version;
    private static readonly Regex _assemblyRegex = new(@"VirtualDesktop\.10\.0\.(?<build>\d+\.\d+)(\.\w*|)\.dll");
    private static readonly Regex _buildNumberRegex = new(@"\.Build(?<build>\d+\.\d+)\.");
    private static readonly Version osBuild = OS.Build;
    private readonly VirtualDesktopCompilerConfiguration _configuration = config;
    private static ComInterfaceAssembly? _assembly;

    private static void SetAssembly(Assembly currentAssembly)
    {
        _assembly ??= new ComInterfaceAssembly(currentAssembly);
    }

    public ComInterfaceAssembly GetAssembly()
    {
        SetAssembly(this.LoadExistingAssembly() ?? this.CreateAssembly());
        return _assembly!;
    }

    private Assembly? LoadExistingAssembly()
    {
        if (_configuration.ForceRebuildAssembly)
            return null;

        if (_configuration.CompiledAssemblySaveDirectory.Exists)
        {
            foreach (FileInfo? file in _configuration.CompiledAssemblySaveDirectory.GetFiles())
            {
                if (Version.TryParse(OS.VersionPrefix + _assemblyRegex.Match(file.Name).Groups["build"].ToString(), out Version? build)
                    && build == osBuild)
                {
                    try
                    {
                        AssemblyName name = AssemblyName.GetAssemblyName(file.FullName);
                        if (name.Version >= _requireVersion)
                        {
                            Debug.WriteLine($"Assembly found: {file.FullName}");
#pragma warning disable IDE0079
#pragma warning disable S3885
                            return Assembly.LoadFile(file.FullName);
#pragma warning restore S3885
#pragma warning restore IDE0079
                        }
                        else
                        {
                            Debug.WriteLine($"Outdated assembly: {name.Version} < {_requireVersion}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed to load assembly: ");
                        Debug.WriteLine(ex);

                        File.Delete(file.FullName);
                    }
                }
            }
        }

        return null;
    }

    private Assembly CreateAssembly()
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        List<string> compileTargets = [];

        string assemblyInfo = executingAssembly.GetManifestResourceNames().Single(x => x.Contains("AssemblyInfo.cs"));
        Stream stream = executingAssembly.GetManifestResourceStream(assemblyInfo);
        if (stream != null)
        {
            using StreamReader reader = new(stream, Encoding.UTF8);
            string sourceCode = reader
                .ReadToEnd()
                .Replace(_placeholderOsBuild, osBuild.ToString())
                .Replace(_placeholderAssemblyVersion, _requireVersion!.ToString(3));
            compileTargets.Add(sourceCode);
        }

        string[] interfaceNames = [.. executingAssembly
            .GetTypes()
            .Select(x => x.GetComInterfaceNameIfWrapper())
            .Where(x => !string.IsNullOrEmpty(x))
            .Cast<string>()];
        Dictionary<string, Guid> iids = Iid.GetIIDs(interfaceNames);

        Dictionary<string, SortedList<Version, string>> interfaceSourceFiles = [];

        // This is where we decide which interface variant goes into our generated DLL assembly
        foreach (string? name in executingAssembly.GetManifestResourceNames())
        {
            string interfaceName = Path.GetFileNameWithoutExtension(name).Split('.').LastOrDefault();
            if (interfaceName != null
                && interfaceNames.Contains(interfaceName)
                && Version.TryParse(OS.VersionPrefix + _buildNumberRegex.Match(name.Replace('_', '.')).Groups["build"].ToString(), out Version? build))
            {
                if (!interfaceSourceFiles.TryGetValue(interfaceName, out SortedList<Version, string>? sourceFiles))
                {
                    sourceFiles = [];
                    interfaceSourceFiles.Add(interfaceName, sourceFiles);
                }

                sourceFiles.Add(build, name);
            }
        }

        foreach (KeyValuePair<string, SortedList<Version, string>> kvp in interfaceSourceFiles)
        {
            string resourceName = kvp.Value.Aggregate("", (current, kvp) =>
            {
                return kvp.Key <= osBuild ? kvp.Value : current;
            });

            Stream asmStream = executingAssembly.GetManifestResourceStream(resourceName);
            if (asmStream == null)
                continue;

            using StreamReader reader = new(asmStream, Encoding.UTF8);
            string sourceCode = reader.ReadToEnd().Replace(_placeholderInterfaceId, iids[kvp.Key].ToString());
            compileTargets.Add(sourceCode);
        }

        return this.Compile([.. compileTargets]);
    }

    private Assembly Compile(IEnumerable<string> sources)
    {
        try
        {
            string name = string.Format(_assemblyName, osBuild);
            ParseOptions po = new CSharpParseOptions(LanguageVersion.Latest);
            IEnumerable<SyntaxTree> syntaxTrees = sources.Select(x => SyntaxFactory.ParseSyntaxTree(x, po));
            IEnumerable<PortableExecutableReference> references = AppDomain.CurrentDomain.GetAssemblies()
                .Concat([Assembly.GetExecutingAssembly(),])
                .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
                .Select(x =>
                {
                    return MetadataReference.CreateFromFile(x.Location);
                });
            CSharpCompilationOptions options = new(OutputKind.DynamicallyLinkedLibrary,
                                                   optimizationLevel: _configuration.BuildDebugConfiguration ? OptimizationLevel.Debug : OptimizationLevel.Release,
                                                   checkOverflow: _configuration.BuildDebugConfiguration);
            CSharpCompilation compilation = CSharpCompilation.Create(name)
                .WithOptions(options)
                .WithReferences(references)
                .AddSyntaxTrees(syntaxTrees);

            string? errorMessage;

            if (_configuration.SaveCompiledAssembly)
            {
                DirectoryInfo dir = _configuration.CompiledAssemblySaveDirectory;
                if (!dir.Exists)
                    dir.Create();

                string path = Path.Combine(dir.FullName, name);
                string? pdbPath = null;
                if (_configuration.BuildDebugConfiguration)
                    pdbPath = Path.Combine(dir.FullName, Path.GetFileNameWithoutExtension(name) + ".pdb");
                EmitResult result = compilation.Emit(path, pdbPath);
#pragma warning disable IDE0079
#pragma warning disable S3885
                if (result.Success)
                    return Assembly.LoadFrom(path);
#pragma warning restore S3885
#pragma warning restore IDE0079

                File.Delete(path);
                errorMessage = string.Join(Environment.NewLine, result.Diagnostics.Select(x => $"  {x.GetMessage()}"));
            }
            else
            {
                using MemoryStream stream = new();
                EmitResult result = compilation.Emit(stream);
                if (result.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(stream.ToArray());
                }

                errorMessage = string.Join(Environment.NewLine, result.Diagnostics.Select(x => $"  {x.GetMessage()}"));
            }

            throw new VirtualDesktopException("Failed to compile COM interfaces assembly." + Environment.NewLine + errorMessage);
        }
        finally
        {
#pragma warning disable IDE0079
#pragma warning disable S1215
            GC.Collect();
#pragma warning restore S1215
#pragma warning restore IDE0079
        }
    }
}
