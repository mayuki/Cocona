using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Application;
using FluentAssertions;
using Xunit;

#if COCONA_LITE
using CoconaApp = Cocona.CoconaLiteApp;
#endif

namespace Cocona.Test.Integration
{
    [Collection("End to End")] // NOTE: Test cases use `Console` and does not run in parallel.
    public class EndToEndTest
    {
        private (string StandardOut, string StandardError, int ExitCode) Run<T>(string[] args)
        {
            var stdOutWriter = new StringWriter();
            var stdErrWriter = new StringWriter();

            Console.SetOut(stdOutWriter);
            Console.SetError(stdErrWriter);

            CoconaApp.Create()
                .Run<T>(args);

            return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
        }

        private async Task<(string StandardOut, string StandardError, int ExitCode)> RunAsync<T>(string[] args, CancellationToken cancellationToken)
        {
            var stdOutWriter = new StringWriter();
            var stdErrWriter = new StringWriter();

            Console.SetOut(stdOutWriter);
            Console.SetError(stdErrWriter);

            await CoconaApp.Create()
                .RunAsync<T>(args, cancellationToken: cancellationToken);

            return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
        }

        private (string StandardOut, string StandardError, int ExitCode) Run(string[] args, Type[] types)
        {
            var stdOutWriter = new StringWriter();
            var stdErrWriter = new StringWriter();

            Console.SetOut(stdOutWriter);
            Console.SetError(stdErrWriter);

            CoconaApp.Create()
                .Run(args, types);

            return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
        }

        private async Task<(string StandardOut, string StandardError, int ExitCode)> RunAsync<T>(string[] args, Type[] types, CancellationToken cancellationToken)
        {
            var stdOutWriter = new StringWriter();
            var stdErrWriter = new StringWriter();

            Console.SetOut(stdOutWriter);
            Console.SetError(stdErrWriter);

            await CoconaApp.Create()
                .RunAsync(args, types, cancellationToken: cancellationToken);

            return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
        }


        [Fact]
        public void CoconaApp_Run_Single()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Single>(new string[] { });

            stdOut.Should().Be("Hello Konnichiwa!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        class TestCommand_Single
        {
            public void Hello()
            {
                Console.WriteLine("Hello Konnichiwa!");
            }
        }

        [Fact]
        public void CoconaApp_Run_Multiple_Command()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Multiple>(new string[] { "konnichiwa" });

            stdOut.Should().Be("Konnichiwa!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public void CoconaApp_Run_Multiple_Index()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Multiple>(new string[] { });

            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain("Commands:");
            stdOut.Should().Contain("  konnichiwa");
            stdOut.Should().Contain("  hello");
            exitCode.Should().Be(0);
        }

        [Fact]
        public void CoconaApp_Run_Multiple_ExitCode()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Multiple>(new string[] { "exit-code" });

            stdOut.Should().Contain("ExitCode=128");
            exitCode.Should().Be(128);
        }

        [Fact]
        public void CoconaApp_Run_Multiple_CommandMissing()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Multiple>(new string[] {"axit-mode"});

            stdOut.Should().BeEmpty();
            stdErr.Should().Contain("Similar");
            stdErr.Should().Contain("exit-code");
            exitCode.Should().Be(1);
        }

        [Fact]
        public async Task CoconaApp_Run_Multiple_Task()
        {
            var (stdOut, stdErr, exitCode) = (await RunAsync<TestCommand_Multiple>(new string[] { "long-running" }, new CancellationTokenSource(1000).Token));

            stdOut.Should().Contain("Begin");
            stdOut.Should().Contain("Canceled");
            stdErr.Should().BeEmpty();
            exitCode.Should().Be(127);
        }

        [Fact]
        public void CoconaApp_Run_MultipleClass()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { }, new [] { typeof(TestCommand_Multiple), typeof(TestCommand2) });

            stdOut.Should().Contain("exit-code");
            stdOut.Should().Contain("foo-bar");
            stdErr.Should().BeEmpty();
            exitCode.Should().Be(0);
        }

        [Fact]
        public void CoconaApp_Run_ArgTest_1()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Multiple>(new string[] { "arg-test", "Alice" });

            stdOut.Should().Contain("Hello Alice (17)!");
            stdErr.Should().BeEmpty();
            exitCode.Should().Be(0);
        }

        [Fact]
        public void CoconaApp_Run_ArgTest_2()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Multiple>(new string[] { "arg-test", "Karen", "18" });

            stdOut.Should().Contain("Hello Karen (18)!");
            stdErr.Should().BeEmpty();
            exitCode.Should().Be(0);
        }

        [Fact]
        public void CoconaApp_Run_OptionTest_1()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Multiple>(new string[] { "option-test", "--name", "Alice" });

            stdOut.Should().Contain("Hello Alice (17)!");
            stdErr.Should().BeEmpty();
            exitCode.Should().Be(0);
        }

        [Fact]
        public void CoconaApp_Run_OptionTest_2()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Multiple>(new string[] { "option-test", "--name", "Karen", "-a", "18" });

            stdOut.Should().Contain("Hello Karen (18)!");
            stdErr.Should().BeEmpty();
            exitCode.Should().Be(0);
        }

        [Fact]
        public void CoconaApp_Run_Completion()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Multiple>(new string[] { "--completion", "zsh" });

            stdOut.Should().Contain("#compdef");
            stdErr.Should().BeEmpty();
            exitCode.Should().Be(0);
        }

        class TestCommand_Multiple
        {
            public void Hello()
            {
                Console.WriteLine("Hello!");
            }

            public void Konnichiwa()
            {
                Console.WriteLine("Konnichiwa!");
            }

            public int ExitCode()
            {
                Console.WriteLine("ExitCode=128");
                return 128;
            }

            public async Task<int> LongRunning([FromService]ICoconaAppContextAccessor context)
            {
                Console.WriteLine("Begin");
                while (!context.Current.CancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1);
                }
                Console.WriteLine("Canceled");

                return 127;
            }

            public void ArgTest([Argument] string name, [Argument]int age = 17)
            {
                Console.WriteLine($"Hello {name} ({age})!");
            }

            public void OptionTest([Option] string name, [Option('a')]int age = 17)
            {
                Console.WriteLine($"Hello {name} ({age})!");
            }

            public enum TestValues
            {
                Alice,
                Karen
            }
        }

        class TestCommand2
        {
            public void FooBar() { }
        }

        [Fact]
        public void CoconaApp_Run_Nested()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Nested>(new string[] { "nested", "hello", "Karen" });

            stdOut.Should().Contain("Hello Karen");
            stdErr.Should().BeEmpty();
            exitCode.Should().Be(0);
        }

        [Fact]
        public void CoconaApp_Run_Nested_CommandHelp()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Nested>(new string[] { "nested", "hello", "--help" });

            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain(" nested hello [--help] arg0");
            stdOut.Should().Contain("Arguments:");
        }

        [Fact]
        public void CoconaApp_Run_Nested_Index_0()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Nested>(new string[] { });

            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain("Commands:");
            stdOut.Should().Contain("  konnichiwa");
            stdOut.Should().Contain("  nested");
        }

        [Fact]
        public void CoconaApp_Run_Nested_Index_1()
        {
            var (stdOut, stdErr, exitCode) = Run<TestCommand_Nested>(new string[] { "nested" });

            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain("Commands:");
            stdOut.Should().Contain("  hello");
            stdOut.Should().Contain("  bye");
        }

        [HasSubCommands(typeof(Nested))]
        class TestCommand_Nested
        {
            public void Konnichiwa()
            {
                Console.WriteLine("Konnichiwa");
            }

            class Nested
            {
                public void Hello([Argument] string arg0)
                {
                    Console.WriteLine($"Hello {arg0}");
                }
                public void Bye([Argument] string arg0)
                {
                    Console.WriteLine($"Bye {arg0}");
                }
            }
        }
    }
}
