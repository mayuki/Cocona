using System;
using System.Collections.Generic;
using System.Text;
using Cocona.Command;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.Command.CommandProvider
{
    public class NestedSubCommandTest
    {
        [Fact]
        public void GetCommandCollection()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_TopLevel) });
            var commands = provider.GetCommandCollection();
            commands.All.Should().HaveCount(3);
            commands.All[2].SubCommands.Should().NotBeNull(); // Hello, SubCommand, NestedSubCommand
            commands.All[2].Name.Should().Be("NestedSubCommand");
            commands.All[2].Flags.Should().Be(CommandFlags.SubCommandsEntryPoint);

            commands.All[2].SubCommands.Primary.Should().BeNull();
            commands.All[2].SubCommands.All.Should().HaveCount(2);
            commands.All[2].SubCommands.All[0].Name.Should().Be("SubSubCommand1");
        }

        [HasSubCommands(typeof(NestedSubCommand))]
        class TestCommand_TopLevel
        {
            public void Hello()
            { }

            public void SubCommand()
            { }

            class NestedSubCommand
            {
                public void SubSubCommand1()
                { }
                public void SubSubCommand2()
                { }
            }
        }

        [Fact]
        public void GetCommandCollection_Single()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_Single_TopLevel) });
            var commands = provider.GetCommandCollection();
            commands.All.Should().HaveCount(3);
            commands.All[2].SubCommands.Should().NotBeNull(); // Hello, SubCommand, NestedSubCommand
            commands.All[2].Name.Should().Be("NestedSubCommand");
            commands.All[2].Flags.Should().Be(CommandFlags.SubCommandsEntryPoint);

            commands.All[2].SubCommands.Primary.Should().NotBeNull();
            commands.All[2].SubCommands.All.Should().HaveCount(1);
            commands.All[2].SubCommands.All[0].Name.Should().Be("SubSubCommand1");
        }

        [HasSubCommands(typeof(NestedSubCommand))]
        class TestCommand_Single_TopLevel
        {
            public void Hello()
            { }

            public void SubCommand()
            { }

            class NestedSubCommand
            {
                public void SubSubCommand1()
                { }
            }
        }

        [Fact]
        public void GetCommandCollection_Deep()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_Deep) });
            var commands = provider.GetCommandCollection();
            commands.All.Should().HaveCount(3);
            commands.All[2].SubCommands.Should().NotBeNull(); // Hello, SubCommand, NestedSubCommand
            commands.All[2].Name.Should().Be("NestedSubCommand");
            commands.All[2].Flags.Should().Be(CommandFlags.SubCommandsEntryPoint);

            commands.All[2].SubCommands.Primary.Should().BeNull();
            commands.All[2].SubCommands.All.Should().HaveCount(3); // SubSubCommand1, SubSubCommand2, NestedSubCommand2
            commands.All[2].SubCommands.All[0].Name.Should().Be("SubSubCommand1");
            commands.All[2].SubCommands.All[1].Name.Should().Be("SubSubCommand2");
            commands.All[2].SubCommands.All[2].Name.Should().Be("NestedSubCommand2");
            commands.All[2].SubCommands.All[2].SubCommands.Should().NotBeNull();
            commands.All[2].SubCommands.All[2].SubCommands.All.Should().HaveCount(2); // SubSubSubCommand1, SubSubSubCommand2
            commands.All[2].SubCommands.All[2].SubCommands.All[0].Name.Should().Be("SubSubSubCommand1");
            commands.All[2].SubCommands.All[2].SubCommands.All[1].Name.Should().Be("SubSubSubCommand2");
        }

        [HasSubCommands(typeof(NestedSubCommand))]
        class TestCommand_Deep
        {
            public void Hello()
            { }

            public void SubCommand()
            { }

            [HasSubCommands(typeof(NestedSubCommand2))]
            class NestedSubCommand
            {
                public void SubSubCommand1()
                { }
                public void SubSubCommand2()
                { }

                class NestedSubCommand2
                {
                    public void SubSubSubCommand1()
                    { }
                    public void SubSubSubCommand2()
                    { }
                }
            }
        }

        [Fact]
        public void CommandName()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_CommandName) });
            var commands = provider.GetCommandCollection();
            commands.All.Should().HaveCount(3);
            commands.All[2].SubCommands.Should().NotBeNull(); // Hello, SubCommand, NestedSubCommand
            commands.All[2].Name.Should().Be("NESTED");
            commands.All[2].Flags.Should().Be(CommandFlags.SubCommandsEntryPoint);

            commands.All[2].SubCommands.Primary.Should().BeNull();
            commands.All[2].SubCommands.All.Should().HaveCount(2);
        }

        [HasSubCommands(typeof(NestedSubCommand), "NESTED")]
        class TestCommand_CommandName
        {
            public void Hello()
            { }

            public void SubCommand()
            { }

            class NestedSubCommand
            {
                public void SubSubCommand1()
                { }
                public void SubSubCommand2()
                { }
            }
        }

        [Fact]
        public void CommandDescription()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_CommandDescription) });
            var commands = provider.GetCommandCollection();
            commands.All.Should().HaveCount(3);
            commands.All[2].SubCommands.Should().NotBeNull(); // Hello, SubCommand, NestedSubCommand
            commands.All[2].Description.Should().Be("NESTED_DESC");
            commands.All[2].Flags.Should().Be(CommandFlags.SubCommandsEntryPoint);

            commands.All[2].SubCommands.Primary.Should().BeNull();
            commands.All[2].SubCommands.All.Should().HaveCount(2);
        }

        [HasSubCommands(typeof(NestedSubCommand), Description = "NESTED_DESC")]
        class TestCommand_CommandDescription
        {
            public void Hello()
            { }

            public void SubCommand()
            { }

            class NestedSubCommand
            {
                public void SubSubCommand1()
                { }
                public void SubSubCommand2()
                { }
            }
        }

        [Fact]
        public void SingleMethod_NotPrimary()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_SingleMethod_NotPrimary) });
            var commands = provider.GetCommandCollection();
            commands.Primary.Should().BeNull();
            commands.All[0].IsPrimaryCommand.Should().BeFalse();
            commands.All.Should().HaveCount(2);
            commands.All[1].SubCommands.Should().NotBeNull(); // Hello, SubCommand, NestedSubCommand
            commands.All[1].Name.Should().Be("NestedSubCommand");
            commands.All[1].Flags.Should().Be(CommandFlags.SubCommandsEntryPoint);
        }

        [HasSubCommands(typeof(NestedSubCommand))]
        class TestCommand_SingleMethod_NotPrimary
        {
            public void Hello()
            { }

            class NestedSubCommand
            {
                public void SubSubCommand1()
                { }
                public void SubSubCommand2()
                { }
            }
        }

        [Fact]
        public void HasManyNestedCommands()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_HasManyNestedCommands) });
            var commands = provider.GetCommandCollection();
            commands.All.Should().HaveCount(4);
            commands.All[2].SubCommands.Should().NotBeNull(); // Hello, SubCommand, NestedSubCommand
            commands.All[2].Name.Should().Be("nested1");
            commands.All[2].Flags.Should().Be(CommandFlags.SubCommandsEntryPoint);
            commands.All[3].SubCommands.Should().NotBeNull(); // Hello, SubCommand, NestedSubCommand
            commands.All[3].Name.Should().Be("nested2");
            commands.All[3].Flags.Should().Be(CommandFlags.SubCommandsEntryPoint);
        }

        [HasSubCommands(typeof(NestedSubCommand), "nested1")]
        [HasSubCommands(typeof(NestedSubCommand), "nested2")]
        class TestCommand_HasManyNestedCommands
        {
            public void Hello()
            { }

            public void SubCommand()
            { }

            class NestedSubCommand
            {
                public void SubSubCommand1()
                { }
                public void SubSubCommand2()
                { }
            }
        }
    }
}
