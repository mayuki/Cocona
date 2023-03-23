using System;

namespace Cocona.Builder.Internal
{
    internal class CoconaAppHostOptions
    {
        public Action<ICoconaCommandsBuilder>? ConfigureApplication { get; set; }
    }
}
