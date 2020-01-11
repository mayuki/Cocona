using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Dispatcher;
using Cocona.CommandLine;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cocona.Test.Command.CommandDispatcher
{
    public class DispatcherTest
    {
        [Fact]
        public async Task SimpleSingleCommandDispatch()
        {
            var services = new ServiceCollection();
            {
                services.AddTransient<ICoconaCommandProvider>(serviceProvider => new CoconaCommandProvider(new Type[] { typeof(TestCommand) }));
                services.AddTransient<ICoconaParameterBinder, CoconaParameterBinder>();
                services.AddTransient<ICoconaValueConverter, CoconaValueConverter>();
                services.AddTransient<ICoconaCommandLineParser, CoconaCommandLineParser>();
                services.AddTransient<ICoconaCommandDispatcher, CoconaCommandDispatcher>();

                services.AddSingleton<TestCommand>();
            }
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var result = await dispatcher.DispatchAsync(new string[] { "--option0=hogehoge" });
            serviceProvider.GetService<TestCommand>().Log[0].Should().Be($"{nameof(TestCommand.Test)}:option0 -> hogehoge");
        }

        public class TestCommand
        {
            public List<string> Log { get; } = new List<string>();

            public void Test(string option0)
            {
                Log.Add($"{nameof(TestCommand.Test)}:{nameof(option0)} -> {option0}");
            }
        }
    }
}
