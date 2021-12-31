using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cocona.Command;
using Cocona.Filters;

namespace Cocona.Builder
{
    /// <summary>
    /// Defines a class that provides the mechanism to configure application commands.
    /// </summary>
    public interface ICoconaCommandsBuilder
    {
        ICoconaCommandsBuilder Add(ICommandDataSource commandDataSource);
        IDictionary<string, object?> Properties { get; }
        ICoconaCommandsBuilder New();
        IReadOnlyList<ICommandData> Build();
    }

    public class CoconaCommandsBuilder : ICoconaCommandsBuilder
    {
        private readonly List<ICommandDataSource> _commandDataSources = new();
        private readonly Dictionary<string, object?> _properties = new();

        internal const string PropertyKeyFilters = "Cocona.Builder.CoconaCommandsBuilder+Filters";

        public IDictionary<string, object?> Properties => _properties;

        public CoconaCommandsBuilder()
        {
        }

        private CoconaCommandsBuilder(ICoconaCommandsBuilder parent)
        {
            // Copy properties to a new builder.
            foreach (var keyValue in parent.Properties)
            {
                // Copy filters to new properties bag.
                if (keyValue.Key == CoconaCommandsBuilder.PropertyKeyFilters && keyValue.Value is IList<object> filters)
                {
                    _properties[keyValue.Key] = new List<object>(filters);
                }
                else
                {
                    _properties[keyValue.Key] = keyValue.Value;
                }
            }
        }

        ICoconaCommandsBuilder ICoconaCommandsBuilder.New()
            => new CoconaCommandsBuilder(this);

        ICoconaCommandsBuilder ICoconaCommandsBuilder.Add(ICommandDataSource commandDataSource)
        {
            _commandDataSources.Add(commandDataSource);
            return this;
        }

        public IReadOnlyList<ICommandData> Build()
            => _commandDataSources.Select(x => x.Build()).ToArray();
    }
}

namespace Cocona
{
    using Cocona.Builder;
    using Cocona.Internal;

    public static class CommandsBuilderExtensions
    {
        internal static IList<object> GetFilters(this ICoconaCommandsBuilder builder)
        {
            if (!builder.Properties.TryGetValue(CoconaCommandsBuilder.PropertyKeyFilters, out var filters) || filters is not IList<object> filtersTyped)
            {
                builder.Properties[CoconaCommandsBuilder.PropertyKeyFilters] = filtersTyped = new List<object>();
            }
            return filtersTyped;
        }

        public static ICoconaCommandsBuilder UseFilter(this ICoconaCommandsBuilder builder, IFilterMetadata filter)
        {
            ThrowHelper.ThrowIfNull(filter);
            builder.GetFilters().Add(filter);
            return builder;
        }

        public static ICoconaCommandsBuilder UseFilter(this ICoconaCommandsBuilder builder, IFilterFactory filterFactory)
        {
            ThrowHelper.ThrowIfNull(filterFactory);
            builder.GetFilters().Add(filterFactory);
            return builder;
        }

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
        /// Adds a command definition delegate to the builder.
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
}
