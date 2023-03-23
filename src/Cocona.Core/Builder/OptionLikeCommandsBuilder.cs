using System;
using System.Collections.Generic;
using Cocona.Builder;
using Cocona.Internal;

namespace Cocona.Builder
{
    public interface IOptionLikeCommandsBuilder
    {
        IOptionLikeCommandsBuilder Add(OptionLikeDelegateCommandDataSource commandDataSource);
    }

    public class OptionLikeCommandsBuilder : IOptionLikeCommandsBuilder
    {
        private readonly List<OptionLikeDelegateCommandDataSource> _commandDataSources = new();
        private readonly Dictionary<string, object?> _properties = new();

        internal const string PropertyKeyFilters = "Cocona.Builder.OptionLikeCommandsBuilder+Filters";

        public IDictionary<string, object?> Properties => _properties;

        IOptionLikeCommandsBuilder IOptionLikeCommandsBuilder.Add(OptionLikeDelegateCommandDataSource commandDataSource)
        {
            _commandDataSources.Add(commandDataSource);
            return this;
        }

        public IReadOnlyList<OptionLikeDelegateCommandData> Build()
        {
            var commandDataItems = new List<OptionLikeDelegateCommandData>(_commandDataSources.Count);

            foreach (var commandDataSource in _commandDataSources)
            {
                var commandData = (OptionLikeDelegateCommandData)commandDataSource.Build();
                commandDataItems.Add(commandData);
            }

            return commandDataItems;
        }
    }
}

namespace Cocona
{
    public static class OptionLikeCommandsBuilderExtensions
    {
        public static CommandConventionBuilder Add(this IOptionLikeCommandsBuilder builder, string name, Delegate commandBody)
        {
            return builder.Add(name, Array.Empty<char>(), commandBody);
        }

        public static CommandConventionBuilder Add(this IOptionLikeCommandsBuilder builder, string name, IReadOnlyList<char>? shortNames, Delegate commandBody)
        {
            ThrowHelper.ThrowIfNull(builder);
            ThrowHelper.ThrowIfNull(name);
            ThrowHelper.ThrowIfNull(shortNames);
            ThrowHelper.ThrowIfNull(commandBody);

            var conventions = new List<Action<ICommandBuilder>>();

            builder.Add(new OptionLikeDelegateCommandDataSource(commandBody, shortNames ?? Array.Empty<char>(), conventions, Array.Empty<object>()));

            return new CommandConventionBuilder(conventions).WithName(name);
        }
    }
}
