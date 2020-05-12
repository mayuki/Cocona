namespace Cocona.Command
{
    public interface ICoconaCommandResolver
    {
        CommandResolverResult ParseAndResolve(string[] args);
    }
}
