#if COCONA_LITE
using CoconaApp = Cocona.CoconaLiteApp;
#endif

namespace Cocona.Test.Integration
{
    public class ParameterInjectionTest : EndToEndTestBase
    {
        [Fact]
        public void ParameterInjection_CoconaApp_CreateBuilder_Delegate()
        {
            var (stdOut, stdErr, exitCode) = Run(new[] { "--age", "18" }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
#if COCONA_LITE
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
        public void ParameterInjection_CoconaApp_CreateBuilder_Type()
        {
            {
                var (stdOut, stdErr, exitCode) = Run(new[] { "hello-without-from-service", "--age", "18" }, args =>
                {
                    var builder = CoconaApp.CreateBuilder(args);
#if COCONA_LITE
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
                    var builder = CoconaApp.CreateBuilder(args);
#if COCONA_LITE
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
        public void ParameterInjection_CoconaAppContext_AutoWiring_CoconaApp_CreateBuilder_Delegate()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] {}, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommand((CoconaAppContext appContext) => Console.WriteLine($"appContext is null: {appContext is null}"));
                app.Run();
            });

            stdOut.Should().Be("appContext is null: False" + Environment.NewLine);
            exitCode.Should().Be(0);
        }

        [Fact]
        public void ParameterInjection_CoconaAppContext_AutoWiring_CoconaApp_CreateBuilder_Type()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommands<ParameterInjectionTestCommands_CoconaAppContext>();
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

        class ParameterInjectionTestCommands_CoconaAppContext
        {
            public void Hello([FromService] CoconaAppContext appContext) => Console.WriteLine($"appContext is null: {appContext is null}");
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
}
