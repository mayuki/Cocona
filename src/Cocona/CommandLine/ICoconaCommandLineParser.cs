using Cocona.Command;

namespace Cocona.CommandLine
{
    public interface ICoconaCommandLineParser
    {
        ParsedCommandLine ParseCommand(string[] args, CommandOptionDescriptor[] options, CommandArgumentDescriptor[] arguments);
    }
}
