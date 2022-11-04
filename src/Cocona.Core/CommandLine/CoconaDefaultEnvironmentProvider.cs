using System;
using System.Linq;

namespace Cocona.CommandLine
{
    public class DefaultCoconaEnvironmentProvider : ICoconaEnvironmentProvider
    {
        public string[] GetCommandLineArgs() => Environment.GetCommandLineArgs() switch
        {
            { Length: > 0 } args => args.Skip(1).ToArray(),
            _ => Array.Empty<string>(),
        };

    }
}
