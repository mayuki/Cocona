using Cocona.Builder.Metadata;
using Cocona.Filters;
using Cocona.Filters.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Cocona.Test.Filters;

public class FilterHelperTest
{
    [Fact]
    public void GetFilterFactories()
    {
        var factories = FilterHelper.GetFilterFactories(typeof(TestCommand).GetMethod(nameof(TestCommand.TestFilter)));
        factories.Should().HaveCount(2);
    }

    [Fact]
    public void GetFilters_Method()
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<MyService>()
            .BuildServiceProvider();

        var filters = FilterHelper.GetFilters<ICommandFilter>(typeof(TestCommand).GetMethod(nameof(TestCommand.TestFilter)), serviceProvider);
        filters.Should().HaveCount(2);
        filters[1].Should().BeAssignableTo<TestFilterImplementsFactoryAttribute.FilterImpl>();
        filters[1].As<TestFilterImplementsFactoryAttribute.FilterImpl>().Service.Should().NotBeNull();
    }

    [Fact]
    public void GetFilters_Class()
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<MyService>()
            .BuildServiceProvider();

        var filters = FilterHelper.GetFilters<ICommandFilter>(typeof(TestCommand_Class).GetMethod(nameof(TestCommand_Class.A)), serviceProvider);
        filters.Should().HaveCount(1);
    }

    [Fact]
    public void GetFilters_Ordered()
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<MyService>()
            .BuildServiceProvider();

        var filters = FilterHelper.GetFilters<ICommandFilter>(typeof(TestCommand).GetMethod(nameof(TestCommand.TestOrderedFilter)), serviceProvider);
        filters.Should().HaveCount(5);

        // First, Unordered, Mid, Mid2, Last
        filters[0].Should().BeAssignableTo<OrderedFilterAttribute>();
        filters[0].As<OrderedFilterAttribute>().Name.Should().Be("First");
            
        filters[1].Should().BeAssignableTo<UnorderedFilterAttribute>();

        filters[2].Should().BeAssignableTo<OrderedFilterAttribute>();
        filters[2].As<OrderedFilterAttribute>().Name.Should().Be("Mid");
            
        filters[3].Should().BeAssignableTo<OrderedFilterViaFactoryAttribute.FilterImpl>();
        filters[3].As<OrderedFilterViaFactoryAttribute.FilterImpl>().Name.Should().Be("Mid2");
            
        filters[4].Should().BeAssignableTo<OrderedFilterAttribute>();
        filters[4].As<OrderedFilterAttribute>().Name.Should().Be("Last");
    }

    [Fact]
    public void GetFilters_Metadata()
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<MyService>()
            .BuildServiceProvider();

        var metadata = new object[]
        {
            new CommandNameMetadata("Command"),
            new TestFilterAttribute(),
            new TestFilterImplementsFactoryAttribute(),
        };
        var filters = FilterHelper.GetFilters<ICommandFilter>(metadata, serviceProvider);
        filters.Should().HaveCount(2);

        filters[0].Should().BeOfType<TestFilterAttribute>();
        filters[1].Should().BeOfType<TestFilterImplementsFactoryAttribute.FilterImpl>();
    }

    class TestCommand
    {
        [TestFilter]
        [TestFilterImplementsFactory]
        public void TestFilter() { }

        [OrderedFilter("Mid", 123)]
        [OrderedFilter("Last", int.MaxValue)]
        [UnorderedFilter]
        [OrderedFilterViaFactory("Mid2", 456)]
        [OrderedFilter("First", int.MinValue)]
        public void TestOrderedFilter() { }
    }

    [TestFilter]
    class TestCommand_Class
    {
        public void A() { }
    }

    class MyService { }

    class UnorderedFilterAttribute : CommandFilterAttribute
    {
        public override ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
        {
            throw new NotImplementedException();
        }
    }

    class OrderedFilterAttribute : CommandFilterAttribute, IOrderedFilter
    {
        public int Order { get; }
        public string Name { get; }

        public OrderedFilterAttribute(string name, int order)
        {
            Name = name;
            Order = order;
        }

        public override ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
        {
            throw new NotImplementedException();
        }
    }
    class OrderedFilterViaFactoryAttribute : Attribute, IFilterFactory, IOrderedFilter
    {
        public int Order { get; }
        public string Name { get; }

        public OrderedFilterViaFactoryAttribute(string name, int order)
        {
            Name = name;
            Order = order;
        }

        public class FilterImpl : ICommandFilter
        {
            public string Name { get; }

            public FilterImpl(string name) => Name = name;

            public ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
            {
                throw new NotImplementedException();
            }
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new FilterImpl(Name);
        }
    }

    class TestFilterAttribute : CommandFilterAttribute
    {
        public override ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
        {
            throw new NotImplementedException();
        }
    }

    class TestFilterImplementsFactoryAttribute : Attribute, IFilterFactory
    {
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new FilterImpl(serviceProvider.GetService<MyService>());
        }

        public class FilterImpl : ICommandFilter
        {
            public MyService Service { get; }

            public FilterImpl(MyService myService)
            {
                Service = myService;
            }

            public ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
            {
                throw new NotImplementedException();
            }
        }
    }
}