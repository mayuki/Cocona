using WildCoco;
#if WildCoco_LITE
using WildCocoApp = WildCoco.WildCocoLiteApp;
#endif

namespace WildCoco.Test.Integration;

public class ParameterInjectionTest : EndToEndTestBase
{
    [Fact]
    public void ParameterInjection_WildCocoApp_CreateBuilder_Delegate()
    {
        var (stdOut, stdErr, exitCode) = Run(new[] { "--age", "18" }, args =>
        {
            var builder = WildCocoApp.CreateBuilder(args);
#if WildCoco_LITE
            builder.Services.Add(new Lite.ServiceDescriptor(typeof(IMyService), (_, _) => new MyService(), singleton: true));
#else
            builder.Services.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(typeof(IMyService), new MyService()));
#endif
            var app = builder.Build();
            app.AddCommand((int age, IMyService myService) => Console.WriteLine($"Hello {myService.GetName()} ({age})!"));
            app.Run();
        });

        stdOut.Should().Be("Hello Alice (18)!" + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    [Fact]
    public void ParameterInjection_WildCocoApp_CreateBuilder_Type()
    {
        {
            var (stdOut, stdErr, exitCode) = Run(new[] { "hello-without-from-service", "--age", "18" }, args =>
            {
                var builder = WildCocoApp.CreateBuilder(args);
#if WildCoco_LITE
                builder.Services.Add(new Lite.ServiceDescriptor(typeof(IMyService), (_, _) => new MyService(), singleton: true));
#else
                builder.Services.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(typeof(IMyService), new MyService()));
#endif
                var app = builder.Build();
                app.AddCommands<ParameterInjectionTestCommands>();
                app.Run();
            });

            stdErr.Should().Contain("'--my-service' is required");
            exitCode.Should().Be(1);
        }

        {
            var (stdOut, stdErr, exitCode) = Run(new[] { "hello-with-from-service", "--age", "18" }, args =>
            {
                var builder = WildCocoApp.CreateBuilder(args);
#if WildCoco_LITE
                builder.Services.Add(new Lite.ServiceDescriptor(typeof(IMyService), (_, _) => new MyService(), singleton: true));
#else
                builder.Services.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(typeof(IMyService), new MyService()));
#endif
                var app = builder.Build();
                app.AddCommands<ParameterInjectionTestCommands>();
                app.Run();
            });

            stdOut.Should().Be("Hello Alice (18)!" + Environment.NewLine);
            exitCode.Should().Be(0);
        }
    }

    [Fact]
    public void ParameterInjection_WildCocoAppContext_AutoWiring_WildCocoApp_CreateBuilder_Delegate()
    {
        var (stdOut, stdErr, exitCode) = Run(new string[] {}, args =>
        {
            var builder = WildCocoApp.CreateBuilder(args);
            var app = builder.Build();
            app.AddCommand((WildCocoAppContext appContext) => Console.WriteLine($"appContext is null: {appContext is null}"));
            app.Run();
        });

        stdOut.Should().Be("appContext is null: False" + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    [Fact]
    public void ParameterInjection_WildCocoAppContext_AutoWiring_WildCocoApp_CreateBuilder_Type()
    {
        var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
        {
            var builder = WildCocoApp.CreateBuilder(args);
            var app = builder.Build();
            app.AddCommands<ParameterInjectionTestCommands_WildCocoAppContext>();
            app.Run();
        });

        stdOut.Should().Be("appContext is null: False" + Environment.NewLine);
        exitCode.Should().Be(0);
    }

    class ParameterInjectionTestCommands
    {
        public void HelloWithoutFromService(int age, IMyService myService) => Console.WriteLine($"Hello {myService.GetName()} ({age})!");
        public void HelloWithFromService(int age, [FromService] IMyService myService) => Console.WriteLine($"Hello {myService.GetName()} ({age})!");
    }

    class ParameterInjectionTestCommands_WildCocoAppContext
    {
        public void Hello([FromService] WildCocoAppContext appContext) => Console.WriteLine($"appContext is null: {appContext is null}");
    }

    interface IMyService
    {
        string GetName();
    }
    class MyService : IMyService
    {
        public string GetName() => "Alice";
    }
}
