using System.Collections.Generic;
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
    private const string DEFAULT_PRI_FILES = "resources.pri";

    public static string? PathOfUwp(string appUserModelId)
    {
        PackageManager packageManager = new();
        IEnumerable<Package> packages = packageManager.FindPackagesForUser(string.Empty, appUserModelId);
        Package package = packages.FirstOrDefault();
        return package?.InstalledLocation?.Path;
    }

    public static string? StringOfUwp(string appUserModelId, string msResource)
    {
        string? path = PathOfUwp(appUserModelId);
        if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
        {
            msResource = msResource.Replace("ms-resource:///Resources/", "");
            if (File.Exists(Path.Combine(path, DEFAULT_PRI_FILES)))
            {
                string? result = GetStringFromPri(Path.Combine(path, DEFAULT_PRI_FILES), msResource);
                if (result != null)
                    return result;
            }
            foreach (string otherPriFile in Directory.GetFiles(path, "*.pri").Where(f => Path.GetFileName(f).ToLower() != DEFAULT_PRI_FILES))
            {
                string? result = GetStringFromPri(otherPriFile, msResource);
                if (result != null)
                    return result;
            }
        }
        return null;
    }

    public static string? GetStringFromPri(string path, string msResource)
    {
        using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
        PriFile priFile = PriFile.Parse(fs);
        if (priFile.PriDescriptorSection.PrimaryResourceMapSection != null)
        {
            ResourceMapSection resourceMapSection = priFile.GetSectionByRef(priFile.PriDescriptorSection.PrimaryResourceMapSection.Value);
            HierarchicalSchemaSection schemaSection = priFile.GetSectionByRef(resourceMapSection.SchemaSection);
            ResourceMapItem? item = schemaSection.Items.FirstOrDefault(section => section.Name == msResource);
            if (item != null &&
                resourceMapSection.CandidateSets.TryGetValue(item.Index, out CandidateSet? candidateSet))
            {
                Candidate candidate = candidateSet.Candidates.FirstOrDefault(c => c.DataItem != null &&
                                                                                  (c.Type == EResourceValueType.String || c.Type == EResourceValueType.AsciiString || c.Type == EResourceValueType.Utf8String));
                if (candidate != null)
                {
                    ByteSpan data = priFile.GetDataItemByRef(candidate.DataItem!.Value);
                    fs.Seek(data.Offset, SeekOrigin.Begin);
                    byte[] buffer = new byte[data.Length];
                    _ = fs.Read(buffer, 0, buffer.Length);
                    Encoding encoding = Encoding.ASCII;
                    if (candidate.Type == EResourceValueType.Utf8String)
                        encoding = Encoding.UTF8;
                    string result = encoding.GetString(buffer);
                    return result.TrimEnd((char)0);
                }
            }
        }
        return null;
    }
}
