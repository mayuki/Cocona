using Cocona.Builder;
using Cocona.Filters;
using Cocona.Internal;

namespace Cocona;

public static class CoconaCommandsBuilderExtensions
{
    internal static IList<object> GetFilters(this ICoconaCommandsBuilder builder)
    {
        if (!builder.Properties.TryGetValue(CoconaCommandsBuilder.PropertyKeyFilters, out var filters) || filters is not IList<object> filtersTyped)
        {
            builder.Properties[CoconaCommandsBuilder.PropertyKeyFilters] = filtersTyped = new List<object>();
        }
        return filtersTyped;
    }

    /// <summary>
    /// Adds a command filter and apply it to commands after this call.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static ICoconaCommandsBuilder UseFilter(this ICoconaCommandsBuilder builder, IFilterMetadata filter)
    {
        ThrowHelper.ThrowIfNull(filter);
        builder.GetFilters().Add(filter);
        return builder;
    }

    /// <summary>
    /// Adds a command filter and apply it to commands after this call.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="filterFactory"></param>
    /// <returns></returns>
    public static ICoconaCommandsBuilder UseFilter(this ICoconaCommandsBuilder builder, IFilterFactory filterFactory)
    {
        ThrowHelper.ThrowIfNull(filterFactory);
        builder.GetFilters().Add(filterFactory);
        return builder;
    }

    /// <summary>
    /// Adds a command filter delegate defined in-line and apply it to commands after this call.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="filterFunc"></param>
    /// <returns></returns>
    public static ICoconaCommandsBuilder UseFilter(this ICoconaCommandsBuilder builder, Func<CoconaCommandExecutingContext, CommandExecutionDelegate, ValueTask<int>> filterFunc)
    {
        ThrowHelper.ThrowIfNull(filterFunc);
        builder.GetFilters().Add(new DelegateCommandFilter(filterFunc));
        return builder;
    }

    /// <summary>
    /// Adds a pre-built command data to the builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="commandData"></param>
    /// <returns></returns>
    public static ICoconaCommandsBuilder AddCommand(this ICoconaCommandsBuilder builder, ICommandData commandData)
    {
        ThrowHelper.ThrowIfNull(commandData);

        builder.Add(new CommandDataDataSource(commandData));
        return builder;
    }

    /// <summary>
    /// Adds a primary command definition delegate to the builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="commandBody"></param>
    /// <returns></returns>
    public static CommandConventionBuilder AddCommand(this ICoconaCommandsBuilder builder, Delegate commandBody)
    {
        ThrowHelper.ThrowIfNull(commandBody);

        var conventions = new List<Action<ICommandBuilder>>();
        var commandSource = new DelegateCommandDataSource(commandBody, conventions, builder.GetFilters().ToArray());
        builder.Add(commandSource);
        return new CommandConventionBuilder(conventions).FromBuilder().WithMetadata(new PrimaryCommandAttribute());
    }

    /// <summary>
    /// Adds a command definition delegate to the builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="commandBody"></param>
    /// <returns></returns>
    public static CommandConventionBuilder AddCommand(this ICoconaCommandsBuilder builder, string name, Delegate commandBody)
    {
        ThrowHelper.ThrowIfNull(name);
        ThrowHelper.ThrowIfNull(commandBody);

        var conventions = new List<Action<ICommandBuilder>>();
        var commandSource = new DelegateCommandDataSource(commandBody, conventions, builder.GetFilters().ToArray());
        builder.Add(commandSource);

        return new CommandConventionBuilder(conventions).FromBuilder().WithName(name);
    }

    /// <summary>
    /// Creates and adds a sub-command set to the builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static CommandConventionBuilder AddSubCommand(this ICoconaCommandsBuilder builder, string name, Action<ICoconaCommandsBuilder> configure)
    {
        ThrowHelper.ThrowIfNull(name);
        ThrowHelper.ThrowIfNull(configure);

        var newBuilder = builder.New();
        configure(newBuilder);

        var conventions = new List<Action<ICommandBuilder>>();
        var commandSource = new SubCommandsDataSource(newBuilder.Build(), conventions, builder.GetFilters().ToArray());
        builder.Add(commandSource);

        return new CommandConventionBuilder(conventions).FromBuilder().WithName(name);
    }

    /// <summary>
    /// Adds a commands type to the builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static CommandTypeConventionBuilder AddCommands<T>(this ICoconaCommandsBuilder builder)
        => builder.AddCommands(typeof(T));

    /// <summary>
    /// Adds a commands type to the builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="commandType"></param>
    /// <returns></returns>
    public static CommandTypeConventionBuilder AddCommands(this ICoconaCommandsBuilder builder, Type commandType)
    {
        ThrowHelper.ThrowIfNull(commandType);

        var conventions = new List<Action<ICommandBuilder>>();
        builder.Add(new TypeCommandDataSource(commandType, conventions, builder.GetFilters().ToArray()));

        return new CommandTypeConventionBuilder(conventions);
    }

    /// <summary>
    /// Adds commands types to the builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="commandTypes"></param>
    /// <returns></returns>
    public static ICoconaCommandsBuilder AddCommands(this ICoconaCommandsBuilder builder, IEnumerable<Type> commandTypes)
    {
        foreach (var t in commandTypes)
        {
            builder.AddCommands(t);
        }
        return builder;
    }
}
