using Cocona.Application;
using Cocona.Command;
using Cocona.Help;
using Cocona.Help.DocumentModel;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

        [TransformHelp(typeof(TestTransformer))]
        private void __Dummy_Transform() { }

        class TestTransformer : ICoconaHelpTransformer
        {
            public void TransformHelp(HelpMessage helpMessage, CommandDescriptor command)
            {
                helpMessage.Children.Add(new HelpSection(new HelpHeading("Hello, Konnichiwa!")));
            }
        }

        [Fact]
        public void Transform_CreateCommandHelp()
        {
            var commandDescriptor = new CommandDescriptor(
                typeof(CoconaCommandHelpProviderTransformTest).GetMethod(nameof(CoconaCommandHelpProviderTransformTest.__Dummy_Transform), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance),
                "Test",
                Array.Empty<string>(),
                "command description",
                new CommandParameterDescriptor[0]
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName Test

command description

Hello, Konnichiwa!
".TrimStart());
        }


        [Fact]
        public void Transform_CreateCommandsIndexHelp()
        {
            var commandDescriptor = new CommandDescriptor(
                typeof(CoconaCommandHelpProviderTransformTest).GetMethod(nameof(CoconaCommandHelpProviderTransformTest.__Dummy_Transform), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance),
                "Test",
                Array.Empty<string>(),
                "command description",
                new CommandParameterDescriptor[0],
                isPrimaryCommand:true
            );

            var provider = new CoconaCommandHelpProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
            var help = provider.CreateCommandsIndexHelp(new CommandCollection(new[] { commandDescriptor }));
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName [command]

Hello, Konnichiwa!
".TrimStart());
        }
    }
}
