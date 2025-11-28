using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using ExploripPlugins;

namespace Explorip.Plugins;

public static class PluginsManager
{
    private static readonly List<IExploripToolbar> _listPlugins = [];

    public static void LoadPlugins()
    {
        string root = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "Plugins");
        if (!Directory.Exists(root))
            return;
        LoadPlugins(root);
        foreach (string dir in Directory.GetDirectories(root))
            LoadPlugins(dir);
    }

    public static void LoadPlugins(string folder)
    {
        foreach (string files in Directory.GetFiles(folder, "*.dll"))
        {
            try
            {
#pragma warning disable IDE0079
#pragma warning disable S3885 // "Assembly.Load" should be used
                Assembly assembly = Assembly.LoadFrom(files);
#pragma warning restore S3885 // "Assembly.Load" should be used
#pragma warning restore IDE0079
                foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IExploripToolbar).IsAssignableFrom(t)))
                {
                    try
                    {
                        IExploripToolbar plugin = (IExploripToolbar)Activator.CreateInstance(type);
                        _listPlugins.Add(plugin);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error loading plugin type {type.Name} from {files}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading plugin {files}: {ex.Message}");
            }
        }
    }

    public static IEnumerable<string> ListName()
    {
        return _listPlugins.Select(plugin => plugin.Name);
    }

    public static List<IExploripToolbar> ListPlugins()
    {
        return _listPlugins;
    }

    public static IExploripToolbar GetPlugin(string name)
    {
        return _listPlugins.FirstOrDefault(plugin => plugin.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public static IExploripToolbar GetPlugin(Guid guid)
    {
        return _listPlugins.FirstOrDefault(plugin => plugin.GuidKey.Equals(guid));
    }

    public static void Reload()
    {
        _listPlugins.Clear();
        LoadPlugins();
    }
}
