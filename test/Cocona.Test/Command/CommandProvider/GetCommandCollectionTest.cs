using Cocona.Command;
using Cocona.CommandLine;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;


namespace Cocona.Test.Command.CommandProvider
{
    public class GetCommandCollectionTest
    {
        [Fact]
        public void HasProperty()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestHasProperty) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
        }

        [Fact]
        public void NoCommand()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestNoCommand) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().BeEmpty();
            commands.Primary.Should().BeNull();
        }

        [Fact]
        public void SingleCommand()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestSingleCommand) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.Primary.Should().NotBeNull();
        }

        [Fact]
        public void MultipleMainCommand_BuiltInPrimary()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestMultipleMainCommand) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(2); // A, B
            commands.Primary.Should().BeNull();
        }

        [Fact]
        public void IgnoreCommand_Method()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestIgnoreCommand_Method) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(2); // A, C
        }

        [Fact]
        public void IgnoreCommand_Class()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestIgnoreCommand_Class) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().BeEmpty();
        }

        [Fact]
        public void IgnoreCommand_Parameter()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestIgnoreCommand_Parameter) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.All[0].Options.Should().HaveCount(1);
        }

        [Fact]
        public void NonPublicCommand()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestNonPublicCommand) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
        }

        [Fact]
        public void PrimaryCommand()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestPrimaryCommand) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(2);
            commands.All[0].Name.Should().Be("A");
            commands.All[0].IsPrimaryCommand.Should().BeTrue();
            commands.All[1].Name.Should().Be("B");
            commands.All[1].IsPrimaryCommand.Should().BeFalse();

            commands.Primary.Should().NotBeNull();
        }

        [Fact]
        public void PrimaryCommand_Argument()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestPrimaryCommand_Argument) });
            var ex = Assert.Throws<CoconaException>(() => provider.GetCommandCollection());
        }

        [Fact]
        public void PrimaryCommand_Multiple()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestPrimaryCommand_Multiple) });
            var ex = Assert.Throws<CoconaException>(() => provider.GetCommandCollection());
        }

        [Fact]
        public void DefaultPrimaryCommand_Argument()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestDefaultPrimaryCommand_Argument) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.Primary.Should().NotBeNull();
        }

        public class CommandTestDefaultPrimaryCommand_Argument
        {
            public void A([Argument]string[] args) { }
        }

        public class CommandTestHasProperty
        {
            public void A() { }
            public string B => string.Empty;
        }

        public class CommandTestNoCommand
        {
        }
        
        public class CommandTestSingleCommand
        {
            public void A(string name) { }
        }

        public class CommandTestMultipleMainCommand
        {
            public void A(string name) { }
            public void B(string name) { }
        }

        [Ignore]
        public class CommandTestIgnoreCommand_Class
        {
            public void A(string name) { }
            public void B(string name) { }
            public void C(string name) { }
        }

        public class CommandTestIgnoreCommand_Method
        {
            public void A(string name) { }
            [Ignore]
            public void B(string name) { }
            public void C(string name) { }
        }

        public class CommandTestIgnoreCommand_Parameter
        {
            public void A(string name, [Ignore]int ignored) { }
        }

        public class CommandTestNonPublicCommand
        {
            [Command]
            private void A(string name, int ignored) { }
        }

        public class CommandTestPrimaryCommand
        {
            [PrimaryCommand]
            public void A([Option]string name) { }

            public void B([Argument]string name) { }
        }

        public class CommandTestPrimaryCommand_Argument
        {
            [PrimaryCommand]
            public void A([Argument]string name) { }
            public void B([Argument]string name) { }
        }

        public class CommandTestPrimaryCommand_Multiple
        {
            [PrimaryCommand]
            public void A(string name) { }
            [PrimaryCommand]
            public void B(string name) { }
        }
    }
}
