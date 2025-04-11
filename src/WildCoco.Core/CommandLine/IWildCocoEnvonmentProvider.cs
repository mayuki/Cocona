namespace WildCoco.CommandLine;

/// <summary>
/// A provider that abstracts calls to static System.Environment
/// </summary>
public interface IWildCocoEnvironmentProvider
{
    string[] GetCommandLineArgs();
}
