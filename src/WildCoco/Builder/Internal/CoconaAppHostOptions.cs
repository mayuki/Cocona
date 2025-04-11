using WildCoco.Builder;

namespace WildCoco.Builder.Internal;

internal sealed class WildCocoAppHostOptions
{
    public Action<IWildCocoCommandsBuilder>? ConfigureApplication { get; set; }
}
