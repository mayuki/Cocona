using System;
using System.Collections.Generic;
using System.Text;
using Cocona.Command;
using Xunit;

namespace Cocona.Test.Command.CommandProvider
{
    public class NestedSubCommandTest
    {
        [Fact]
        public void Test()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(TestCommand_TopLevel) });
            var commands = provider.GetCommandCollection();
        }

        class TestCommand_TopLevel
        {
            public void Hello()
            { }

            public void SubCommand()
            { }

            [SubCommands(typeof(TestCommand_NestedSubCommand))]
            public void Nested()
            { }
        }

        class TestCommand_NestedSubCommand
        {
            public void SubSubCommand1()
            { }
            public void SubSubCommand2()
            { }
        }

        class TestCommand_Single_TopLevel
        {
            public void Hello()
            { }

            public void SubCommand()
            { }

            [SubCommands(typeof(TestCommand_NestedSubCommand))]
            public void Nested()
            { }
        }

        class TestCommand_Single_NestedSubCommand
        {
            public void SubSubCommand1()
            { }
        }
    }
}
