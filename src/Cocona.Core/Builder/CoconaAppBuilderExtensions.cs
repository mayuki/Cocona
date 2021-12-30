using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Builder;

namespace Cocona
{
    public static class CoconaAppBuilderExtensions
    {
        public static void Run(this ICoconaAppBuilder app, Delegate commandBody)
        {
            app.AddCommand(commandBody);
            app.Run();
        }

        public static Task RunAsync(this ICoconaAppBuilder app, Delegate commandBody, CancellationToken cancellationToken = default)
        {
            app.AddCommand(commandBody);
            return app.RunAsync(cancellationToken);
        }

        public static void Run(this ICoconaAppBuilder app, Type commandType)
        {
            app.AddCommands(commandType);
            app.Run();
        }

        public static Task RunAsync(this ICoconaAppBuilder app, Type commandType, CancellationToken cancellationToken = default)
        {
            app.AddCommands(commandType);
            return app.RunAsync(cancellationToken);
        }

        public static void Run(this ICoconaAppBuilder app, IEnumerable<Type> commandTypes)
        {
            app.AddCommands(commandTypes);
            app.Run();
        }

        public static Task RunAsync(this ICoconaAppBuilder app, IEnumerable<Type> commandTypes, CancellationToken cancellationToken = default)
        {
            app.AddCommands(commandTypes);
            return app.RunAsync(cancellationToken);
        }

        public static void Run<T>(this ICoconaAppBuilder app)
        {
            app.AddCommands(typeof(T));
            app.Run();
        }

        public static Task RunAsync<T>(this ICoconaAppBuilder app, CancellationToken cancellationToken = default)
        {
            app.AddCommands(typeof(T));
            return app.RunAsync(cancellationToken);
        }
    }
}
