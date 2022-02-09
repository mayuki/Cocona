using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Builder;
using Cocona.Builder.Metadata;
using Cocona.Command;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.Command.CommandProvider
{
    public class GetCommandCollectionFromCommandDataTest
    {
        [Fact]
        public void Complex()
        {
            void TestMethod() { }

            var provider = new CoconaCommandProvider(new ICommandData[]
            {
                new DelegateCommandData(new Action(TestMethod).Method, this, new[] { new CommandNameMetadata("TestMethod") }),
                new TypeCommandData(typeof(CommandTestSingleCommand), Array.Empty<object>()),
            });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(2);
            collection.All[0].Name.Should().Be("TestMethod");
            collection.All[1].Name.Should().Be("A");
        }

        [Fact]
        public void SubCommands()
        {
            void TestMethod() { }

            var provider = new CoconaCommandProvider(new ICommandData[]
            {
                new SubCommandData(new ICommandData[]
                {
                    new DelegateCommandData(new Action(TestMethod).Method, this, new[] { new CommandNameMetadata("TestMethod") }),
                    new TypeCommandData(typeof(CommandTestSingleCommand), Array.Empty<object>()),
                }, new [] { new CommandNameMetadata("sub-command") }),
            });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(1);
            collection.All[0].Name.Should().Be("sub-command");
            collection.All[0].SubCommands.All.Should().HaveCount(2);
            collection.All[0].SubCommands.All[0].Name.Should().Be("TestMethod");
            collection.All[0].SubCommands.All[1].Name.Should().Be("A");
        }

        [Fact]
        public void DelegateCommandData()
        {
            void TestMethod() { }

            var provider = new CoconaCommandProvider(new[] { new DelegateCommandData(new Action(TestMethod).Method, this, new[] { new CommandNameMetadata("TestMethod") }) });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(1);
            collection.All[0].Name.Should().Be("TestMethod");
        }

        [Fact]
        public void DelegateCommandData_Static()
        {
            var provider = new CoconaCommandProvider(new[] { new DelegateCommandData(new Action<string>(CommandTest_Static.A).Method, null, new[] { new CommandNameMetadata("A") }) });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(1);
            collection.All[0].Name.Should().Be("A");
            collection.All[0].Target.Should().BeNull();
            collection.All[0].Method.IsStatic.Should().BeTrue();
        }

        [Fact]
        public void TypeCommandData_SingleCommand()
        {
            var provider = new CoconaCommandProvider(new[] { new TypeCommandData(typeof(CommandTestSingleCommand), Array.Empty<object>()) });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(1);
            collection.All[0].Name.Should().Be("A");
        }

        [Fact]
        public void TypeCommandData_PrimaryCommand_Duplicate()
        {
            var provider = new CoconaCommandProvider(new[]
            {
                new TypeCommandData(typeof(CommandTestSingleCommand), new object[] { new CommandNameMetadata("A"), new PrimaryCommandAttribute() }),
                new TypeCommandData(typeof(CommandTestSingleCommand), new object[] { new CommandNameMetadata("B"), new PrimaryCommandAttribute() }),
            });

            Assert.Throws<CoconaException>(() => provider.GetCommandCollection()).Message.Should().Contain("The commands contain more than one primary command.");
        }

        [Fact]
        public void TypeCommandData_MultipleCommands()
        {
            var provider = new CoconaCommandProvider(new[] { new TypeCommandData(typeof(CommandTestMultipleCommand), Array.Empty<object>()) });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(2);
            collection.All[0].Name.Should().Be("A");
            collection.All[1].Name.Should().Be("B");
        }

        [Fact]
        public void TypeCommandData_MultipleTypes()
        {
            var provider = new CoconaCommandProvider(new[] { new TypeCommandData(typeof(CommandTestSingleCommand), Array.Empty<object>()), new TypeCommandData(typeof(CommandTestSingleCommand2), Array.Empty<object>()) });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(2);
            collection.All[0].Name.Should().Be("A");
            collection.All[1].Name.Should().Be("A2");
        }

        [Fact]
        public void TypeCommandData_HasSameNameCommands()
        {
            var provider = new CoconaCommandProvider(new[] { new TypeCommandData(typeof(CommandTestSingleCommand), Array.Empty<object>()), new TypeCommandData(typeof(CommandTestMultipleCommand), Array.Empty<object>()) });
            Assert.Throws<CoconaException>(() => provider.GetCommandCollection());
        }

        [Fact]
        public void OptionLikeCommand()
        {
            void TestMethod() { }
            var provider = new CoconaCommandProvider(new[]
            {
                new DelegateCommandData(new Action(TestMethod).Method, this, new object[]
                {
                    new CommandNameMetadata("TestMethod"),
                    new OptionLikeCommandMetadata(
                        new OptionLikeDelegateCommandData(new [] { 'i' }, new Action(TestMethod).Method, null, new object[] { new CommandNameMetadata("info"), })),
                    new OptionLikeCommandMetadata(
                        new OptionLikeDelegateCommandData(new [] { 'f' }, new Action(TestMethod).Method, null, new object[] { new CommandNameMetadata("foo"), new CommandDescriptionMetadata("Foo-Description") })),
                    new OptionLikeCommandMetadata(
                        new OptionLikeDelegateCommandData(new [] { 'f' }, new Action(TestMethod).Method, null, new object[] { new CommandNameMetadata("hidden"), new HiddenAttribute() })),
                })
            });
            var collection = provider.GetCommandCollection();
            collection.All[0].OptionLikeCommands.Should().HaveCount(3);
            collection.All[0].OptionLikeCommands[0].Name.Should().Be("info");
            collection.All[0].OptionLikeCommands[1].Name.Should().Be("foo");
            collection.All[0].OptionLikeCommands[1].Description.Should().Be("Foo-Description");
            collection.All[0].OptionLikeCommands[2].Name.Should().Be("hidden");
            collection.All[0].OptionLikeCommands[2].Flags.Should().HaveFlag(CommandOptionFlags.Hidden);
        }

        public static class CommandTest_Static
        {
            public static void A(string name) { }
        }
        public class CommandTestSingleCommand
        {
            public void A(string name) { }
        }
        public class CommandTestSingleCommand2
        {
            public void A2(string name) { }
        }
        public class CommandTestMultipleCommand
        {
            public void A(string name) { }
            public void B(string name) { }

            [Ignore]
            public void C(string name) { }
        }
    }
}
