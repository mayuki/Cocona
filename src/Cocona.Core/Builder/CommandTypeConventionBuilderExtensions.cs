using Cocona.Builder;
using Cocona.Internal;

// ReSharper disable once CheckNamespace
namespace Cocona;

public static class CommandTypeConventionBuilderExtensions
{
    /// <summary>
    /// Sets the metadata using the provided filter delegate for the builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public static CommandTypeConventionBuilder WithMetadata(this CommandTypeConventionBuilder builder, params object[] items)
    {
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
}
