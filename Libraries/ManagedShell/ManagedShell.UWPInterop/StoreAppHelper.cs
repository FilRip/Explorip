﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

using ManagedShell.Common.Enums;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;

namespace ManagedShell.UWPInterop;

public static class StoreAppHelper
{
    const string defaultColor = "#00111111";

    private static string userSID;
    private static double scale;

    public readonly static StoreAppList AppList = new();

    internal static List<StoreApp> GetStoreApps()
    {
        List<StoreApp> apps = [];

        try
        {
            foreach (Windows.ApplicationModel.Package package in GetPackages(new Windows.Management.Deployment.PackageManager(), string.Empty))
            {
                string packagePath = GetPackagePath(package);

                if (string.IsNullOrEmpty(packagePath))
                    continue;

                XmlDocument manifest = GetManifest(packagePath);
                XmlNamespaceManager xmlnsManager = GetNamespaceManager(manifest);
                XmlNodeList appNodeList = manifest.SelectNodes("/ns:Package/ns:Applications/ns:Application",
                    xmlnsManager);

                if (appNodeList == null)
                    continue;

                foreach (XmlNode appNode in appNodeList)
                {
                    // packages can contain multiple apps

                    XmlNode showEntry = GetXmlnsNode("uap:VisualElements/@AppListEntry", appNode, xmlnsManager);
                    if (showEntry == null || showEntry.Value.ToLower() == "true" ||
                        showEntry.Value.ToLower() == "default")
                    {
                        // App is visible in the app list
                        StoreApp storeApp = GetAppFromNode(package, packagePath, appNode, xmlnsManager);

                        if (storeApp == null)
                            continue;

                        bool canAdd = true;
                        foreach (StoreApp added in apps)
                        {
                            if (added.AppUserModelId == storeApp.AppUserModelId)
                            {
                                canAdd = false;
                                break;
                            }
                        }

                        if (canAdd)
                            apps.Add(storeApp);
                    }
                }
            }
        }
        catch (Exception e)
        {
            ShellLogger.Error($"StoreAppHelper: Exception retrieving apps: {e.Message}");
        }

        return apps;
    }

    private static StoreApp GetAppFromNode(Windows.ApplicationModel.Package package, string packagePath,
        XmlNode appNode, XmlNamespaceManager xmlnsManager)
    {
        XmlNode appIdNode = appNode.SelectSingleNode("@Id", xmlnsManager);

        if (appIdNode == null)
            return null;

        Dictionary<IconSize, string> icons = GetIcons(packagePath, appNode, xmlnsManager);

        StoreApp storeApp = new(package.Id.FamilyName + "!" + appIdNode.Value)
        {
            DisplayName = GetDisplayName(package.Id.Name, packagePath, appNode, xmlnsManager),
            SmallIconPath = icons[IconSize.Small],
            MediumIconPath = icons[IconSize.Medium],
            LargeIconPath = icons[IconSize.Large],
            ExtraLargeIconPath = icons[IconSize.ExtraLarge],
            JumboIconPath = icons[IconSize.Jumbo],
            IconColor = GetPlateColor(icons[IconSize.Small], appNode, xmlnsManager),
        };

        return storeApp;
    }

    private static XmlDocument GetManifest(string path)
    {
        XmlDocument manifest = new();
        string manPath = path + "\\AppxManifest.xml";

        if (ShellHelper.Exists(manPath))
            manifest.Load(manPath);

        return manifest;
    }

    private static XmlNamespaceManager GetNamespaceManager(XmlDocument manifest)
    {
        XmlNamespaceManager xmlnsManager = new(manifest.NameTable);
        xmlnsManager.AddNamespace("ns", $"http://schemas.microsoft.com/appx/manifest/foundation/windows10");
        xmlnsManager.AddNamespace("uap", $"http://schemas.microsoft.com/appx/manifest/uap/windows10");
        xmlnsManager.AddNamespace("uap2", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/2");
        xmlnsManager.AddNamespace("uap3", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/3");
        xmlnsManager.AddNamespace("uap4", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/4");
        xmlnsManager.AddNamespace("uap5", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/5");
        xmlnsManager.AddNamespace("uap6", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/6");
        xmlnsManager.AddNamespace("uap7", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/7");
        xmlnsManager.AddNamespace("uap8", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/8");
        xmlnsManager.AddNamespace("uap10", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/10");
        xmlnsManager.AddNamespace("uap11", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/11");
        xmlnsManager.AddNamespace("uap12", $"http://schemas.microsoft.com/appx/manifest/uap/windows/10/12");
        xmlnsManager.AddNamespace("uap13", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/13");
        xmlnsManager.AddNamespace("uap15", $"http://schemas.microsoft.com/appx/manifest/foundation/windows10/15");
        xmlnsManager.AddNamespace("uap17", $"http://schemas.microsoft.com/appx/manifest/uap/windows10/17");

        return xmlnsManager;
    }

    private static XmlNode GetXmlnsNode(string nodeText, XmlNode app, XmlNamespaceManager xmlnsManager)
    {
        XmlNode node = app.SelectSingleNode(nodeText, xmlnsManager);

        if (node == null && nodeText.Contains("uap:"))
        {
            int i = 0;
            string[] namespaces = ["uap:", "uap2:", "uap3:", "uap4:", "uap5:", "uap6:", "uap7:", "uap8:", "uap10:", "uap11:", "uap12:", "uap13:", "uap15:", "uap17:"];
            while (node == null && i <= 3)
            {
                nodeText = nodeText.Replace(namespaces[i], namespaces[i + 1]);
                node = app.SelectSingleNode(nodeText, xmlnsManager);
                i++;
            }
        }

        return node;
    }

    private static string GetDisplayName(string packageName, string packagePath, XmlNode app, XmlNamespaceManager xmlnsManager)
    {
        XmlNode nameNode = GetXmlnsNode("uap:VisualElements/@DisplayName", app, xmlnsManager);

        if (nameNode == null)
            return packageName;

        string nameKey = nameNode.Value;

        if (!Uri.TryCreate(nameKey, UriKind.Absolute, out Uri nameUri))
            return nameKey;

        string resourceKey = $"ms-resource://{packageName}/resources/{nameUri.Segments[nameUri.Segments.Length - 1]}";
        string name = ExtractStringFromPRIFile(packagePath + "\\resources.pri", resourceKey);
        if (!string.IsNullOrEmpty(name))
            return name;

        resourceKey = $"ms-resource://{packageName}/{nameUri.Segments[nameUri.Segments.Length - 1]}";
        name = ExtractStringFromPRIFile(packagePath + "\\resources.pri", resourceKey);
        if (!string.IsNullOrEmpty(name))
            return name;

        return ExtractStringFromPRIFile(packagePath + "\\resources.pri", nameUri.ToString());
    }

    private static string GetPlateColor(string iconPath, XmlNode app, XmlNamespaceManager xmlnsManager)
    {
        if (iconPath.EndsWith("_altform-unplated.png"))
            return defaultColor;

        XmlNode colorKey = GetXmlnsNode("uap:VisualElements/@BackgroundColor", app, xmlnsManager);

        if (colorKey != null && !string.IsNullOrEmpty(colorKey.Value) && colorKey.Value.ToLower() != "transparent")
            return colorKey.Value;

        return defaultColor;
    }

    private static Dictionary<IconSize, string> GetIcons(string path, XmlNode app, XmlNamespaceManager xmlnsManager)
    {
        Dictionary<IconSize, string> icons = [];
        XmlNode iconNode = GetXmlnsNode("uap:VisualElements/@Square44x44Logo", app, xmlnsManager);

        if (iconNode == null)
            return icons;

        string iconPath = path + Path.DirectorySeparatorChar + (iconNode.Value).Replace(".png", "");

        // get all resources, then use the first match
        string[] files = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
        string baseName = Path.GetFileNameWithoutExtension(iconPath + ".png").ToLower();

        icons[IconSize.Small] = GetIconPath(files, baseName, IconSize.Small);
        icons[IconSize.Medium] = GetIconPath(files, baseName, IconSize.Medium);
        icons[IconSize.Large] = GetIconPath(files, baseName, IconSize.Large);
        icons[IconSize.ExtraLarge] = GetIconPath(files, baseName, IconSize.ExtraLarge);
        icons[IconSize.Jumbo] = GetIconPath(files, baseName, IconSize.Jumbo);

        return icons;
    }

    private static string GetIconPath(string[] files, string baseName, IconSize size)
    {
        List<string> iconAssets = [
            ".targetsize-32_altform-unplated.png",
            ".targetsize-32_altform-unplated_contrast-black.png",
            ".targetsize-36_altform-unplated.png",
            ".targetsize-36_altform-unplated_contrast-black.png",
            ".targetsize-40_altform-unplated.png",
            ".targetsize-40_altform-unplated_contrast-black.png",
            ".targetsize-48_altform-unplated.png",
            ".targetsize-48_altform-unplated_contrast-black.png",
            ".png",
            "_contrast-black.png",
            ".targetsize-32.png",
            ".targetsize-32_contrast-black.png",
            ".targetsize-36.png",
            ".targetsize-36_contrast-black.png",
            ".targetsize-40.png",
            ".targetsize-40_contrast-black.png",
            ".targetsize-44.png",
            ".targetsize-44_contrast-black.png",
            ".targetsize-48.png",
            ".targetsize-48_contrast-black.png",
            ".targetsize-256_altform-unplated.png",
            ".targetsize-256_altform-unplated_contrast-black.png",
            ".scale-200.png",
            ".scale-200_contrast-black.png",
            ".targetsize-24_altform-unplated.png",
            ".targetsize-24_altform-unplated_contrast-black.png",
            ".targetsize-16_altform-unplated.png",
            ".targetsize-16_altform-unplated_contrast-black.png",
            ".targetsize-24.png",
            ".targetsize-24_contrast-black.png",
            ".targetsize-16.png",
            ".targetsize-16_contrast-black.png",
            ".scale-100.png",
            ".scale-100_contrast-black.png",
            ".targetsize-256.png",
            ".targetsize-256_contrast-black.png"
        ];

        // do some sorting based on DPI for prettiness
        if (scale == 0)
            scale = DpiHelper.DpiScale;

        int numMoved = 0;
        for (int i = 0; i < iconAssets.Count; i++)
        {
            if ((scale < 1.25 && size == IconSize.Small && iconAssets[i].Contains("16")) ||
                (((scale >= 1.25 && scale < 1.75 && size == IconSize.Small) || (scale < 1.25 && size == IconSize.Medium)) && iconAssets[i].Contains("24")) ||
                (((scale >= 1.5 && size == IconSize.Medium) || (scale >= 1.25 && scale <= 1.75 && size == 0)) && iconAssets[i].Contains("48")) ||
                (((scale >= 1.5 && size != IconSize.Small) || (scale >= 1.25 && size == 0) || size == IconSize.Jumbo) && (iconAssets[i].Contains("200") || iconAssets[i].Contains("100") || iconAssets[i].Contains("256"))))
            {
                string copy = iconAssets[i];
                iconAssets.RemoveAt(i);
                iconAssets.Insert(numMoved, copy);
                numMoved++;
            }
        }

        foreach (string iconName in iconAssets)
        {
            string fullName = baseName + iconName;

            string filename = files.FirstOrDefault(fileName => string.Equals(Path.GetFileName(fileName), fullName, StringComparison.OrdinalIgnoreCase) && File.Exists(fileName));
            if (!string.IsNullOrWhiteSpace(filename))
                return filename;
        }

        return string.Empty;
    }

    private static IEnumerable<Windows.ApplicationModel.Package> GetPackages(Windows.Management.Deployment.PackageManager pman, string packageFamilyName)
    {
        userSID ??= System.Security.Principal.WindowsIdentity.GetCurrent().User?.ToString();

        if (userSID == null)
            return [];

        try
        {
            if (string.IsNullOrEmpty(packageFamilyName))
                return pman.FindPackagesForUser(userSID);

            return pman.FindPackagesForUser(userSID, packageFamilyName);
        }
        catch
        {
            return [];
        }
    }

    private static string GetPackagePath(Windows.ApplicationModel.Package package)
    {
        string path = "";

        // need to catch a system-thrown exception...
        try
        {
            path = package.InstalledLocation.Path;
        }
        catch { /* Ignore errors */ }

        return path;
    }

    internal static StoreApp GetStoreApp(string appUserModelId)
    {
        string[] pkgAppId = appUserModelId.Split('!');
        string packageFamilyName;
        string appId;

        if (pkgAppId.Count() > 1)
        {
            packageFamilyName = pkgAppId[0];
            appId = pkgAppId[1];
        }
        else
            return null;

        foreach (Windows.ApplicationModel.Package package in GetPackages(new Windows.Management.Deployment.PackageManager(), packageFamilyName))
        {
            string packagePath = GetPackagePath(package);

            if (string.IsNullOrEmpty(packagePath))
                continue;

            XmlDocument manifest = GetManifest(packagePath);
            XmlNamespaceManager xmlnsManager = GetNamespaceManager(manifest);
            XmlNodeList appNodeList =
                manifest.SelectNodes("/ns:Package/ns:Applications/ns:Application", xmlnsManager);

            if (appNodeList == null)
                return null;

            foreach (XmlNode appNode in appNodeList)
            {
                // get specific app in package

                if (appNode.SelectSingleNode("@Id", xmlnsManager)?.Value == appId)
                {
                    // return values
                    return GetAppFromNode(package, packagePath, appNode, xmlnsManager);
                }
            }
        }

        return null;
    }

    [DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false, ThrowOnUnmappableChar = true)]
    private static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);

    internal static string ExtractStringFromPRIFile(string pathToPRI, string resourceKey)
    {
        string sWin8ManifestString = $"@{{{pathToPRI}? {resourceKey}}}";
        StringBuilder outBuff = new(256);
        SHLoadIndirectString(sWin8ManifestString, outBuff, outBuff.Capacity, IntPtr.Zero);
        return outBuff.ToString();
    }
}
