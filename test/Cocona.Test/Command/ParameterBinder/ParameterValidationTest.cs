using System.ComponentModel.DataAnnotations;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Binder.Validation;
using Cocona.CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace Cocona.Test.Command.ParameterBinder;

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

    private static CoconaParameterBinder CreateCoconaParameterBinder(Action<IServiceCollection>? registerDependencies = null)
    {
        var services = new ServiceCollection();
        registerDependencies?.Invoke(services);
        return new CoconaParameterBinder(services.BuildServiceProvider(), new CoconaValueConverter(), new DataAnnotationsParameterValidatorProvider());
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
        var command = CreateCommand(new[]
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
    public void Bind_Enumerable_Argument_DataAnnotationsParameterValidator_Single()
    {
        var command = CreateCommand(new[] { new CommandArgumentDescriptor(typeof(List<int>), "arg0", 0, "", CoconaDefaultValue.None, new[] { new IsEvenEnumerableAttribute() }) });

        var binder = CreateCoconaParameterBinder();
        var result = binder.Bind(command, Array.Empty<CommandOption>(), new[]
        {
            new CommandArgument("10", 0),
            new CommandArgument("20", 1),
            new CommandArgument("30", 2),
            new CommandArgument("40", 3),
        });
        result.Should().HaveCount(1);
    }
    
    [Fact]
    public void Bind_Enumerable_Argument_DataAnnotationsParameterValidator_Error()
    {
        var command = CreateCommand(new[] { new CommandArgumentDescriptor(typeof(List<int>), "arg0", 0, "", CoconaDefaultValue.None, new[] { new IsEvenEnumerableAttribute() }) });

        var binder = CreateCoconaParameterBinder();
        var act = () => binder.Bind(
            command,
            Array.Empty<CommandOption>(), 
            new[] { new CommandArgument("10", 0), new CommandArgument("15", 1), new CommandArgument("20", 2), new CommandArgument("25", 3), });

        act.Should().Throw<ParameterBinderException>().And.Result.Should().Be(ParameterBinderResult.ValidationFailed);
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
    
    [Fact]
    public void Bind_Argument_DataAnnotationsParameterValidator_UsingDependencyInjection()
    {
        var command = CreateCommand(new[]
        {
            new CommandArgumentDescriptor(typeof(int), "arg0", 0, "", CoconaDefaultValue.None, new [] { new IsEvenUsingDependencyInjectionAttribute() } )
        });

        var binder = CreateCoconaParameterBinder(services => services.AddSingleton<Calculator>());
        var result = binder.Bind(command, Array.Empty<CommandOption>(), new[] { new CommandArgument("2", 0) });
        result.Should().HaveCount(1);
    }
    
    class MyAttribute : Attribute
    {
    }

    class IsEvenEnumerableAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IEnumerable<int> numbers)
            {
                return new ValidationResult($"Could not validate collection, values's type is {value?.GetType()}");
            }

            return numbers.All(x => x % 2 == 0)
                ? ValidationResult.Success
                : new ValidationResult("List contains uneven numbers.");
        }
    }
    
    class IsEvenUsingDependencyInjectionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not int number)
            {
                return new ValidationResult($"Could not validate value, values's type is {value?.GetType()}");
            }
            
            var calculator = validationContext.GetRequiredService<Calculator>();
            return calculator.IsEven(number)
                ? ValidationResult.Success
                : new ValidationResult("Value is an uneven number.");
        }
    }
    
    class Calculator
    {
        public bool IsEven(int number) => number % 2 == 0;
    }

    class CommandParameterValidationTest
    {
        public void Dummy() { }
    }
}
