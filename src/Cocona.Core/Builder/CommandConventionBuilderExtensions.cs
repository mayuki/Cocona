using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Builder;
using Cocona.Builder.Metadata;
using Cocona.Internal;

namespace Cocona
{
    public static class CommandConventionBuilderExtensions
    {
        public static CommandConventionBuilder WithAliases(this CommandConventionBuilder builder, params string[] aliases)
        {
            ThrowHelper.ThrowIfNull(aliases);
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(new CommandAliasesMetadata(aliases));
            });
            return builder;
        }

        public static CommandConventionBuilder WithDescription(this CommandConventionBuilder builder, string description)
        {
            ThrowHelper.ThrowIfNull(description);
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(new CommandDescriptionMetadata(description));
            });
            return builder;
        }

        public static CommandConventionBuilder WithName(this CommandConventionBuilder builder, string name)
        {
            ThrowHelper.ThrowIfNull(name);
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(new CommandNameMetadata(name));
            });
            return builder;
        }

        internal static CommandConventionBuilder FromBuilder(this CommandConventionBuilder builder)
        {
            builder.Add(commandBuilder =>
            {
                commandBuilder.Metadata.Add(new CommandFromBuilderMetadata());
            });
            return builder;
        }
    }
}
