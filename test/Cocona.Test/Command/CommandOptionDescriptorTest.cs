using System;
using System.Collections.Generic;
using System.Text;
using Cocona.Command;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.Command
{
    public class CommandOptionDescriptorTest
    {
        [Fact]
        public void ValueName_Default_NotArray()
        {
            var command = new CommandOptionDescriptor(typeof(string), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None, null, CommandOptionFlags.None, Array.Empty<Attribute>());
            command.ValueName.Should().Be("String");
        }

        [Fact]
        public void ValueName_NotArray()
        {
            var command = new CommandOptionDescriptor(typeof(string), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None, "ValueName0", CommandOptionFlags.None, Array.Empty<Attribute>());
            command.ValueName.Should().Be("ValueName0");
        }

        [Fact]
        public void ValueName_Array()
        {
            var command = new CommandOptionDescriptor(typeof(string[]), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None, null, CommandOptionFlags.None, Array.Empty<Attribute>());
            command.ValueName.Should().Be("String[]");
        }
    }
}
