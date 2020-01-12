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

        public CommandExitedException(int exitCode)
            : this($"The command has exited with code '{exitCode}'.", exitCode)
        { }

        protected CommandExitedException(string message, int exitCode)
            : base(message)
        {
            ExitCode = exitCode;
        }
    }
}
