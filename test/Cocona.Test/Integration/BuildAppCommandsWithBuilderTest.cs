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

            stdErr.Should().Contain("The commands contains more then one primary command");
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

        [Fact]
        public void Unnamed_OptionLikeCommand()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommand(() => Console.WriteLine("Hello Konnichiwa!"))
                    .OptionLikeCommand(x =>
                    {
                        x.Add("info", () => Console.WriteLine("Info"));
                        x.Add("foo", () => Console.WriteLine("Foo"))
                            .WithDescription("Foo-Description");
                    });
                app.Run();
            });

            stdOut.Should().Be("Hello Konnichiwa!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public void Unnamed_OptionLikeCommand_Index()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { "--help" }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();

                app.AddCommand(() => Console.WriteLine("Hello Konnichiwa!"))
                    .OptionLikeCommand(x =>
                    {
                        x.Add("info", () => Console.WriteLine("Info"));
                        x.Add("foo", () => Console.WriteLine("Foo"))
                            .WithDescription("Foo-Description");
                        x.Add("hidden", () => Console.WriteLine("Hidden"))
                            .WithMetadata(new HiddenAttribute());
                    });

                app.Run();
            });

            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain("--info");
            stdOut.Should().Contain("--foo");
            stdOut.Should().Contain("Foo-Description");
            stdOut.Should().NotContain("--hidden");
            exitCode.Should().Be(129);
        }

        [Fact]
        public void Parameter_NotNull_Or_Unknown_Required_RefType()
        {
            {
                var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
                {
                    var app = CoconaApp.Create(args);
                    app.AddCommand((string name) => Console.WriteLine($"Hello {name}!")); // [NotNull]
                    app.Run();
                });

                stdErr.Should().Contain("'--name' is required");
                exitCode.Should().Be(1);
            }
#nullable disable
            {
                var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
                {
                    var app = CoconaApp.Create(args);
                    app.AddCommand((string name) => Console.WriteLine($"Hello {name}!")); // [Unknown]
                    app.Run();
                });

                stdErr.Should().Contain("'--name' is required");
                exitCode.Should().Be(1);
            }
#nullable restore
        }

        [Fact]
        public void Parameter_Nullable_Optional_RefType()
        {
            {
                var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
                {
                    var app = CoconaApp.Create(args);
                    app.AddCommand((string? name) => Console.WriteLine($"Hello {(name ?? "Guest")}!"));
                    app.Run();
                });

                stdErr.Should().BeEmpty();
                stdOut.Should().Be("Hello Guest!" + Environment.NewLine);
                exitCode.Should().Be(0);
            }
            {
                var (stdOut, stdErr, exitCode) = Run(new string[] { "--name", "Alice" }, args =>
                {
                    var app = CoconaApp.Create(args);
                    app.AddCommand((string? name) => Console.WriteLine($"Hello {(name ?? "Guest")}!"));
                    app.Run();
                });

                stdErr.Should().BeEmpty();
                stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
                exitCode.Should().Be(0);
            }
        }

        [Fact]
        public void Parameter_Nullable_Optional_ValueType()
        {
            {
                var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
                {
                    var app = CoconaApp.Create(args);
                    app.AddCommand((int? year) => Console.WriteLine($"Hello {(year ?? 2022)}!"));
                    app.Run();
                });

                stdErr.Should().BeEmpty();
                stdOut.Should().Be("Hello 2022!" + Environment.NewLine);
                exitCode.Should().Be(0);
            }
            {
                var (stdOut, stdErr, exitCode) = Run(new string[] { "--year", "2000" }, args =>
                {
                    var app = CoconaApp.Create(args);
                    app.AddCommand((int? year) => Console.WriteLine($"Hello {(year ?? 2022)}!"));
                    app.Run();
                });

                stdErr.Should().BeEmpty();
                stdOut.Should().Be("Hello 2000!" + Environment.NewLine);
                exitCode.Should().Be(0);
            }
        }

        [Fact]
        public void Parameter_Nullable_Optional_Boolean()
        {
            {
                var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
                {
                    var app = CoconaApp.Create(args);
                    app.AddCommand((bool? flag) => Console.WriteLine($"Flag: {(flag.HasValue ? flag.Value : "(null)")}"));
                    app.Run();
                });

                stdErr.Should().BeEmpty();
                stdOut.Should().Be("Flag: (null)" + Environment.NewLine);
                exitCode.Should().Be(0);
            }
            {
                var (stdOut, stdErr, exitCode) = Run(new string[] { "--flag" }, args =>
                {
                    var app = CoconaApp.Create(args);
                    app.AddCommand((bool? flag) => Console.WriteLine($"Flag: {(flag.HasValue ? flag.Value : "(null)")}"));
                    app.Run();
                });

                stdErr.Should().BeEmpty();
                stdOut.Should().Be("Flag: True" + Environment.NewLine);
                exitCode.Should().Be(0);
            }
            {
                var (stdOut, stdErr, exitCode) = Run(new string[] { "--flag=false" }, args =>
                {
                    var app = CoconaApp.Create(args);
                    app.AddCommand((bool? flag) => Console.WriteLine($"Flag: {(flag.HasValue ? flag.Value : "(null)")}"));
                    app.Run();
                });

                stdErr.Should().BeEmpty();
                stdOut.Should().Be("Flag: False" + Environment.NewLine);
                exitCode.Should().Be(0);
            }
        }
    }
}
