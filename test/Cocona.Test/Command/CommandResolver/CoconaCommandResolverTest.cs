using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Cocona.Command;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher;
using Cocona.CommandLine;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.Command.CommandResolver
{
    public class CoconaCommandResolverTest
    {
        [Fact]
        public void Empty()
        {
            var commandCollection = new CommandCollection(new CommandDescriptor[] { });
            var resolver = new CoconaCommandResolver(
                 new CoconaCommandLineParser(),
                 new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(commandCollection, new string[] { });
            resolve.Success.Should().BeFalse();
        }

        [Fact]
        public void Single()
        {
            var commandCollection = new CommandCollection(new CommandDescriptor[]
            {
                CreateCommand("Primary", new ICommandParameterDescriptor[]{}, true),
            });
            var resolver = new CoconaCommandResolver(
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(commandCollection, new string[] { });
            resolve.Success.Should().BeTrue();
            resolve.MatchedCommand.Name.Should().Be("Primary");
        }

        [Fact]
        public void Single_NoPrimary()
        {
            var commandCollection = new CommandCollection(new CommandDescriptor[]
            {
                CreateCommand("Single", new ICommandParameterDescriptor[]{}, false),
            });
            var resolver = new CoconaCommandResolver(
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            Assert.Throws<CommandNotFoundException>(() => resolver.ParseAndResolve(commandCollection, new string[] { }));
        }

        [Fact]
        public void Multiple()
        {
            var commandCollection = new CommandCollection(new CommandDescriptor[]
            {
                CreateCommand("Primary", new ICommandParameterDescriptor[]{}, true),
                CreateCommand("Foo", new ICommandParameterDescriptor[]{}, false),
                CreateCommand("Bar", new ICommandParameterDescriptor[]{}, false),
            });
            var resolver = new CoconaCommandResolver(
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(commandCollection, new string[] { "Foo" });
            resolve.Success.Should().BeTrue();
            resolve.MatchedCommand.Name.Should().Be("Foo");
        }

        [Fact]
        public void Multiple_1()
        {
            var commandCollection = new CommandCollection(new CommandDescriptor[]
            {
                CreateCommand("Primary", new ICommandParameterDescriptor[]{}, true),
                CreateCommand("Foo", new ICommandParameterDescriptor[]
                {
                    new CommandOptionDescriptor(
                        typeof(int),
                        "opt1",
                        Array.Empty<char>(),
                        "",
                        CoconaDefaultValue.None,
                        null,
                        CommandOptionFlags.None,
                        Array.Empty<Attribute>()
                    ),
                }, false),
                CreateCommand("Bar", new ICommandParameterDescriptor[]{}, false),
            });
            var resolver = new CoconaCommandResolver(
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(commandCollection, new string[] { "Foo", "--opt1", "123" });
            resolve.Success.Should().BeTrue();
            resolve.MatchedCommand.Name.Should().Be("Foo");
            resolve.ParsedCommandLine.Options.Should().HaveCount(1);
        }

        [Fact]
        public void OptionLikeCommand_Single_Primary_1()
        {
            var optLikeCmd1 = CreateOptionLikeCommand("optlikecmd1", Array.Empty<char>(), "OptLikeCmd1");
            var optLikeCmd2 = CreateOptionLikeCommand("optlikecmd2", Array.Empty<char>(), "OptLikeCmd2");

            var commandCollection = new CommandCollection(new CommandDescriptor[]
            {
                CreateCommand("Primary", new ICommandParameterDescriptor[]{},
                    new CommandOptionLikeCommandDescriptor[]
                    {
                        optLikeCmd1,
                    },
                    true),
            });
            var resolver = new CoconaCommandResolver(
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(commandCollection, new string[] { "--optlikecmd1", "Foo", "--opt1", "123", "--help", "--version" });
            resolve.Success.Should().BeTrue();
            resolve.CommandCollection.Should().Be(commandCollection);
            resolve.MatchedCommand.Name.Should().Be("OptLikeCmd1");
            resolve.ParsedCommandLine.Options.Should().BeEmpty();
            resolve.ParsedCommandLine.UnknownOptions.Should().HaveCount(3);
        }

        [Fact]
        public void OptionLikeCommand_Multiple_Primary_1()
        {
            var optLikeCmd1 = CreateOptionLikeCommand("optlikecmd1", Array.Empty<char>(), "OptLikeCmd1");
            var optLikeCmd2 = CreateOptionLikeCommand("optlikecmd2", Array.Empty<char>(), "OptLikeCmd2");

            var commandCollection = new CommandCollection(new CommandDescriptor[]
            {
                CreateCommand("Primary", new ICommandParameterDescriptor[]{},
                    new CommandOptionLikeCommandDescriptor[]
                    {
                        optLikeCmd1,
                    },
                    true),
                CreateCommand("Foo", new ICommandParameterDescriptor[]
                    {
                        new CommandOptionDescriptor(
                            typeof(int),
                            "opt1",
                            Array.Empty<char>(),
                            "",
                            CoconaDefaultValue.None,
                            null,
                            CommandOptionFlags.None,
                            Array.Empty<Attribute>()
                        ),
                    }, new CommandOptionLikeCommandDescriptor[]
                    {
                        optLikeCmd2
                    },
                    false),
                CreateCommand("Bar", new ICommandParameterDescriptor[]{}, false),
            });
            var resolver = new CoconaCommandResolver(
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(commandCollection, new string[] { "--optlikecmd1", "Foo", "--opt1", "123", "--help", "--version" });
            resolve.Success.Should().BeTrue();
            resolve.CommandCollection.Should().Be(commandCollection);
            resolve.MatchedCommand.Name.Should().Be("OptLikeCmd1");
            resolve.ParsedCommandLine.Options.Should().BeEmpty();
            resolve.ParsedCommandLine.UnknownOptions.Should().HaveCount(3);
            resolve.SubCommandStack.Should().HaveCount(1);
        }

        [Fact]
        public void OptionLikeCommand_Multiple_SubCommand_1()
        {
            var optLikeCmd1 = CreateOptionLikeCommand("optlikecmd1", Array.Empty<char>(), "OptLikeCmd1");
            var optLikeCmd2 = CreateOptionLikeCommand("optlikecmd2", Array.Empty<char>(), "OptLikeCmd2");
            var commandCollection = new CommandCollection(new CommandDescriptor[]
            {
                CreateCommand("Primary", new ICommandParameterDescriptor[]{}, true),
                CreateCommand("Foo", new ICommandParameterDescriptor[]
                    {
                        new CommandOptionDescriptor(
                            typeof(int),
                            "opt1",
                            Array.Empty<char>(),
                            "",
                            CoconaDefaultValue.None,
                            null,
                            CommandOptionFlags.None,
                            Array.Empty<Attribute>()
                        ),
                    }, new CommandOptionLikeCommandDescriptor[]
                    {
                        optLikeCmd1,
                        optLikeCmd2,
                    },
                    false),
                CreateCommand("Bar", new ICommandParameterDescriptor[]{}, false),
            });
            var resolver = new CoconaCommandResolver(
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(commandCollection, new string[] { "Foo", "--opt1", "123", "--optlikecmd1", "--version" });
            resolve.Success.Should().BeTrue();
            resolve.CommandCollection.Should().Be(commandCollection);
            resolve.MatchedCommand.Name.Should().Be("OptLikeCmd1");
            resolve.ParsedCommandLine.Options.Should().BeEmpty();
            resolve.ParsedCommandLine.UnknownOptions.Should().HaveCount(1);
            resolve.SubCommandStack.Should().HaveCount(1);
        }

        [Fact]
        public void OptionLikeCommand_Multiple_SubCommand_Nested()
        {
            var optLikeCmd1 = CreateOptionLikeCommand("optlikecmd1", Array.Empty<char>(), "OptLikeCmd1");
            var optLikeCmd2 = CreateOptionLikeCommand("optlikecmd2", Array.Empty<char>(), "OptLikeCmd2");
            var commandCollectionNested = new CommandCollection(new CommandDescriptor[]
            {
                CreateCommand("Primary", new ICommandParameterDescriptor[]{}, true),
                CreateCommand("Level2", new ICommandParameterDescriptor[] { },
                    new CommandOptionLikeCommandDescriptor[] {optLikeCmd1, optLikeCmd2,},
                    false),
            });
            var commandCollection = new CommandCollection(new CommandDescriptor[]
            {
                CreateCommand("Primary",
                    new ICommandParameterDescriptor[]{},
                    null,
                    null,
                    CommandFlags.SubCommandsEntryPoint),
                CreateCommand("Level1",
                    new ICommandParameterDescriptor[]{},
                    null,
                    commandCollectionNested,
                    CommandFlags.None),
            });
            var resolver = new CoconaCommandResolver(
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(commandCollection, new string[] { "Level1", "Level2", "--opt1", "123", "--optlikecmd1", "--version" });
            resolve.Success.Should().BeTrue();
            resolve.CommandCollection.Should().Be(commandCollectionNested);
            resolve.MatchedCommand.Name.Should().Be("OptLikeCmd1");
            resolve.ParsedCommandLine.Options.Should().BeEmpty();
            resolve.ParsedCommandLine.UnknownOptions.Should().HaveCount(1);
            resolve.SubCommandStack.Should().HaveCount(2);
            resolve.SubCommandStack[0].Name.Should().Be("Level1");
            resolve.SubCommandStack[1].Name.Should().Be("Level2");
        }


        public class TestCommandProvider : ICoconaCommandProvider
        {
            private readonly CommandCollection _commandCollection;

            public TestCommandProvider(CommandCollection commandCollection)
            {
                _commandCollection = commandCollection;
            }

            public CommandCollection GetCommandCollection()
                => _commandCollection;
        }


        private CommandOptionLikeCommandDescriptor CreateOptionLikeCommand(string optionName, char[] shortNames, string name)
        {
            Action action = () => { };
            return new CommandOptionLikeCommandDescriptor(optionName, shortNames, new CommandDescriptor(
                action.Method,
                default,
                name,
                Array.Empty<string>(),
                "",
                Array.Empty<object>(),
                Array.Empty<CommandServiceParameterDescriptor>(),
                Array.Empty<CommandOptionDescriptor>(),
                Array.Empty<CommandArgumentDescriptor>(),
                Array.Empty<CommandOverloadDescriptor>(),
                Array.Empty<CommandOptionLikeCommandDescriptor>(),
                CommandFlags.None,
                null
            ), CommandOptionFlags.None);
        }

        private CommandDescriptor CreateCommand(string name, ICommandParameterDescriptor[] parameterDescriptors, bool isPrimary)
        {
            return CreateCommand(name, parameterDescriptors, null, isPrimary);
        }

        private CommandDescriptor CreateCommand(string name, ICommandParameterDescriptor[] parameterDescriptors, CommandOptionLikeCommandDescriptor[]? optionLikeCommandDescriptors, bool isPrimary)
        {
            return CreateCommand(name, parameterDescriptors, optionLikeCommandDescriptors, null, isPrimary ? CommandFlags.Primary : CommandFlags.None);
        }

        private CommandDescriptor CreateCommand(string name, ICommandParameterDescriptor[] parameterDescriptors, CommandOptionLikeCommandDescriptor[]? optionLikeCommandDescriptors, CommandCollection? subCommands, CommandFlags commandFlags)
        {
            Action action = () => { };
            return new CommandDescriptor(
                action.Method,
                action.Target,
                name,
                Array.Empty<string>(),
                "",
                Array.Empty<object>(),
                parameterDescriptors,
                parameterDescriptors.OfType<CommandOptionDescriptor>().ToArray(),
                parameterDescriptors.OfType<CommandArgumentDescriptor>().ToArray(),
                Array.Empty<CommandOverloadDescriptor>(),
                optionLikeCommandDescriptors ?? Array.Empty<CommandOptionLikeCommandDescriptor>(),
                commandFlags,
                subCommands
            );
        }
    }
}
