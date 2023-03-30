using Cocona.Builder;

namespace Cocona.Lite.Builder.Internal
{
    internal class CoconaLiteAppHostOptions
    {
        public Action<ICoconaCommandsBuilder>? ConfigureApplication { get; set; }
    }
}
