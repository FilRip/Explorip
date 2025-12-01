using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoolBytes.Helpers;

public static class ExtensionsAssembly
{
    public static List<string> ListReferences()
    {
        return ListReferences(Assembly.GetEntryAssembly(), true, true);
    }

    public static List<string> ListReferences(Assembly asm, bool recursifSurExe, bool includeGAC)
    {
        List<string> ret = [];
        Assembly ass;
        foreach (AssemblyName an in asm.GetReferencedAssemblies().Where(a => !a.Name.StartsWith("vshost") && !a.Name.StartsWith("mscorlib")).ToList())
        {
            ass = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assemble => assemble.GetName().Name == an.Name && !string.IsNullOrWhiteSpace(assemble.Location));
            if (ass != null)
            {
                if (ass.GlobalAssemblyCache)
                {
                    if (includeGAC)
                        ret.Add(Path.GetFileName(ass.Location));
                }
                else
                    ret.Add(ass.Location);

                if (recursifSurExe && Path.GetExtension(ass.Location) == "exe")
                    ret.AddRange(ListReferences(ass, recursifSurExe, includeGAC));
            }
        }
        return [.. ret.Distinct()];
    }

    public static List<Assembly> ListAssemblyInMemory()
    {
        return [.. AppDomain.CurrentDomain.GetAssemblies()];
    }

    public static List<Assembly> ListAssembly()
    {
        return ListeAssembly(Assembly.GetEntryAssembly(), true, true);
    }

    public static List<Assembly> ListeAssembly(Assembly asm, bool recursiveOnExe, bool includeGAC)
    {
        List<Assembly> ret = [];
        Assembly ass;
        foreach (AssemblyName an in asm.GetReferencedAssemblies().Where(a => !a.Name.StartsWith("vshost") && !a.Name.StartsWith("mscorlib")).ToList())
        {
            ass = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assemble => assemble.GetName().Name == an.Name && !string.IsNullOrWhiteSpace(assemble.Location));
            if (ass != null)
            {
                if (!ass.GlobalAssemblyCache || includeGAC)
                    ret.Add(ass);

                if (recursiveOnExe && Path.GetExtension(ass.Location).ToLower() == ".exe")
                    ret.AddRange(ListeAssembly(ass, recursiveOnExe, includeGAC));
            }
        }
        return [.. ret.Distinct()];
    }

    public static List<Assembly> ListAssemblyInAppDomain()
    {
        return ListAssemblyInAppDomain(AppDomain.CurrentDomain, true, true, false);
    }

    public static List<Assembly> ListAssemblyInAppDomain(bool recursiveOnExe, bool includeGAC)
    {
        return ListAssemblyInAppDomain(AppDomain.CurrentDomain, recursiveOnExe, includeGAC, false);
    }

    public static List<Assembly> ListAssemblyInAppDomain(AppDomain domain, bool recursiveOnExe, bool includeGAC, bool includeDynamic, bool inclureReflectionOnly = false)
    {
        List<Assembly> ret = [];
        foreach (Assembly ass in domain.GetAssemblies())
        {
            LoopAssembly(ass);
        }

        if (inclureReflectionOnly)
            foreach (Assembly ass in domain.ReflectionOnlyGetAssemblies())
            {
                LoopAssembly(ass);
            }

        void LoopAssembly(Assembly ass)
        {
            if (ass != null)
            {
                if (ass.IsDynamic && !includeDynamic)
                    return;

                if (!ass.GlobalAssemblyCache || includeGAC)
                    ret.Add(ass);

                if (recursiveOnExe && Path.GetExtension(ass.Location).ToLower() == ".exe")
                    ret.AddRange(ListeAssembly(ass, recursiveOnExe, includeGAC));
            }
        }

        return [.. ret.Distinct()];
    }

    public static List<string> ListNamespace()
    {
        return ListNamespace(Assembly.GetEntryAssembly());
    }

    public static List<string> ListNamespace(string assemblyName)
    {
        Assembly asm = Array.Find(AppDomain.CurrentDomain.GetAssemblies(), item => item.GetName().Name == assemblyName) ?? throw new ArgumentNullException(nameof(assemblyName));
        return ListNamespace([asm]);
    }

    public static List<string> ListNamespace(Assembly asm)
    {
        if (asm == null)
            throw new ArgumentNullException(nameof(asm));
        return ListNamespace([asm]);
    }

    public static List<string> ListNamespace(IEnumerable<Assembly> assemblies)
    {
        if (assemblies == null)
            throw new ArgumentNullException(nameof(assemblies));
        List<string> ret = [];
        foreach (Assembly asm in assemblies)
            ret = [.. ret, .. asm.GetTypes().Where(item => !string.IsNullOrWhiteSpace(item.Namespace)).Select(item => item.Namespace).Distinct()];
        return [.. ret.Distinct()];
    }

    public static List<string> ListAllNamespace()
    {
        return ListAllNamespace(AppDomain.CurrentDomain);
    }

    public static List<string> ListAllNamespace(AppDomain domain, bool inclureReflectionOnly = false)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));
        List<string> ret = [];
        foreach (Assembly asm in domain.GetAssemblies())
            ret = [.. ret.Concat(ListNamespace(asm)).Distinct()];

        if (inclureReflectionOnly)
            foreach (Assembly asm in domain.ReflectionOnlyGetAssemblies())
                ret = [.. ret.Concat(ListNamespace(asm)).Distinct()];

        return [.. ret.Distinct()];
    }

    public static List<Type> ListAllTypes()
    {
        return AppDomain.CurrentDomain.ListAllTypes();
    }

    public static List<Type> ListAllTypes(this AppDomain domain, bool inclureReflectionOnly = false)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        List<Type> ret = [];
        foreach (Assembly asm in domain.GetAssemblies())
            ret = [.. ret, .. asm.GetTypes()];

        if (inclureReflectionOnly)
            foreach (Assembly asm in domain.ReflectionOnlyGetAssemblies())
                ret = [.. ret, .. asm.GetTypes()];

        return ret;
    }

    public static Type SearchType(string typeName)
    {
        return AppDomain.CurrentDomain.SearchType(typeName);
    }

    public static Type SearchType(string typeName, bool ignoreCase)
    {
        return AppDomain.CurrentDomain.SearchType(typeName, ignoreCase);
    }

    public static Type SearchType(this AppDomain domain, string typeName)
    {
        return domain.SearchType(typeName, false);
    }

    public static Type SearchType(this AppDomain domain, string typeName, bool ignoreCase, bool inclureReflectionOnly = false)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        Type ret;
        foreach (Assembly asm in domain.GetAssemblies())
        {
            ret = asm.GetType(typeName, false, ignoreCase);
            if (ret != null)
                return ret;
        }

        if (inclureReflectionOnly)
            foreach (Assembly asm in domain.ReflectionOnlyGetAssemblies())
            {
                ret = asm.GetType(typeName, false, ignoreCase);
                if (ret != null)
                    return ret;
            }

        return null;
    }

    public static Type SearchTypeWithoutNamespace(string typeName)
    {
        return AppDomain.CurrentDomain.SearchTypeWithoutNamespace(typeName);
    }

    public static Type SearchTypeWithoutNamespace(string typeName, bool ignoreCase)
    {
        return AppDomain.CurrentDomain.SearchTypeWithoutNamespace(typeName, ignoreCase);
    }

    public static Type SearchTypeWithoutNamespace(this AppDomain domain, string typeName)
    {
        return domain.SearchTypeWithoutNamespace(typeName, false);
    }

    public static Type SearchTypeWithoutNamespace(this AppDomain domain, string typeName, bool ignoreCase, bool inclureReflectionOnly = false)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        Type ret;
        foreach (Assembly asm in domain.GetAssemblies())
        {
            ret = Array.Find(asm.GetTypes(), item => string.Compare(item.Name, typeName, ignoreCase) == 0);
            if (ret != null)
                return ret;
        }

        if (inclureReflectionOnly)
            foreach (Assembly asm in domain.ReflectionOnlyGetAssemblies())
            {
                ret = Array.Find(asm.GetTypes(), item => string.Compare(item.Name, typeName, ignoreCase) == 0);
                if (ret != null)
                    return ret;
            }

        return null;
    }

    public static byte[] GetBytes(this Assembly assembly)
    {
        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

#pragma warning disable IDE0079
#pragma warning disable S3011
        MethodInfo mi = assembly.GetType().GetMethod("GetRawBytes", BindingFlags.Instance | BindingFlags.NonPublic);
#pragma warning restore S3011
#pragma warning restore IDE0079

        if (mi != null)
            return (byte[])mi.Invoke(assembly, null);
        else
            return [];
    }

    public static bool Save(this Assembly assembly, string filename)
    {
        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));
        if (string.IsNullOrWhiteSpace(filename))
            throw new ArgumentNullException(nameof(filename));

        byte[] bytes = assembly.GetBytes();
        if (bytes != null && bytes.Length > 0)
        {
            try
            {
                File.WriteAllBytes(filename, bytes);
                return true;
            }
            catch (Exception) { /* Ignore les erreurs d'écriture sur le disque (disque plein, accès répertoire refusé, ... */ }
        }
        return false;
    }
}
