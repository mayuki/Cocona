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
        class ParameterInjectionTestCommands
        {
            public void HelloWithoutFromService(int age, IMyService myService) => Console.WriteLine($"Hello {myService.GetName()} ({age})!");
            public void HelloWithFromService(int age, [FromService] IMyService myService) => Console.WriteLine($"Hello {myService.GetName()} ({age})!");
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
