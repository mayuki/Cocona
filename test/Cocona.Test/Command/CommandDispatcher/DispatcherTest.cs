using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Dispatcher;
using Cocona.Command.Dispatcher.Middlewares;
using Cocona.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cocona.Command.Binder.Validation;
using Cocona.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cocona.Test.Command.CommandDispatcher;

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
        services.AddTransient<ICoconaConsoleProvider>(sp => new NullConsoleProvider());
        services.AddSingleton<ICoconaAppContextAccessor, CoconaAppContextAccessor>();
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddSingleton<ICoconaInstanceActivator, CoconaInstanceActivator>();
        services.AddSingleton<ICoconaServiceProviderScopeSupport, CoconaServiceProviderScopeSupport>();
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

    class NullConsoleProvider : ICoconaConsoleProvider
    {
        public TextWriter Output => TextWriter.Null;
        public TextWriter Error => TextWriter.Null;
    }

    [Fact]
    public async Task SimpleSingleCommandDispatch()
    {
        var services = CreateDefaultServices<TestCommand>(new string[] { "--option0=hogehoge" });
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);
        serviceProvider.GetService<TestCommand>().Log[0].Should().Be($"{nameof(TestCommand.Test)}:option0 -> hogehoge");
    }

    [Fact]
    public async Task RejectUnknownOptionMiddleware_UnknownOptions()
    {
        var services = CreateDefaultServices<TestCommand>(new string[] { "--option0=hogehoge", "--unknown-option" }, builder => { builder.UseMiddleware<RejectUnknownOptionsMiddleware>(); });
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);
        result.Should().Be(129);
    }

    [Fact]
    public async Task RejectUnknownOptionMiddleware_IgnoreUnknownOptions()
    {
        var services = CreateDefaultServices<TestCommand_IgnoreUnknownOption>(new string[] { "--option0=hogehoge", "--unknown-option" }, builder => { builder.UseMiddleware<RejectUnknownOptionsMiddleware>(); });
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);
        result.Should().Be(0);
        serviceProvider.GetService<TestCommand_IgnoreUnknownOption>().Log[0].Should().Be($"{nameof(TestCommand.Test)}:option0 -> hogehoge");
    }

    [Fact]
    public async Task MultipleCommand_Option1()
    {
        var services = CreateDefaultServices<TestMultipleCommand>(new string[] { "A", "--option0", "Hello" });
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);
        serviceProvider.GetService<TestMultipleCommand>().Log[0].Should().Be($"{nameof(TestMultipleCommand.A)}:option0 -> Hello");
    }

    [Fact]
    public async Task CommandNotFound_Single_NotImplementedYet()
    {
        var services = CreateDefaultServices<NoCommand>(new string[] { "C" });
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var ex = await Assert.ThrowsAsync<CommandNotFoundException>(async () => await dispatcher.DispatchAsync(resolvedCommand));
        ex.Message.Should().Contain("Command not yet implemented");
        ex.Command.Should().BeEmpty();
        ex.ImplementedCommands.All.Should().BeEmpty();
    }

    [Fact]
    public async Task CommandNotFound_Multiple_Resolver()
    {
        var services = CreateDefaultServices<TestMultipleCommand>(new string[] { "C" });
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var ex = await Assert.ThrowsAsync<CommandNotFoundException>(async () =>
        {
            var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
                serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
                serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
            );

            await dispatcher.DispatchAsync(resolvedCommand);
        });
        ex.Command.Should().Be("C");
        ex.ImplementedCommands.All.Should().HaveCount(2); // A, B
    }

    [Fact]
    public async Task MultipleCommand_Primary()
    {
        var services = CreateDefaultServices<TestMultipleCommand_Primary>(new string[] { "--option0=123" });
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);
    }

    [Fact]
    public async Task CommandNotFound_Multiple_Primary_Resolver()
    {
        var services = CreateDefaultServices<TestMultipleCommand>(new string[] { });
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var ex = await Assert.ThrowsAsync<CommandNotFoundException>(async () =>
        {
            var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
                serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
                serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
            );

            await dispatcher.DispatchAsync(resolvedCommand);
        });
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
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);

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
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);

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
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);

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
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);

        nestedCommand.Log.Should().BeEmpty();
        nestedCommandNested.Log.Should().BeEmpty();
        nestedCommandNested2.Log.Should().Contain("C");
    }
        
    [Fact]
    public async Task DelegateSimpleSingleCommandDispatch()
    {
        var command = new TestCommandDelegate();
        var action = new Action<string>(command.Test);

        var services = CreateDefaultServices<TestCommandDelegate>(new string[] { "--option0=alice" });
        var optionDesc = new CommandOptionDescriptor(
            typeof(string),
            "option0",
            Array.Empty<char>(),
            string.Empty,
            CoconaDefaultValue.None,
            default,
            CommandOptionFlags.None,
            Array.Empty<Attribute>());
        var commands = new[]
        {
            new CommandDescriptor(
                action.Method,
                action.Target,
                nameof(TestCommandStatic.Test),
                Array.Empty<string>(),
                string.Empty,
                Array.Empty<object>(),
                new []{ optionDesc },
                new []{ optionDesc },
                Array.Empty<CommandArgumentDescriptor>(),
                Array.Empty<CommandOverloadDescriptor>(),
                Array.Empty<CommandOptionLikeCommandDescriptor>(),
                CommandFlags.Primary,
                default
            )
        };
        services.Replace(ServiceDescriptor.Transient<ICoconaCommandProvider>(sp => new StaticCommandsCommandProvider(commands)));
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);
        command.Log[0].Should().Be($"{nameof(TestCommandDelegate.Test)}:{command.Id}:option0 -> alice");
    }

    class StaticCommandsCommandProvider : ICoconaCommandProvider
    {
        private readonly IReadOnlyList<CommandDescriptor> _commands;

        public StaticCommandsCommandProvider(IEnumerable<CommandDescriptor> commands)
        {
            _commands = commands.ToArray();
        }
            
        public CommandCollection GetCommandCollection()
        {
            return new CommandCollection(_commands);
        }
    }

    [Fact]
    public async Task StaticSimpleSingleCommandDispatch()
    {
        // NOTE: Once TestCommand is passed instead of TestCommandStatic (Generic type parameter doesn't accept a static class)
        var services = CreateDefaultServices<TestCommand>(new string[] { "--option0=alice" });
        var optionDesc = new CommandOptionDescriptor(
            typeof(string),
            "option0",
            Array.Empty<char>(),
            string.Empty,
            CoconaDefaultValue.None,
            default,
            CommandOptionFlags.None,
            Array.Empty<Attribute>());
        var commands = new[]
        {
            new CommandDescriptor(
                typeof(TestCommandStatic).GetMethod(nameof(TestCommandStatic.Test)),
                default,
                nameof(TestCommandStatic.Test),
                Array.Empty<string>(),
                string.Empty,
                Array.Empty<object>(),
                new []{ optionDesc },
                new []{ optionDesc },
                Array.Empty<CommandArgumentDescriptor>(),
                Array.Empty<CommandOverloadDescriptor>(),
                Array.Empty<CommandOptionLikeCommandDescriptor>(),
                CommandFlags.Primary,
                default
            )
        };
        services.Replace(ServiceDescriptor.Transient<ICoconaCommandProvider>(sp => new StaticCommandsCommandProvider(commands)));
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);
        TestCommandStatic.Log[0].Should().Be($"{nameof(TestCommandStatic.Test)}:option0 -> alice");
    }

    [Fact]
    public async Task CommandInstance_Dispose_After_Dispatch()
    {
        var services = CreateDefaultServices<TestCommand_Dispose_After_Dispatch>(new string[] { });
        services.AddSingleton<DisposeCounter>();
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);
        serviceProvider.GetRequiredService<DisposeCounter>().Count.Should().Be(1);
    }

    public class DisposeCounter
    {
        public int Count { get; set; }
    }

    public class TestCommand_Dispose_After_Dispatch : IDisposable
    {
        private readonly DisposeCounter _counter;

        public TestCommand_Dispose_After_Dispatch(DisposeCounter counter)
        {
            _counter = counter;
        }

        public void Hello() {}

        void IDisposable.Dispose()
        {
            if (_counter.Count > 0)
            {
                throw new InvalidOperationException("Dispose should be called only once.");
            }

            _counter.Count++;
        }
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1
    [Fact]
    public async Task CommandInstance_DisposeAsync_After_Dispatch()
    {
        var services = CreateDefaultServices<TestCommand_DisposeAsync_After_Dispatch>(new string[] { });
        services.AddSingleton<DisposeCounter>();
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);

        serviceProvider.GetRequiredService<DisposeCounter>().Count.Should().Be(1);
    }

    public class TestCommand_DisposeAsync_After_Dispatch : IAsyncDisposable
    {
        private readonly DisposeCounter _counter;

        public TestCommand_DisposeAsync_After_Dispatch(DisposeCounter counter)
        {
            _counter = counter;
        }

        public void Hello() { }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_counter.Count > 0)
            {
                throw new InvalidOperationException("DisposeAsync should be called only once.");
            }

            _counter.Count++;

            return default;
        }
    }
#endif

    [Fact]
    public async Task ServiceProvider_Scoped()
    {
        var services = CreateDefaultServices<TestCommand_ServiceProvider_Scoped>(new string[] { });
        services.AddSingleton<DisposeCounter>();
        services.AddScoped<TestService_ServiceProvider_Scoped>();
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();
        var resolvedCommand = serviceProvider.GetRequiredService<ICoconaCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<ICoconaCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<ICoconaCommandLineArgumentProvider>().GetArguments()
        );
        var result = await dispatcher.DispatchAsync(resolvedCommand);
        serviceProvider.GetRequiredService<DisposeCounter>().Count.Should().Be(1);
    }

    public class TestService_ServiceProvider_Scoped : IDisposable
    {
        private readonly DisposeCounter _counter;

        public TestService_ServiceProvider_Scoped(DisposeCounter counter)
        {
            _counter = counter;
        }

        public void Dispose()
        {
            _counter.Count++;
        }
    }

    public class TestCommand_ServiceProvider_Scoped
    {
        public TestCommand_ServiceProvider_Scoped(TestService_ServiceProvider_Scoped counter)
        {
        }
        public void Hello() { }
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
        
        
    public static class TestCommandStatic
    {
        public static List<string> Log { get; } = new List<string>();

        public static void Test(string option0)
        {
            Log.Clear();
            Log.Add($"{nameof(TestCommandStatic.Test)}:{nameof(option0)} -> {option0}");
        }
    }

    public class TestCommandDelegate
    {
        public List<string> Log { get; } = new List<string>();

        public Guid Id { get; } = Guid.NewGuid();

        public void Test(string option0)
        {
            Log.Add($"{nameof(TestCommandDelegate.Test)}:{Id}:{nameof(option0)} -> {option0}");
        }
    }

}