using System;

namespace ManagedShell.Common.Logging;

/// <summary>
/// Contains log specific event data for log events.
/// </summary>
/// <remarks>
/// Constructor of LogEventArgs.
/// </remarks>
/// <param name="severity">Log severity.</param>
/// <param name="message">Log message</param>
/// <param name="exception">Inner exception.</param>
/// <param name="date">Log date.</param>
public class LogEventArgs(LogSeverity severity, string message, Exception exception, DateTime date) : EventArgs
{

    /// <summary>
    /// Gets and sets the log severity.
    /// </summary>        
    public LogSeverity Severity { get; private set; } = severity;

    /// <summary>
    /// Gets and sets the log message.
    /// </summary>        
    public string Message { get; private set; } = message;

    /// <summary>
    /// Gets and sets the optional inner exception.
    /// </summary>        
    public Exception Exception { get; private set; } = exception;

    /// <summary>
    /// Gets and sets the log date and time.
    /// </summary>        
    public DateTime Date { get; private set; } = date;

    /// <summary>
    /// Friendly string that represents the severity.
    /// </summary>
    public string SeverityString
    {
        get { return Severity.ToString("G"); }
    }

    /// <summary>
    /// LogEventArgs as a string representation.
    /// </summary>
    /// <returns>String representation of the LogEventArgs.</returns>
    public override string ToString()
    {
        return string.Format("{0} - {1} - {2} - {3}", Date, SeverityString, Message, Exception);
    }
}