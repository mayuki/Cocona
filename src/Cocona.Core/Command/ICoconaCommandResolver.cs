namespace Cocona.Command
{
    public interface ICoconaCommandResolver
    {
        CommandResolverResult ParseAndResolve(CommandCollection commandCollection, IReadOnlyList<string> args);
    }
}
