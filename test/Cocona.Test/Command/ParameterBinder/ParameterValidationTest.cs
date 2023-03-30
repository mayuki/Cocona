using System.ComponentModel.DataAnnotations;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Binder.Validation;
using Cocona.CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace Cocona.Test.Command.ParameterBinder
{
    public class ParameterValidationTest
    {
        private CommandDescriptor CreateCommand(ICommandParameterDescriptor[] parameterDescriptors)
        {
            return new CommandDescriptor(
                typeof(CommandParameterValidationTest).GetMethod(nameof(CommandParameterValidationTest.Dummy)),
                default,
                "Test",
                Array.Empty<string>(),
                "",
                Array.Empty<object>(),
                parameterDescriptors,
                parameterDescriptors.OfType<CommandOptionDescriptor>().ToArray(),
                parameterDescriptors.OfType<CommandArgumentDescriptor>().ToArray(),
                Array.Empty<CommandOverloadDescriptor>(),
                Array.Empty<CommandOptionLikeCommandDescriptor>(),
                CommandFlags.None,
                null
            );
        }
        private static CoconaParameterBinder CreateCoconaParameterBinder()
        {
            return new CoconaParameterBinder(new ServiceCollection().BuildServiceProvider(), new CoconaValueConverter(), new DataAnnotationsParameterValidatorProvider());
        }

        [Fact]
        public void Bind_Option_DataAnnotationsParameterValidator_Empty()
        {
            var command = CreateCommand(new[]
            {
                new CommandOptionDescriptor(typeof(int), "arg0", Array.Empty<char>(), "", CoconaDefaultValue.None, null, CommandOptionFlags.None, new Attribute[] { } )
            });

            var binder = CreateCoconaParameterBinder();
            var result = binder.Bind(command, new[] { new CommandOption(command.Options[0], "0", 0) }, Array.Empty<CommandArgument>());
            result.Should().HaveCount(1);
        }

        [Fact]
        public void Bind_Option_DataAnnotationsParameterValidator_Single()
        {
            var command = CreateCommand(new []
            {
                new CommandOptionDescriptor(typeof(int), "arg0", Array.Empty<char>(), "", CoconaDefaultValue.None, null, CommandOptionFlags.None, new [] { new RangeAttribute(0, 100) } )
            });

            var binder = CreateCoconaParameterBinder();
            var result = binder.Bind(command, new[] { new CommandOption(command.Options[0], "0", 0) }, Array.Empty<CommandArgument>());
            result.Should().HaveCount(1);
        }

        [Fact]
        public void Bind_Option_DataAnnotationsParameterValidator_Error()
        {
            var command = CreateCommand(new[]
            {
                new CommandOptionDescriptor(typeof(int), "arg0", Array.Empty<char>(), "", CoconaDefaultValue.None, null, CommandOptionFlags.None, new [] { new RangeAttribute(0, 100) } )
            });

            var binder = CreateCoconaParameterBinder();
            var ex = Assert.Throws<ParameterBinderException>(() => binder.Bind(command, new[] { new CommandOption(command.Options[0], "123", 0) }, Array.Empty<CommandArgument>()));
            ex.Result.Should().Be(ParameterBinderResult.ValidationFailed);
        }

        [Fact]
        public void Bind_Option_DataAnnotationsParameterValidator_UnknownAttribute()
        {
            var command = CreateCommand(new[]
            {
                new CommandOptionDescriptor(typeof(int), "arg0", Array.Empty<char>(), "", CoconaDefaultValue.None, null, CommandOptionFlags.None, new Attribute[] { new MyAttribute(), new RangeAttribute(0, 100) } )
            });

            var binder = CreateCoconaParameterBinder();
            var result = binder.Bind(command, new[] { new CommandOption(command.Options[0], "0", 0) }, Array.Empty<CommandArgument>());
            result.Should().HaveCount(1);
        }


        [Fact]
        public void Bind_Argument_DataAnnotationsParameterValidator_Empty()
        {
            var command = CreateCommand(new[]
            {
                new CommandArgumentDescriptor(typeof(int), "arg0", 0, "", CoconaDefaultValue.None, new Attribute[] { } )
            });

            var binder = CreateCoconaParameterBinder();
            var result = binder.Bind(command, Array.Empty<CommandOption>(), new[] { new CommandArgument("0", 0) });
            result.Should().HaveCount(1);
        }

        [Fact]
        public void Bind_Argument_DataAnnotationsParameterValidator_Single()
        {
            var command = CreateCommand(new[]
            {
                new CommandArgumentDescriptor(typeof(int), "arg0", 0, "", CoconaDefaultValue.None, new [] { new RangeAttribute(0, 100) } )
            });

            var binder = CreateCoconaParameterBinder();
            var result = binder.Bind(command, Array.Empty<CommandOption>(), new[] { new CommandArgument("0", 0) });
            result.Should().HaveCount(1);
        }

        [Fact]
        public void Bind_Argument_DataAnnotationsParameterValidator_Error()
        {
            var command = CreateCommand(new[]
            {
                new CommandArgumentDescriptor(typeof(int), "arg0", 0, "", CoconaDefaultValue.None, new [] { new RangeAttribute(0, 100) } )
            });

            var binder = CreateCoconaParameterBinder();
            var ex = Assert.Throws<ParameterBinderException>(() => binder.Bind(command, Array.Empty<CommandOption>(), new[] { new CommandArgument("123", 0) }));
            ex.Result.Should().Be(ParameterBinderResult.ValidationFailed);
        }

        [Fact]
        public void Bind_Argument_DataAnnotationsParameterValidator_UnknownAttribute()
        {
            var command = CreateCommand(new[]
            {
                new CommandArgumentDescriptor(typeof(int), "arg0", 0, "", CoconaDefaultValue.None, new Attribute[] { new MyAttribute(), new RangeAttribute(0, 100) } )
            });

            var binder = CreateCoconaParameterBinder();
            var result = binder.Bind(command, Array.Empty<CommandOption>(), new[] { new CommandArgument("0", 0) });
            result.Should().HaveCount(1);
        }


        class MyAttribute : Attribute { }

        class CommandParameterValidationTest
        {
            public void Dummy() { }
        }
    }
}
