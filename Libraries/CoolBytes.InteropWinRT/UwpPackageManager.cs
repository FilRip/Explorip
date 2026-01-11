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
        msResource = msResource.Replace("ms-resource:///Resources/", "");
        if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
        {
            if (File.Exists(Path.Combine(path, "resources.pri")))
            {
                using FileStream fs = new(Path.Combine(path, "resources.pri"), FileMode.Open, FileAccess.Read);
                PriFile priFile = PriFile.Parse(fs);
                if (priFile.PriDescriptorSection.PrimaryResourceMapSection != null)
                {
                    ResourceMapSection resourceMapSection = priFile.GetSectionByRef(priFile.PriDescriptorSection.PrimaryResourceMapSection.Value);
                    HierarchicalSchemaSection schemaSection = priFile.GetSectionByRef(resourceMapSection.SchemaSection);
                    foreach (ResourceMapItem item in schemaSection.Items)
                    {
                        if (item.Name == msResource)
                        {
                            if (resourceMapSection.CandidateSets.TryGetValue(item.Index, out CandidateSet? candidateSet))
                            {
                                foreach (Candidate candidate in candidateSet.Candidates)
                                {
                                    if (candidate.Type == EResourceValueType.String || candidate.Type == EResourceValueType.AsciiString || candidate.Type == EResourceValueType.Utf8String)
                                    {
                                        if (candidate.DataItem == null)
                                            continue;
                                        ByteSpan data = priFile.GetDataItemByRef(candidate.DataItem.Value);
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
                        }
                    }
                }
            }
        }
        return null;
    }
}
