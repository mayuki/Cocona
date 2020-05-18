using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Cocona.Command;
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
                 new TestCommandProvider(commandCollection),
                 new CoconaCommandLineParser(),
                 new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(new string[] { });
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
                new TestCommandProvider(commandCollection),
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(new string[] { });
            resolve.Success.Should().BeTrue();
            resolve.MatchedCommand.Name.Should().Be("Primary");
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
                new TestCommandProvider(commandCollection),
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(new string[] { "Foo" });
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
                new TestCommandProvider(commandCollection),
                new CoconaCommandLineParser(),
                new CoconaCommandMatcher()
            );

            var resolve = resolver.ParseAndResolve(new string[] { "Foo", "--opt1", "123" });
            resolve.Success.Should().BeTrue();
            resolve.MatchedCommand.Name.Should().Be("Foo");
            resolve.ParsedCommandLine.Options.Should().HaveCount(1);
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

        private CommandDescriptor CreateCommand(string name, ICommandParameterDescriptor[] parameterDescriptors, bool isPrimary)
        {
            Action action = () => { };
            return new CommandDescriptor(
                action.Method,
                name,
                Array.Empty<string>(),
                "",
                parameterDescriptors,
                parameterDescriptors.OfType<CommandOptionDescriptor>().ToArray(),
                parameterDescriptors.OfType<CommandArgumentDescriptor>().ToArray(),
                Array.Empty<CommandOverloadDescriptor>(),
                isPrimary ? CommandFlags.Primary : CommandFlags.None,
                null
            );
        }
    }
}
