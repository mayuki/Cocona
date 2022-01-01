using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

#if COCONA_LITE
using CoconaApp = Cocona.CoconaLiteApp;
#endif

namespace Cocona.Test.Integration
{
    public class BuildAppCommandsWithBuilderTest : EndToEndTestBase
    {
        [Fact]
        public void Unnamed_SingleCommand()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommand(() => Console.WriteLine("Hello Konnichiwa!"));
                app.Run();
            });

            stdOut.Should().Be("Hello Konnichiwa!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public void Unnamed_SingleCommand_Duplicated()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommand(() => Console.WriteLine("Hello Konnichiwa!"));
                app.AddCommand(() => Console.WriteLine("Hello Konnichiwa!"));
                app.Run();
            });

            stdErr.Should().Contain("One unnamed primary command can be registered");
            exitCode.Should().Be(1);
        }

        [Fact]
        public void Named_SingleCommand_Index()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommand("hello", ([Argument] string name) => Console.WriteLine($"Hello {name}!"));
                app.Run();
            });

            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain("hello");
            exitCode.Should().Be(0);
        }

        [Fact]
        public void Named_SingleCommand_Invoke()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { "hello", "Alice" }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommand("hello", ([Argument] string name) => Console.WriteLine($"Hello {name}!"));
                app.Run();
            });

            stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public void MultipleCommand_Index()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommand("hello", ([Argument] string name) => Console.WriteLine($"Hello {name}!"));
                app.AddCommand("konnichiwa", ([Argument] string name) => Console.WriteLine($"Konnichiwa {name}!"));
                app.Run();
            });

            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain("hello");
            stdOut.Should().Contain("konnichiwa");
            exitCode.Should().Be(0);
        }

        [Fact]
        public void MultipleCommand_Invoke()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { "hello", "Alice" }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommand("hello", ([Argument] string name) => Console.WriteLine($"Hello {name}!"));
                app.AddCommand("konnichiwa", ([Argument] string name) => Console.WriteLine($"Konnichiwa {name}!"));
                app.Run();
            });

            stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public void SubCommand_MultipleCommand_Index()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { "sub-command" }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddSubCommand("sub-command", x =>
                {
                    x.AddCommand("hello", ([Argument] string name) => Console.WriteLine($"Hello {name}!"));
                    x.AddCommand("konnichiwa", ([Argument] string name) => Console.WriteLine($"Konnichiwa {name}!"));
                });
                app.Run();
            });

            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain("hello");
            stdOut.Should().Contain("konnichiwa");
            exitCode.Should().Be(0);
        }

        [Fact]
        public void SubCommand_MultipleCommand_Invoke()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { "sub-command", "hello", "Alice" }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddSubCommand("sub-command", x =>
                {
                    x.AddCommand("hello", ([Argument] string name) => Console.WriteLine($"Hello {name}!"));
                    x.AddCommand("konnichiwa", ([Argument] string name) => Console.WriteLine($"Konnichiwa {name}!"));
                });
                app.Run();
            });

            stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public void SubCommand_Named_SingleCommand_Index()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { "sub-command" }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddSubCommand("sub-command", x =>
                {
                    x.AddCommand("hello", ([Argument] string name) => Console.WriteLine($"Hello {name}!"));
                });
                app.Run();
            });

            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain("hello");
            exitCode.Should().Be(0);
        }

        [Fact]
        public void SubCommand_Named_SingleCommand_Invoke()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { "sub-command", "hello", "Alice" }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddSubCommand("sub-command", x =>
                {
                    x.AddCommand("hello", ([Argument] string name) => Console.WriteLine($"Hello {name}!"));
                });
                app.Run();
            });

            stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }
    }
}
