namespace Cocona.Application;

public interface ICoconaConsoleProvider
{
    TextWriter Output { get; }
    TextWriter Error { get; }
}