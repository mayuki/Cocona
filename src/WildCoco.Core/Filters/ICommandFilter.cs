namespace WildCoco.Filters;

public delegate ValueTask<int> CommandExecutionDelegate(WildCocoCommandExecutingContext ctx);

/// <summary>
/// An interface for filter that surrounds execution of command.
/// </summary>
public interface ICommandFilter : IFilterMetadata
{
    ValueTask<int> OnCommandExecutionAsync(WildCocoCommandExecutingContext ctx, CommandExecutionDelegate next);
}
