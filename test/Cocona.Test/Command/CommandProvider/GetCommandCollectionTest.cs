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
        public void SingleCommand_Description()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestSingleCommand_Description) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.Primary.Should().NotBeNull();
            commands.Primary.Description.Should().Be("Description of A");
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

        [Fact]
        public void DuplicateSameNameCommand()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestDuplicateSameNameCommand) });
            var ex = Assert.Throws<CoconaException>(() => provider.GetCommandCollection());
        }

        [Fact]
        public void DuplicateSameNameInAliasCommand()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestDuplicateSameNameInAliasCommand) });
            var ex = Assert.Throws<CoconaException>(() => provider.GetCommandCollection());
        }

        [Fact]
        public void DontTreatPublicMethodsAsCommand()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestDontTreatPublicMethodsAsCommands) }, treatPublicMethodsAsCommands: false);
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.Primary.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.All[0].Name.Should().Be("B");
        }

        [Fact]
        public void DontTreatPublicMethodsAsCommand_Primary()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTestDontTreatPublicMethodsAsCommands_Primary) }, treatPublicMethodsAsCommands: false);
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.Primary.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.All[0].Name.Should().Be("B");
        }

        [Fact]
        public void Hidden_Command()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTest_HiddenCommand) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.Primary.Should().BeNull();
            commands.All.Should().HaveCount(2);
            commands.All[0].Name.Should().Be("A");
            commands.All[0].Flags.Should().HaveFlag(CommandFlags.None);
            commands.All[0].IsHidden.Should().BeFalse();
            commands.All[1].Name.Should().Be("B");
            commands.All[1].Flags.Should().HaveFlag(CommandFlags.Hidden);
            commands.All[1].IsHidden.Should().BeTrue();
        }

        [Fact]
        public void Hidden_Option()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTest_HiddenOption) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.All[0].Name.Should().Be("A");
            commands.All[0].Options[0].Flags.Should().HaveFlag(CommandOptionFlags.Hidden);
            commands.All[0].Options[1].Flags.Should().HaveFlag(CommandOptionFlags.None);
        }

        [Fact]
        public void Static_SingleCommand()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTest_Static_SingleCommand) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1); // If the method has no "Command" attribute, it will be ignored.
            commands.All[0].Name.Should().Be("A");
        }

        [Fact]
        public void Static_MultipleCommands()
        {
            var provider = new CoconaCommandProvider(new[] { typeof(CommandTest_Static_MultipleCommands) });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(2); // If the method has no "Command" attribute, it will be ignored.
            commands.All[0].Name.Should().Be("A");
            commands.All[0].Method.IsStatic.Should().BeTrue();
            commands.All[1].Name.Should().Be("B");
            commands.All[1].Method.IsStatic.Should().BeTrue();
        }

        [Fact]
        public void Delegate_Static_SingleCommand()
        {
            Func<bool, bool, int> methodA = CommandTest_Static_MultipleCommands.A;
            var provider = new CoconaCommandProvider(Array.Empty<Type>(), new[] { methodA });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.All[0].Name.Should().Be("A");
        }

        [Fact]
        public void Delegate_Static_MultipleCommands()
        {
            Func<bool, bool, int> methodA = CommandTest_Static_MultipleCommands.A;
            Action methodB = CommandTest_Static_MultipleCommands.B;
            var provider = new CoconaCommandProvider(Array.Empty<Type>(), new[] { (Delegate)methodA, methodB });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(2);
            commands.All[0].Name.Should().Be("A");
            commands.All[1].Name.Should().Be("B");
        }

        [Fact]
        public void Delegate()
        {
            Action<string> methodA = new CommandTestSingleCommand().A;
            var provider = new CoconaCommandProvider(Array.Empty<Type>(), new[] { methodA });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
            commands.All[0].Name.Should().Be("A");
            commands.All[0].CommandType.Should().Be<CommandTestSingleCommand>();
        }

        [Fact]
        public void Delegate_Unnamed_Single()
        {
            Action<string> methodA = (string name) => {};
            var provider = new CoconaCommandProvider(Array.Empty<Type>(), new[] { methodA });
            var commands = provider.GetCommandCollection();
            commands.Should().NotBeNull();
            commands.All.Should().HaveCount(1);
        }
        
        [Fact]
        public void Delegate_Unnamed_Multiple()
        {
            Action<string> methodA = (string name) => {};
            Action<string> methodB = (string name) => {};
            var provider = new CoconaCommandProvider(Array.Empty<Type>(), new[] { methodA, methodB });
            Assert.Throws<CoconaException>(() => provider.GetCommandCollection());
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

        public class CommandTestSingleCommand_Description
        {
            [Command(Description = "Description of A")]
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

        public class CommandTestDuplicateSameNameCommand
        {
            public void A() { }
            [Command("a")]
            public void B() { }
        }

        public class CommandTestDuplicateSameNameInAliasCommand
        {
            public void A() { }
            [Command("B", Aliases = new [] { "A" })]
            public void B() { }
        }

        public class CommandTestDontTreatPublicMethodsAsCommands
        {
            public void A() { }

            [Command]
            public void B() { }
        }

        public class CommandTestDontTreatPublicMethodsAsCommands_Primary
        {
            public void A() { }

            [PrimaryCommand]
            public void B() { }
        }

        public class CommandTestDontTreatPublicMethodsAsCommands_Multiple
        {
            public void A() { }

            [Command]
            public void B() { }

            [Command]
            public void C() { }
        }

        public class CommandTest_HiddenCommand
        {
            public void A() { }

            [Hidden]
            public void B() { }
        }

        public class CommandTest_HiddenOption
        {
            public void A([Hidden]bool option0, bool option1) { }
        }

        public class CommandTest_Static_SingleCommand
        {
            [Command]
            public static int A(bool option0, bool option1) => 0;

            public static int NoCommandAttribute() => 0;
        }

        public class CommandTest_Static_MultipleCommands
        {
            [Command]
            public static int A(bool option0, bool option1) => 0;
            [Command]
            public static void B() { }

            public static int NoCommandAttribute() => 0;
        }
    }
}
