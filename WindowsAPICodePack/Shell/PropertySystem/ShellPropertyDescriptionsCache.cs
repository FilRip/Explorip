//Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem
{
    internal class ShellPropertyDescriptionsCache
    {
        private ShellPropertyDescriptionsCache()
        {
            propsDictionary = new Dictionary<PropertyKey, ShellPropertyDescription>();
        }

        private readonly IDictionary<PropertyKey, ShellPropertyDescription> propsDictionary;
        private static ShellPropertyDescriptionsCache cacheInstance;

        public static ShellPropertyDescriptionsCache Cache
        {
            get
            {
                cacheInstance ??= new ShellPropertyDescriptionsCache();
                return cacheInstance;
            }
        }

        public ShellPropertyDescription GetPropertyDescription(PropertyKey key)
        {
            if (!propsDictionary.ContainsKey(key))
            {
                propsDictionary.Add(key, new ShellPropertyDescription(key));
            }
            return propsDictionary[key];
        }
    }
}
