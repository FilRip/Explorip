using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using CoolBytes.Helpers;
using CoolBytes.Scripting.Interfaces;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace CoolBytes.Scripting.Models;

public abstract class Roslyn<TLanguage, TCompiler, TInterpreter> where TLanguage : IRoslyn, new()
                                                                 where TCompiler : Compilation
                                                                 where TInterpreter : SyntaxTree
{
    #region Compile file(s)

    public static ScriptingResult CompileFiles(string[] filenames)
    {
        return CompileFiles(filenames, ExtensionsString.RandomString());
    }

    public static ScriptingResult CompileFiles(string[] filenames, string compiledAssemblyName)
    {
        return CompileFiles(filenames, null, null, compiledAssemblyName);
    }

    public static ScriptingResult CompileFiles(string[] filenames, string[] assembliesFiles)
    {
        return CompileFiles(filenames, assembliesFiles, ExtensionsString.RandomString());
    }

    public static ScriptingResult CompileFiles(string[] filenames, string[] assembliesFiles, string compiledAssemblyName)
    {
        return CompileFiles(filenames, assembliesFiles, null, compiledAssemblyName);
    }

    public static ScriptingResult CompileFiles(string[] filenames, Assembly[] assemblies)
    {
        return CompileFiles(filenames, assemblies, ExtensionsString.RandomString());
    }

    public static ScriptingResult CompileFiles(string[] filenames, Assembly[] assemblies, string compiledAssemblyName)
    {
        return CompileFiles(filenames, null, assemblies, compiledAssemblyName);
    }

    public static ScriptingResult CompileFiles(string[] filenames, string[] assembliesFiles, Assembly[] assemblies)
    {
        return CompileFiles(filenames, assembliesFiles, assemblies, ExtensionsString.RandomString());
    }

    public static ScriptingResult CompileFiles(string[] filenames, string[] assembliesFiles, Assembly[] assemblies, string compiledAssemblyName)
    {
        ScriptingResult ret = new();
        TLanguage compiler;
        try
        {
            compiler = new();

            ParseOptions parserOptions = compiler.ReturnParserOptions();
            SyntaxTree[] syntaxTrees = [];
            foreach (string file in filenames)
                syntaxTrees = syntaxTrees.Add(compiler.ReturnPrecompiledCode(File.ReadAllText(file), parserOptions));

            ret = CompileAssembly(string.IsNullOrWhiteSpace(compiledAssemblyName) ? ExtensionsString.RandomString() : compiledAssemblyName, syntaxTrees, assembliesFiles, assemblies, compiler);
        }
        catch (Exception ex)
        {
            ret.ExceptionThrowed = ex;
        }
        return ret;
    }

    #endregion

    #region Execute file(s)

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName)
    {
        return ExecuteFiles(filenames, typeName, methodName, null, null, null, null);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, string compiledAssemblyName)
    {
        return ExecuteFiles(filenames, typeName, methodName, null, null, compiledAssemblyName, null);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, string compiledAssemblyName, params object[] parameters)
    {
        return ExecuteFiles(filenames, typeName, methodName, null, null, compiledAssemblyName, parameters);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, params object[] parameters)
    {
        return ExecuteFiles(filenames, typeName, methodName, null, null, null, parameters);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, string[] assembliesFiles)
    {
        return ExecuteFiles(filenames, typeName, methodName, assembliesFiles, null, null, null);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, string[] assembliesFiles, string compiledAssemblyName)
    {
        return ExecuteFiles(filenames, typeName, methodName, assembliesFiles, null, compiledAssemblyName, null);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, string[] assembliesFiles, string compiledAssemblyName, params object[] parameters)
    {
        return ExecuteFiles(filenames, typeName, methodName, assembliesFiles, null, compiledAssemblyName, parameters);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, string[] assembliesFiles, params object[] parameters)
    {
        return ExecuteFiles(filenames, typeName, methodName, assembliesFiles, null, null, parameters);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, Assembly[] assemblies)
    {
        return ExecuteFiles(filenames, typeName, methodName, assemblies, null, null);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, Assembly[] assemblies, string compiledAssemblyName)
    {
        return ExecuteFiles(filenames, typeName, methodName, null, assemblies, compiledAssemblyName);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, Assembly[] assemblies, string compiledAssemblyName, params object[] parameters)
    {
        return ExecuteFiles(filenames, typeName, methodName, null, assemblies, compiledAssemblyName, parameters);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, Assembly[] assemblies, params object[] parameters)
    {
        return ExecuteFiles(filenames, typeName, methodName, null, assemblies, null, parameters);
    }

    public static ScriptingResult ExecuteFiles(string[] filenames, string typeName, string methodName, string[] assembliesFiles, Assembly[] assemblies, string nomAssemblyCompilee, params object[] parameters)
    {
        ScriptingResult ret = new();
        TLanguage compiler;

        try
        {
            compiler = new();

            ParseOptions parserOptions = compiler.ReturnParserOptions();

            SyntaxTree[] syntaxTrees = [];
            foreach (string file in filenames)
                syntaxTrees = syntaxTrees.Add(compiler.ReturnPrecompiledCode(File.ReadAllText(file), parserOptions));

            ret = CompileAssembly(string.IsNullOrWhiteSpace(nomAssemblyCompilee) ? ExtensionsString.RandomString() : nomAssemblyCompilee, syntaxTrees, assembliesFiles, assemblies, compiler);

            if (ret.Success)
            {
                object myDynamicObject;
                MethodInfo myDynamicMethod;
                object result;

                myDynamicObject = ret.AssemblyGenerate.CreateInstance(typeName);
#pragma warning disable S3011
                myDynamicMethod = myDynamicObject.GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
#pragma warning restore S3011

                result = myDynamicMethod.Invoke(myDynamicObject, parameters);
                if (result != null)
                    ret.Result = result;
            }
        }
        catch (Exception ex)
        {
            ret.ExceptionThrowed = ex;
        }
        return ret;
    }

    #endregion

    #region Execute script(s)

    public static ScriptingResult ExecuteScript(string script)
    {
        return ExecuteScript(script, null, null, null, null);
    }

    public static ScriptingResult ExecuteScript(string script, string compiledAssemblyName)
    {
        return ExecuteScript(script, null, null, null, compiledAssemblyName);
    }

    public static ScriptingResult ExecuteScript(string script, string[] assembliesFiles)
    {
        return ExecuteScript(script, assembliesFiles, null, null, null);
    }

    public static ScriptingResult ExecuteScript(string script, string[] assembliesFiles, string compiledAssemblyName)
    {
        return ExecuteScript(script, assembliesFiles, null, null, compiledAssemblyName);
    }

    public static ScriptingResult ExecuteScript(string script, Assembly[] assemblies)
    {
        return ExecuteScript(script, null, assemblies, null, null);
    }

    public static ScriptingResult ExecuteScript(string script, Assembly[] assemblies, string compiledAssemblyName)
    {
        return ExecuteScript(script, null, assemblies, null, compiledAssemblyName);
    }

    public static ScriptingResult ExecuteScript(string script, string[] assembliesFiles, string[] usings)
    {
        return ExecuteScript(script, assembliesFiles, null, usings, null);
    }

    public static ScriptingResult ExecuteScript(string script, string[] assembliesFiles, string[] usings, string compiledAssemblyName)
    {
        return ExecuteScript(script, assembliesFiles, null, usings, compiledAssemblyName);
    }

    public static ScriptingResult ExecuteScript(string script, Assembly[] assemblies, string[] usings)
    {
        return ExecuteScript(script, null, assemblies, usings);
    }

    public static ScriptingResult ExecuteScript(string script, Assembly[] assemblies, string[] usings, string compiledAssemblyName)
    {
        return ExecuteScript(script, null, assemblies, usings, compiledAssemblyName);
    }

    public static ScriptingResult ExecuteScript(string script, string[] assembliesFiles, Assembly[] assemblies)
    {
        return ExecuteScript(script, assembliesFiles, assemblies, null, null);
    }

    public static ScriptingResult ExecuteScript(string script, string[] assembliesFiles, Assembly[] assemblies, string compiledAssemblyName)
    {
        return ExecuteScript(script, assembliesFiles, assemblies, null, compiledAssemblyName);
    }

    public static ScriptingResult ExecuteScript(string script, string[] assembliesFiles, Assembly[] assemblies, string[] usings)
    {
        return ExecuteScript(script, assembliesFiles, assemblies, usings, null);
    }

    public static ScriptingResult ExecuteScript(string script, string[] assembliesFiles, Assembly[] assemblies, string[] usings, string compiledAssemblyName)
    {
        ScriptingResult ret = new();
        TLanguage compiler;

        try
        {
            StringBuilder myClass = new();

            compiler = new();

            if (usings != null && usings.Length > 0)
                foreach (string import in usings.Where(import => !string.IsNullOrWhiteSpace(import)))
                    myClass.AppendLine(compiler.NamespaceUsing.Replace("<%namespace%>", import));

            myClass.Append(compiler.Header);
            myClass.AppendLine(script);
            myClass.Append(compiler.Footer);

            ParseOptions parserOptions = compiler.ReturnParserOptions();
            ret = CompileAssembly(string.IsNullOrWhiteSpace(compiledAssemblyName) ? ExtensionsString.RandomString() : compiledAssemblyName, [compiler.ReturnPrecompiledCode(myClass.ToString(), parserOptions)], assembliesFiles, assemblies, compiler);

            if (ret.Success)
            {
                object myDynamicObject;
                MethodInfo myDynamicMethod;
                object result;

                myDynamicObject = ret.AssemblyGenerate.CreateInstance("MyDynamicNamespace.MyDynamicClass");
                myDynamicMethod = myDynamicObject.GetType().GetMethod("MyDynamicMethod", BindingFlags.Static | BindingFlags.Public);

                result = myDynamicMethod.Invoke(null, null);
                if (result != null)
                    ret.Result = result;
            }
        }
        catch (Exception ex)
        {
            ret.ExceptionThrowed = ex;
        }
        return ret;
    }

    #endregion

    protected static MetadataReference[] ReturnReferences(string[] assembliesFiles = null, Assembly[] assemblies = null)
    {
        MetadataReference[] listReferences = [];
        listReferences = listReferences.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location)); // Add mscorlib minimum
        listReferences = listReferences.Add(MetadataReference.CreateFromFile(typeof(System.Diagnostics.Process).Assembly.Location)); // Add System.dll
        listReferences = listReferences.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)); // Add System.Core.dll for NET 4.0 and more

        if (assembliesFiles != null && assembliesFiles.Length > 0)
        {
            assembliesFiles = [.. assembliesFiles.Where(a => !string.IsNullOrWhiteSpace(a)).Distinct(nom => nom)];
            foreach (string assemblie in assembliesFiles.Where(assemblie => assemblie.ToLower() != "mscorlib" && assemblie.ToLower() != "system.core"))
                listReferences = listReferences.Add(MetadataReference.CreateFromFile(assemblie));
        }

        if (assemblies != null && assemblies.Length > 0)
        {
            assemblies = [.. assemblies.Where(a => a != null).Distinct()];
            foreach (Assembly assemblie in assemblies)
                if (assemblie.GetName()?.Name.ToLower() != "mscorlib" && assemblie.GetName()?.Name.ToLower() != "system.core" && (assembliesFiles == null || !assembliesFiles.Contains(assemblie.GetName()?.Name, StringComparer.OrdinalIgnoreCase)))
                {
                    byte[] bytesAssembly = assemblie.GetBytes();
                    if (bytesAssembly != null)
                        listReferences = listReferences.Add(MetadataReference.CreateFromStream(new MemoryStream(bytesAssembly)));
                }
        }

        return listReferences;
    }

    protected static ScriptingResult CompileAssembly(string assemblyName, SyntaxTree[] listScripts, string[] assembliesFiles, Assembly[] assemblies, TLanguage instanceObject)
    {
        ScriptingResult ret = new();
        MemoryStream ms = null;

        try
        {
            MetadataReference[] listReferences = ReturnReferences(assembliesFiles, assemblies);

            Type compilerType = typeof(TCompiler);
            MethodInfo mi = compilerType.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
            Compilation compilation = (Compilation)mi.Invoke(null, [assemblyName, listScripts, listReferences, instanceObject.ReturnCompilerOptions()]);
            ms = new MemoryStream();
            EmitResult er = compilation.Emit(ms);
            ret.Success = er.Success;
            if (er.Success)
            {
                ret.AssemblyGenerate = Assembly.Load(ms.ToArray());
            }
            else
            {
                ret.Errors = [.. er.Diagnostics];
            }
        }
        catch (Exception ex)
        {
            ret.ExceptionThrowed = ex;
        }
        finally
        {
            ms?.Dispose();
        }

        return ret;
    }

    public virtual MethodInfo ReturnParserMethod(ParseOptions parserOptions)
    {
        return typeof(TInterpreter).GetMethod("ParseText", BindingFlags.Static | BindingFlags.Public, null, [typeof(string), parserOptions.GetType(), typeof(string), typeof(Encoding), typeof(CancellationToken)], null);
    }

    public virtual SyntaxTree ReturnPrecompiledCode(string code, ParseOptions parserOptions)
    {
        return (SyntaxTree)ReturnParserMethod(parserOptions).Invoke(null, [code, parserOptions, Type.Missing, Type.Missing, Type.Missing]);
    }
}
