using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Application;
using Cocona.Command.BuiltIn;
using Cocona.CommandLine;
using Cocona.ShellCompletion.Candidate;
using FluentAssertions;
using Xunit;

#if COCONA_LITE
using CoconaApp = Cocona.CoconaLiteApp;
#endif

namespace Cocona.Test.Integration
{
    [Collection("End to End")] // NOTE: Test cases use `Console` and does not run in parallel.
    public class CoconaAppStaticTest
    {
        private (string StandardOut, string StandardError, int ExitCode) Run(string[] args, Action<string[]> action)
        {
            var stdOutWriter = new StringWriter();
            var stdErrWriter = new StringWriter();

            Console.SetOut(stdOutWriter);
            Console.SetError(stdErrWriter);

            action(args);

            return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
        }

        private async Task<(string StandardOut, string StandardError, int ExitCode)> RunAsync(string[] args, Func<string[], Task> action)
        {
            var stdOutWriter = new StringWriter();
            var stdErrWriter = new StringWriter();

            Console.SetOut(stdOutWriter);
            Console.SetError(stdErrWriter);

            await action(args);

            return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
        }

        [Fact]
        public void CoconaApp_RunOfT()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { "--name", "Alice" }, args =>
            {
                CoconaApp.Run<TestCommand_Single>(args);
            });

            stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public async Task CoconaApp_RunAsyncOfT()
        {
            var (stdOut, stdErr, exitCode) = await RunAsync(new string[] { "--name", "Alice" }, async args =>
            {
                await CoconaApp.RunAsync<TestCommand_Single>(args);
            });

            stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public void CoconaApp_Run_Types()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { "hello", "--name", "Alice" }, args =>
            {
                CoconaApp.Run(args, new[] { typeof(TestCommand_Single), typeof(TestCommand_Single2) });
            });

            stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public async Task CoconaApp_RunAsync_Types()
        {
            var (stdOut, stdErr, exitCode) = await RunAsync(new string[] { "hello", "--name", "Alice" }, async args =>
            {
                await CoconaApp.RunAsync(args, new[] { typeof(TestCommand_Single), typeof(TestCommand_Single2) });
            });

            stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public void CoconaApp_Run_Delegate()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { "--name", "Alice" }, args =>
            {
                CoconaApp.Run((string name) => Console.WriteLine($"Hello {name}!"), args);
            });

            stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public async Task CoconaApp_RunAsync_Delegate()
        {
            var (stdOut, stdErr, exitCode) = await RunAsync(new string[] { "--name", "Alice" }, async args =>
            {
                await CoconaApp.RunAsync((string name) => Console.WriteLine($"Hello {name}!"), args);
            });

            stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        class TestCommand_Single
        {
            public void Hello(string name)
            {
                Console.WriteLine($"Hello {name}!");
            }
        }

        class TestCommand_Single2
        {
            public void Konnichiwa(string name)
            {
                Console.WriteLine($"Konnichiwa {name}!");
            }
        }
    }
}
