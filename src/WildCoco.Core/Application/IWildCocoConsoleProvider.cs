namespace WildCoco.Application;

public interface IWildCocoConsoleProvider
{
    TextWriter Output { get; }
    TextWriter Error { get; }
}
