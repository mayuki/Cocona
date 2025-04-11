using WildCoco.Builder;

namespace WildCoco.Lite.Builder.Internal;

internal class WildCocoLiteAppHostOptions
{
    public Action<IWildCocoCommandsBuilder>? ConfigureApplication { get; set; }
}
