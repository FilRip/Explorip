using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace Explorip.Updater;

internal static class AutoUpdater
{
    private const string ApplicationName = "Explorip";
    internal const string UpdateFolder = "AutoUpdate";
    private const string UpdateExtension = ".update";
    private static readonly string AutoUpdateDirectory = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), UpdateFolder);

    internal static Version LatestVersion(bool beta)
    {
        ExtendWebClient client = null;
        Version result = null;
        try
        {
            client = new ExtendWebClient();
            if (beta)
            {
                client.Headers.Add("User-Agent", ApplicationName);
                string json = client.DownloadString(new Uri($"https://api.github.com/repos/filrip/{ApplicationName}/releases"));
                Releases[] deserialized = JsonSerializer.Deserialize<Releases[]>(json);
                if (deserialized != null)
                {
                    Releases lastBeta = Array.Find(deserialized, r => r.PreRelease);
                    if (lastBeta != null)
                    {
                        Version.TryParse(lastBeta.Tag.Replace("v", ""), out result);
                    }
                }
            }
            else
            {
                string readme = client.DownloadString(new Uri($"https://github.com/FilRip/{ApplicationName}/releases/latest"));
                if (!string.IsNullOrWhiteSpace(readme))
                    Version.TryParse(client.LastUri.Segments[client.LastUri.Segments.Length - 1].Replace("v", ""), out result);
            }
        }
        catch (Exception) { /* Ignore errors */ }
        finally
        {
            client?.Dispose();
        }
        return result;
    }

#pragma warning disable S112 // General or reserved exceptions should never be thrown
    internal static void InstallNewVersion(string version, bool beta)
    {
        ExtendWebClient client = null;
        try
        {
            string destFile = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), $"{ApplicationName}.zip");
            if (File.Exists(destFile))
                File.Delete(destFile);
            client = new ExtendWebClient();
            client.DownloadFile(new Uri($"https://github.com/FilRip/{ApplicationName}/releases/download/v{version}/{ApplicationName}{(beta ? "Beta" : "")}.zip"), destFile);
            if (!File.Exists(destFile) || new FileInfo(destFile).Length == 0)
                throw new Exception("Can't download new version");
            if (!ZipManager.Extract(AutoUpdateDirectory, destFile))
                throw new Exception("Can't extract zip file of new version");
            File.Delete(destFile);
            if (!UpdateFile(AutoUpdateDirectory))
                throw new Exception("Can't install new version");
            RemoveAllAutoUpdateDir(AutoUpdateDirectory);
            if (MessageBox.Show(Constants.Localization.ASK_INSTALL_NEW_VERSION.Replace("%1", ApplicationName), ApplicationName, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Restart();
        }
        catch { /* Ignore errors */ }
        finally
        {
            client?.Dispose();
        }
    }
#pragma warning restore S112 // General or reserved exceptions should never be thrown

    internal static void SearchNewVersion(bool beta)
    {
        CleanUpUpdate();
        try
        {
            Version version = LatestVersion(beta);
            if (version != null)
            {
                Version latestVersion = new(version.ToString());
                if (Assembly.GetEntryAssembly().GetName().Version.CompareTo(latestVersion) < 0 &&
                    MessageBox.Show(Constants.Localization.ASK_DOWNLOAD_NEW_VERSION.Replace("%1", ApplicationName), ApplicationName, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    InstallNewVersion(version.ToString(), beta);
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }

    private static void CleanUpUpdate()
    {
        if (Directory.Exists(AutoUpdateDirectory))
            RemoveAllAutoUpdateDir(AutoUpdateDirectory);
        RemoveAllOlderFile(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]));
    }

    private static void RemoveAllAutoUpdateDir(string dir)
    {
        foreach (string file in Directory.GetFiles(dir))
        {
            File.Delete(file);
        }

        foreach (string subDir in Directory.GetDirectories(dir))
        {
            RemoveAllAutoUpdateDir(subDir);
            Directory.Delete(subDir);
        }
    }

    private static void RemoveAllOlderFile(string dir)
    {
        foreach (string file in Directory.GetFiles(dir, $"*{UpdateExtension}"))
        {
            File.Delete(file);
        }

        foreach (string subDir in Directory.GetDirectories(dir))
        {
            RemoveAllOlderFile(subDir);
        }
    }

    private static bool UpdateFile(string directory)
    {
        try
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                string destFile = file.Replace($"{UpdateFolder}\\", "");
                if (File.Exists(destFile + UpdateExtension))
                    File.Delete(destFile + UpdateExtension);
                if (File.Exists(destFile))
                    File.Move(destFile, destFile + UpdateExtension);
                File.Move(file, destFile);
            }
            foreach (string dir in Directory.GetDirectories(directory))
            {
                string destDir = dir.Replace($"{UpdateFolder}\\", "");
                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);
                if (!UpdateFile(dir))
                    return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    private static void Restart()
    {
        Task.Run(() =>
        {
            StringBuilder args = new();
            if (Environment.GetCommandLineArgs().Length > 1)
                for (int i = 1; i < Environment.GetCommandLineArgs().Length; i++)
                {
                    if (args.Length > 0)
                        args.Append(" ");
                    args.Append(Environment.GetCommandLineArgs()[i]);
                }
            Process.Start(Environment.GetCommandLineArgs()[0], args.ToString());
            Environment.Exit(0);
        });
    }

    public class Releases
    {
        [JsonPropertyName("tag_name")]
        public string Tag { get; set; }
        [JsonPropertyName("prerelease")]
        public bool PreRelease { get; set; }
    }
}

internal class ExtendWebClient : WebClient
{
    private Uri _lastUri;

    public Uri LastUri
    {
        get { return _lastUri; }
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
        WebResponse response = base.GetWebResponse(request);
        if (response != null)
            _lastUri = response.ResponseUri;
        return response;
    }

    protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
    {
        WebResponse response = base.GetWebResponse(request, result);
        if (response != null)
            _lastUri = response.ResponseUri;
        return response;
    }
}
