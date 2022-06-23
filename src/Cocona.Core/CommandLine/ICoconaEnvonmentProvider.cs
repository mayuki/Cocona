namespace Cocona.CommandLine
{
    /// <summary>
    /// A provider that abstracts calls to static System.Environment
    /// </summary>
    public interface ICoconaEnvironmentProvider
    {
        string[] GetCommandLineArgs();
    }
}
