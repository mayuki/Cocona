#if !COCONA_LITE
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Cocona.Test.Integration
{
    public class CoconaHostTest : EndToEndTestBase
    {
        [Fact]
        public void CoconaApp_Run_DisposeServices()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
            {
                var service = new DisposeService();

                var builder = CoconaApp.CreateBuilder(args);
                builder.Services.AddSingleton(_ => service);
                var app = builder.Build();
                app.AddCommand((DisposeService _) =>
                {
                });
                app.Run();

                service.IsDisposed.Should().BeTrue();
            });
        }

        [Fact]
        public void CoconaApp_Run_DisposeServicesAsync()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
            {
                var service = new DisposeServiceAsync();

                var builder = CoconaApp.CreateBuilder(args);
                builder.Services.AddSingleton(_ => service);
                var app = builder.Build();
                app.AddCommand((DisposeServiceAsync _) =>
                {
                });
                app.Run();

                service.IsDisposed.Should().BeTrue();
            });
        }

        class DisposeService : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
                => IsDisposed = true;
        }


        class DisposeServiceAsync : IAsyncDisposable
        {
            public bool IsDisposed { get; private set; }

            public ValueTask DisposeAsync()
            {
                IsDisposed = true;
                return default;
            }
        }

        [Fact]
        public void CoconaApp_Run_DisposeHost()
        {
            var (stdOut, stdErr, exitCode) = Run(new string[] { }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                TestConfigurationSource? source = default;
                builder.Configuration.Add<TestConfigurationSource>(x => { source = x; });
                {
                    var app = builder.Build();
                    app.AddCommand(() => { });
                    app.Run();
                }

                (source?.IsProviderDisposed).Should().BeTrue();
            });

            Run(new string[] { }, args =>
            {
                var builder = CoconaApp.CreateBuilder(args);
                TestConfigurationSource? source = default;
                builder.Configuration.Add<TestConfigurationSource>(x => { source = x; });
                {
                    var app = builder.Build();
                    app.Dispose();
                }

                (source?.IsProviderDisposed).Should().BeTrue();
            });
        }

        class TestConfigurationSource : IConfigurationSource
        {
            public bool IsProviderDisposed { get; private set; }

            public IConfigurationProvider Build(IConfigurationBuilder builder)
                => new ConfigurationProvider(this);

            class ConfigurationProvider : IConfigurationProvider, IDisposable
            {
                private readonly TestConfigurationSource _source;

                public ConfigurationProvider(TestConfigurationSource source)
                {
                    _source = source;
                }

                public void Dispose() => _source.IsProviderDisposed = true;

                public bool TryGet(string key, out string value)
                {
                    value = default;
                    return false;
                }

                public void Set(string key, string value)
                {
                }

                public IChangeToken GetReloadToken()
                {
                    return new CancellationChangeToken(default);
                }

                public void Load()
                {
                }

                public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
                {
                    return Enumerable.Empty<string>();
                }
            }
        }
    }
}
#endif
