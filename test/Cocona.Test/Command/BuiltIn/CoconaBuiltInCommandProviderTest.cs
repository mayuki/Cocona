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
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInPrimaryCommand) }), true);
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(3); // A, B, BuiltInPrimary
            commands.Primary.Should().NotBeNull();
        }

        [Fact]
        public void RewriteCommandNamesAsLowerCase()
        {
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInPrimaryCommand) }), true);
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
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInPrimaryCommand) }), true);
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.Primary.Options.Should().HaveCount(0);
            commands.Primary.OptionLikeCommands.Should().HaveCount(4); // --completion-candidates, --completion, --help, --version
            commands.Primary.OptionLikeCommands[0].Should().Be(BuiltInOptionLikeCommands.CompletionCandidates); // CompletionCandidates option must be first.
            commands.Primary.OptionLikeCommands[1].Should().Be(BuiltInOptionLikeCommands.Completion);
            commands.Primary.OptionLikeCommands[2].Should().Be(BuiltInOptionLikeCommands.Help);
            commands.Primary.OptionLikeCommands[3].Should().Be(BuiltInOptionLikeCommands.Version);
        }

        [Fact]
        public void DisableShellCompletionSupport_BuiltInOptionHelpAndVersion_BuiltInPrimaryCommand()
        {
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInPrimaryCommand) }), false);
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.Primary.Options.Should().HaveCount(0);
            commands.Primary.OptionLikeCommands.Should().HaveCount(2); // --help, --version
            commands.Primary.OptionLikeCommands[0].Should().Be(BuiltInOptionLikeCommands.Help);
            commands.Primary.OptionLikeCommands[1].Should().Be(BuiltInOptionLikeCommands.Version);
        }

        [Fact]
        public void BuiltInOptionHelp_NonPrimaryCommand()
        {
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInPrimaryCommand) }), true);
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All[0].Options.Should().HaveCount(0);
            commands.All[0].OptionLikeCommands.Should().HaveCount(1); // --help
            commands.All[0].OptionLikeCommands[0].Should().Be(BuiltInOptionLikeCommands.Help);
        }

        [Fact]
        public void BuiltInOptionHelp_UserOptions()
        {
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInHelpUserOptionCommand) }), true);
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All[0].Options.Should().HaveCount(1); // User-implemented --version
            commands.All[0].OptionLikeCommands.Should().HaveCount(3); // --completion-candidates, --completion, --help
            commands.All[0].Name.Should().Be("A_PrimaryHasVersionOption");
            commands.All[0].Options[0].Should().NotBe(BuiltInOptionLikeCommands.Version); // User-implemented --version
            commands.All[1].Name.Should().Be("B_HasShortHelpOption");
            commands.All[1].OptionLikeCommands.Should().HaveCount(0); // -h
            commands.All[1].Options[0].Should().NotBe(BuiltInOptionLikeCommands.Help);  // User-implemented -h
            commands.All[2].Name.Should().Be("C_HasLongHelpOption");
            commands.All[2].OptionLikeCommands.Should().HaveCount(0); // --help
            commands.All[2].Options[0].Should().NotBe(BuiltInOptionLikeCommands.Help);  // User-implemented --help
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
