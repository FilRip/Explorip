using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Win32;

using WindowsDesktop.Properties;

namespace WindowsDesktop.Interop
{
    public static class IID
    {
        private static readonly Regex _osBuildRegex = new Regex(@"v_(?<build>\d{5}?)");

        // ReSharper disable once InconsistentNaming
        public static Dictionary<string, Guid> GetIIDs(string[] targets)
        {
            var known = new Dictionary<string, Guid>();

            foreach (SettingsProperty prop in Settings.Default.Properties.OfType<SettingsProperty>())
            {
                if (int.TryParse(_osBuildRegex.Match(prop.Name).Groups["build"]?.ToString(), out int build)
                    && build == ProductInfo.OSBuild)
                {
                    foreach (string str in (StringCollection)Settings.Default[prop.Name])
                    {
                        string[] pair = str.Split(',');
                        if (pair.Length != 2) continue;

                        string @interface = pair[0];
                        if (targets.All(x => @interface != x) || known.ContainsKey(@interface)) continue;

                        if (!Guid.TryParse(pair[1], out Guid guid)) continue;

                        known.Add(@interface, guid);
                    }

                    break;
                }
            }

            string[] except = targets.Except(known.Keys).ToArray();
            if (except.Length > 0)
            {
                Dictionary<string, Guid> fromRegistry = GetIIDsFromRegistry(except);
                foreach (KeyValuePair<string, Guid> kvp in fromRegistry) known.Add(kvp.Key, kvp.Value);
            }

            return known;
        }

        // ReSharper disable once InconsistentNaming
        private static Dictionary<string, Guid> GetIIDsFromRegistry(string[] targets)
        {
            using (RegistryKey interfaceKey = Registry.ClassesRoot.OpenSubKey("Interface"))
            {
                if (interfaceKey == null)
                {
                    throw new Exception(@"Registry key '\HKEY_CLASSES_ROOT\Interface' is missing.");
                }

                Dictionary<string, Guid> result = new Dictionary<string, Guid>();
                string[] names = interfaceKey.GetSubKeyNames();

                foreach (string name in names)
                {
                    using (RegistryKey key = interfaceKey.OpenSubKey(name))
                    {
                        if (key?.GetValue("") is string value)
                        {
                            string match = targets.FirstOrDefault(x => x == value);
                            if (match != null && Guid.TryParse(key.Name.Split('\\').Last(), out var guid))
                            {
                                result[match] = guid;
                            }
                        }
                    }
                }

                return result;
            }
        }
    }
}
