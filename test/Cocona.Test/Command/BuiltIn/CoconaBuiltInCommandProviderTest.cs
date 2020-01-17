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
            commands.All[0].Name.Should().Be("a");
            commands.All[1].Name.Should().Be("b");
            commands.Primary.Should().NotBeNull();
        }

        [Fact]
        public void BuiltInOptionHelpAndVersion_BuiltInPrimaryCommand()
        {
            var provider = new CoconaBuiltInCommandProvider(new CoconaCommandProvider(new[] { typeof(CommandTestBuiltInPrimaryCommand) }));
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.Primary.Options.Should().HaveCount(2); // --help, --version
            commands.Primary.Options[0].Should().Be(BuiltInCommandOption.Help);
            commands.Primary.Options[1].Should().Be(BuiltInCommandOption.Version);
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
            commands.All[0].Options.Should().HaveCount(2); // --help, --version
            commands.All[0].Name.Should().Be("a_primaryhasversionoption");
            commands.All[0].Options[0].Should().NotBe(BuiltInCommandOption.Version); // User-implmented --version
            commands.All[0].Options[1].Should().Be(BuiltInCommandOption.Help);
            commands.All[1].Name.Should().Be("b_hasshorthelpoption");
            commands.All[1].Options.Should().HaveCount(1); // -h
            commands.All[1].Options[0].Should().NotBe(BuiltInCommandOption.Help);  // User-implmented -h
            commands.All[2].Name.Should().Be("c_haslonghelpoption");
            commands.All[2].Options.Should().HaveCount(1); // --help
            commands.All[2].Options[0].Should().NotBe(BuiltInCommandOption.Help);  // User-implmented --help
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
