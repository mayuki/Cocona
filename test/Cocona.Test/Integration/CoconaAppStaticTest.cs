#if COCONA_LITE
using Cocona.Lite;
#else
using Microsoft.Extensions.DependencyInjection;
#endif

#if COCONA_LITE
using CoconaApp = Cocona.CoconaLiteApp;
#endif

namespace Cocona.Test.Integration;

public class CoconaAppStaticTest : EndToEndTestBase
{
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

    [Fact]
    public async Task CoconaApp_CreateBuilder_UsePrebuiltServiceOnBuildingApp()
    {
        var (stdOut, stdErr, exitCode) = await RunAsync(Array.Empty<string>(), async args =>
        {
            var builder = CoconaApp.CreateBuilder(args);
            builder.Services.AddTransient<IMyService, MyService>();
            var app = builder.Build();
            var text = $"Hello {app.Services.GetRequiredService<IMyService>().GetName()}!";
            app.AddCommand(() => Console.WriteLine(text));
            await app.RunAsync();
        });

        stdOut.Should().Be("Hello Alice!" + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    interface IMyService { string GetName(); }
    class MyService : IMyService
    {
        public string GetName() => "Alice";
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
