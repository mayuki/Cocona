using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Binder.Validation;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher;
using Cocona.Command.Dispatcher.Middlewares;
using Cocona.CommandLine;
using Cocona.Help;
using Cocona.Lite;

namespace Cocona
{
    public class CoconaLiteApp
    {
        public static async Task RunAsync<T>(string[] args, Action<CoconaLiteAppOptions>? configureOptions = null)
        {
            var services = new CoconaLiteServiceProvider();

            var options = new CoconaLiteAppOptions()
            {
                CommandTypes = new[] { typeof(T) },
            };

            configureOptions?.Invoke(options);

            services.AddSingleton(options);

            services.AddSingleton<ICoconaInstanceActivator, CoconaLiteInstanceActivator>();
            services.AddSingleton<ICoconaCommandLineArgumentProvider>(serviceProvider =>
                new CoconaCommandLineArgumentProvider(args));
            services.AddSingleton<ICoconaCommandProvider>(serviceProvider =>
            {
                var options = (CoconaLiteAppOptions)serviceProvider.GetService(typeof(CoconaLiteAppOptions));

                return new CoconaBuiltInCommandProvider(
                    new CoconaCommandProvider(
                        options.CommandTypes.ToArray(),
                        options.TreatPublicMethodsAsCommands,
                        options.EnableConvertOptionNameToLowerCase,
                        options.EnableConvertCommandNameToLowerCase
                    )
                );
            });
            services.AddSingleton<ICoconaCommandDispatcherPipelineBuilder, CoconaCommandDispatcherPipelineBuilder>();
            services.AddSingleton<ICoconaApplicationMetadataProvider, CoconaApplicationMetadataProvider>();
            services.AddSingleton<ICoconaConsoleProvider, CoconaConsoleProvider>();
            services.AddSingleton<ICoconaParameterValidatorProvider, DataAnnotationsParameterValidatorProvider>();

            services.AddSingleton<ICoconaParameterBinder, CoconaParameterBinder>();
            services.AddSingleton<ICoconaValueConverter, CoconaValueConverter>();
            services.AddSingleton<ICoconaCommandLineParser, CoconaCommandLineParser>();
            services.AddSingleton<ICoconaCommandDispatcher, CoconaCommandDispatcher>();
            services.AddSingleton<ICoconaCommandMatcher, CoconaCommandMatcher>();
            services.AddSingleton<ICoconaHelpRenderer, CoconaHelpRenderer>();
            services.AddSingleton<ICoconaCommandHelpProvider, CoconaCommandHelpProvider>();

            IServiceProvider serviceProvider = services;
            serviceProvider.GetService<ICoconaCommandDispatcherPipelineBuilder>()
                .UseMiddleware<BuiltInCommandMiddleware>()
                .UseMiddleware<HandleExceptionAndExitMiddleware>()
                .UseMiddleware<HandleParameterBindExceptionMiddleware>()
                .UseMiddleware<RejectUnknownOptionsMiddleware>()
                .UseMiddleware<CommandFilterMiddleware>()
                .UseMiddleware<CoconaCommandInvokeMiddleware>();

            var commandDispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();

            Environment.ExitCode = await Task.Run(async () => await commandDispatcher.DispatchAsync(default));
        }
    }
}
