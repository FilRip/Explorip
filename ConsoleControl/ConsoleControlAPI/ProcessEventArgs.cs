using System;

namespace ConsoleControlAPI
{
    /// <summary>
    /// The ProcessEventArgs are arguments for a console event.
    /// </summary>
    public class ProcessEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        public ProcessEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public ProcessEventArgs(string content)
        {
            //  Set the content and code.
            Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        public ProcessEventArgs(int code)
        {
            //  Set the content and code.
            ErrorCode = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="code">The code.</param>
        public ProcessEventArgs(string content, int code)
        {
            //  Set the content and code.
            Content = content;
            ErrorCode = code;
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int? ErrorCode { get; private set; }

        /// <summary>
        /// Specify the color of this text in the console
        /// </summary>
        public ConsoleColor? Color { get; private set; }

        /// <summary>
        /// Specify the background color of this text in the console
        /// </summary>
        public ConsoleColor? BackgroundColor { get; private set; }
    }
}