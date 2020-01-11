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
        public void NoCommand()
        {
            var provider = new CoconaCommandProvider();
            var commands = provider.GetCommandCollection(new[] { typeof(CommandTestNoCommand) });
            commands.Should().NotBeNull();
            commands.All.Should().BeEmpty();
        }

        [Fact]
        public void SingleCommand()
        {
            var provider = new CoconaCommandProvider();
            var commands = provider.GetCommandCollection(new[] { typeof(CommandTestSingleCommand) });
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
        }

        [Fact]
        public void MultipleMainCommand()
        {
            var provider = new CoconaCommandProvider();
            var commands = provider.GetCommandCollection(new[] { typeof(CommandTestMultipleMainCommand) });
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(2);
        }

        [Fact]
        public void IgnoreCommand_Method()
        {
            var provider = new CoconaCommandProvider();
            var commands = provider.GetCommandCollection(new[] { typeof(CommandTestIgnoreCommand_Method) });
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(2);
        }

        [Fact]
        public void IgnoreCommand_Class()
        {
            var provider = new CoconaCommandProvider();
            var commands = provider.GetCommandCollection(new[] { typeof(CommandTestIgnoreCommand_Class) });
            commands.Should().NotBeNull();
            commands.All.Should().BeEmpty();
        }

        [Fact]
        public void IgnoreCommand_Parameter()
        {
            var provider = new CoconaCommandProvider();
            var commands = provider.GetCommandCollection(new[] { typeof(CommandTestIgnoreCommand_Parameter) });
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.All[0].Options.Should().HaveCount(1);
        }

        [Fact]
        public void NonPublicCommand()
        {
            var provider = new CoconaCommandProvider();
            var commands = provider.GetCommandCollection(new[] { typeof(CommandTestNonPublicCommand) });
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
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
    }
}
