using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Builder;
using Cocona.Builder.Metadata;
using Cocona.Filters;
using Cocona.Internal;

// ReSharper disable once CheckNamespace
namespace Cocona
{
    public static class CommandConventionBuilderExtensions
    {
        /// <summary>
        /// Sets the command aliases to the provided aliases for the builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="aliases"></param>
        /// <returns></returns>
        public static CommandConventionBuilder WithAliases(this CommandConventionBuilder builder, params string[] aliases)
        {
            ThrowHelper.ThrowIfNull(builder);
            ThrowHelper.ThrowIfNull(aliases);
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(new CommandAliasesMetadata(aliases));
            });
            return builder;
        }

        /// <summary>
        /// Sets the command description to the provided description for the builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static CommandConventionBuilder WithDescription(this CommandConventionBuilder builder, string description)
        {
            ThrowHelper.ThrowIfNull(builder);
            ThrowHelper.ThrowIfNull(description);
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(new CommandDescriptionMetadata(description));
            });
            return builder;
        }

        /// <summary>
        /// Sets the command name to the provided name for the builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CommandConventionBuilder WithName(this CommandConventionBuilder builder, string name)
        {
            ThrowHelper.ThrowIfNull(builder);
            ThrowHelper.ThrowIfNull(name);
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(new CommandNameMetadata(name));
            });
            return builder;
        }

        /// <summary>
        /// Sets the command filter to the provided filter for the builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static CommandConventionBuilder WithFilter(this CommandConventionBuilder builder, IFilterMetadata filter)
        {
            ThrowHelper.ThrowIfNull(builder);
            ThrowHelper.ThrowIfNull(filter);
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(filter);
            });
            return builder;
        }

        /// <summary>
        /// Sets the command filter to the provided filter for the builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static CommandConventionBuilder WithFilter(this CommandConventionBuilder builder, IFilterFactory filter)
        {
            ThrowHelper.ThrowIfNull(builder);
            ThrowHelper.ThrowIfNull(filter);
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(filter);
            });
            return builder;
        }

        /// <summary>
        /// Sets the command filter using the provided filter delegate for the builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="filterFunc"></param>
        /// <returns></returns>
        public static CommandConventionBuilder WithFilter(this CommandConventionBuilder builder, Func<CoconaCommandExecutingContext, CommandExecutionDelegate, ValueTask<int>> filterFunc)
        {
            ThrowHelper.ThrowIfNull(builder);
            ThrowHelper.ThrowIfNull(filterFunc);
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(new DelegateCommandFilter(filterFunc));
            });
            return builder;
        }

        /// <summary>
        /// Sets the metadata using the provided filter delegate for the builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static CommandConventionBuilder WithMetadata(this CommandConventionBuilder builder, params object[] items)
        {
            ThrowHelper.ThrowIfNull(builder);
            ThrowHelper.ThrowIfNull(items);
            builder.Add(commandBuilder =>
            {
                foreach (var item in items)
                {
                    commandBuilder.Metadata.Add(item);
                }
            });
            return builder;
        }


        /// <summary>
        /// Adds a option-like command definition delegate to the builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptionLikeCommands"></param>
        /// <returns></returns>
        public static CommandConventionBuilder OptionLikeCommand(this CommandConventionBuilder builder, Action<IOptionLikeCommandsBuilder> configureOptionLikeCommands)
        {
            ThrowHelper.ThrowIfNull(builder);
            ThrowHelper.ThrowIfNull(configureOptionLikeCommands);

            builder.Add(commandBuilder =>
            {
                var builderOptionLikeCommands = new OptionLikeCommandsBuilder();
                configureOptionLikeCommands(builderOptionLikeCommands);
                var optionLikeCommands = builderOptionLikeCommands.Build();

                foreach (var optionLikeCommand in optionLikeCommands)
                {
                    commandBuilder.Metadata.Add(new OptionLikeCommandMetadata(optionLikeCommand));
                }
            });

            return builder;
        }

        internal static CommandConventionBuilder FromBuilder(this CommandConventionBuilder builder)
        {
            ThrowHelper.ThrowIfNull(builder);
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(new CommandFromBuilderMetadata());
            });
            return builder;
        }
    }
}
