using Cocona.Application;
using Cocona.Command;
using Cocona.Help;
using Cocona.Help.DocumentModel;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Cocona.Test.Help
{
    public class CoconaCommandHelpProviderTransformTest
    {
        class FakeApplicationMetadataProvider : ICoconaApplicationMetadataProvider
        {
            public string ProductName { get; set; } = "ProductName";
            public string Description { get; set; } = string.Empty;
            public string GetDescription() => Description;
            public string GetExecutableName() => "ExeName";
            public string GetProductName() => ProductName;
            public string GetVersion() => "1.0.0.0";
        }

        class TestTransformer : ICoconaHelpTransformer
        {
            public void TransformHelp(HelpMessage helpMessage, CommandDescriptor command)
            {
                helpMessage.Children.Add(new HelpSection(new HelpHeading("Hello, Konnichiwa!")));
            }
        }


        class TestTransformHelpAttribute : TransformHelpAttribute
        {
            public override void TransformHelp(HelpMessage helpMessage, CommandDescriptor command)
            {
                helpMessage.Children.Add(new HelpSection(new HelpHeading("Hi!")));
            }
        }

        private CommandDescriptor CreateCommand<T>(string methodName, ICommandParameterDescriptor[] parameterDescriptors, bool isPrimaryCommand)
        {
            return new CommandDescriptor(
                typeof(T).GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance),
                default,
                methodName,
                Array.Empty<string>(),
                "command description",
                parameterDescriptors,
                parameterDescriptors.OfType<CommandOptionDescriptor>().ToArray(),
                parameterDescriptors.OfType<CommandArgumentDescriptor>().ToArray(),
                Array.Empty<CommandOverloadDescriptor>(),
                Array.Empty<CommandOptionLikeCommandDescriptor>(),
                isPrimaryCommand ? CommandFlags.Primary : CommandFlags.None,
                null
            );
        }

        private IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICoconaInstanceActivator, CoconaInstanceActivator>();
            return services.BuildServiceProvider();
        }

        [Fact]
        public void Transform_CreateCommandHelp()
        {
            var commandDescriptor = CreateCommand<TestCommand>(
                nameof(TestCommand.A),
                new ICommandParameterDescriptor[0],
                isPrimaryCommand: false
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), CreateServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor, Array.Empty<CommandDescriptor>());
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName A

command description

Hello, Konnichiwa!
".TrimStart());
        }

        [Fact]
        public void Transform_CreateCommandHelp_InheritedAttribute()
        {
            var commandDescriptor = CreateCommand<TestCommand_InheritedAttribute>(
                nameof(TestCommand_InheritedAttribute.A),
                new ICommandParameterDescriptor[0],
                isPrimaryCommand: false
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), CreateServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor, Array.Empty<CommandDescriptor>());
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName A

command description

Hi!
".TrimStart());
        }

        [Fact]
        public void Transform_CreateCommandsIndexHelp()
        {
            var commandDescriptor = CreateCommand<TestCommand>(
                nameof(TestCommand.A),
                new ICommandParameterDescriptor[0],
                isPrimaryCommand: true
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), CreateServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor }), Array.Empty<CommandDescriptor>());
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName

command description

Hello, Konnichiwa!
".TrimStart());
        }

        [Fact]
        public void Transform_CreateCommandsIndexHelp_Primary_Class()
        {
            var commandDescriptor = CreateCommand<TestCommand_Primary>(
                nameof(TestCommand_Primary.Default),
                new ICommandParameterDescriptor[0],
                isPrimaryCommand: true
            );
            var commandDescriptor1 = CreateCommand<TestCommand_Primary>(
                nameof(TestCommand_Primary.A),
                new ICommandParameterDescriptor[0],
                isPrimaryCommand: false
            );
            var commandDescriptor2 = CreateCommand<TestCommand_Primary>(
                nameof(TestCommand_Primary.B),
                new ICommandParameterDescriptor[0],
                isPrimaryCommand: false
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), CreateServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor, commandDescriptor1, commandDescriptor2 }), Array.Empty<CommandDescriptor>());
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [command]

command description

Commands:
  A    command description
  B    command description

Hello, Konnichiwa!
".TrimStart());
        }

        [Fact]
        public void Transform_CreateCommandsIndexHelp_Primary_Class_InheritedAttribute()
        {
            var commandDescriptor = CreateCommand<TestCommand_Primary_InheritedAttribute>(
                nameof(TestCommand_Primary_InheritedAttribute.Default),
                new ICommandParameterDescriptor[0],
                isPrimaryCommand: true
            );
            var commandDescriptor1 = CreateCommand<TestCommand_Primary_InheritedAttribute>(
                nameof(TestCommand_Primary_InheritedAttribute.A),
                new ICommandParameterDescriptor[0],
                isPrimaryCommand: false
            );
            var commandDescriptor2 = CreateCommand<TestCommand_Primary_InheritedAttribute>(
                nameof(TestCommand_Primary_InheritedAttribute.B),
                new ICommandParameterDescriptor[0],
                isPrimaryCommand: false
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), CreateServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor, commandDescriptor1, commandDescriptor2 }), Array.Empty<CommandDescriptor>());
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [command]

command description

Commands:
  A    command description
  B    command description

Hi!
".TrimStart());
        }

        class TestCommand
        {
            [TransformHelpFactory(typeof(TestTransformer))]
            public void A() { }
        }

        class TestCommand_InheritedAttribute
        {
            [TestTransformHelp]
            public void A() { }
        }

        [TransformHelpFactory(typeof(TestTransformer))]
        class TestCommand_Primary
        {
            [PrimaryCommand]
            public void Default() { }
            public void A() { }
            public void B() { }
        }

        [TestTransformHelp]
        class TestCommand_Primary_InheritedAttribute
        {
            [PrimaryCommand]
            public void Default() { }
            public void A() { }
            public void B() { }
        }
    }
}
