using Cocona.Filters;

#if COCONA_LITE
using CoconaApp = Cocona.CoconaLiteApp;
#endif

namespace Cocona.Test.Integration;

public class CommandFilterTest : EndToEndTestBase
{
    [Fact]
    public void CommandFilter_CommandType_Attributes()
    {
        var (stdOut, stdErr, exitCode) = Run(Array.Empty<string>(), args =>
        {
            var app = CoconaApp.Create(args);
            app.AddCommands<TestCommand_HasFilter>();
            app.Run();
        });

        stdOut.Should().Be(String.Join(Environment.NewLine, new[] { "Class#Begin", "Method#Begin", "Hello", "Method#End", "Class#End" }) + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    [Fact]
    public void CommandFilter_CommandDelegate()
    {
        var (stdOut, stdErr, exitCode) = Run(Array.Empty<string>(), args =>
        {
            var app = CoconaApp.Create(args);
            app.AddCommand([TestCommand_HasFilter.MyCommandFilter("Method")]() => Console.WriteLine("Hello"))
                .WithFilter(new TestCommand_HasFilter.MyCommandFilter("Builder"));
            app.Run();
        });

        stdOut.Should().Be(String.Join(Environment.NewLine, new[] { "Method#Begin", "Builder#Begin", "Hello", "Builder#End", "Method#End" }) + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    [Fact]
    public void CommandFilter_CommandDelegate_Multiple()
    {
        var (stdOut, stdErr, exitCode) = Run(Array.Empty<string>(), args =>
        {
            // Filters are applied from the outer side
            // Class -> Builder2 -> Builder1 -> Method
            var app = CoconaApp.Create(args);
            app.AddCommand([TestCommand_HasFilter.MyCommandFilter("Method")]() => Console.WriteLine("Hello"))
                .WithFilter(new TestCommand_HasFilter.MyCommandFilter("Builder1"))
                .WithFilter(new TestCommand_HasFilter.MyCommandFilter("Builder2"));
            app.Run();
        });

        stdOut.Should().Be(String.Join(Environment.NewLine, new[] { "Method#Begin", "Builder1#Begin", "Builder2#Begin", "Hello", "Builder2#End", "Builder1#End", "Method#End" }) + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    [Fact]
    public void CommandFilter_UseFilter_CommandDelegate()
    {
        var (stdOut, stdErr, exitCode) = Run(Array.Empty<string>(), args =>
        {
            // Filters are applied from the outer side
            // Builder0 (UseFilter) -> Method -> Builder1 -> Builder2
            var app = CoconaApp.Create(args);
            app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder0"));
            app.AddCommand([TestCommand_HasFilter.MyCommandFilter("Method")]() => Console.WriteLine("Hello")) // Same as .WithFilter()
                .WithFilter(new TestCommand_HasFilter.MyCommandFilter("Builder1"))
                .WithFilter(new TestCommand_HasFilter.MyCommandFilter("Builder2")); // Method, Builder1, Builder2
            app.Run();
        });

        stdOut.Should().Be(String.Join(Environment.NewLine, new[] { "Builder0#Begin", "Method#Begin", "Builder1#Begin", "Builder2#Begin", "Hello", "Builder2#End", "Builder1#End", "Method#End", "Builder0#End" }) + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    [Fact]
    public void CommandFilter_UseFilter_CommandType()
    {
        var (stdOut, stdErr, exitCode) = Run(Array.Empty<string>(), args =>
        {
            // Filters are applied from the outer side
            // Builder0 (UseFilter) -> Class -> Method
            var app = CoconaApp.Create(args);
            app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder0"));
            app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder1"));
            app.AddCommands<TestCommand_HasFilter>();
            app.Run();
        });

        stdOut.Should().Be(String.Join(Environment.NewLine, new[] { "Builder0#Begin", "Builder1#Begin", "Class#Begin", "Method#Begin", "Hello", "Method#End", "Class#End", "Builder1#End", "Builder0#End" }) + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    [Fact]
    public void CommandFilter_UseFilter_CommandDelegate_SubCommand()
    {
        var (stdOut, stdErr, exitCode) = Run(new[] { "sub-command", "command1" }, args =>
        {
            // Filters are applied from the outer side
            // Builder0 (UseFilter) -> Class -> Method
            var app = CoconaApp.Create(args);
            app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder0"));
            app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder1"));
            app.AddSubCommand("sub-command", x =>
            {
                x.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder2"));
                x.AddCommand("command1", () => Console.WriteLine("Hello"));
                x.AddCommand("command2", () => Console.WriteLine("A"));
            });

            app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder3"));
            app.AddSubCommand("sub-command-2", x =>
            {
                app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder4"));
                x.AddCommand("command1", () => Console.WriteLine("B"));
                x.AddCommand("command2", () => Console.WriteLine("C"));
            });
            app.Run();
        });

        stdOut.Should().Be(String.Join(Environment.NewLine, new[] { "Builder0#Begin", "Builder1#Begin", "Builder2#Begin", "Hello", "Builder2#End", "Builder1#End", "Builder0#End" }) + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    [Fact]
    public void CommandFilter_UseFilter_CommandType_SubCommand()
    {
        var (stdOut, stdErr, exitCode) = Run(new[] { "sub-command", "hello" }, args =>
        {
            // Filters are applied from the outer side
            // Builder0 (UseFilter) -> Class -> Method
            var app = CoconaApp.Create(args);
            app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder0"));
            app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder1"));
            app.AddSubCommand("sub-command", x =>
            {
                x.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder2"));
                x.AddCommands<TestCommandMany_HasFilter>();
            });

            app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder3"));
            app.AddSubCommand("sub-command-2", x =>
            {
                app.UseFilter(new TestCommand_HasFilter.MyCommandFilter("Builder4"));
                x.AddCommand("command1", () => Console.WriteLine("A"));
                x.AddCommand("command2", () => Console.WriteLine("B"));
            });
            app.Run();
        });

        stdOut.Should().Be(String.Join(Environment.NewLine, new[] { "Builder0#Begin", "Builder1#Begin", "Builder2#Begin", "Class#Begin", "Method#Begin", "Hello", "Method#End", "Class#End", "Builder2#End", "Builder1#End", "Builder0#End" }) + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    [MyCommandFilter("Class")]
    class TestCommand_HasFilter
    {
        [MyCommandFilter("Method")]
        public void Hello() => Console.WriteLine("Hello");

        public class MyCommandFilter : CommandFilterAttribute
        {
            private string _label;
            public MyCommandFilter(string label)
            {
                _label = label;
            }
            public override async ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
            {
                Console.WriteLine($"{_label}#Begin");
                var result = await next(ctx);
                Console.WriteLine($"{_label}#End");
                return result;
            }
        }
    }

    [MyCommandFilter("Class")]
    class TestCommandMany_HasFilter
    {
        [MyCommandFilter("Method")]
        public void Hello() => Console.WriteLine("Hello");
        public void Konnichiwa() => Console.WriteLine("Konnichiwa");

        public class MyCommandFilter : CommandFilterAttribute
        {
            private string _label;
            public MyCommandFilter(string label)
            {
                _label = label;
            }
            public override async ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
            {
                Console.WriteLine($"{_label}#Begin");
                var result = await next(ctx);
                Console.WriteLine($"{_label}#End");
                return result;
            }
        }
    }
}
