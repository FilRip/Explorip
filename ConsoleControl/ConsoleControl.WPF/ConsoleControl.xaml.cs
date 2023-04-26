using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ConsoleControlAPI;

namespace ConsoleControl.WPF
{
    /// <summary>
    /// Interaction logic for ConsoleControl.xaml
    /// </summary>
    public partial class ConsoleControl : UserControl, IDisposable
    {
        #region Fields

        private readonly object _lockInput;
        private int offset;
        /// <summary>
        /// The internal process interface used to interface with the process.
        /// </summary>
        private readonly ProcessInterface processInterface;

        /// <summary>
        /// Current position that input starts at.
        /// </summary>
        private int inputStartPos;

        /// <summary>
        /// The last input string (used so that we can make sure we don't echo input twice).
        /// </summary>
        private string lastInput;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleControl"/> class.
        /// </summary>
        public ConsoleControl()
        {
            InitializeComponent();
            _lockInput = new object();
            offset = 6;
            processInterface = new ProcessInterface();
            IsInputEnabled = true;
            _currentPosInHistoric = -1;

            //  Handle process events.
            processInterface.OnProcessOutput += ProcessInterface_OnProcessOutput;
            processInterface.OnProcessError += ProcessInterface_OnProcessError;
            processInterface.OnProcessInput += ProcessInterface_OnProcessInput;
            processInterface.OnProcessExit += ProcessInterface_OnProcessExit;
        }

        #endregion

        #region Events ProcessInterface

        /// <summary>
        /// Handles the OnProcessError event of the processInterace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ProcessEventArgs"/> instance containing the event data.</param>
        private void ProcessInterface_OnProcessError(object sender, ProcessEventArgs args)
        {
            //  Write the output, in red
            WriteOutput(args.Content, new SolidColorBrush(Colors.Red));

            //  Fire the output event.
            FireProcessOutputEvent(args);
        }

        /// <summary>
        /// Handles the OnProcessOutput event of the processInterace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ProcessEventArgs"/> instance containing the event data.</param>
        private void ProcessInterface_OnProcessOutput(object sender, ProcessEventArgs args)
        {
            //  Write the output, in white
            WriteOutput(args.Content, richTextBoxConsole.Foreground);

            //  Fire the output event.
            FireProcessOutputEvent(args);
        }

        /// <summary>
        /// Handles the OnProcessInput event of the processInterace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ProcessEventArgs"/> instance containing the event data.</param>
        private void ProcessInterface_OnProcessInput(object sender, ProcessEventArgs args)
        {
            FireProcessInputEvent(args);
        }

        /// <summary>
        /// Handles the OnProcessExit event of the processInterace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ProcessEventArgs"/> instance containing the event data.</param>
        private void ProcessInterface_OnProcessExit(object sender, ProcessEventArgs args)
        {
            //  Read only again.
            RunOnUIDispatcher(() =>
            {
                //  Are we showing diagnostics?
                if (ShowDiagnostics)
                {
                    WriteOutput(Environment.NewLine + processInterface.ProcessFileName + " exited.", new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)));
                }

                richTextBoxConsole.IsReadOnly = true;

                //  And we're no longer running.
                IsProcessRunning = false;
            });
        }

        #endregion

        #region Keyboard input

        /// <summary>
        /// Handles the KeyDown event of the richTextBoxConsole control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void RichTextBoxConsole_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            lock (_lockInput)
            {
                int caretPosition = richTextBoxConsole.GetCaretPosition();
                int delta = caretPosition - inputStartPos;
                bool inReadOnlyZone = delta < 0;

                //  If we're at the input point and it's backspace, bail.
                if (delta == 0 && e.Key == Key.Back)
                    e.Handled = true;

                if (e.Key == Key.Up || e.Key == Key.Down)
                {
                    NavigateHistoric(e.Key);
                    e.Handled = true;
                }

                if (e.Key == Key.Escape)
                {
                    SetCommand();
                    e.Handled = true;
                }

                if (e.Key == Key.Left && ((caretPosition + (offset - 4)) <= inputStartPos))
                {
                    e.Handled = true;
                }

                if (e.Key == Key.Home)
                {
                    richTextBoxConsole.CaretPosition = richTextBoxConsole.GetPointerAt(inputStartPos);
                    e.Handled = true;
                }

                //  Are we in the read-only zone?
                //  Allow arrows and Ctrl-C.
                if (inReadOnlyZone && (!(e.Key == Key.Left ||
                    e.Key == Key.Right ||
                    (e.Key == Key.C && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) ||
                    e.Key == Key.Tab)))
                {
                    e.Handled = true;
                }

                if (e.Key == Key.Return)
                {
                    richTextBoxConsole.SetCaretToEnd();
                }
            }
        }

        private void RichTextBoxConsole_KeyUp(object sender, KeyEventArgs e)
        {
            //  Is it the return key?
            if (e.Key == Key.Return)
            {
                int caretPosition = richTextBoxConsole.GetCaretPosition();
                int delta = caretPosition - inputStartPos;
                //  Get the input.
                string rtb = new TextRange(richTextBoxConsole.Document.ContentStart, richTextBoxConsole.Document.ContentEnd).Text.Trim();
                string cmd = rtb.Substring(rtb.Length - delta + offset);
                if (offset == 6)
                    offset = 4;
                //  Write the input (without echoing).
                WriteInput(cmd, Colors.White, false);
            }
        }

        /// <summary>
        /// Writes the input to the console control.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="color">The color.</param>
        /// <param name="echo">if set to <c>true</c> echo the input.</param>
        public void WriteInput(string input, Color color, bool echo)
        {
            RunOnUIDispatcher(() =>
            {
                //  Are we echoing?
                if (echo)
                {
                    richTextBoxConsole.Selection.ApplyPropertyValue(TextBlock.ForegroundProperty, new SolidColorBrush(color));
                    richTextBoxConsole.AppendText(input);
                    inputStartPos = richTextBoxConsole.GetEndPosition();
                }

                lastInput = input;

                //  Write the input.
                processInterface.WriteInput(input);

                if (!string.IsNullOrWhiteSpace(input) && _currentPosInHistoric >= 0)
                    _currentPosInHistoric++;

                //  Fire the event.
                FireProcessInputEvent(new ProcessEventArgs(input));
            });
        }

        #endregion

        #region Output console

        /// <summary>
        /// Writes the output to the console control.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="color">The color.</param>
        public void WriteOutput(string output, Brush color)
        {
            if (!string.IsNullOrEmpty(lastInput) &&
                (output == lastInput || output.Replace("\r\n", "") == lastInput))
                return;

            RunOnUIDispatcher(() =>
            {
                //  Write the output.
                TextRange range = new(richTextBoxConsole.GetEndPointer(), richTextBoxConsole.GetEndPointer())
                {
                    Text = output
                };
                range.ApplyPropertyValue(TextElement.ForegroundProperty, color);

                //  Record the new input start.
                richTextBoxConsole.ScrollToEnd();
                richTextBoxConsole.SetCaretToEnd();
                inputStartPos = richTextBoxConsole.GetCaretPosition();
            });
        }

        /// <summary>
        /// Clears the output.
        /// </summary>
        public void ClearOutput()
        {
            richTextBoxConsole.Document.Blocks.Clear();
            inputStartPos = 0;
        }

        #endregion

        #region Process manager

        /// <summary>
        /// Runs a process.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="arguments">The arguments.</param>
        public void StartProcess(string fileName, string arguments)
        {
            StartProcess(new ProcessStartInfo(fileName, arguments));
        }

        /// <summary>
        /// Runs a process.
        /// </summary>
        /// <param name="processStartInfo"><see cref="ProcessStartInfo"/> to pass to the process.</param>
        public void StartProcess(ProcessStartInfo processStartInfo)
        {
            //  Are we showing diagnostics?
            if (ShowDiagnostics)
            {
                WriteOutput("Preparing to run " + processStartInfo.FileName, new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)));
                if (!string.IsNullOrEmpty(processStartInfo.Arguments))
                    WriteOutput(" with arguments " + processStartInfo.Arguments + "." + Environment.NewLine, new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)));
                else
                    WriteOutput("." + Environment.NewLine, new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)));
            }

            //  Start the process.
            processInterface.StartProcess(processStartInfo);

            RunOnUIDispatcher(() =>
            {
                //  If we enable input, make the control not read only.
                if (IsInputEnabled)
                    richTextBoxConsole.IsReadOnly = false;

                //  We're now running.
                IsProcessRunning = true;

            });
        }

        /// <summary>
        /// Stops the process.
        /// </summary>
        public void StopProcess()
        {
            //  Stop the interface.
            processInterface.StopProcess();
        }

        /// <summary>
        /// Fires the console output event.
        /// </summary>
        /// <param name="args">The <see cref="ProcessEventArgs"/> instance containing the event data.</param>
        private void FireProcessOutputEvent(ProcessEventArgs args)
        {
            //  Fire the event if it is set.
            OnProcessOutput?.Invoke(this, args);
        }

        /// <summary>
        /// Fires the console input event.
        /// </summary>
        /// <param name="args">The <see cref="ProcessEventArgs"/> instance containing the event data.</param>
        private void FireProcessInputEvent(ProcessEventArgs args)
        {
            //  Fire the event if it is set.
            OnProcessInput?.Invoke(this, args);
        }

        /// <summary>
        /// Occurs when console output is produced.
        /// </summary>
        public event ProcessEventHandler OnProcessOutput;

        /// <summary>
        /// Occurs when console input is produced.
        /// </summary>
        public event ProcessEventHandler OnProcessInput;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to show diagnostics.
        /// </summary>
        /// <value>
        ///   <c>true</c> if show diagnostics; otherwise, <c>false</c>.
        /// </value>
        public bool ShowDiagnostics { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has input enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has input enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsInputEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has a process running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a process running; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessRunning { get; private set; }

        /// <summary>
        /// Gets the internally used process interface.
        /// </summary>
        /// <value>
        /// The process interface.
        /// </value>
        public ProcessInterface ProcessInterface
        {
            get { return processInterface; }
        }

        #endregion

        #region UserControl management

        public void SetFocus()
        {
            richTextBoxConsole.Focus();
        }

        /// <summary>
        /// Runs the on UI dispatcher.
        /// </summary>
        /// <param name="action">The action.</param>
        private void RunOnUIDispatcher(Action action)
        {
            if (Dispatcher.CheckAccess())
            {
                //  Invoke the action.
                action();
            }
            else
            {
                Dispatcher.BeginInvoke(action, null);
            }
        }

        public void SetForeground(Brush foreground)
        {
            richTextBoxConsole.Foreground = foreground;
        }

        public void SetBackground(Brush background)
        {
            richTextBoxConsole.Background = background;
        }

        #endregion

        #region Historic manager

        private int _currentPosInHistoric;
        private void NavigateHistoric(Key key)
        {
            if (processInterface.HistoricCommands.Count == 0)
                return;

            if (key == Key.Up && _currentPosInHistoric < processInterface.HistoricCommands.Count - 1)
                _currentPosInHistoric++;
            if (key == Key.Down)
                if (_currentPosInHistoric > 0)
                    _currentPosInHistoric--;
                else
                {
                    SetCommand();
                    return;
                }

            SetCommand(processInterface.HistoricCommands[_currentPosInHistoric]);
        }

        private void SetCommand(string command = "")
        {
            richTextBoxConsole.Selection.Select(richTextBoxConsole.GetPointerAt(inputStartPos), richTextBoxConsole.Document.ContentEnd);
            richTextBoxConsole.Selection.Text = command;
            richTextBoxConsole.Selection.Select(richTextBoxConsole.Document.ContentEnd, richTextBoxConsole.Document.ContentEnd);
        }

        #endregion

        #region IDisposable

        private bool disposedValue;
        public bool IsDisposed
        {
            get { return disposedValue; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && processInterface != null)
                {
                    processInterface.OnProcessOutput -= ProcessInterface_OnProcessOutput;
                    processInterface.OnProcessError -= ProcessInterface_OnProcessError;
                    processInterface.OnProcessInput -= ProcessInterface_OnProcessInput;
                    processInterface.OnProcessExit -= ProcessInterface_OnProcessExit;
                    processInterface.StopProcess();
                    processInterface.Dispose();
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
}
