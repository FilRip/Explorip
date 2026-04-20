using System;
using System.IO;
using System.Reflection;

namespace ExploripCopy;

internal static class MyAppDomain
{
    [STAThread()]
    internal static void Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        Program.Main(args);
    }

    private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        string assemblyName = new AssemblyName(args.Name).Name + ".dll";

        // Search in same folder first
        string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName);
        if (File.Exists(dllPath))
            return Assembly.LoadFrom(dllPath);

        // Then in subdir of "dll" if not found
        dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", assemblyName);
        if (File.Exists(dllPath))
            return Assembly.LoadFrom(dllPath);

        return null;
    }
}
