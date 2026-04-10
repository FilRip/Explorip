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

using Explorip.Exceptions;

using static ExploripConfig.Helpers.ExtensionsCommandLineArguments;

namespace Explorip.Updater;

internal static class AutoUpdater
{
    private const string ApplicationName = "Explorip";
    internal const string UpdateFolder = "AutoUpdate";
    private const string UpdateExtension = ".update";
    private static readonly string AutoUpdateDirectory = Path.Combine(ArgumentPathExe(), UpdateFolder);

    internal static Version LatestVersion(bool beta)
    {
        ExtendWebClient client = null;
        Version result = null;
        try
        {
            client = new ExtendWebClient();
            client.Headers.Add("User-Agent", ApplicationName);
            string json = client.DownloadString(new Uri($"https://api.github.com/repos/filrip/{ApplicationName}/releases"));
            Releases[] deserialized = JsonSerializer.Deserialize<Releases[]>(json);
            if (deserialized != null)
            {
                Releases lastBeta = Array.Find(deserialized, r => r.PreRelease == beta);
                if (lastBeta != null)
                    Version.TryParse(lastBeta.Tag.Replace("v", ""), out result);
            }
        }
        catch (Exception) { /* Ignore errors */ }
        finally
        {
            client?.Dispose();
        }
        return result;
    }

    internal static void InstallNewVersion(string version, bool beta)
    {
        ExtendWebClient client = null;
        try
        {
            string destFile = Path.Combine(ArgumentPathExe(), $"{ApplicationName}.zip");
            if (File.Exists(destFile))
                File.Delete(destFile);
            client = new ExtendWebClient();
            client.DownloadFile(new Uri($"https://github.com/FilRip/{ApplicationName}/releases/download/v{version}/{ApplicationName}{(beta ? "Beta" : "")}.zip"), destFile);
            if (!File.Exists(destFile) || new FileInfo(destFile).Length == 0)
                throw new ExploripException("Can't download new version");
            if (!ZipManager.Extract(AutoUpdateDirectory, destFile))
                throw new ExploripException("Can't extract zip file of new version");
            File.Delete(destFile);
            if (!UpdateFile(AutoUpdateDirectory))
                throw new ExploripException("Can't install new version");
            Directory.Delete(AutoUpdateDirectory, true);
            if (MessageBox.Show(Constants.Localization.ASK_INSTALL_NEW_VERSION.Replace("%1", ApplicationName), ApplicationName, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Restart();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Constants.Localization.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            client?.Dispose();
        }
    }

    internal static void SearchNewVersion(bool beta)
    {
        CleanUpUpdate();
        try
        {
            Version version = LatestVersion(beta);
            if (version != null && Assembly.GetEntryAssembly().GetName().Version.CompareTo(version) < 0 &&
                MessageBox.Show(Constants.Localization.ASK_DOWNLOAD_NEW_VERSION.Replace("%1", ApplicationName), ApplicationName, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                InstallNewVersion(version.ToString(), beta);
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }

    private static void CleanUpUpdate()
    {
        if (Directory.Exists(AutoUpdateDirectory))
            Directory.Delete(AutoUpdateDirectory, true);
        RemoveAllOlderFile(ArgumentPathExe());
    }

    private static void RemoveAllOlderFile(string dir)
    {
        try
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
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to delete older file : \r\n{ex.Message}", Constants.Localization.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static bool UpdateFile(string directory)
    {
        try
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                string destFile = file.Replace($"{Path.DirectorySeparatorChar}{UpdateFolder}{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}");
                if (File.Exists(destFile + UpdateExtension))
                    File.Delete(destFile + UpdateExtension);
                if (File.Exists(destFile))
                    File.Move(destFile, destFile + UpdateExtension);
                File.Move(file, destFile);
            }
            foreach (string dir in Directory.GetDirectories(directory))
            {
                string destDir = dir.Replace($"{Path.DirectorySeparatorChar}{UpdateFolder}{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}");
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
            Process.Start(ArgumentFullPathExe(), Arguments());
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
