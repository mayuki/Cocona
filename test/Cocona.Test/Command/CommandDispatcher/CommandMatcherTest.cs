using Cocona.Command;
using Cocona.Command.Dispatcher;
using Cocona.CommandLine;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Cocona.Test.Command.CommandDispatcher
{
    public class CommandMatcherTest
    {
        private CommandCollection CreateCommandCollection(params CommandDescriptor[] commands)
        {
            return new CommandCollection(commands);
        }

        private CommandDescriptor CreateCommand(string name, CommandParameterDescriptor[] parameterDescriptors, CommandOverloadDescriptor[] overloads = null, bool isPrimaryCommand = false)
        {
            return new CommandDescriptor(
                typeof(CommandMatcherTest).GetMethod(nameof(CommandMatcherTest.__Dummy), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance),
                name,
                Array.Empty<string>(),
                string.Empty,
                parameterDescriptors,
                parameterDescriptors.OfType<CommandOptionDescriptor>().ToArray(),
                parameterDescriptors.OfType<CommandArgumentDescriptor>().ToArray(),
                overloads ?? Array.Empty<CommandOverloadDescriptor>(),
                isPrimaryCommand ? CommandFlags.Primary : CommandFlags.None
            );
        }

        private void __Dummy() { }

        [Fact]
        public void TryGetCommand()
        {
            var matcher = new CoconaCommandMatcher();
            var result = matcher.TryGetCommand("A", CreateCommandCollection(CreateCommand("A", Array.Empty<CommandParameterDescriptor>())), out var command);
            result.Should().BeTrue();
            command.Should().NotBeNull();
        }

        [Fact]
        public void TryGetCommand_Multiple()
        {
            var matcher = new CoconaCommandMatcher();
            var result = matcher.TryGetCommand("A", CreateCommandCollection(CreateCommand("A", Array.Empty<CommandParameterDescriptor>()), CreateCommand("B", Array.Empty<CommandParameterDescriptor>())), out var command);
            result.Should().BeTrue();
            command.Should().NotBeNull();
        }

        [Fact]
        public void TryGetCommand_Unknown()
        {
            var matcher = new CoconaCommandMatcher();
            var result = matcher.TryGetCommand("null", CreateCommandCollection(CreateCommand("A", Array.Empty<CommandParameterDescriptor>())), out var command);
            result.Should().BeFalse();
            command.Should().BeNull();
        }

        [Fact]
        public void TryGetCommand_Empty()
        {
            var matcher = new CoconaCommandMatcher();
            var result = matcher.TryGetCommand("A", CreateCommandCollection(), out var command);
            result.Should().BeFalse();
            command.Should().BeNull();
        }

        [Fact]
        public void ResolveOverload_No_Overloads()
        {
            var matcher = new CoconaCommandMatcher();
            var command = CreateCommand("A", Array.Empty<CommandParameterDescriptor>());
            var parsedCommandLine = new ParsedCommandLine(new CommandOption[] { }, new CommandArgument[] { }, new string[] { });
            var resolved = matcher.ResolveOverload(command, parsedCommandLine);
            resolved.Should().NotBeNull();
        }

        [Fact]
        public void ResolveOverload_Overload_1()
        {
            var matcher = new CoconaCommandMatcher();
            var commandOption = new CommandOptionDescriptor(typeof(string), "mode", new[] { 'm' }, string.Empty, CoconaDefaultValue.None, null);
            var command = CreateCommand(
                "A",
                new CommandParameterDescriptor[]
                {
                    commandOption
                },
                new CommandOverloadDescriptor[]
                {
                    new CommandOverloadDescriptor(commandOption, "foo", CreateCommand("A2", Array.Empty<CommandParameterDescriptor>()), StringComparer.OrdinalIgnoreCase),
                    new CommandOverloadDescriptor(commandOption, "bar", CreateCommand("A3", Array.Empty<CommandParameterDescriptor>()), StringComparer.OrdinalIgnoreCase),
                });
            var parsedCommandLine = new ParsedCommandLine(new CommandOption[] { }, new CommandArgument[] { }, new string[] { });
            var resolved = matcher.ResolveOverload(command, parsedCommandLine);
            resolved.Should().NotBeNull();
            resolved.Name.Should().Be("A");
        }

        [Fact]
        public void ResolveOverload_Overload_2()
        {
            var matcher = new CoconaCommandMatcher();
            var commandOption = new CommandOptionDescriptor(typeof(string), "mode", new[] { 'm' }, string.Empty, CoconaDefaultValue.None, null);
            var command = CreateCommand(
                "A",
                new CommandParameterDescriptor[]
                {
                    commandOption
                },
                new CommandOverloadDescriptor[]
                {
                    new CommandOverloadDescriptor(commandOption, "foo", CreateCommand("A2", Array.Empty<CommandParameterDescriptor>()), StringComparer.OrdinalIgnoreCase),
                    new CommandOverloadDescriptor(commandOption, "bar", CreateCommand("A3", Array.Empty<CommandParameterDescriptor>()), StringComparer.OrdinalIgnoreCase),
                });
            var parsedCommandLine = new ParsedCommandLine(new CommandOption[] { new CommandOption(commandOption, "foo") }, new CommandArgument[] { }, new string[] { });
            var resolved = matcher.ResolveOverload(command, parsedCommandLine);
            resolved.Should().NotBeNull();
            resolved.Name.Should().Be("A2");
        }

        [Fact]
        public void ResolveOverload_Overload_NoValue()
        {
            var matcher = new CoconaCommandMatcher();
            var commandOption0 = new CommandOptionDescriptor(typeof(bool), "foo", new[] { 'm' }, string.Empty, CoconaDefaultValue.None, null);
            var commandOption1 = new CommandOptionDescriptor(typeof(bool), "bar", new[] { 'm' }, string.Empty, CoconaDefaultValue.None, null);
            var command = CreateCommand(
                "A",
                new CommandParameterDescriptor[]
                {
                    commandOption0, commandOption1
                },
                new CommandOverloadDescriptor[]
                {
                    new CommandOverloadDescriptor(commandOption0, null, CreateCommand("A2", Array.Empty<CommandParameterDescriptor>()), StringComparer.OrdinalIgnoreCase),
                    new CommandOverloadDescriptor(commandOption1, null, CreateCommand("A3", Array.Empty<CommandParameterDescriptor>()), StringComparer.OrdinalIgnoreCase),
                });
            var parsedCommandLine = new ParsedCommandLine(new CommandOption[] { new CommandOption(commandOption0, "true") }, new CommandArgument[] { }, new string[] { });
            var resolved = matcher.ResolveOverload(command, parsedCommandLine);
            resolved.Should().NotBeNull();
            resolved.Name.Should().Be("A2");
        }

        [Fact]
        public void ResolveOverload_Ambiguous()
        {
            var matcher = new CoconaCommandMatcher();
            var commandOption0 = new CommandOptionDescriptor(typeof(bool), "foo", new[] { 'm' }, string.Empty, CoconaDefaultValue.None, null);
            var commandOption1 = new CommandOptionDescriptor(typeof(bool), "bar", new[] { 'm' }, string.Empty, CoconaDefaultValue.None, null);
            var command = CreateCommand(
                "A",
                new CommandParameterDescriptor[]
                {
                    commandOption0, commandOption1
                },
                new CommandOverloadDescriptor[]
                {
                    new CommandOverloadDescriptor(commandOption0, null, CreateCommand("A2", Array.Empty<CommandParameterDescriptor>()), StringComparer.OrdinalIgnoreCase),
                    new CommandOverloadDescriptor(commandOption0, null, CreateCommand("A3", Array.Empty<CommandParameterDescriptor>()), StringComparer.OrdinalIgnoreCase),
                });
            var parsedCommandLine = new ParsedCommandLine(new CommandOption[] { new CommandOption(commandOption0, "true") }, new CommandArgument[] { }, new string[] { });
            Assert.Throws<CoconaException>(() => matcher.ResolveOverload(command, parsedCommandLine));
        }
    }
}
