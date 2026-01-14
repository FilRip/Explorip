using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using CoolBytes.InteropWinRT.PriReader;
using CoolBytes.InteropWinRT.PriReader.Constants;

using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace CoolBytes.InteropWinRT;

public static class UwpPackageManager
{
    public static string? PathOfUwp(string appUserModelId)
    {
        PackageManager packageManager = new();
        IEnumerable<Package> packages = packageManager.FindPackagesForUser(string.Empty, appUserModelId);
        Package package = packages.FirstOrDefault();
        return package?.InstalledLocation?.Path;
    }

    public static string? PathOfLocalizedResources(string appUserModelId)
    {
        PackageManager packageManager = new();
        Package package = packageManager.FindPackagesForUserWithPackageTypes(string.Empty, PackageTypes.Resource).FirstOrDefault(pkg => pkg.Id.FamilyName == appUserModelId && pkg.Id.ResourceId.Contains("-" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName));
        if (package != null)
            return package.InstalledLocation.Path;
        return null;
    }

    public static string? StringOfUwp(string appUserModelId, string msResource)
    {
        msResource = msResource.Replace("ms-resource:///Resources/", "");
        string? path = PathOfUwp(appUserModelId);
        ushort itemId = 0;
        string? result = null;
        if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
        {
            // Search in default language of application first (and to retrieve Index of resource for localized PRI files)
            result = BrowsePriFiles(path!, msResource, ref itemId);
        }
        if (itemId > 0)
        {
            path = PathOfLocalizedResources(appUserModelId);
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                // Search in localized resources
                string? localized = BrowsePriFiles(path!, msResource, ref itemId);
                if (!string.IsNullOrWhiteSpace(localized))
                    return localized;
            }
        }
        return result;
    }

    public static string? BrowsePriFiles(string path, string msResource, ref ushort itemId)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;
        foreach (string otherPriFile in Directory.GetFiles(path, "*.pri"))
        {
            string? result = GetStringFromPri(otherPriFile, msResource, ref itemId);
            if (result != null)
                return result;
        }
        return null;
    }

    public static string? GetStringFromPri(string path, string msResource, ref ushort itemId)
    {
        using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
        PriFile priFile = PriFile.Parse(fs);
        if (priFile.PriDescriptorSection.PrimaryResourceMapSection != null)
        {
            ResourceMapSection resourceMapSection = priFile.GetSectionByRef(priFile.PriDescriptorSection.PrimaryResourceMapSection.Value);
            if (itemId != 0 && resourceMapSection.CandidateSets.TryGetValue(itemId, out CandidateSet? candidateSetLocalized))
            {
                Candidate candidateLocalized = candidateSetLocalized.Candidates.FirstOrDefault(c => c.DataItem != null &&
                                                                                                    (c.Type == EResourceValueType.String || c.Type == EResourceValueType.AsciiString || c.Type == EResourceValueType.Utf8String));
                ByteSpan dataLocalized = priFile.GetDataItemByRef(candidateLocalized.DataItem!.Value);
                string? localizedString = dataLocalized.ReadString(fs, (candidateLocalized.Type == EResourceValueType.Utf8String ? Encoding.UTF8 : Encoding.ASCII));
                if (!string.IsNullOrWhiteSpace(localizedString))
                    return localizedString;
            }
            HierarchicalSchemaSection schemaSection = priFile.GetSectionByRef(resourceMapSection.SchemaSection);
            ResourceMapItem? item = schemaSection.Items.FirstOrDefault(section => section.Name == msResource);
            if (item != null &&
                resourceMapSection.CandidateSets.TryGetValue(item.Index, out CandidateSet? candidateSet))
            {
                Candidate candidate = candidateSet.Candidates.FirstOrDefault(c => c.DataItem != null &&
                                                                                  (c.Type == EResourceValueType.String || c.Type == EResourceValueType.AsciiString || c.Type == EResourceValueType.Utf8String));
                if (candidate != null)
                {
                    itemId = item.Index;
                    ByteSpan data = priFile.GetDataItemByRef(candidate.DataItem!.Value);
                    string result = data.ReadString(fs, (candidate.Type == EResourceValueType.Utf8String ? Encoding.UTF8 : Encoding.ASCII));
                    return result;
                }
            }
        }
        return null;
    }
}
