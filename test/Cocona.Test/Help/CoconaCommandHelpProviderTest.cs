using Cocona.Application;
using Cocona.Command;
using Cocona.Help;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private CommandDescriptor CreateCommand(string name, string description, CommandParameterDescriptor[] parameterDescriptors, bool isPrimaryCommand = false)
        {
            return new CommandDescriptor(
                typeof(CoconaCommandHelpProviderTest).GetMethod(nameof(CoconaCommandHelpProviderTest.__Dummy), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance),
                name,
                Array.Empty<string>(),
                description,
                parameterDescriptors,
                Array.Empty<CommandOverloadDescriptor>(),
                isPrimaryCommand
            );
        }
        private CommandOptionDescriptor CreateCommandOption(Type optionType, string name, IReadOnlyList<char> shortName, string description, CoconaDefaultValue defaultValue)
        {
            return new CommandOptionDescriptor(optionType, name, shortName, description, defaultValue, null);
        }

        [Fact]
        public void CommandHelp1()
        {
            // void Test(string arg0, string arg1, string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2" }
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new CommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "option0", Array.Empty<char>(), "option description - option0", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "option1", Array.Empty<char>(), "option description - option1", CoconaDefaultValue.None),
                    new CommandIgnoredParameterDescriptor(typeof(bool), true),
                    new CommandServiceParameterDescriptor(typeof(bool)),
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "description - arg0", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "description - arg1", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "description - arg2", CoconaDefaultValue.None),
                }
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
        }

        [Fact]
        public void CommandHelp_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new CommandParameterDescriptor[]
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
                new CommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "src files", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "dest dir", CoconaDefaultValue.None),
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
                new CommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                },
                isPrimaryCommand: true
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
        public void CreateCommandsIndexHelp_Commands_Rendered()
        {
            var commandDescriptor = CreateCommand(
                "Test",
                "command description",
                new CommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(int), "bar", new [] { 'b' }, "has default value", new CoconaDefaultValue(123)),
                },
                isPrimaryCommand: true
            );
            var commandDescriptor2 = CreateCommand(
                "Test2",
                "command description",
                new CommandParameterDescriptor[0],
                isPrimaryCommand: false
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [command]
Usage: ExeName [--foo <String>] [--looooooong-option] [--bar <Int32>]

Commands:
  Test2    command description

Options:
  -f, --foo <String>         Foo option (Required)
  -l, --looooooong-option    Long name option
  -b, --bar <Int32>          has default value (DefaultValue: 123)
".TrimStart());
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
