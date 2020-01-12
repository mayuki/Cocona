using Cocona.Command.Dispatcher;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cocona.Test.Command.CommandDispatcher
{
    public class PipelineBuilderTest
    {
        [Fact]
        public async Task Empty()
        {
            var services = new ServiceCollection();
            {
                services.AddTransient<ICoconaCommandDispatcherPipelineBuilder, CoconaCommandDispatcherPipelineBuilder>();
            }
            var serviceProvider = services.BuildServiceProvider();

            var builder = serviceProvider.GetService<ICoconaCommandDispatcherPipelineBuilder>();
            builder.Should().NotBeNull();
            var pipeline = builder.Build();
            var result = await pipeline(new CommandDispatchContext(null, null));
            result.Should().Be(0);
        }

        [Fact]
        public async Task UseMiddleware_Instance()
        {
            var services = new ServiceCollection();
            {
                services.AddTransient<ICoconaCommandDispatcherPipelineBuilder, CoconaCommandDispatcherPipelineBuilder>();
            }
            var serviceProvider = services.BuildServiceProvider();

            var builder = serviceProvider.GetService<ICoconaCommandDispatcherPipelineBuilder>();
            builder.Should().NotBeNull();
            var called = false;
            builder.UseMiddleware(async (next, ctx) =>
            {
                called = true;
                return 123;
            });
            var pipeline = builder.Build();
            var result = await pipeline(new CommandDispatchContext(null, null));
            result.Should().Be(123);
            called.Should().BeTrue();
        }

        [Fact]
        public async Task UseMiddleware_Type()
        {
            var services = new ServiceCollection();
            {
                services.AddTransient<ICoconaCommandDispatcherPipelineBuilder, CoconaCommandDispatcherPipelineBuilder>();
                services.AddSingleton<List<string>>(); // for log
            }
            var serviceProvider = services.BuildServiceProvider();

            var log = serviceProvider.GetService<List<string>>();
            var builder = serviceProvider.GetService<ICoconaCommandDispatcherPipelineBuilder>();
            builder.Should().NotBeNull();
            builder.UseMiddleware<TestMiddleware>();
            var pipeline = builder.Build();
            var result = await pipeline(new CommandDispatchContext(null, null));
            result.Should().Be(456);
            log.Should().NotBeEmpty();
            log[0].Should().Be("Called");
        }

        [Fact]
        public async Task UseMiddleware_Type_Cascade()
        {
            var services = new ServiceCollection();
            {
                services.AddTransient<ICoconaCommandDispatcherPipelineBuilder, CoconaCommandDispatcherPipelineBuilder>();
                services.AddSingleton<List<string>>(); // for log
            }
            var serviceProvider = services.BuildServiceProvider();

            var log = serviceProvider.GetService<List<string>>();
            var builder = serviceProvider.GetService<ICoconaCommandDispatcherPipelineBuilder>();
            builder.Should().NotBeNull();
            builder.UseMiddleware<Test2Middleware>();
            builder.UseMiddleware<Test3Middleware>();
            builder.UseMiddleware<Test4Middleware>();
            var pipeline = builder.Build();
            var result = await pipeline(new CommandDispatchContext(null, null));
            result.Should().Be(0);
            log.Should().NotBeEmpty();
            log[0].Should().Be("Begin:Test2Middleware");
            log[1].Should().Be("Begin:Test3Middleware");
            log[2].Should().Be("Begin:Test4Middleware");
            log[3].Should().Be("CallNext(0)");
            log[4].Should().Be("End:Test4Middleware");
            log[5].Should().Be("CallNext(0)");
            log[6].Should().Be("End:Test3Middleware");
            log[7].Should().Be("CallNext(0)");
            log[8].Should().Be("End:Test2Middleware");
        }

        class TestMiddleware : CommandDispatcherMiddleware
        {
            private readonly List<string> _logger;

            public TestMiddleware(CommandDispatchDelegate next, List<string> logger)
                : base(next)
            {
                _logger = logger;
            }

            public override async ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
            {
                _logger.Add("Called");
                return 456;
            }
        }

        class Test2Middleware : CommandDispatcherMiddleware
        {
            private readonly List<string> _logger;

            public Test2Middleware(CommandDispatchDelegate next, List<string> logger)
                : base(next)
            {
                _logger = logger;
            }

            public override async ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
            {
                _logger.Add($"Begin:{GetType().Name}");
                var result = await Next(ctx);
                _logger.Add($"CallNext({result})");
                _logger.Add($"End:{GetType().Name}");
                return result;
            }
        }

        class Test3Middleware : Test2Middleware
        {
            public Test3Middleware(CommandDispatchDelegate next, List<string> logger)
                : base(next, logger) { }
        }
        class Test4Middleware : Test2Middleware
        {
            public Test4Middleware(CommandDispatchDelegate next, List<string> logger)
                : base(next, logger) { }
        }
    }
}
