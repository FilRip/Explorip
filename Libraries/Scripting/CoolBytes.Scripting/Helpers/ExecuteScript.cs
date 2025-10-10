using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using CoolBytes.Helpers;
using CoolBytes.Scripting.Models;
using CoolBytes.Scripting.Exceptions;
using CoolBytes.Scripting.Enums;

namespace CoolBytes.Scripting.Helpers;

public static class ExecuteScript
{
    public static ScriptingResult ExecuteScripts(string script, List<string> listUsings, SupportedLanguage language, List<Assembly> listAssembly, List<string> listReferences, bool autoAddAllCustomUsings, string repertoireAssembly, Assembly[] assembliesEnMemoire)
    {
        ScriptingResult ret = null;

        List<string> newListUsings = [];
        List<string> listNotReferences = [];
        bool loop = true;
        int countAssembly = listAssembly?.Count ?? 0;

        if (listAssembly != null && listAssembly.Count > 0)
        {
            while (loop)
            {
                loop = false;
                try
                {
                    foreach (Assembly ass in listAssembly)
                    {
                        if (!ass.IsDynamic)
                            listNotReferences.Add(ass.Location);

                        if (autoAddAllCustomUsings && !ass.GlobalAssemblyCache && !ass.IsDynamic && ass != Assembly.GetExecutingAssembly())
                            ass.GetTypes().Where(t => t.IsPublic).Select(leType => leType.Namespace).Distinct().ToList().ForEach(ns =>
                            {
                                if (!string.IsNullOrWhiteSpace(ns) && !ns.EndsWith(".My.Resources") && !ns.EndsWith(".My"))
                                    newListUsings.Add(ns);
                            });
                    }
                }
                catch (Exception)
                {
                    if (listAssembly.Count != countAssembly)
                    {
                        countAssembly = listAssembly.Count;
                        loop = true;
                    }
                }
            }
        }

        if (listUsings != null && listUsings.Count > 0)
            newListUsings.AddRange(listUsings);

        if (listReferences != null && listReferences.Count > 0)
        {
            List<string> listForSearchNamespace = [];

            foreach (string reference in listReferences.Where(s => !string.IsNullOrWhiteSpace(s) && !s.Contains("mscorlib")))
            {
                listNotReferences.Add(reference);
                if (!listForSearchNamespace.Contains(reference) && !string.IsNullOrWhiteSpace(Path.GetDirectoryName(reference)))
                    listForSearchNamespace.Add(reference);
            }
            if (autoAddAllCustomUsings && listForSearchNamespace.Count > 0)
            {
                AppDomainSetup appDomainSetup = AppDomain.CurrentDomain.SetupInformation;
                appDomainSetup.ApplicationBase = repertoireAssembly;
                AppDomain appDomain = AppDomain.CreateDomain("CoolBytes.ScriptInterpreter.Reflection", null, appDomainSetup);
                appDomain.ReflectionOnlyAssemblyResolve += AppDomain_ReflectionOnlyAssemblyResolve;
                appDomain.SetData("References", listForSearchNamespace);
                appDomain.DoCallBack(ExecuteSearchNamespace);
                object result;
                result = appDomain.GetData("Namespaces");
                if (result != null)
                {
                    List<string> listNamespace = (List<string>)result;
                    newListUsings.AddRange(listNamespace);
                }
                AppDomain.Unload(appDomain);
            }
        }

        newListUsings.RemoveAll(imp => string.IsNullOrWhiteSpace(imp));
        newListUsings = [.. newListUsings.Distinct()];

        listNotReferences.RemoveAll(refer => string.IsNullOrWhiteSpace(refer) || refer.ToLower() == "mscorlib.dll");
        listNotReferences = [.. listNotReferences.Distinct()];

        if (language == SupportedLanguage.VBNET)
            ret = VisualBasic.ExecuteScript(script, [.. listNotReferences], assembliesEnMemoire, [.. newListUsings]);
        else if (language == SupportedLanguage.CSHARP)
            ret = CSharp.ExecuteScript(script, [.. listNotReferences], assembliesEnMemoire, [.. newListUsings]);
        else
            throw new LanguageNotSupportedException();

        return ret;
    }

    private static Assembly AppDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
    {
        try
        {
            Assembly loadedAssembly = Array.Find(AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies(), asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));

            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            return Assembly.ReflectionOnlyLoadFrom(args.Name);
        }
        catch { /* Ignore errors */ }
        try
        {
            return Assembly.ReflectionOnlyLoadFrom(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.DirectorySeparatorChar + args.Name.Split(',')[0] + ".dll");
        }
        catch { /* Ignore errors */ }
        try
        {
            return Assembly.ReflectionOnlyLoadFrom(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.DirectorySeparatorChar + args.Name.Split(',')[0] + ".exe");
        }
        catch { /* Ignore errors */ }
        return null;
    }

    private static void ExecuteSearchNamespace()
    {
        List<string> listReferences = (List<string>)AppDomain.CurrentDomain.GetData("References");
        List<string> listNamespace = [];
        foreach (string reference in listReferences)
        {
            try
            {
                Assembly ass = Assembly.ReflectionOnlyLoadFrom(reference);
                if (ass != null && !ass.GlobalAssemblyCache && !ass.IsDynamic)
                {
                    ass.GetTypes().Where(t => t.IsPublic).Select(leType => leType.Namespace).Distinct().ToList().ForEach(ns =>
                    {
                        if (!string.IsNullOrWhiteSpace(ns) && !ns.EndsWith(".My.Resources") && !ns.EndsWith(".My"))
                            listNamespace.Add(ns);
                    });
                }
            }
            catch (Exception) { /* Ignore errors */ }
        }
        AppDomain.CurrentDomain.SetData("Namespaces", listNamespace);
    }

    public static List<Assembly> ListDefaultAssembly()
    {
        List<Assembly> listAssembly;
        listAssembly = ExtensionsAssembly.ListAssembly();
        if (!listAssembly.Contains(Assembly.GetEntryAssembly()))
            listAssembly.Add(Assembly.GetEntryAssembly());
        return listAssembly;
    }
}
