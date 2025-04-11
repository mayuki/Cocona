namespace WildCoco.CommandLine;

public class DefaultWildCocoEnvironmentProvider : IWildCocoEnvironmentProvider
{
    public string[] GetCommandLineArgs() => Environment.GetCommandLineArgs() switch
    {
        { Length: > 0 } args => args.Skip(1).ToArray(),
        _ => Array.Empty<string>(),
    };

}
