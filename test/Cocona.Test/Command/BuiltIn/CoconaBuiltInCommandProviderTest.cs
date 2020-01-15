using Cocona.Command;
using Cocona.Command.BuiltIn;
using FluentAssertions;
using System;
using System.Collections.Generic;
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

        public class CommandTestBuiltInPrimaryCommand
        {
            public void A() { }
            public void B() { }
        }
    }
}
