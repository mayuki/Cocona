using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace Cocona.Lite.Test
{
    public class CoconaLiteServiceCollectionTest
    {
        [Fact]
        public void AddTransient_Factory()
        {
            var services = new CoconaLiteServiceCollection();
            services.AddTransient<IMyService>(_ => new MyService());

            services.Should().HaveCount(1);
            services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
        }

        [Fact]
        public void AddSingleton_Factory()
        {
            var services = new CoconaLiteServiceCollection();
            services.AddSingleton<IMyService>(_ => new MyService());

            services.Should().HaveCount(1);
            services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
        }

        [Fact]
        public void AddTransient_Type()
        {
            var services = new CoconaLiteServiceCollection();
            services.AddTransient<IMyService, MyService>();

            services.Should().HaveCount(1);
            services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
        }

        [Fact]
        public void AddSingleton_Type()
        {
            var services = new CoconaLiteServiceCollection();
            services.AddSingleton<IMyService, MyService>();

            services.Should().HaveCount(1);
            services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
        }

        [Fact]
        public void TryAddSingleton()
        {
            var services = new CoconaLiteServiceCollection();
            services.TryAddSingleton<IMyService, MyService>();
            services.TryAddSingleton<IMyService, MyService2>();

            services.Should().HaveCount(1);
            services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
        }

        [Fact]
        public void TryAddTransient()
        {
            var services = new CoconaLiteServiceCollection();
            services.TryAddTransient<IMyService, MyService>();
            services.TryAddTransient<IMyService, MyService2>();

            services.Should().HaveCount(1);
            services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
        }

        public interface IMyService
        {
        }

        public class MyService : IMyService
        {
        }
        public class MyService2 : IMyService
        {
        }
    }
}
