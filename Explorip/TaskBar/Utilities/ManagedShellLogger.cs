﻿using System;
using System.IO;
using System.Linq;

using ManagedShell.Common.Logging;
using ManagedShell.Common.Logging.Observers;

namespace Explorip.TaskBar.Utilities;

class ManagedShellLogger : IDisposable
{
    private readonly string _logPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoolBytes\\Explorip"), "Logs");
    private readonly string _logName = DateTime.Now.ToString("yyyy-MM-dd_HHmmssfff");
    private readonly string _logExt = "log";
    private readonly LogSeverity _logSeverity = LogSeverity.Debug;
    private TimeSpan _logRetention = new(7, 0, 0);
    private FileLog _fileLog;
    private bool disposedValue;

    public ManagedShellLogger()
    {
        SetupLogging();
    }

    private void SetupLogging()
    {
        ShellLogger.Severity = _logSeverity;

        SetupFileLog();

        ShellLogger.Attach(new ConsoleLog());
    }

    private void SetupFileLog()
    {
        DeleteOldLogFiles();

        _fileLog = new FileLog(Path.Combine(_logPath, $"{_logName}.{_logExt}"));
        _fileLog.Open();

        ShellLogger.Attach(_fileLog);
    }

    private void DeleteOldLogFiles()
    {
        try
        {
            // look for all of the log files
            DirectoryInfo info = new(_logPath);
            FileInfo[] files = info.GetFiles($"*.{_logExt}", SearchOption.TopDirectoryOnly);

            // delete any files that are older than the retention period
            DateTime now = DateTime.Now;
            foreach (FileInfo file in files.Where(file => now.Subtract(file.LastWriteTime) > _logRetention))
            {
                file.Delete();
            }
        }
        catch (Exception ex)
        {
            ShellLogger.Debug($"Unable to delete old log files: {ex.Message}");
        }
    }

    public bool IsDisposed
    {
        get { return disposedValue; }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _fileLog?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
