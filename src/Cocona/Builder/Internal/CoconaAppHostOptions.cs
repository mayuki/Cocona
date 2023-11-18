namespace Cocona.Builder.Internal;

internal sealed class CoconaAppHostOptions
{
    public Action<ICoconaCommandsBuilder>? ConfigureApplication { get; set; }
}
