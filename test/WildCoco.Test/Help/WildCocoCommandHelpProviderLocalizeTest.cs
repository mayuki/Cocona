using WildCoco.Application;
using WildCoco.Command;
using WildCoco.Help;
using WildCoco.Internal;
using WildCoco.Localization;
using WildCoco.Application;
using WildCoco.Command;
using WildCoco.Help;
using WildCoco.Internal;
using WildCoco.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace WildCoco.Test.Help;

public class WildCocoCommandHelpProviderLocalizeTest
{
    private void __Dummy() { }

    class FakeApplicationMetadataProvider : IWildCocoApplicationMetadataProvider
    {
        public string ProductName { get; set; } = "ProductName";
        public string Description { get; set; } = string.Empty;

        public string GetDescription() => Description;

        public string GetExecutableName() => "ExeName";

        public string GetProductName() => ProductName;

        public string GetVersion() => "1.0.0.0";
    }

    private CommandDescriptor CreateCommand(string name, string description, ICommandParameterDescriptor[] parameterDescriptors, CommandFlags flags = CommandFlags.None)
    {
        return new CommandDescriptor(
            typeof(WildCocoCommandHelpProviderLocalizeTest).GetMethod(nameof(WildCocoCommandHelpProviderLocalizeTest.__Dummy), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!,
            default,
            name,
            Array.Empty<string>(),
            description,
            Array.Empty<object>(),
            parameterDescriptors,
            parameterDescriptors.OfType<CommandOptionDescriptor>().ToArray(),
            parameterDescriptors.OfType<CommandArgumentDescriptor>().ToArray(),
            Array.Empty<CommandOverloadDescriptor>(),
            Array.Empty<CommandOptionLikeCommandDescriptor>(),
            flags,
            null
        );
    }

    private CommandOptionDescriptor CreateCommandOption(Type optionType, string name, IReadOnlyList<char> shortName, string description, WildCocoDefaultValue defaultValue, CommandOptionFlags flags = CommandOptionFlags.None)
    {
        var optionValueName = (DynamicListHelper.IsArrayOrEnumerableLike(optionType) ? DynamicListHelper.GetElementType(optionType) : optionType).Name;

        return new CommandOptionDescriptor(optionType, name, shortName, description, defaultValue, optionValueName, flags, Array.Empty<Attribute>());
    }

    [Fact]
    public void CommandsIndexHelp_Commands_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", WildCocoDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new WildCocoDefaultValue(false)),
                CreateCommandOption(typeof(int), "bar", new [] { 'b' }, "has default value", new WildCocoDefaultValue(123)),
            },
            CommandFlags.Primary
        );
        var commandDescriptor2 = CreateCommand(
            "Test2",
            "command2 description",
            new ICommandParameterDescriptor[0],
            CommandFlags.None
        );

        var services = new ServiceCollection();
        services.AddSingleton<IWildCocoLocalizer, FakeWildCocoLocalizer>();

        var provider = new WildCocoCommandHelpProvider(new FakeApplicationMetadataProvider(), services.BuildServiceProvider());
        var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }), Array.Empty<CommandDescriptor>());
        var text = new WildCocoHelpRenderer().Render(help);
        text.Should().Be(@"
Usage: ExeName [command]
Usage: ExeName [--foo <String>] [--looooooong-option] [--bar <Int32>]

__Localized[command description]__

Commands:
  Test2    __Localized[command2 description]__

Options:
  -f, --foo <String>         __Localized[Foo option]__ (Required)
  -l, --looooooong-option    __Localized[Long name option]__
  -b, --bar <Int32>          __Localized[has default value]__ (Default: 123)
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Arguments_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", WildCocoDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new WildCocoDefaultValue(false)),
                new CommandArgumentDescriptor(typeof(string[]), "src", 0, "src files", WildCocoDefaultValue.None, Array.Empty<Attribute>()),
                new CommandArgumentDescriptor(typeof(string), "dest", 0, "dest dir", WildCocoDefaultValue.None, Array.Empty<Attribute>()),
            }
        );

        var services = new ServiceCollection();
        services.AddSingleton<IWildCocoLocalizer, FakeWildCocoLocalizer>();

        var provider = new WildCocoCommandHelpProvider(new FakeApplicationMetadataProvider(), services.BuildServiceProvider());
        var help = provider.CreateCommandHelp(commandDescriptor, Array.Empty<CommandDescriptor>());
        var text = new WildCocoHelpRenderer().Render(help);
        text.Should().Be(@"
Usage: ExeName Test [--foo <String>] [--looooooong-option] src0 ... srcN dest

__Localized[command description]__

Arguments:
  0: src     __Localized[src files]__ (Required)
  1: dest    __Localized[dest dir]__ (Required)

Options:
  -f, --foo <String>         __Localized[Foo option]__ (Required)
  -l, --looooooong-option    __Localized[Long name option]__
".TrimStart());
    }

    class FakeWildCocoLocalizer : IWildCocoLocalizer
    {
        public string GetCommandDescription(CommandDescriptor command)
            => $"__Localized[{command.Description}]__";

        public string GetOptionDescription(CommandDescriptor command, ICommandOptionDescriptor option)
            => $"__Localized[{option.Description}]__";

        public string GetArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument)
            => $"__Localized[{argument.Description}]__";
    }
}
