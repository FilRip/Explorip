using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ConsoleControlAPI
{
    /// <summary>
    /// A ProcessEventHandler is a delegate for process input/output events.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="ProcessEventArgs"/> instance containing the event data.</param>
    public delegate void ProcessEventHandler(object sender, ProcessEventArgs args);

    /// <summary>
    /// A class the wraps a process, allowing programmatic input and output.
    /// </summary>
    public class ProcessInterface : IDisposable
    {
        private const int BUFFER_SIZE = 1;
        private readonly object _lockInput;
        private readonly List<string> _historicCommands;
        private readonly object _lockOutput;
        private readonly StringBuilder _builderOutput, _builderError;
        private readonly Thread _threadDetectEnd;
        private readonly AutoResetEvent _eventDetectEnd;
        private readonly Thread _threadDetectEndError;
        private readonly AutoResetEvent _eventDetectEndError;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInterface"/> class.
        /// </summary>
        public ProcessInterface()
        {
            //  Configure the output worker.
            outputWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };
            outputWorker.DoWork += OutputWorker_DoWork;
            outputWorker.ProgressChanged += OutputWorker_ProgressChanged;

            //  Configure the error worker.
            errorWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };
            errorWorker.DoWork += ErrorWorker_DoWork;
            errorWorker.ProgressChanged += ErrorWorker_ProgressChanged;

            _lockInput = new object();
            _lockOutput = new object();
            _historicCommands = new List<string>();
            _builderOutput = new StringBuilder();
            _builderError = new StringBuilder();
            _threadDetectEnd = new(new ThreadStart(DetectEndOutput));
            _threadDetectEnd.Start();
            _eventDetectEnd = new(true);
            _threadDetectEndError = new(new ThreadStart(DetectEndError));
            _threadDetectEndError.Start();
            _eventDetectEndError = new(true);
        }

        #region Output

        /// <summary>
        /// Handles the ProgressChanged event of the outputWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void OutputWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //  Fire the output event.
            FireProcessOutputEvent(e.UserState.ToString());
        }

        /// <summary>
        /// Handles the DoWork event of the outputWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void OutputWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int count;
            char[] buffer = new char[BUFFER_SIZE];

            while (outputWorker?.CancellationPending == false)
            {
                do
                {
                    if (outputReader == null)
                        break;

                    _eventDetectEnd.Reset();
                    count = outputReader.Read(buffer, 0, BUFFER_SIZE);
                    _eventDetectEnd.Set();
                    if (count > 0)
                    {
                        lock (_lockOutput)
                        {
                            if (BUFFER_SIZE > 1 || buffer[0] != (char)13)
                                _builderOutput.Append(buffer, 0, count);
                        }
                    }
                } while (count > 0);
                Thread.Sleep(10);
            }
        }

        private void DetectEndOutput()
        {
            while (true)
            {
                try
                {
                    if (IsProcessRunning && outputWorker?.CancellationPending == false)
                    {
                        if (!_eventDetectEnd.WaitOne(500))
                        {
                            lock (_lockOutput)
                            {
                                if (_builderOutput.Length > 0)
                                {
                                    outputWorker?.ReportProgress(0, _builderOutput.ToString());
                                    _builderOutput.Clear();
                                    Thread.Sleep(10);
                                }
                            }
                        }
                        else
                        {
                            lock (_lockOutput)
                            {
                                if (_builderOutput.Length >= BUFFER_SIZE)
                                {
                                    outputWorker?.ReportProgress(0, _builderOutput.ToString());
                                    _builderOutput.Clear();
                                    Thread.Sleep(10);
                                }
                            }
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception) { /* Ignore all errors */}
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Fires the process output event.
        /// </summary>
        /// <param name="content">The content.</param>
        private void FireProcessOutputEvent(string content)
        {
            //  Get the event and fire it.
            OnProcessOutput?.Invoke(this, new ProcessEventArgs(content));
        }

        #endregion

        #region Errors

        /// <summary>
        /// Handles the ProgressChanged event of the errorWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void ErrorWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //  Fire the error event.
            FireProcessErrorEvent((string)e.UserState);
        }

        /// <summary>
        /// Handles the DoWork event of the errorWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void ErrorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int count;
            char[] buffer = new char[BUFFER_SIZE];

            while (errorWorker?.CancellationPending == false)
            {
                do
                {
                    if (errorReader == null)
                        break;

                    _eventDetectEndError.Reset();
                    count = errorReader.Read(buffer, 0, BUFFER_SIZE);
                    _eventDetectEndError.Set();
                    if (count > 0)
                    {
                        lock (_lockOutput)
                        {
                            if (BUFFER_SIZE > 1 || buffer[0] != (char)13)
                                _builderError.Append(buffer, 0, count);
                        }
                    }
                } while (count > 0);
                Thread.Sleep(10);
            }
        }

        private void DetectEndError()
        {
            while (true)
            {
                try
                {
                    if (IsProcessRunning && errorWorker?.CancellationPending == false)
                    {
                        if (!_eventDetectEndError.WaitOne(500))
                        {
                            lock (_lockOutput)
                            {
                                if (_builderError.Length > 0)
                                {
                                    errorWorker?.ReportProgress(0, _builderError.ToString());
                                    _builderError.Clear();
                                    Thread.Sleep(10);
                                }
                            }
                        }
                        else
                        {
                            lock (_lockOutput)
                            {
                                if (_builderError.Length >= BUFFER_SIZE)
                                {
                                    errorWorker?.ReportProgress(0, _builderError.ToString());
                                    _builderError.Clear();
                                    Thread.Sleep(10);
                                }
                            }
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception) { /* Ignore all errors */}
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Fires the process error output event.
        /// </summary>
        /// <param name="content">The content.</param>
        private void FireProcessErrorEvent(string content)
        {
            //  Get the event and fire it.
            OnProcessError?.Invoke(this, new ProcessEventArgs(content));
        }

        #endregion

        #region Progress manager

        /// <summary>
        /// Runs a process.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="arguments">The arguments.</param>
        public void StartProcess(string fileName, string arguments)
        {
            //  Create the process start info.
            ProcessStartInfo processStartInfo = new(fileName, arguments);
            StartProcess(processStartInfo);
        }

        /// <summary>
        /// Runs a process.
        /// </summary>
        /// <param name="processStartInfo"><see cref="ProcessStartInfo"/> to pass to the process.</param>
        public void StartProcess(ProcessStartInfo processStartInfo)
        {
            //  Set the options.
            processStartInfo.UseShellExecute = false;
            processStartInfo.ErrorDialog = false;
            processStartInfo.CreateNoWindow = true;

            //  Specify redirection.
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;

            //  Create the process.
            process = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = processStartInfo,
            };
            process.Exited += CurrentProcess_Exited;

            //  Start the process.
            try
            {
                process.Start();
            }
            catch (Exception e)
            {
                //  Trace the exception.
                Trace.WriteLine("Failed to start process " + processStartInfo.FileName + " with arguments '" + processStartInfo.Arguments + "'");
                Trace.WriteLine(e.ToString());
                return;
            }

            //  Store name and arguments.
            processFileName = processStartInfo.FileName;
            processArguments = processStartInfo.Arguments;

            //  Create the readers and writers.
            inputWriter = process.StandardInput;
            outputReader = TextReader.Synchronized(process.StandardOutput);
            errorReader = TextReader.Synchronized(process.StandardError);

            //  Run the workers that read output and error.
            outputWorker.RunWorkerAsync();
            errorWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Stops the process.
        /// </summary>
        public void StopProcess()
        {
            //  Handle the trivial case.
            if (!IsProcessRunning)
                return;

            //  Kill the process.
            process.Kill();
        }

        /// <summary>
        /// Handles the Exited event of the currentProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentProcess_Exited(object sender, EventArgs e)
        {
            //  Fire process exited.
            FireProcessExitEvent(process.ExitCode);

            //  Disable the threads.
            outputWorker.CancelAsync();
            errorWorker.CancelAsync();
            inputWriter = null;
            outputReader = null;
            errorReader = null;
            process = null;
            processFileName = null;
            processArguments = null;
        }

        /// <summary>
        /// Fires the process exit event.
        /// </summary>
        /// <param name="code">The code.</param>
        private void FireProcessExitEvent(int code)
        {
            //  Get the event and fire it.
            OnProcessExit?.Invoke(this, new ProcessEventArgs(code));
        }

        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessRunning
        {
            get
            {
                try
                {
                    return (process != null && !process.HasExited);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the internal process.
        /// </summary>
        public Process Process
        {
            get { return process; }
        }

        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        /// <value>
        /// The name of the process.
        /// </value>
        public string ProcessFileName
        {
            get { return processFileName; }
        }

        /// <summary>
        /// Gets the process arguments.
        /// </summary>
        public string ProcessArguments
        {
            get { return processArguments; }
        }

        #endregion

        #region Input

        /// <summary>
        /// Fires the process input event.
        /// </summary>
        /// <param name="content">The content.</param>
#pragma warning disable S1144, IDE0051 // Unused private types or members should be removed
        private void FireProcessInputEvent(string content)
        {
            //  Get the event and fire it.
            OnProcessInput?.Invoke(this, new ProcessEventArgs(content));
        }
#pragma warning restore S1144, IDE0051 // Unused private types or members should be removed

        /// <summary>
        /// Writes the input.
        /// </summary>
        /// <param name="input">The input.</param>
        public void WriteInput(string input)
        {
            lock (_lockInput)
            {
                if (IsProcessRunning)
                {
                    if (!string.IsNullOrWhiteSpace(input))
                        _historicCommands.Insert(0, input);
                    inputWriter.WriteLine(input);
                    inputWriter.Flush();
                }
            }
        }

        public List<string> HistoricCommands
        {
            get { return _historicCommands; }
        }

        #endregion

        #region IDisposable

        private bool disposedValue;
        public bool IsDisposed
        {
            get { return disposedValue; }
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="native">
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool native)
        {
            if (!disposedValue)
            {
                if (outputWorker != null)
                {
                    outputWorker.CancelAsync();
                    outputWorker.Dispose();
                    outputWorker = null;
                }
                if (errorWorker != null)
                {
                    errorWorker.CancelAsync();
                    errorWorker.Dispose();
                    errorWorker = null;
                }
                if (process != null)
                {
                    process.Dispose();
                    process = null;
                }
                if (inputWriter != null)
                {
                    inputWriter.Dispose();
                    inputWriter = null;
                }
                if (outputReader != null)
                {
                    outputReader.Dispose();
                    outputReader = null;
                }
                if (errorReader != null)
                {
                    errorReader.Dispose();
                    errorReader = null;
                }
                _historicCommands?.Clear();
                if (_threadDetectEnd != null && _threadDetectEnd.IsAlive)
                    _threadDetectEnd.Abort();
                _eventDetectEnd.Dispose();
                if (_threadDetectEndError != null && _threadDetectEndError.IsAlive)
                    _threadDetectEndError.Abort();
                _eventDetectEndError.Dispose();
                _builderOutput.Clear();
                _builderError.Clear();
                disposedValue = true;
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// The current process.
        /// </summary>
        private Process process;

        /// <summary>
        /// The input writer.
        /// </summary>
        private StreamWriter inputWriter;

        /// <summary>
        /// The output reader.
        /// </summary>
        private TextReader outputReader;

        /// <summary>
        /// The error reader.
        /// </summary>
        private TextReader errorReader;

        /// <summary>
        /// The output worker.
        /// </summary>
        private BackgroundWorker outputWorker;

        /// <summary>
        /// The error worker.
        /// </summary>
        private BackgroundWorker errorWorker;

        /// <summary>
        /// Current process file name.
        /// </summary>
        private string processFileName;

        /// <summary>
        /// Arguments sent to the current process.
        /// </summary>
        private string processArguments;

        /// <summary>
        /// Occurs when process output is produced.
        /// </summary>
        public event ProcessEventHandler OnProcessOutput;

        /// <summary>
        /// Occurs when process error output is produced.
        /// </summary>
        public event ProcessEventHandler OnProcessError;

        /// <summary>
        /// Occurs when process input is produced.
        /// </summary>
        public event ProcessEventHandler OnProcessInput;

        /// <summary>
        /// Occurs when the process ends.
        /// </summary>
        public event ProcessEventHandler OnProcessExit;
    }
}
