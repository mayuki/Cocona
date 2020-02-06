using Cocona.Application;
using Cocona.Command;
using Cocona.Help;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocona.Internal;
using Xunit;

namespace Cocona.Test.Help
{
    public class CoconaCommandHelpProviderTest
    {
        private void __Dummy() { }

        class FakeApplicationMetadataProvider : ICoconaApplicationMetadataProvider
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
                typeof(CoconaCommandHelpProviderTest).GetMethod(nameof(CoconaCommandHelpProviderTest.__Dummy), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance),
                name,
                Array.Empty<string>(),
                description,
                parameterDescriptors,
                parameterDescriptors.OfType<CommandOptionDescriptor>().ToArray(),
                parameterDescriptors.OfType<CommandArgumentDescriptor>().ToArray(),
                Array.Empty<CommandOverloadDescriptor>(),
                flags,
                null
            );
        }

        private CommandOptionDescriptor CreateCommandOption(Type optionType, string name, IReadOnlyList<char> shortName, string description, CoconaDefaultValue defaultValue, CommandOptionFlags flags = CommandOptionFlags.None)
        {
            var optionValueName = (DynamicListHelper.IsArrayOrEnumerableLike(optionType) ? DynamicListHelper.GetElementType(optionType) : optionType).Name;

            return new CommandOptionDescriptor(optionType, name, shortName, description, defaultValue, optionValueName, flags, Array.Empty<Attribute>());
        }

        [Fact]
        public void CommandHelp1()
        {
            // void Test(string arg0, string arg1, string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2" }
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "option0", Array.Empty<char>(), "option description - option0", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "option1", Array.Empty<char>(), "option description - option1", CoconaDefaultValue.None),
                    new CommandIgnoredParameterDescriptor(typeof(bool), "ignored0", true),
                    new CommandServiceParameterDescriptor(typeof(bool), "fromService0"),
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "description - arg0", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "description - arg1", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "description - arg2", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
        }

        [Fact]
        public void CommandIndexHelp_Single_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                },
                CommandFlags.Primary
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [--foo <String>] [--looooooong-option]

command description

Options:
  -f, --foo <String>         Foo option (Required)
  -l, --looooooong-option    Long name option
".TrimStart());
        }

        [Fact]
        public void CommandIndexHelp_Single_NoDescription_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                },
                CommandFlags.Primary
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [--foo <String>] [--looooooong-option]

Options:
  -f, --foo <String>         Foo option (Required)
  -l, --looooooong-option    Long name option
".TrimStart());
        }

        [Fact]
        public void CommandIndexHelp_Single_DescriptionFromMetadata_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                },
                CommandFlags.Primary
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider() { Description = "via metadata" }, new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [--foo <String>] [--looooooong-option]

via metadata

Options:
  -f, --foo <String>         Foo option (Required)
  -l, --looooooong-option    Long name option
".TrimStart());
        }

        [Fact]
        public void CommandIndexHelp_Single_NoParams_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "",
                new ICommandParameterDescriptor[0],
                CommandFlags.Primary
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider() { Description = "via metadata" }, new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName

via metadata
".TrimStart());
        }

        [Fact]
        public void CommandHelp_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                }
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName Test [--foo <String>] [--looooooong-option]

command description

Options:
  -f, --foo <String>         Foo option (Required)
  -l, --looooooong-option    Long name option
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
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "src files", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "dest dir", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName Test [--foo <String>] [--looooooong-option] src0 ... srcN dest

command description

Arguments:
  0: src     src files (Required)
  1: dest    dest dir (Required)

Options:
  -f, --foo <String>         Foo option (Required)
  -l, --looooooong-option    Long name option
".TrimStart());
        }

        [Fact]
        public void CreateCommandsIndexHelp_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                },
                CommandFlags.Primary
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [--foo <String>] [--looooooong-option]

command description

Options:
  -f, --foo <String>         Foo option (Required)
  -l, --looooooong-option    Long name option
".TrimStart());
        }

        [Fact]
        public void CreateCommandsIndexHelp_Arguments_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "Argument description", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                },
                CommandFlags.Primary
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [--foo <String>] [--looooooong-option] arg0

command description

Arguments:
  0: arg0    Argument description (Required)

Options:
  -f, --foo <String>         Foo option (Required)
  -l, --looooooong-option    Long name option
".TrimStart());
        }

        [Fact]
        public void CreateCommandsIndexHelp_Commands_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(int), "bar", new [] { 'b' }, "has default value", new CoconaDefaultValue(123)),
                },
                CommandFlags.Primary
            );
            var commandDescriptor2 = CreateCommand(
                "Test2",
                "command2 description",
                new ICommandParameterDescriptor[0],
                CommandFlags.None
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [command]
Usage: ExeName [--foo <String>] [--looooooong-option] [--bar <Int32>]

command description

Commands:
  Test2    command2 description

Options:
  -f, --foo <String>         Foo option (Required)
  -l, --looooooong-option    Long name option
  -b, --bar <Int32>          has default value (Default: 123)
".TrimStart());
        }

        [Fact]
        public void CreateCommandsIndexHelp_Commands_NoOptionInPrimary_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[0],
                CommandFlags.Primary
            );
            var commandDescriptor2 = CreateCommand(
                "Test2",
                "command2 description",
                new ICommandParameterDescriptor[0],
                CommandFlags.None
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [command]

command description

Commands:
  Test2    command2 description
".TrimStart());
        }

        [Fact]
        public void CreateCommandsIndexHelp_Commands_Hidden_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[0],
                CommandFlags.Hidden
            );
            var commandDescriptor2 = CreateCommand(
                "Test2",
                "command2 description",
                new ICommandParameterDescriptor[0],
                CommandFlags.None
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [command]

Commands:
  Test2    command2 description
".TrimStart());
        }

        [Fact]
        public void CommandHelp_Options_Enum_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(CommandHelpEnumValue), "enum", new [] { 'e' }, "Enum option", CoconaDefaultValue.None),
                }
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName Test [--enum <CommandHelpEnumValue>]

command description

Options:
  -e, --enum <CommandHelpEnumValue>    Enum option (Required) (Allowed values: Alice, Karen, Other)
".TrimStart());
        }

        [Fact]
        public void CommandHelp_Options_Boolean_DefaultFalse_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(bool), "flag", new [] { 'f' }, "Boolean option", new CoconaDefaultValue(false)),
                }
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName Test [--flag]

command description

Options:
  -f, --flag    Boolean option
".TrimStart());
        }

        [Fact]
        public void CommandHelp_Options_Boolean_DefaultTrue_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(bool), "flag", new [] { 'f' }, "Boolean option", new CoconaDefaultValue(true)),
                }
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName Test [--flag=<true|false>]

command description

Options:
  -f, --flag=<true|false>    Boolean option (Default: True)
".TrimStart());
        }


        [Fact]
        public void CommandHelp_Options_Hidden_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(bool), "flag", new [] { 'f' }, "Boolean option", new CoconaDefaultValue(true), CommandOptionFlags.Hidden),
                }
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName Test

command description
".TrimStart());
        }

        [Fact]
        public void CommandHelp_Options_Array_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(int[]), "option0", new [] { 'o' }, "Int option values", CoconaDefaultValue.None),
                }
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName Test [--option0 <Int32>...]

command description

Options:
  -o, --option0 <Int32>...    Int option values (Required)
".TrimStart());
        }

        [Fact]
        public void CommandHelp_Options_Generics_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(List<int>), "option0", new [] { 'o' }, "Int option values", CoconaDefaultValue.None),
                }
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName Test [--option0 <Int32>...]

command description

Options:
  -o, --option0 <Int32>...    Int option values (Required)
".TrimStart());
        }

        public enum CommandHelpEnumValue
        {
            Alice, Karen, Other
        }

        [Fact]
        public void CreateVersionHelp_VersionOnly()
        {
            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateVersionHelp();
            help.Children.Should().HaveCount(1);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be("ProductName 1.0.0.0" + Environment.NewLine);
        }

        [Fact]
        public void CreateVersionHelp_Version_Description()
        {
            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider() { Description = "Description of the application" }, new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateVersionHelp();
            help.Children.Should().HaveCount(1);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be("ProductName 1.0.0.0" + Environment.NewLine);
        }
    }
}
