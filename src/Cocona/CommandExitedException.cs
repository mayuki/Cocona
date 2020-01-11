using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    public class CommandExitedException : Exception
    {
        public int ExitCode { get; }

        public CommandExitedException(string message)
            : this(message, -1)
        { }

        public CommandExitedException(string message, int exitCode)
            : base(message)
        {
            ExitCode = exitCode;
        }
    }
}
