using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    /// <summary>
    /// The exception that thrown when a command exited immediately.
    /// </summary>
    public class CommandExitedException : Exception
    {
        /// <summary>
        /// Gets a exit code of the current command.
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// Gets an message on the command exited.
        /// </summary>
        public string? ExitMessage { get; }

        public CommandExitedException(int exitCode)
            : this(null, exitCode)
        { }

        public CommandExitedException(string? exitMessage, int exitCode)
            : base($"The command has exited with code '{exitCode}'." + (string.IsNullOrEmpty(exitMessage) ? string.Empty : " " + exitMessage))
        {
            ExitMessage = exitMessage;
            ExitCode = exitCode;
        }
    }
}
