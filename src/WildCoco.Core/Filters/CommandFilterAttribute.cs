namespace WildCoco.Filters;

/// <summary>
/// An abstract filter that surrounds execution of command.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public abstract class CommandFilterAttribute : Attribute, ICommandFilter
{
    public abstract ValueTask<int> OnCommandExecutionAsync(WildCocoCommandExecutingContext ctx, CommandExecutionDelegate next);
}
