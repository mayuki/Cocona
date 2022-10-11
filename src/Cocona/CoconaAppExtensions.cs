using Cocona.Builder;

namespace Cocona
{
    public static class CoconaAppExtensions 
    {
        public static CoconaApp AsCoconaApp(this ICoconaCommandsBuilder coconaCommandsBuilder) =>
            (CoconaApp)coconaCommandsBuilder;
    }
}
