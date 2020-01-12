using Cocona.Command;
using Cocona.Help;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cocona.Test.Help
{
    public class CoconaCommandHelpProviderTest
    {
        private void __Dummy() { }

        [Fact]
        public void CommandHelp1()
        {
            // void Test(string arg0, string arg1, string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2" }
            var commandDescriptor = new CommandDescriptor(
                typeof(CoconaCommandHelpProviderTest).GetMethod(nameof(CoconaCommandHelpProviderTest.__Dummy), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance),
                "Test",
                Array.Empty<string>(),
                "command description",
                new CommandParameterDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(string), "option0", Array.Empty<char>(), "option description - option0", CoconaDefaultValue.None),
                    new CommandOptionDescriptor(typeof(bool), "option1", Array.Empty<char>(), "option description - option1", CoconaDefaultValue.None),
                    new CommandIgnoredParameterDescriptor(typeof(bool), true),
                    new CommandServiceParameterDescriptor(typeof(bool)),
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "description - arg0", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "description - arg1", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "description - arg2", CoconaDefaultValue.None),
                }
            );

            var provider = new CoconaCommandHelpProvider(() => "ExeName");
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
        }


        [Fact]
        public void CommandHelp_Rendered()
        {
            var commandDescriptor = new CommandDescriptor(
                typeof(CoconaCommandHelpProviderTest).GetMethod(nameof(CoconaCommandHelpProviderTest.__Dummy), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance),
                "Test",
                Array.Empty<string>(),
                "command description",
                new CommandParameterDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                    new CommandOptionDescriptor(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                }
            );

            var provider = new CoconaCommandHelpProvider(() => "ExeName");
            var help = provider.CreateCommandHelp(commandDescriptor);
            var text = new CoconaHelpRenderer().Render(help);
            text.Should().Be(@"
Usage: ExeName Test [options...]

command description

Options:
  --foo, -f <String>         Foo option (Required)
  --looooooong-option, -l    Long name option (DefaultValue=False)
".TrimStart());

        }
    }
}
