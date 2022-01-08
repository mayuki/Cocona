namespace Cocona.CommandLine
{
    /// <summary>
    /// A provider that provides command-line arguments.
    /// </summary>
    public interface ICoconaCommandLineArgumentProvider
    {
        string[] GetArguments();
    }
}
