namespace WildCoco.Application;

public class WildCocoConsoleProvider : IWildCocoConsoleProvider
{
#pragma warning disable RS0030 // Do not used banned APIs
    public TextWriter Output => Console.Out;
    public TextWriter Error => Console.Error;
#pragma warning restore RS0030 // Do not used banned APIs
}
