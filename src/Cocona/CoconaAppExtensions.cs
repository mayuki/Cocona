using System;
using Cocona.Builder;

namespace Cocona
{
    public static class CoconaAppExtensions 
    {
        public static CoconaApp AsCoconaApp(this ICoconaCommandsBuilder coconaCommandsBuilder) =>
            (CoconaApp)coconaCommandsBuilder;

        public static CoconaApp AddCommand(this CoconaApp app, string name, Delegate @delegate)
        {
            _ = CommandsBuilderExtensions.AddCommand(app, name, @delegate);
            return app;
        }
    }
}
