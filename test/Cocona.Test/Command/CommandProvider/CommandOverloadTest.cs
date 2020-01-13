using Cocona.Command;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cocona.Test.Command.CommandProvider
{
    public class CommandOverloadTest
    {
        [Fact]
        public void Single()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_Overload_Single) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.Primary.Should().NotBeNull();
            commands.Primary.Overloads.Should().HaveCount(2);
        }

        [Fact]
        public void Multiple()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_Overload_Multiple) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(2);
            commands.Primary.Should().BeNull();
            commands.All[0].Overloads.Should().HaveCount(2);
        }

        [Fact]
        public void Multiple_UnknownOption()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_Overload_Multiple_UnknownOption) });
            Assert.Throws<CoconaException>(() => provider.GetCommandCollection());
        }

        class TestCommand_Overload_Single
        {
            public void Command(string mode, bool opt0, [Argument]string arg0) => throw new NotImplementedException();

            [CommandOverload(nameof(Command), "mode", "network")]
            public void Command_Mode_Network(string mode, int port, string host) => throw new NotImplementedException();

            [CommandOverload(nameof(Command), "mode", "extra")]
            public void Command_Mode_Extra(string mode, bool extraOption0) => throw new NotImplementedException();
        }


        class TestCommand_Overload_Multiple
        {
            public void CommandA(string mode, bool opt0, [Argument]string arg0) => throw new NotImplementedException();
            public void CommandB(string mode, bool opt0, [Argument]string arg0) => throw new NotImplementedException();

            [CommandOverload(nameof(CommandA), "mode", "network")]
            public void CommandA_Mode_Network(string mode, int port, string host) => throw new NotImplementedException();

            [CommandOverload(nameof(CommandA), "mode", "extra")]
            public void CommandA_Mode_Extra(string mode, bool extraOption0) => throw new NotImplementedException();
        }

        class TestCommand_Overload_Multiple_UnknownOption
        {
            public void CommandA(string mode, bool opt0, [Argument]string arg0) => throw new NotImplementedException();

            [CommandOverload(nameof(CommandA), "xxxx", "hello")]
            public void CommandA_Mode_Network(string mode, int port, string host) => throw new NotImplementedException();

            [CommandOverload(nameof(CommandA), "yyyy", "konnichiwa")]
            public void CommandA_Mode_Extra(string mode, bool extraOption0) => throw new NotImplementedException();
        }
    }
}
