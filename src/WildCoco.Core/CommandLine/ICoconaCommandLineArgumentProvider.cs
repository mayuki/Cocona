namespace WildCoco.CommandLine;

/// <summary>
/// A provider that provides command-line arguments.
/// </summary>
public interface IWildCocoCommandLineArgumentProvider
{
    string[] GetArguments();
}
