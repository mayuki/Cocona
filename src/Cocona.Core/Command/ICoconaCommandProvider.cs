namespace Cocona.Command;

public interface ICoconaCommandProvider
{
    CommandCollection GetCommandCollection();
}