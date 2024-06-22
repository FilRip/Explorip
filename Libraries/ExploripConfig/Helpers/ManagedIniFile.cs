using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace ExploripConfig.Helpers;

public class ManagedIniFile : IDisposable
{
    private List<string> _commentary = [";", "#", "//"];
    private readonly Mutex _fileAccess;
    private FileStream _configFile;
    private string _currentFileName = "";
    private string[] _section;
    private string _currentSectionName = "";
    private Encoding _currentEncoding;

    public static ManagedIniFile OpenIniFile(string filename)
    {
        return OpenIniFile(filename, Encoding.UTF8, true);
    }

    public static ManagedIniFile OpenIniFile(string filename, bool createIfNotExist)
    {
        return OpenIniFile(filename, Encoding.UTF8, createIfNotExist);
    }

    public static ManagedIniFile OpenIniFile(string filename, Encoding encoding)
    {
        return OpenIniFile(filename, encoding, true);
    }

    public static ManagedIniFile OpenIniFile(string filename, Encoding encoding, bool createIfNotExist)
    {
        ManagedIniFile result = new();
        if (createIfNotExist)
            result.CreateIni(filename);
        result.OpenIni(filename, encoding);
        return result;
    }

    public ManagedIniFile() : base()
    {
        _currentEncoding = Encoding.Default;
        _fileAccess = new Mutex(false, "ExplripConfigFile");
    }

    public ManagedIniFile(string filename) : this()
    {
        OpenIni(filename, Encoding.UTF8);
    }

    public ManagedIniFile(string filename, Encoding encoding) : this()
    {
        OpenIni(filename, encoding);
    }

    public bool OpenIni(string filename, Encoding encoding)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return false;

        try
        {
            _fileAccess.WaitOne();
            long size = new FileInfo(filename).Length;
            if (size > int.MaxValue)
                return false;
            if (size > 0)
            {
                _configFile = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _currentFileName = filename;
                _currentEncoding = encoding;
                _currentSectionName = "";
                _section = [];
                return true;
            }
        }
        catch { /* Ignore errors */ }
        finally
        {
            _fileAccess.ReleaseMutex();
        }
        return false;
    }

    public bool OpenIni(string filename)
    {
        return OpenIni(filename, Encoding.UTF8);
    }

    public bool CreateIni(string filename)
    {
        return CreateIni(filename, Encoding.UTF8);
    }

    public bool CreateIni(string filename, Encoding encoding)
    {
        if (File.Exists(filename))
            return false;
        try
        {
            File.WriteAllText(filename, "[Explorip]", encoding);
            return OpenIni(filename, encoding);
        }
        catch { /* Ignore errors */ }
        return false;
    }

    public void Close()
    {
        try
        {
            _fileAccess.WaitOne();

            _configFile?.Close();
            _configFile = null;
            _currentFileName = "";
            _section = null;
            _currentSectionName = "";
        }
        finally
        {
            _fileAccess.ReleaseMutex();
        }
    }

    public bool WriteString(string sectionName, string paramName, string value)
    {
        if (string.IsNullOrWhiteSpace(_currentFileName))
            return false;

        try
        {
            _fileAccess.WaitOne();
            CheckFileStream();

            if (File.Exists(_currentFileName))
            {
                string filename = _currentFileName;
                Close();
                _currentFileName = filename;
                string[] lines = File.ReadAllLines(_currentFileName);
                bool sectionFound = false;
                bool paramFound = false;
                int lastSectionLine = 0;
                for (int numLine = 0; numLine < lines.Length; numLine++)
                {
                    if (lines[numLine].Trim().StartsWith($"[{sectionName}]", StringComparison.InvariantCultureIgnoreCase) && !sectionFound)
                        sectionFound = true;
                    else
                    {
                        if (sectionFound)
                        {
                            if (lines[numLine].Trim().StartsWith($"{paramName}=", StringComparison.InvariantCultureIgnoreCase))
                            {
                                paramFound = true;
                                lines[numLine] = $"{paramName}={value}";
                                break;
                            }
                            if (lines[numLine].Trim().StartsWith("["))
                                break;
                            lastSectionLine = numLine;
                        }
                    }
                }

                if (!paramFound)
                {
                    if (sectionFound)
                    {
                        while (lines[lastSectionLine].Trim() == "" && lastSectionLine > 0)
                            lastSectionLine--;
                        lines = lines.Insert($"{paramName}={value}", lastSectionLine + 1);
                    }
                    else
                    {
                        lines = lines.Add($"[{sectionName}]");
                        lines = lines.Add($"{paramName}={value}");
                    }
                }
                if (string.IsNullOrWhiteSpace(lines[0]))
                    lines = lines.RemoveAt(0);
                File.Delete(_currentFileName);
                File.AppendAllLines(_currentFileName, lines, _currentEncoding);
            }
            else
                File.WriteAllLines(_currentFileName, [$"[{sectionName}]", $"{paramName}={value}"]);
            OpenIni(_currentFileName, _currentEncoding);
            ReadStartOfSection(sectionName);
            return true;
        }
        catch { /* Ignore errors */ }
        finally
        {
            _fileAccess.ReleaseMutex();
        }
        return false;
    }

    public string ReadString(string sectionName, string paramName)
    {
        try
        {
            _fileAccess.WaitOne();
            CheckFileStream();

            if (string.IsNullOrWhiteSpace(_currentFileName))
                return "";

            return ReadSection(sectionName, paramName);
        }
        finally
        {
            _fileAccess.ReleaseMutex();
        }
    }

    public bool ReadBoolean(string sectionName, string paramName)
    {
        string result = ReadString(sectionName, paramName);
        if (!string.IsNullOrWhiteSpace(result) &&
            ((result.Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase)) ||
            (result.Trim().Equals("yes", StringComparison.InvariantCultureIgnoreCase)) ||
            (result == "1")))
        {
            return true;
        }
        return false;
    }

    public int ReadInteger(string sectionName, string paramName)
    {
        string result = ReadString(sectionName, paramName);
        if (!string.IsNullOrWhiteSpace(result) && int.TryParse(result, out int convert))
            return convert;
        return 0;
    }

    public long ReadLong(string sectionName, string paramName)
    {
        string result = ReadString(sectionName, paramName);
        if (!string.IsNullOrWhiteSpace(result) && long.TryParse(result, out long convert))
            return convert;
        return 0;
    }

    public double ReadDouble(string sectionName, string paramName)
    {
        string result = ReadString(sectionName, paramName);
        if (!string.IsNullOrWhiteSpace(result) && double.TryParse(result, out double convert))
            return convert;
        return 0d;
    }

    public float ReadFloat(string sectionName, string paramName)
    {
        string result = ReadString(sectionName, paramName);
        if (!string.IsNullOrWhiteSpace(result) && float.TryParse(result, out float convert))
            return convert;
        return 0f;
    }

    public DateTime? ReadDateTime(string nomSection, string nomParam)
    {
        string result = ReadString(nomSection, nomParam);
        if (!string.IsNullOrWhiteSpace(result) && DateTime.TryParse(result, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime convert))
            return convert;
        return null;
    }

    public Color ReadColor(string sectionName, string paramName)
    {
        string result = ReadString(sectionName, paramName);
        if (!string.IsNullOrWhiteSpace(result))
        {
            string[] splitter = result.Replace("(", "").Replace(")", "").Split(',');
            return Color.FromArgb(byte.Parse(splitter[0]), byte.Parse(splitter[1]), byte.Parse(splitter[2]), byte.Parse(splitter[3]));
        }

        return Color.White;
    }

    public T ReadEnum<T>(string sectionName, string paramName) where T : struct
    {
        string result = ReadString(sectionName, paramName);
        if (!string.IsNullOrWhiteSpace(result) &&
            Enum.TryParse(result, out T cast))
        {
            return cast;
        }
        return default;
    }

    public Rectangle ReadRectangle(string sectionName, string paramName)
    {
        string result = ReadString(sectionName, paramName);
        if (!string.IsNullOrWhiteSpace(result))
        {
            string[] splitter = result.Replace("(", "").Replace(")", "").Split(',');
            return new Rectangle(int.Parse(splitter[0]), int.Parse(splitter[1]), int.Parse(splitter[2]), int.Parse(splitter[3]));
        }
        return default;
    }

    public string CurrentFileName
    {
        get { return _currentFileName; }
    }

    private void CheckFileStream()
    {
        if (_configFile == null && !string.IsNullOrWhiteSpace(_currentFileName))
            OpenIni(_currentFileName, _currentEncoding);
    }

    private string ReadStartOfSection(string param)
    {
        if (!string.IsNullOrWhiteSpace(_currentFileName))
        {
            _configFile.Seek(0, SeekOrigin.Begin);
            string line;
            while (true)
            {
                line = ReadLine();
                if (line == null)
                    break;
                if (line.Trim().Equals(param.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    return line;
            }
        }
        _section = null;
        _currentSectionName = "";
        return "";
    }

    private string ReadSection(string sectionName, string paramName)
    {
        string result = "";
        bool commentary;

        if (!string.IsNullOrWhiteSpace(_currentFileName) && (sectionName != null) && (sectionName.Trim() != "") && (paramName != null) && (paramName.Trim() != ""))
        {
            if (!_currentSectionName.Equals(sectionName.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                string currentLine;
                List<string> lines = [];

                currentLine = ReadStartOfSection("[" + sectionName + "]");
                if ((currentLine != null) && (currentLine.Trim() != ""))
                {
                    while (true)
                    {
                        currentLine = ReadLine();
                        if ((currentLine == null) || (currentLine.Trim().StartsWith("[")))
                            break;
                        commentary = false;
                        if (_commentary?.Count > 0 &&
                            _commentary.Exists(c => currentLine.Trim().StartsWith(c)))
                        {
                            commentary = true;
                        }
                        if (!commentary)
                            lines.Add(currentLine);
                    }
                    _section = lines.ToArray();
                    _currentSectionName = sectionName.Trim().ToLower();
                }
            }

            if ((_section != null) && (_section.Length > 0))
            {
                for (int j = 0; j < _section.Length; j++)
                {
                    if (_section[j].Trim().StartsWith(paramName.Trim() + "=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int pos = _section[j].IndexOf('=') + 1;
                        result = _section[j].Substring(pos).Trim();
                        break;
                    }
                }
            }
        }
        return result;
    }

    public string[] SectionContent
    {
        get { return _section; }
    }

    public string NameOfCurrentSection
    {
        get { return _currentSectionName; }
    }

    public void SetCommentary(List<string> newCommentaryChars)
    {
        _commentary = newCommentaryChars;
    }

    public void AddCommentary(string commentaryToAdd)
    {
        if ((_commentary != null) && (!_commentary.Contains(commentaryToAdd)))
        {
            _commentary.Add(commentaryToAdd);
        }
    }

    public void RemoveCommentary(string commentaryToRemove)
    {
        if ((_commentary != null) && (_commentary.Contains(commentaryToRemove)))
        {
            _commentary.Remove(commentaryToRemove);
        }
    }

    public int NumberOfSection(string sectionName)
    {
        try
        {
            _fileAccess.WaitOne();
            CheckFileStream();

            int nbSections = 0;
            if (!string.IsNullOrWhiteSpace(_currentFileName))
            {
                _configFile.Seek(0, SeekOrigin.Begin);
                string line;
                while (true)
                {
                    line = ReadLine();
                    if (line == null)
                        break;
                    if (line.Trim().StartsWith("[" + sectionName.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        nbSections++;
                }
            }
            return nbSections;
        }
        finally
        {
            _fileAccess.ReleaseMutex();
        }
    }

    private string ReadLine()
    {
        List<byte> result = [];
        int dataRead;
        while ((dataRead = _configFile.ReadByte()) >= 0)
        {
            // Skip BOM of file
            if (dataRead > 0xB0 || (result.Count == 0 && dataRead == 0x00))
                continue;

            // If we encounter CR or LF
            if (dataRead == 13 || dataRead == 10)
            {
                // And it's not the begining of the read
                if (result.Count > 0)
                    break; // Then it's the end of the line
            }
            else // If it's a char, we add it to the list to nuild the line
                result.Add((byte)dataRead);
        }
        return (result.Count == 0 ? null : _currentEncoding.GetString(result.ToArray()));
    }

    #region IDisposable Support

    public bool IsDisposed
    {
        get { return disposedValue; }
    }

    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Close();
                if ((_section != null) && (_section.Length > 0))
                {
                    Array.Clear(_section, 0, _section.Length);
                    Array.Resize(ref _section, 0);
                }
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
