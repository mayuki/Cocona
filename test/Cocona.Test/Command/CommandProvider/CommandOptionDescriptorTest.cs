using System.Reflection;
using Cocona.Command;

namespace Cocona.Test.Command.CommandProvider;

public class CommandOptionDescriptorTest
{
    [Fact]
    public void ValueName_NotArray()
    {
        var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ValueName_NotArray)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
        command.Options[0].ValueName.Should().Be("String");
    }

    [Fact]
    public void ValueName_Array()
    {
        var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ValueName_Array)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
        command.Options[0].ValueName.Should().Be("String");
    }

    [Fact]
    public void ValueName_IEnumerable()
    {
        var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ValueName_IEnumerable)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
        command.Options[0].ValueName.Should().Be("String");
    }

    [Fact]
    public void ValueName_Custom()
    {
        var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ValueName_Custom)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
        command.Options[0].ValueName.Should().Be("ValueName0");
    }

    class CommandTest
    {
        public void ValueName_NotArray(string option0) { }
        public void ValueName_Array(string[] options) { }
        public void ValueName_IEnumerable(List<string> options) { }
        public void ValueName_Custom([Option(ValueName = "ValueName0")]string option0) { }
    }

    private static MethodInfo GetMethod<T>(string methodName)
    {
        return typeof(T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
    }

}