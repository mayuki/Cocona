namespace WildCoco.CommandLine;

public class WildCocoCommandLineArgumentProvider : IWildCocoCommandLineArgumentProvider
{
    private readonly string[] _args;

    public WildCocoCommandLineArgumentProvider(string[] args)
    {
        _args = args;
    }

    public string[] GetArguments()
        => _args;
}
