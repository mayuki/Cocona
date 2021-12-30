using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Builder
{
    /// <summary>
    /// Defines a class that provides the mechanism to configure application commands.
    /// </summary>
    public interface ICoconaCommandsBuilder
    {
        List<ICommandDataSource> CommandDataSources { get; }
        ICoconaCommandsBuilder New();
        IReadOnlyList<ICommandData> Build();
    }

    public class CoconaCommandsBuilder : ICoconaCommandsBuilder
    {
        List<ICommandDataSource> ICoconaCommandsBuilder.CommandDataSources { get; } = new();
        ICoconaCommandsBuilder ICoconaCommandsBuilder.New()
            => new CoconaCommandsBuilder();

        public IReadOnlyList<ICommandData> Build()
            => ((ICoconaCommandsBuilder)this).CommandDataSources.Select(x => x.Build()).ToArray();
    }
}

namespace Cocona
{
    using Cocona.Builder;
    using Cocona.Internal;

    public static class CommandsBuilderExtensions
    {
        /// <summary>
        /// Adds a pre-built command data to the builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="commandData"></param>
        /// <returns></returns>
        public static ICoconaCommandsBuilder AddCommand(this ICoconaCommandsBuilder builder, ICommandData commandData)
        {
            ThrowHelper.ThrowIfNull(commandData);

            builder.CommandDataSources.Add(new CommandDataDataSource(commandData));
            return builder;
        }

        /// <summary>
        /// Adds a command definition delegate to the builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="commandBody"></param>
        /// <returns></returns>
        public static CommandConventionBuilder AddCommand(this ICoconaCommandsBuilder builder, Delegate commandBody)
        {
            ThrowHelper.ThrowIfNull(commandBody);

            var conventions = new List<Action<ICommandBuilder>>();
            var commandSource = new DelegateCommandDataSource(commandBody, conventions);
            builder.CommandDataSources.Add(commandSource);
            return new CommandConventionBuilder(conventions).FromBuilder();
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
            var commandSource = new DelegateCommandDataSource(commandBody, conventions);
            builder.CommandDataSources.Add(commandSource);

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

            var newBuilder = new CoconaCommandsBuilder();
            configure(newBuilder);

            var conventions = new List<Action<ICommandBuilder>>();
            var commandSource = new SubCommandsDataSource(newBuilder.Build(), conventions);
            builder.CommandDataSources.Add(commandSource);

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
            builder.CommandDataSources.Add(new TypeCommandDataSource(commandType, conventions));

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
}
