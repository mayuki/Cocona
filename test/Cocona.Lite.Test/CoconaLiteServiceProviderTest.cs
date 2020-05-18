using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace Cocona.Lite.Test
{
    public class CoconaLiteServiceProviderTest
    {
        [Fact]
        public void NoDependency_Singleton_TService_TImplementation()
        {
            var services = new CoconaLiteServiceCollection();
            services.AddSingleton<IMyService, MyService>();
            var serviceProvider = new CoconaLiteServiceProvider(services);
            var instance = serviceProvider.GetService<IMyService>();

            instance.Should().NotBeNull();
            instance.Should().BeOfType<MyService>();

            var instance2 = serviceProvider.GetService<IMyService>();
            instance.Should().BeSameAs(instance2);
            instance.Id.Should().Be(instance2.Id);

            serviceProvider.Dispose();
            ((MyService)instance).IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void NoDependency_Transient_TService_TImplementation()
        {
            var services = new CoconaLiteServiceCollection();
            services.AddTransient<IMyService, MyService>();
            var serviceProvider = new CoconaLiteServiceProvider(services);
            var instance = serviceProvider.GetService<IMyService>();

            instance.Should().NotBeNull();
            instance.Should().BeOfType<MyService>();

            var instance2 = serviceProvider.GetService<IMyService>();
            instance.Should().NotBeSameAs(instance2);
            instance.Id.Should().NotBe(instance2.Id);

            serviceProvider.Dispose();
            ((MyService)instance).IsDisposed.Should().BeTrue();
            ((MyService)instance2).IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void NoDependency_Singleton_TService_Factory()
        {
            var services = new CoconaLiteServiceCollection();
            services.AddSingleton<IMyService>(_ => new MyService());
            var serviceProvider = new CoconaLiteServiceProvider(services);
            var instance = serviceProvider.GetService<IMyService>();

            instance.Should().NotBeNull();
            instance.Should().BeOfType<MyService>();

            var instance2 = serviceProvider.GetService<IMyService>();
            instance.Should().BeSameAs(instance2);
            instance.Id.Should().Be(instance2.Id);

            serviceProvider.Dispose();
            ((MyService)instance).IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void NoDependency_Transient_TService_Factory()
        {
            var services = new CoconaLiteServiceCollection();
            services.AddTransient<IMyService>(_ => new MyService());
            var serviceProvider = new CoconaLiteServiceProvider(services);
            var instance = serviceProvider.GetService<IMyService>();

            instance.Should().NotBeNull();
            instance.Should().BeOfType<MyService>();

            var instance2 = serviceProvider.GetService<IMyService>();
            instance.Should().NotBeSameAs(instance2);
            instance.Id.Should().NotBe(instance2.Id);

            serviceProvider.Dispose();
            ((MyService)instance).IsDisposed.Should().BeTrue();
            ((MyService)instance2).IsDisposed.Should().BeTrue();
        }

        public interface IMyService
        {
            int Id { get; }
        }

        public class MyService : IMyService, IDisposable
        {
            private static int _seq;

            public bool IsDisposed { get; private set; }
            public int Id { get; }

            public MyService()
            {
                Id = Interlocked.Increment(ref _seq);
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        [Fact]
        public void HasDependency_Singleton_TService_TImplementation()
        {
            var services = new CoconaLiteServiceCollection();
            services.AddSingleton<IMyService, MyService>();
            services.AddSingleton<IYetAnotherService, YetAnotherService>();
            var serviceProvider = new CoconaLiteServiceProvider(services);
            var instance = serviceProvider.GetService<IYetAnotherService>();

            instance.Should().NotBeNull();
            instance.Should().BeOfType<YetAnotherService>();
            ((YetAnotherService)instance).MyService.Should().NotBeNull();

            var instanceOfMyService = serviceProvider.GetService<IMyService>();
            ((YetAnotherService)instance).MyService.Should().BeSameAs(instanceOfMyService);

            serviceProvider.Dispose();
            ((YetAnotherService)instance).IsDisposed.Should().BeTrue();
            ((MyService)((YetAnotherService)instance).MyService).IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void HasDependency_Transient_TService_TImplementation()
        {
            var services = new CoconaLiteServiceCollection();
            services.AddTransient<IMyService, MyService>();
            services.AddTransient<IYetAnotherService, YetAnotherService>();
            var serviceProvider = new CoconaLiteServiceProvider(services);
            var instance = serviceProvider.GetService<IYetAnotherService>();

            instance.Should().NotBeNull();
            instance.Should().BeOfType<YetAnotherService>();
            ((YetAnotherService)instance).MyService.Should().NotBeNull();

            serviceProvider.Dispose();
            ((YetAnotherService)instance).IsDisposed.Should().BeTrue();
            ((MyService)((YetAnotherService)instance).MyService).IsDisposed.Should().BeTrue();
        }

        public interface IYetAnotherService
        {

        }

        public class YetAnotherService : IYetAnotherService, IDisposable
        {
            public IMyService MyService { get; }

            public YetAnotherService(IMyService myService)
            {
                MyService = myService;
            }

            public bool IsDisposed { get; private set; }
            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
