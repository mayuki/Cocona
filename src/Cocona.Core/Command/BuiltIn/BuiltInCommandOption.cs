using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Command.BuiltIn
{
    public static class BuiltInCommandOption
    {
        public static CommandOptionDescriptor Help { get; }
            = new CommandOptionDescriptor(
                typeof(bool),
                "help",
                new[] { 'h' },
                "Show help message",
                new CoconaDefaultValue(false),
                null,
                CommandOptionFlags.None,
                Array.Empty<Attribute>()
            );

        public static CommandOptionDescriptor Version { get; }
            = new CommandOptionDescriptor(
                typeof(bool),
                "version",
                Array.Empty<char>(),
                "Show version",
                new CoconaDefaultValue(false),
                null,
                CommandOptionFlags.None,
                Array.Empty<Attribute>()
            );

        public static CommandOptionDescriptor Completion { get; }
            = new CommandOptionDescriptor(
                typeof(string),
                "completion",
                Array.Empty<char>(),
                "Generate a shell completion code",
                new CoconaDefaultValue(null),
                "shell",
                CommandOptionFlags.None,
                Array.Empty<Attribute>()
            );

        public static CommandOptionDescriptor CompletionCandidates { get; }
            = new CommandOptionDescriptor(
                typeof(string),
                "completion-candidates",
                Array.Empty<char>(),
                "Generate a shell completion candidates",
                new CoconaDefaultValue(null),
                "shell",
                CommandOptionFlags.Hidden,
                Array.Empty<Attribute>()
            );
    }
}
