using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Dispatcher;
using Cocona.Command.Dispatcher.Middlewares;
using Cocona.CommandLine;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Cocona.Command.Binder.Validation;
using Cocona.Hosting;
using Xunit;

namespace Cocona.Test.Command
{
    public class CoconaConsoleAppBaseTest
    {
        private ServiceCollection CreateDefaultServices<TCommand>(string[] args)
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICoconaParameterValidatorProvider, DataAnnotationsParameterValidatorProvider>();
            services.AddSingleton<ICoconaCommandProvider>(serviceProvider => new CoconaCommandProvider(new Type[] { typeof(TCommand) }));
            services.AddSingleton<ICoconaCommandLineArgumentProvider>(serviceProvider => new CoconaCommandLineArgumentProvider(args));
            services.AddTransient<ICoconaParameterBinder, CoconaParameterBinder>();
            services.AddTransient<ICoconaValueConverter, CoconaValueConverter>();
            services.AddTransient<ICoconaCommandLineParser, CoconaCommandLineParser>();
            services.AddTransient<ICoconaCommandDispatcher, CoconaCommandDispatcher>();
            services.AddTransient<ICoconaCommandMatcher, CoconaCommandMatcher>();
            services.AddTransient<ICoconaCommandResolver, CoconaCommandResolver>();
            services.AddSingleton<ICoconaAppContextAccessor, CoconaAppContextAccessor>();
            services.AddSingleton<ICoconaInstanceActivator, CoconaInstanceActivator>();
            services.AddSingleton<ICoconaServiceProviderScopeSupport, CoconaServiceProviderScopeSupport>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton<ICoconaCommandDispatcherPipelineBuilder>(serviceProvider =>
                new CoconaCommandDispatcherPipelineBuilder(serviceProvider, serviceProvider.GetService<ICoconaInstanceActivator>())
                    .UseMiddleware<InitializeConsoleAppMiddleware>()
                    .UseMiddleware<CoconaCommandInvokeMiddleware>()
            );

            // register a test command as singleton.
            services.AddSingleton<ConsoleAppTest>();

            return services;
        }

        [Fact]
        public async Task InitializeConsoleApp()
        {
            var services = CreateDefaultServices<ConsoleAppTest>(Array.Empty<string>());
            var serviceProvider = services.BuildServiceProvider();

            var commandInstance = serviceProvider.GetService<ConsoleAppTest>();
            var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
                serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
                serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
            );
            var result = await serviceProvider.GetService<ICoconaCommandDispatcher>().DispatchAsync(resolvedCommand);
            result.Should().Be(0);
            commandInstance.HasContext.Should().BeTrue();
        }

        class ConsoleAppTest : CoconaConsoleAppBase
        {
            public bool HasContext { get; set; }

            public void Test()
            {
                HasContext = Context != null;
            }
        }
    }
}
