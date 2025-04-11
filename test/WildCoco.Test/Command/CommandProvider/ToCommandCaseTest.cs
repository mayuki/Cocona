using System.Reflection;
using WildCoco;
using WildCoco.Command;
using WildCoco.Command;

namespace WildCoco.Test.Command.BuiltIn;

public class ToCommandCaseTest
{
    [Fact]
    public void DisableCommandNameConversion()
    {
        var cmd = new WildCocoCommandProvider(Array.Empty<Type>(), options: CommandProviderOptions.TreatPublicMethodAsCommands).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
        cmd.Name.Should().Be("CommandName");
        cmd.Options[0].Name.Should().Be("dryRun");
        cmd.Arguments[0].Name.Should().Be("ArgName0");
    }
    [Fact]
    public void EnableCommandNameConversion()
    {
        var cmd = new WildCocoCommandProvider(Array.Empty<Type>(), options: CommandProviderOptions.TreatPublicMethodAsCommands | CommandProviderOptions.CommandNameToLowerCase).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
        cmd.Name.Should().Be("command-name");
        cmd.Options[0].Name.Should().Be("dryRun");
        cmd.Arguments[0].Name.Should().Be("ArgName0");
    }
    [Fact]
    public void DisableOptionNameConversion()
    {
        var cmd = new WildCocoCommandProvider(Array.Empty<Type>(), options: CommandProviderOptions.TreatPublicMethodAsCommands).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
        cmd.Name.Should().Be("CommandName");
        cmd.Options[0].Name.Should().Be("dryRun");
        cmd.Arguments[0].Name.Should().Be("ArgName0");
    }
    [Fact]
    public void EnableOptionNameConversion()
    {
        var cmd = new WildCocoCommandProvider(Array.Empty<Type>(), options: CommandProviderOptions.TreatPublicMethodAsCommands | CommandProviderOptions.OptionNameToLowerCase).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
        cmd.Name.Should().Be("CommandName");
        cmd.Options[0].Name.Should().Be("dry-run");
        cmd.Arguments[0].Name.Should().Be("ArgName0");
    }
    [Fact]
    public void DisableArgumentNameConversion()
    {
        var cmd = new WildCocoCommandProvider(Array.Empty<Type>(), options: CommandProviderOptions.TreatPublicMethodAsCommands).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
        cmd.Name.Should().Be("CommandName");
        cmd.Options[0].Name.Should().Be("dryRun");
        cmd.Arguments[0].Name.Should().Be("ArgName0");
    }
    [Fact]
    public void EnableArgumentNameConversion()
    {
        var cmd = new WildCocoCommandProvider(Array.Empty<Type>(), options: CommandProviderOptions.TreatPublicMethodAsCommands | CommandProviderOptions.ArgumentNameToLowerCase).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
        cmd.Name.Should().Be("CommandName");
        cmd.Options[0].Name.Should().Be("dryRun");
        cmd.Arguments[0].Name.Should().Be("arg-name0");
    }

    class CommandTest
    {
        // ReSharper disable once InconsistentNaming
        public void CommandName(bool dryRun, [Argument]string ArgName0)
        {
        }
    }

    private static MethodInfo GetMethod<T>(string methodName)
    {
        return typeof(T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
    }

    [Fact]
    public void NoProcess()
    {
        WildCocoCommandProvider.ToCommandCase("foo").Should().Be("foo");
    }

    [Fact]
    public void ToKebabCase()
    {
        WildCocoCommandProvider.ToCommandCase("fooBar").Should().Be("foo-bar");
    }

    [Fact]
    public void ToKebabCase_FollowingUpperCase()
    {
        WildCocoCommandProvider.ToCommandCase("enableUI").Should().Be("enable-ui");
    }

    [Fact]
    public void ToKebabCase_FollowingUpperCase_2()
    {
        WildCocoCommandProvider.ToCommandCase("enableUIWindow").Should().Be("enable-uiwindow");
    }

    [Fact]
    public void ToKebabCase_ManyUpperCase()
    {
        WildCocoCommandProvider.ToCommandCase("BooleanTrueByDefault").Should().Be("boolean-true-by-default");
    }

    [Fact]
    public void UpperCase()
    {
        WildCocoCommandProvider.ToCommandCase("Enable").Should().Be("enable");
    }

    [Fact]
    public void Number()
    {
        WildCocoCommandProvider.ToCommandCase("option0").Should().Be("option0");
    }
}
