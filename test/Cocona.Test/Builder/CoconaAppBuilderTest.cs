using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cocona.Test.Builder
{
    public class CoconaAppBuilderTest
    {
        [Fact]
        public void UpdateConfiguration()
        {
            var builder = CoconaApp.CreateBuilder();
            builder.Configuration.GetSection($"__{nameof(CoconaAppBuilderTest)}__").Value.Should().BeNull();
            builder.Configuration.AddInMemoryCollection(new[] { KeyValuePair.Create($"__{nameof(CoconaAppBuilderTest)}__", "true") });
            builder.Configuration.GetSection($"__{nameof(CoconaAppBuilderTest)}__").Value.Should().Be("true");

            builder.Build().Dispose();
        }

        [Fact]
        public void UpdateEnvironment()
        {
            var builder = CoconaApp.CreateBuilder();
            builder.Environment.ApplicationName = "foobar";
            var app = builder.Build();
            app.Environment.ApplicationName.Should().Be("foobar");
            app.Dispose();
        }

        [Fact]
        public void UseServiceAfterBuilderBuild()
        {
            var builder = CoconaApp.CreateBuilder();
            builder.Services.AddTransient<IMyService, MyService>();
            var app = builder.Build();
            app.Services.GetRequiredService<IMyService>().GetName().Should().Be("Alice");
            app.Dispose();
        }

        [Fact]
        public void UseLoggerAfterBuilderBuild()
        {
            var builder = CoconaApp.CreateBuilder();
            builder.Services.AddTransient<IMyService, MyService>();
            var loggerProvider = new MyLoggerProvider();
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(loggerProvider);
            var app = builder.Build();
            app.Logger.LogInformation("Logging");

            loggerProvider.Logs.Should().HaveCount(1);
            loggerProvider.Logs[0].Should().Contain("|Logging");
            app.Dispose();
        }

        class MyLoggerProvider : ILoggerProvider
        {
            public List<string> Logs { get; } = new List<string>();

            public class Logger : ILogger
            {
                private readonly MyLoggerProvider _parent;
                private readonly string _categoryName;
                public Logger(MyLoggerProvider parent, string categoryName)
                {
                    _parent = parent;
                    _categoryName = categoryName;
                }

                public IDisposable BeginScope<TState>(TState state) => new NullDisposable();
                class NullDisposable : IDisposable
                {
                    public void Dispose() { }
                }

                public bool IsEnabled(LogLevel logLevel) => true;

                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
                    => _parent.Logs.Add(_categoryName + "|" + formatter(state, exception));
            }

            public ILogger CreateLogger(string categoryName)
                => new Logger(this, categoryName);

            public void Dispose()
            {
            }
        }

        interface IMyService { string GetName(); }
        class MyService : IMyService
        {
            public string GetName() => "Alice";
        }
    }
}
