using System.Reflection;
using Cocona.Application;
using Cocona.Builder.Metadata;
using Cocona.Command;

namespace Cocona.Test.Command.CommandProvider;

public class FromServiceTest
{
    [Fact]
    public void FromService_NoMetadata()
    {
        var metadata = new object[0];
        var serviceProviderIsService = new FakeCoconaServiceProviderIsService(typeof(IMyService));
        var cmd = new CoconaCommandProvider(Array.Empty<Type>(), serviceProviderIsService: serviceProviderIsService)
            .CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Hello)), isSingleCommand: true, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default, metadata);

        cmd.Parameters[0].Name.Should().Be("valueA");
        cmd.Parameters[0].Should().NotBeOfType<CommandServiceParameterDescriptor>();
        cmd.Parameters[1].Name.Should().Be("valueB");
        cmd.Parameters[1].Should().NotBeOfType<CommandServiceParameterDescriptor>();
        cmd.Parameters[2].Name.Should().Be("myService");
        cmd.Parameters[2].Should().NotBeOfType<CommandServiceParameterDescriptor>();
    }

    [Fact]
    public void FromService_ByFromBuilder_ServiceProviderIsService()
    {
        var metadata = new object[]
        {
            new CommandFromBuilderMetadata(),
        };
        var serviceProviderIsService = new FakeCoconaServiceProviderIsService(typeof(IMyService));
        var cmd = new CoconaCommandProvider(Array.Empty<Type>(), serviceProviderIsService: serviceProviderIsService)
            .CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Hello)), isSingleCommand: true, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default, metadata);

        cmd.Parameters[0].Name.Should().Be("valueA");
        cmd.Parameters[0].Should().NotBeOfType<CommandServiceParameterDescriptor>();
        cmd.Parameters[1].Name.Should().Be("valueB");
        cmd.Parameters[1].Should().NotBeOfType<CommandServiceParameterDescriptor>();
        cmd.Parameters[2].Name.Should().Be("myService");
        cmd.Parameters[2].Should().BeOfType<CommandServiceParameterDescriptor>().Subject.ServiceType.Should().Be(typeof(IMyService));
    }

    [Fact]
    public void FromService_ByAttribute()
    {
        var metadata = new object[0];
        var serviceProviderIsService = new FakeCoconaServiceProviderIsService(typeof(IMyService));
        var cmd = new CoconaCommandProvider(Array.Empty<Type>(), serviceProviderIsService: serviceProviderIsService)
            .CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.HelloWithAttribute)), isSingleCommand: true, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default, metadata);

        cmd.Parameters[0].Name.Should().Be("valueA");
        cmd.Parameters[0].Should().NotBeOfType<CommandServiceParameterDescriptor>();
        cmd.Parameters[1].Name.Should().Be("valueB");
        cmd.Parameters[1].Should().NotBeOfType<CommandServiceParameterDescriptor>();
        cmd.Parameters[2].Name.Should().Be("myService");
        cmd.Parameters[2].Should().BeOfType<CommandServiceParameterDescriptor>().Subject.ServiceType.Should().Be(typeof(IMyService));
    }

    private static MethodInfo GetMethod<T>(string methodName)
    {
        return typeof(T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
    }

    class CommandTest
    {
        public void Hello(string valueA, int valueB, IMyService myService) { }
        public void HelloWithAttribute(string valueA, int valueB, [FromService]IMyService myService) { }
    }

    interface IMyService { }

    class FakeCoconaServiceProviderIsService : ICoconaServiceProviderIsService
    {
        private readonly Type _target;

        public FakeCoconaServiceProviderIsService(Type target)
        {
            _target = target;
        }

        public bool IsService(Type t) => t == _target;
    }
}