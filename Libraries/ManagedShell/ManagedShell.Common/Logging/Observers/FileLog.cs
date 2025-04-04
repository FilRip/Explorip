﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ManagedShell.Common.Logging.Observers;

/// <summary>
/// Writes log events to a local file.
/// </summary>
/// <remarks>
/// GoF Design Pattern: Observer.
/// The Observer Design Pattern allows this class to attach itself to an
/// the logger and 'listen' to certain events and be notified of the event. 
/// </remarks>
/// <remarks>
/// Constructor of ObserverLogToFile.
/// </remarks>
/// <param name="fileName">File log to.</param>
public class FileLog(string fileName) : ILog, IDisposable
{
    private readonly object _syncRoot = new();
    private bool _disposed;

    private readonly FileInfo _fileInfo = new(fileName);
    private TextWriter _textWriter;

    public void Open()
    {
        if (!Directory.Exists(_fileInfo.DirectoryName))
            Directory.CreateDirectory(_fileInfo.DirectoryName);

        StreamWriter stream = File.AppendText(_fileInfo.FullName);
        stream.AutoFlush = true;

        _textWriter = TextWriter.Synchronized(stream);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public bool IsDisposed
    {
        get { return _disposed; }
    }

    protected virtual void Dispose(bool disposing)
    {
        lock (_syncRoot)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose of managed resources here
                    DisposeOfManagedResources();
                }

                // dispose of unmanaged resources
                DisposeOfUnManagedResources();

                _disposed = true;
            }
        }
    }

    private void DisposeOfManagedResources()
    {
        try
        {
            _textWriter.Flush();
            _textWriter.Close();
            _textWriter.Dispose();
        }
        catch (Exception)
        {
            /* Ignore errors */
        }
    }

    private void DisposeOfUnManagedResources()
    {
        // Nothing to do here ?!
    }

    /// <summary>
    /// Write a log request to a file.
    /// </summary>
    /// <param name="sender">Sender of the log request.</param>
    /// <param name="e">Parameters of the log request.</param>
    public void Log(object sender, LogEventArgs e)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(string.Format("[{0}] {1}: {2}", e.Date, e.SeverityString, e.Message));
        if (e.Exception != null)
        {
            stringBuilder.AppendLine("\t:::Exception Details:::");
            foreach (string line in e.Exception.ToString().Split([Environment.NewLine], StringSplitOptions.None))
                stringBuilder.AppendLine("\t" + line);
        }

        try
        {
            _textWriter.WriteLine(stringBuilder.ToString());
            _textWriter.Flush();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error writing to FileLog: " + ex.ToString());
        }
    }
}