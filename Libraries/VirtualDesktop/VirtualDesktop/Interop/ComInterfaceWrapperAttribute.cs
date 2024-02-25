﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WindowsDesktop.Interop
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ComInterfaceWrapperAttribute : Attribute
    {
        public string InterfaceName { get; set; }

        public uint LatestVersion { get; set; } = 1;

        public int BuildVersion { get; set; } = -1;

        public ComInterfaceWrapperAttribute() { }

        public ComInterfaceWrapperAttribute(string interfaceName)
        {
            InterfaceName = interfaceName;
        }

        public ComInterfaceWrapperAttribute(string interfaceName, int buildVersion)
        {
            InterfaceName = interfaceName;
            BuildVersion = buildVersion;
        }

        public ComInterfaceWrapperAttribute(string interfaceName, uint latestVersion, int buildVersion)
        {
            InterfaceName = interfaceName;
            LatestVersion = latestVersion;
            BuildVersion = buildVersion;
        }

        public ComInterfaceWrapperAttribute(uint latestVersion)
        {
            LatestVersion = latestVersion;
        }

        public static string GetInterfaceName(Type wrapperType)
            => $"I{wrapperType.Name}";
    }

    public static class ComInterfaceWrapperAttributeExtensions
    {
        /// <summary>
        /// Gets COM interface name if specific type has '<see cref="ComInterfaceWrapperAttribute"/>' attribute.
        /// </summary>
        public static string GetComInterfaceNameIfWrapper(this Type type)
        {
            ComInterfaceWrapperAttribute attr = type.GetCustomAttribute<ComInterfaceWrapperAttribute>();
            return attr != null
                ? attr.InterfaceName ?? $"I{type.Name}"
                : null;
        }

        /// <summary>
        /// Gets COM interface names if specific type has '<see cref="ComInterfaceWrapperAttribute"/>' attribute.
        /// </summary>
        public static IEnumerable<string> GetComInterfaceNamesIfWrapper(this Type type, int targetBuild = -1)
        {
            ComInterfaceWrapperAttribute attr = type.GetCustomAttribute<ComInterfaceWrapperAttribute>();
            string baseName = attr != null
                ? attr.InterfaceName ?? $"I{type.Name}"
                : null;

#pragma warning disable IDE0301 // Simplifier l'initialisation des collections
            if (string.IsNullOrEmpty(baseName))
                return Enumerable.Empty<string>();
            if (targetBuild != -1 && attr != null && attr.BuildVersion != -1 && attr.BuildVersion != targetBuild)
                return Enumerable.Empty<string>();

            if (attr != null)
                return Enumerable.Range(1, (int)attr.LatestVersion)
                    .Select(v => v == 1 ? baseName : $"{baseName}{v}");
            else
                return Enumerable.Empty<string>();
#pragma warning restore IDE0301 // Simplifier l'initialisation des collections
        }
    }
}
