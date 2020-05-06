using Cocona.Command;
using Cocona.Command.BuiltIn;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Cocona.Test.Command.BuiltIn
{
    public class CoconaBuiltInCommandProviderTest
    {
        [Fact]
        public void BuiltInPrimaryCommand()
        {
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInPrimaryCommand) }));
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(3); // A, B, BuiltInPrimary
            commands.Primary.Should().NotBeNull();
        }

        [Fact]
        public void RewriteCommandNamesAsLowerCase()
        {
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInPrimaryCommand) }));
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(3); // A, B, BuiltInPrimary
            commands.All[0].Name.Should().Be("A");
            commands.All[1].Name.Should().Be("B");
            commands.Primary.Should().NotBeNull();
        }

        [Fact]
        public void BuiltInOptionHelpAndVersionAndCompletion_BuiltInPrimaryCommand()
        {
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInPrimaryCommand) }));
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.Primary.Options.Should().HaveCount(3); // --help, --version, --completion
            commands.Primary.Options[0].Should().Be(BuiltInCommandOption.Help);
            commands.Primary.Options[1].Should().Be(BuiltInCommandOption.Version);
            commands.Primary.Options[2].Should().Be(BuiltInCommandOption.Completion);
        }

        [Fact]
        public void BuiltInOptionHelp_NonPrimaryCommand()
        {
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInPrimaryCommand) }));
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All[0].Options.Should().HaveCount(1); // --help
            commands.Primary.Options[0].Should().Be(BuiltInCommandOption.Help);
        }


        [Fact]
        public void BuiltInOptionHelp_UserOptions()
        {
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInHelpUserOptionCommand) }));
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All[0].Options.Should().HaveCount(3); // --version, --help, --completion
            commands.All[0].Name.Should().Be("A_PrimaryHasVersionOption");
            commands.All[0].Options[0].Should().NotBe(BuiltInCommandOption.Version); // User-implemented --version
            commands.All[0].Options[1].Should().Be(BuiltInCommandOption.Help);
            commands.All[1].Name.Should().Be("B_HasShortHelpOption");
            commands.All[1].Options.Should().HaveCount(1); // -h
            commands.All[1].Options[0].Should().NotBe(BuiltInCommandOption.Help);  // User-implemented -h
            commands.All[2].Name.Should().Be("C_HasLongHelpOption");
            commands.All[2].Options.Should().HaveCount(1); // --help
            commands.All[2].Options[0].Should().NotBe(BuiltInCommandOption.Help);  // User-implemented --help
        }

        public class CommandTestBuiltInPrimaryCommand
        {
            public void A() { }
            public void B() { }
        }

        public class CommandTestBuiltInHelpUserOptionCommand
        {
            [PrimaryCommand]
            public void A_PrimaryHasVersionOption(bool version) { }
            public void B_HasShortHelpOption([Option('h')]bool _) { }
            public void C_HasLongHelpOption([Option]bool help) { }
        }
    }
}
