using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Command.BuiltIn
{
    public static class BuiltInCommandOption
    {
        public static CommandOptionDescriptor Help { get; } = new CommandOptionDescriptor(typeof(bool), "help", new[] { 'h' }, "Show help message", new CoconaDefaultValue(false), null);
        public static CommandOptionDescriptor Version { get; } = new CommandOptionDescriptor(typeof(bool), "version", Array.Empty<char>(), "Show version", new CoconaDefaultValue(false), null);
    }
}
