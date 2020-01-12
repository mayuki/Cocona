using Cocona.CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Command.Dispatcher
{
    public class CommandDispatchContext
    {
        public CommandDescriptor Command { get;}
        
        public object CommandTarget { get; }

        public ParsedCommandLine ParsedCommandLine { get; }

        public CommandDispatchContext(CommandDescriptor command, ParsedCommandLine parsedCommandLine, object commandTarget)
        {
            Command = command;
            ParsedCommandLine = parsedCommandLine;
            CommandTarget = commandTarget;
        }
    }
}
