using Cocona.Builder;
using Cocona.Internal;

namespace Cocona;

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
