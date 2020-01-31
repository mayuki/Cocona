namespace Cocona.CommandLine
{
    /// <summary>
    /// A provider thats provides command-line arguments.
    /// </summary>
    public interface ICoconaCommandLineArgumentProvider
    {
        string[] GetArguments();
    }
}
