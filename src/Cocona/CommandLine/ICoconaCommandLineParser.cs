using Cocona.Command;
using System;
using System.Collections.Generic;

namespace Cocona.CommandLine
{
    public interface ICoconaCommandLineParser
    {
        ParsedCommandLine ParseCommand(Span<string> args, IReadOnlyList<CommandOptionDescriptor> options, IReadOnlyList<CommandArgumentDescriptor> arguments);
    }
}
