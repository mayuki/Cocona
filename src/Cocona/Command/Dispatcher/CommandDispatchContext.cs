using Cocona.CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Command.Dispatcher
{
    public class CommandDispatchContext
    {
        public CommandDescriptor Command { get; set; }
        public ParsedCommandLine ParsedCommandLine { get; set; }

        public CommandDispatchContext(CommandDescriptor command, ParsedCommandLine parsedCommandLine)
        {
            Command = command;
            ParsedCommandLine = parsedCommandLine;
        }
    }
}
