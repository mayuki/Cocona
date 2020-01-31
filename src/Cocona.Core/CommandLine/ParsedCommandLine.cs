using System;
using System.Collections.Generic;

namespace Cocona.CommandLine
{
    public class ParsedCommandLine
    {
        public IReadOnlyList<CommandOption> Options { get; }
        public IReadOnlyList<CommandArgument> Arguments { get; }
        public IReadOnlyList<string> UnknownOptions { get; }

        public ParsedCommandLine(IReadOnlyList<CommandOption> options, IReadOnlyList<CommandArgument> arguments, IReadOnlyList<string> unknownOptions)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            UnknownOptions = unknownOptions ?? throw new ArgumentNullException(nameof(unknownOptions));
        }
    }
}
