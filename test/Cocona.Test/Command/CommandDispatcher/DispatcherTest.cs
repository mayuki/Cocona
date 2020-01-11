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

        [Fact]
        public async Task CommandNotFound_Single()
        {
            var services = new ServiceCollection();
            {
                services.AddTransient<ICoconaCommandProvider>(serviceProvider => new CoconaCommandProvider(new Type[] { typeof(NoCommand) }));
                services.AddTransient<ICoconaParameterBinder, CoconaParameterBinder>();
                services.AddTransient<ICoconaValueConverter, CoconaValueConverter>();
                services.AddTransient<ICoconaCommandLineParser, CoconaCommandLineParser>();
                services.AddTransient<ICoconaCommandDispatcher, CoconaCommandDispatcher>();

                services.AddSingleton<TestCommand>();
            }
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var ex = await Assert.ThrowsAsync<CommandNotFoundException>(async () => await dispatcher.DispatchAsync(new string[] { "C" }));
            ex.Command.Should().BeEmpty();
            ex.ImplementedCommands.All.Should().BeEmpty();
        }

        [Fact]
        public async Task CommandNotFound_Multiple()
        {
            var services = new ServiceCollection();
            {
                services.AddTransient<ICoconaCommandProvider>(serviceProvider => new CoconaCommandProvider(new Type[] { typeof(TestMultipleCommand) }));
                services.AddTransient<ICoconaParameterBinder, CoconaParameterBinder>();
                services.AddTransient<ICoconaValueConverter, CoconaValueConverter>();
                services.AddTransient<ICoconaCommandLineParser, CoconaCommandLineParser>();
                services.AddTransient<ICoconaCommandDispatcher, CoconaCommandDispatcher>();

                services.AddSingleton<TestCommand>();
            }
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var ex = await Assert.ThrowsAsync<CommandNotFoundException>(async () => await dispatcher.DispatchAsync(new string[] { "C" }));
            ex.Command.Should().Be("C");
            ex.ImplementedCommands.All.Should().HaveCount(2);
        }

        public class NoCommand
        { }

        public class TestCommand
        {
            public List<string> Log { get; } = new List<string>();

            public void Test(string option0)
            {
                Log.Add($"{nameof(TestCommand.Test)}:{nameof(option0)} -> {option0}");
            }
        }

        public class TestMultipleCommand
        {
            public List<string> Log { get; } = new List<string>();

            public void A(string option0)
            {
                Log.Add($"{nameof(TestMultipleCommand.A)}:{nameof(option0)} -> {option0}");
            }
            public void B(bool option0, [Argument]string arg0)
            {
                Log.Add($"{nameof(TestMultipleCommand.B)}:{nameof(option0)} -> {option0}");
            }
        }
    }
}
