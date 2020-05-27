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
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cocona.Command.Binder.Validation;
using Xunit;

namespace Cocona.Test.Command.CommandDispatcher
{
    public class DispatcherTest
    {
        private ServiceCollection CreateDefaultServices<TCommand>(string[] args, Action<CoconaCommandDispatcherPipelineBuilder> configurePipeline = null)
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICoconaParameterValidatorProvider, DataAnnotationsParameterValidatorProvider>();
            services.AddSingleton<ICoconaCommandProvider>(serviceProvider => new CoconaCommandProvider(new Type[] { typeof(TCommand) }));
            services.AddSingleton<ICoconaCommandLineArgumentProvider>(serviceProvider => new CoconaCommandLineArgumentProvider(args));
            services.AddTransient<ICoconaParameterBinder, CoconaParameterBinder>();
            services.AddTransient<ICoconaValueConverter, CoconaValueConverter>();
            services.AddTransient<ICoconaCommandLineParser, CoconaCommandLineParser>();
            services.AddTransient<ICoconaCommandDispatcher, CoconaCommandDispatcher>();
            services.AddTransient<ICoconaCommandResolver, CoconaCommandResolver>();
            services.AddTransient<ICoconaCommandMatcher, CoconaCommandMatcher>();
            services.AddTransient<ICoconaConsoleProvider, CoconaConsoleProvider>();
            services.AddSingleton<ICoconaAppContextAccessor, CoconaAppContextAccessor>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton<ICoconaInstanceActivator, CoconaInstanceActivator>();
            services.AddSingleton<ICoconaCommandDispatcherPipelineBuilder>(
                serviceProvider =>
                {
                    var builder = new CoconaCommandDispatcherPipelineBuilder(serviceProvider, serviceProvider.GetService<ICoconaInstanceActivator>());
                    configurePipeline?.Invoke(builder);
                    builder.UseMiddleware<CoconaCommandInvokeMiddleware>();
                    return builder;
                });

            services.AddSingleton<TestCommand>();
            services.AddSingleton<TestCommand_IgnoreUnknownOption>();
            services.AddSingleton<TestMultipleCommand>();
            services.AddSingleton<TestNestedCommand>();
            services.AddSingleton<TestNestedCommand.TestNestedCommand_Nested>();
            services.AddSingleton<TestNestedCommand_Primary>();
            services.AddSingleton<TestNestedCommand_Primary.TestNestedCommand_Primary_Nested>();
            services.AddSingleton<TestDeepNestedCommand>();
            services.AddSingleton<TestDeepNestedCommand.TestDeepNestedCommand_Nested>();
            services.AddSingleton<TestDeepNestedCommand.TestDeepNestedCommand_Nested_2>();

            return services;
        }

        [Fact]
        public async Task SimpleSingleCommandDispatch()
        {
            var services = CreateDefaultServices<TestCommand>(new string[] { "--option0=hogehoge" });
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var result = await dispatcher.DispatchAsync();
            serviceProvider.GetService<TestCommand>().Log[0].Should().Be($"{nameof(TestCommand.Test)}:option0 -> hogehoge");
        }

        [Fact]
        public async Task RejectUnknownOptionMiddleware_UnknownOptions()
        {
            var services = CreateDefaultServices<TestCommand>(new string[] { "--option0=hogehoge", "--unknown-option" }, builder => { builder.UseMiddleware<RejectUnknownOptionsMiddleware>(); });
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var result = await dispatcher.DispatchAsync();
            result.Should().Be(129);
        }

        [Fact]
        public async Task RejectUnknownOptionMiddleware_IgnoreUnknownOptions()
        {
            var services = CreateDefaultServices<TestCommand_IgnoreUnknownOption>(new string[] { "--option0=hogehoge", "--unknown-option" }, builder => { builder.UseMiddleware<RejectUnknownOptionsMiddleware>(); });
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var result = await dispatcher.DispatchAsync();
            result.Should().Be(0);
            serviceProvider.GetService<TestCommand_IgnoreUnknownOption>().Log[0].Should().Be($"{nameof(TestCommand.Test)}:option0 -> hogehoge");
        }

        [Fact]
        public async Task MultipleCommand_Option1()
        {
            var services = CreateDefaultServices<TestMultipleCommand>(new string[] { "A", "--option0", "Hello" });
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var result = await dispatcher.DispatchAsync();
            serviceProvider.GetService<TestMultipleCommand>().Log[0].Should().Be($"{nameof(TestMultipleCommand.A)}:option0 -> Hello");
        }

        [Fact]
        public async Task CommandNotFound_Single()
        {
            var services = CreateDefaultServices<NoCommand>(new string[] { "C" });
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var ex = await Assert.ThrowsAsync<CommandNotFoundException>(async () => await dispatcher.DispatchAsync());
            ex.Command.Should().BeEmpty();
            ex.ImplementedCommands.All.Should().BeEmpty();
        }

        [Fact]
        public async Task CommandNotFound_Multiple()
        {
            var services = CreateDefaultServices<TestMultipleCommand>(new string[] { "C" });
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var ex = await Assert.ThrowsAsync<CommandNotFoundException>(async () => await dispatcher.DispatchAsync());
            ex.Command.Should().Be("C");
            ex.ImplementedCommands.All.Should().HaveCount(2); // A, B
        }

        [Fact]
        public async Task MultipleCommand_Primary()
        {
            var services = CreateDefaultServices<TestMultipleCommand_Primary>(new string[] { "--option0=123" });
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var result = await dispatcher.DispatchAsync();
        }

        [Fact]
        public async Task CommandNotFound_Multiple_Primary()
        {
            var services = CreateDefaultServices<TestMultipleCommand>(new string[] { });
            var serviceProvider = services.BuildServiceProvider();

            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var ex = await Assert.ThrowsAsync<CommandNotFoundException>(async () => await dispatcher.DispatchAsync());
            ex.Command.Should().BeEmpty();
            ex.ImplementedCommands.All.Should().HaveCount(2);
            ex.ImplementedCommands.Primary.Should().BeNull();
        }

        [Fact]
        public async Task NestedCommand_TopLevel()
        {
            var services = CreateDefaultServices<TestNestedCommand>(new string[] { "A" });
            var serviceProvider = services.BuildServiceProvider();

            var nestedCommand = serviceProvider.GetService<TestNestedCommand>();
            var nestedCommandNested = serviceProvider.GetService<TestNestedCommand.TestNestedCommand_Nested>();
            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var result = await dispatcher.DispatchAsync();

            nestedCommand.Log.Should().Contain("A");
            nestedCommandNested.Log.Should().BeEmpty();
        }

        [Fact]
        public async Task NestedCommand_Nested()
        {
            var services = CreateDefaultServices<TestNestedCommand>(new string[] { "TestNestedCommand_Nested", "B" });
            var serviceProvider = services.BuildServiceProvider();

            var nestedCommand = serviceProvider.GetService<TestNestedCommand>();
            var nestedCommandNested = serviceProvider.GetService<TestNestedCommand.TestNestedCommand_Nested>();
            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var result = await dispatcher.DispatchAsync();

            nestedCommand.Log.Should().BeEmpty();
            nestedCommandNested.Log.Should().Contain("B");
        }

        [Fact]
        public async Task NestedCommand_Primary()
        {
            var services = CreateDefaultServices<TestNestedCommand_Primary>(new string[] { "TestNestedCommand_Primary_Nested" });
            var serviceProvider = services.BuildServiceProvider();

            var nestedCommand = serviceProvider.GetService<TestNestedCommand_Primary>();
            var nestedCommandNested = serviceProvider.GetService<TestNestedCommand_Primary.TestNestedCommand_Primary_Nested>();
            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var result = await dispatcher.DispatchAsync();

            nestedCommand.Log.Should().BeEmpty();
            nestedCommandNested.Log.Should().Contain("B");
        }

        [Fact]
        public async Task DeepNestedCommand()
        {
            var services = CreateDefaultServices<TestDeepNestedCommand>(new string[] { "TestDeepNestedCommand_Nested", "TestDeepNestedCommand_Nested_2", "C" });
            var serviceProvider = services.BuildServiceProvider();

            var nestedCommand = serviceProvider.GetService<TestDeepNestedCommand>();
            var nestedCommandNested = serviceProvider.GetService<TestDeepNestedCommand.TestDeepNestedCommand_Nested>();
            var nestedCommandNested2 = serviceProvider.GetService<TestDeepNestedCommand.TestDeepNestedCommand_Nested_2>();
            var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
            var result = await dispatcher.DispatchAsync();

            nestedCommand.Log.Should().BeEmpty();
            nestedCommandNested.Log.Should().BeEmpty();
            nestedCommandNested2.Log.Should().Contain("C");
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

        public class TestCommand_IgnoreUnknownOption
        {
            public List<string> Log { get; } = new List<string>();

            [IgnoreUnknownOptions]
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

        public class TestMultipleCommand_Primary
        {
            public List<string> Log { get; } = new List<string>();

            [PrimaryCommand]
            public void A(string option0)
            {
                Log.Add($"{nameof(TestMultipleCommand.A)}:{nameof(option0)} -> {option0}");
            }

            public void B(bool option0, [Argument]string arg0)
            {
                Log.Add($"{nameof(TestMultipleCommand.B)}:{nameof(option0)} -> {option0}");
            }
        }

        [HasSubCommands(typeof(TestNestedCommand_Nested))]
        public class TestNestedCommand
        {
            public List<string> Log { get; } = new List<string>();

            public void A() => Log.Add($"{nameof(TestNestedCommand.A)}");
            public void Dummy() { }

            public class TestNestedCommand_Nested
            {
                public List<string> Log { get; } = new List<string>();

                public void B() => Log.Add($"{nameof(TestNestedCommand_Nested.B)}");
                public void C() => Log.Add($"{nameof(TestNestedCommand_Nested.C)}");
            }
        }


        [HasSubCommands(typeof(TestNestedCommand_Primary_Nested))]
        public class TestNestedCommand_Primary
        {
            public List<string> Log { get; } = new List<string>();

            public class TestNestedCommand_Primary_Nested
            {
                public List<string> Log { get; } = new List<string>();

                public void B() => Log.Add($"{nameof(TestNestedCommand_Primary_Nested.B)}");
            }
        }

        [HasSubCommands(typeof(TestDeepNestedCommand_Nested))]
        public class TestDeepNestedCommand
        {
            public List<string> Log { get; } = new List<string>();

            public void A() => Log.Add($"{nameof(TestDeepNestedCommand.A)}");
            public void Dummy() { }

            [HasSubCommands(typeof(TestDeepNestedCommand_Nested_2))]
            public class TestDeepNestedCommand_Nested
            {
                public List<string> Log { get; } = new List<string>();

                public void B() => Log.Add($"{nameof(TestDeepNestedCommand_Nested.B)}");
                public void Dummy() { }
            }

            public class TestDeepNestedCommand_Nested_2
            {
                public List<string> Log { get; } = new List<string>();

                public void C() => Log.Add($"{nameof(TestDeepNestedCommand_Nested_2.C)}");
                public void Dummy() { }
            }
        }
    }
}
