namespace WildCoco.Command;

public interface IWildCocoCommandResolver
{
    CommandResolverResult ParseAndResolve(CommandCollection commandCollection, IReadOnlyList<string> args);
}
