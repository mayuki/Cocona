using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.CommandLine;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocona.Command.Binder.Validation;
using Xunit;

namespace Cocona.Test.Command.ParameterBinder
{
    public class BindParameterTest
    {
        public class MyService { }

        private static CoconaParameterBinder CreateCoconaParameterBinder()
        {
            var services = new ServiceCollection();
            {
                services.AddTransient<MyService>();
            }

            return new CoconaParameterBinder(services.BuildServiceProvider(), new CoconaValueConverter(), new DataAnnotationsParameterValidatorProvider());
        }

#pragma warning disable xUnit1013 // Public method should be marked as test
        public void __Dummy() { }
#pragma warning restore xUnit1013 // Public method should be marked as test

        private CommandDescriptor CreateCommand(ICommandParameterDescriptor[] parameterDescriptors)
        {
            var paramDescriptorsWithParameterSets = parameterDescriptors.SelectMany(x => x switch
                {
                    CommandOptionDescriptor => new ICommandParameterDescriptor[] { x },
                    CommandArgumentDescriptor => new ICommandParameterDescriptor[] { x },
                    CommandIgnoredParameterDescriptor => new ICommandParameterDescriptor[] { x },
                    CommandServiceParameterDescriptor => new ICommandParameterDescriptor[] { x },
                    CommandParameterSetDescriptor paramSetDesc => paramSetDesc.Members
                        .Select(y => y.ParameterDescriptor).ToArray(),
                    CommandParameterizedParameterSetDescriptor parameterizedParamSetDesc => parameterizedParamSetDesc.Parameters,
                    _ => throw new NotSupportedException($"Type '{x.GetType().FullName}' is not supported by CreateCommand."),
                })
                .ToArray();

            return new CommandDescriptor(
                typeof(BindParameterTest).GetMethod(nameof(BindParameterTest.__Dummy))!,
                default,
                "Test",
                Array.Empty<string>(),
                "",
                Array.Empty<object>(),
                parameterDescriptors,
                paramDescriptorsWithParameterSets.OfType<CommandOptionDescriptor>().ToArray(),
                paramDescriptorsWithParameterSets.OfType<CommandArgumentDescriptor>().ToArray(),
                Array.Empty<CommandOverloadDescriptor>(),
                Array.Empty<CommandOptionLikeCommandDescriptor>(),
                CommandFlags.None,
                null
            );
        }

        private CommandOptionDescriptor CreateCommandOption(Type optionType, string name, IReadOnlyList<char> shortName, string description, CoconaDefaultValue defaultValue)
        {
            return new CommandOptionDescriptor(optionType, name, shortName, description, defaultValue, null, CommandOptionFlags.None, Array.Empty<Attribute>());
        }

        [Fact]
        public void BindArguments()
        {
            // void Test(string arg0, string arg1, string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs.Should().Equal("argValue0", "argValue1", "argValue2");
        }

        [Fact]
        public void BindArguments_Ordered()
        {
            // void Test([Argument(Order = 5)]string arg0, [Argument(Order = 4)]string arg1, [Argument(Order = 3)]string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 5, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 4, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 3, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs.Should().Equal("argValue2", "argValue1", "argValue0");
        }

        [Fact]
        public void BindArguments_Insufficient()
        {
            // void Test([Argument]string arg0, [Argument]string arg1, [Argument]string arg2);
            // Arguments: new [] { "argValue0", "argValue1" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1) };

            var ex = Assert.Throws<ParameterBinderException>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Result.Should().Be(ParameterBinderResult.InsufficientArgument);
            ex.Argument.Should().NotBeNull();
        }

        [Fact]
        public void BindArguments_Insufficient_DefaultValue()
        {
            // void Test([Argument]string arg0, [Argument]string arg1, [Argument]string arg2 = "hello");
            // Arguments: new [] { "argValue0", "argValue1" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", new CoconaDefaultValue("hello"), Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1) };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs.Should().Equal("argValue0", "argValue1", "hello");
        }

        [Fact]
        public void BindArguments_Array_First()
        {
            // void Test([Argument]string[] arg0, [Argument]string arg1, [Argument]string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), new CommandArgument("argValue3", 3) };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs[0].Should().Equals(new[] { "argValue0", "argValue1", });
            invokeArgs[1].Should().Be("argValue2");
            invokeArgs[2].Should().Be("argValue3");
        }

        [Fact]
        public void BindArguments_Array_Middle()
        {
            // void Test([Argument]string arg0, [Argument]string[] arg1, [Argument]string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string[]), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), new CommandArgument("argValue3", 3), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs[0].Should().Be("argValue0");
            invokeArgs[1].Should().Equals(new[] { "argValue1", "argValue2", });
            invokeArgs[2].Should().Be("argValue3");
        }

        [Fact]
        public void BindArguments_Array_Last()
        {
            // void Test([Argument]string arg0, [Argument]string arg1, [Argument]string[] arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string[]), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), new CommandArgument("argValue3", 3), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs[0].Should().Be("argValue0");
            invokeArgs[1].Should().Be("argValue1");
            invokeArgs[2].Should().Equals(new[] { "argValue2", "argValue3", });
        }

        [Fact]
        public void BindArguments_Array_Multiple_Invalid()
        {
            // void Test([Argument]string arg0, [Argument]string[] arg1, [Argument]string[] arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string[]), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string[]), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), new CommandArgument("argValue3", 3), };

            var ex = Assert.Throws<ParameterBinderException>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Result.Should().Be(ParameterBinderResult.MultipleArrayInArgument);
            ex.Argument.Should().NotBeNull();
        }

        [Fact]
        public void BindArguments_Array_Empty()
        {
            // void Test([Argument]string[] arg0);
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { };

            var ex = Assert.Throws<ParameterBinderException>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Result.Should().Be(ParameterBinderResult.InsufficientArgument);
            ex.Argument.Should().NotBeNull();
        }

        [Fact]
        public void BindArguments_Array_One()
        {
            // void Test([Argument]string[] arg0);
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0) };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(1);
            invokeArgs[0].Should().Equals(new string[] { "argValue0" });
        }

        [Fact]
        public void BindArguments_Array_Insufficient()
        {
            // void Test([Argument]string arg0, [Argument]string arg1, [Argument]string[] arg2);
            // Arguments: new [] { "argValue0", "argValue1" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string[]), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1) };


            var ex = Assert.Throws<ParameterBinderException>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Result.Should().Be(ParameterBinderResult.InsufficientArgument);
            ex.Argument.Should().NotBeNull();
        }

        [Fact]
        public void BindArguments_Array_Insufficient_2()
        {
            // void Test([Argument]string[] arg0, [Argument]string arg1, [Argument]string arg2, [Argument]string arg3);
            // Arguments: new [] { "argValue0", "argValue1" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg3", 3, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1) };

            var ex = Assert.Throws<ParameterBinderException>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Result.Should().Be(ParameterBinderResult.InsufficientArgument);
            ex.Argument.Should().NotBeNull();
        }

        [Fact]
        public void BindArguments_Array_DefaultValue()
        {
            // void Test([Argument]string[] arg0, [Argument]string arg1 = null, [Argument]string arg2 = null);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", new CoconaDefaultValue(default(string)), Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2) };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs[0].Should().Equals(new[] { "argValue0", });
            invokeArgs[1].Should().Be("argValue1");
            invokeArgs[2].Should().Be("argValue2");
        }

        [Fact]
        public void BindArguments_Array_DefaultValue_2()
        {
            // void Test([Argument]string[] arg0, [Argument]string arg1 = null, [Argument]string arg2 = null);
            // Arguments: new [] { "argValue0", "argValue1" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", new CoconaDefaultValue(default(string)), Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1) };

            var ex = Assert.Throws<ParameterBinderException>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Result.Should().Be(ParameterBinderResult.InsufficientArgument);
            ex.Argument.Should().NotBeNull();
        }

        [Fact]
        public void BindArguments_Enumerable_Multiple_Invalid()
        {
            // void Test([Argument]string arg0, [Argument]string[] arg1, [Argument]IReadOnlyList<string> arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string[]), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(IReadOnlyList<string>), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), new CommandArgument("argValue3", 3), };
            
            var ex = Assert.Throws<ParameterBinderException>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Result.Should().Be(ParameterBinderResult.MultipleArrayInArgument);
            ex.Argument.Should().NotBeNull();
        }

        [Fact]
        public void BindArguments_List()
        {
            // void Test([Argument]string[] arg0, [Argument]string arg1, [Argument]string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(List<string>), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), new CommandArgument("argValue3", 3), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs[0].Should().Equals(new List<string>() { "argValue0", "argValue1", });
            invokeArgs[1].Should().Be("argValue2");
            invokeArgs[2].Should().Be("argValue3");
        }

        [Fact]
        public void BindOptions()
        {
            // void Test([Option]string option0, [Option]bool option1);
            // Options: --option0=foo --option1
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "option1", Array.Empty<char>(), "", CoconaDefaultValue.None),
                }
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo", 0),
                new CommandOption(commandDescriptor.Options[1], "true", 1),
            };
            var commandArguments = new CommandArgument[] { };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(2);
            invokeArgs[0].Should().Be("foo");
            invokeArgs[1].Should().Be(true);
        }

        [Fact]
        public void BindOptions_Insufficient()
        {
            // void Test([Option]string option0);
            // Options: 
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                }
            );

            var commandOptions = new CommandOption[]
            {
            };
            var commandArguments = new CommandArgument[] { };

            var ex = Assert.Throws<ParameterBinderException>(() =>  CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Result.Should().Be(ParameterBinderResult.InsufficientOption);
            ex.Option.Should().NotBeNull();
        }

        [Fact]
        public void BindOptions_Insufficient_Value()
        {
            // void Test([Option]string option0);
            // Options: --option0
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                }
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], null, 0),
            };
            var commandArguments = new CommandArgument[] { };

            var ex = Assert.Throws<ParameterBinderException>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Result.Should().Be(ParameterBinderResult.InsufficientOptionValue);
            ex.Option.Should().NotBeNull();
        }


        [Fact]
        public void BindOptions_DefaultValue_1()
        {
            // void Test([Option]string option0, [Option]bool option1 = false);
            // Options: --option0=foo
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "option1", Array.Empty<char>(), "", new CoconaDefaultValue(false)),
                }
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo", 0),
            };
            var commandArguments = new CommandArgument[] { };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(2);
            invokeArgs[0].Should().Be("foo");
            invokeArgs[1].Should().Be(false);
        }

        [Fact]
        public void BindOptions_DefaultValue_2()
        {
            // void Test([Option]string option0, [Option]bool option1 = false);
            // Options: --option0=foo --option1
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                    CreateCommandOption(typeof(bool), "option1", Array.Empty<char>(), "", new CoconaDefaultValue(false)),
                }
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo", 0),
                new CommandOption(commandDescriptor.Options[1], "true", 1),
            };
            var commandArguments = new CommandArgument[] { };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(2);
            invokeArgs[0].Should().Be("foo");
            invokeArgs[1].Should().Be(true);
        }

        [Fact]
        public void BindOptions_Array_Multiple()
        {
            // void Test([Option]string[] option0);
            // Options: --option0=foo --option0=bar --option0=baz
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(string[]), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                }
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo", 0),
                new CommandOption(commandDescriptor.Options[0], "bar", 1),
                new CommandOption(commandDescriptor.Options[0], "baz", 2),
            };
            var commandArguments = new CommandArgument[] { };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(1);
            invokeArgs[0].As<string[]>().Should().HaveCount(3);
        }

        [Fact]
        public void BindOptions_ListT_Multiple()
        {
            // void Test([Option]List<string> option0);
            // Options: --option0=foo --option0=bar --option0=baz
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(List<string>), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                }
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo", 0),
                new CommandOption(commandDescriptor.Options[0], "bar", 1),
                new CommandOption(commandDescriptor.Options[0], "baz", 2),
            };
            var commandArguments = new CommandArgument[] { };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(1);
            invokeArgs[0].As<List<string>>().Should().HaveCount(3);
        }

        [Fact]
        public void BindOptions_IReadOnlyListT_Multiple()
        {
            // void Test([Option]IReadOnlyList<string> option0);
            // Options: --option0=foo --option0=bar --option0=baz
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(IReadOnlyList<string>), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                }
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo", 0),
                new CommandOption(commandDescriptor.Options[0], "bar", 1),
                new CommandOption(commandDescriptor.Options[0], "baz", 2),
            };
            var commandArguments = new CommandArgument[] { };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(1);
            invokeArgs[0].As<IReadOnlyList<string>>().Should().HaveCount(3);
        }

        [Fact]
        public void BindOptions_CanNotConvertType()
        {
            // void Test([Option]int option0);
            // Options: --option0 hello
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(int), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                }
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "optValue0", 0),
            };
            var commandArguments = new CommandArgument[] { };

            var ex = Assert.Throws<ParameterBinderException>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Option.Name.Should().Be("option0");
            ex.Message.Should().Be("Option 'option0' requires Int32 value. 'optValue0' cannot be converted to Int32 value.");
        }

        [Fact]
        public void BindArguments_CanNotConvertType()
        {
            // void Test([Argument]int option0);
            // Options: hello
            // Arguments: new [] { }
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(int), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[]
            {
            };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0) };

            var ex = Assert.Throws<ParameterBinderException>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
            ex.Argument.Name.Should().Be("arg0");
            ex.Message.Should().Be("Argument 'arg0' requires Int32 value. 'argValue0' cannot be converted to Int32 value.");
        }

        [Fact]
        public void BindService()
        {
            // void Test([FromService]MyService arg0);
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandServiceParameterDescriptor(typeof(MyService), "fromService0"),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(1);
            invokeArgs[0].Should().BeOfType<MyService>();
        }

        [Fact]
        public void BindService_Options()
        {
            // void Test([FromService]MyService arg0);
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    CreateCommandOption(typeof(int), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                    new CommandServiceParameterDescriptor(typeof(MyService), "fromService0"),
                    CreateCommandOption(typeof(int), "option1", Array.Empty<char>(), "", CoconaDefaultValue.None),
                }
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "123", 0),
                new CommandOption(commandDescriptor.Options[1], "456", 1),
            };
            var commandArguments = new CommandArgument[] { };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs[0].Should().Be(123);
            invokeArgs[1].Should().BeOfType<MyService>();
            invokeArgs[2].Should().Be(456);
        }

        [Fact]
        public void BindService_Argument()
        {
            // void Test([FromService]MyService arg0);
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandServiceParameterDescriptor(typeof(MyService), "fromService0"),
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(2);
            invokeArgs[0].Should().BeOfType<MyService>();
            invokeArgs[1].Should().Be("argValue0");
        }

        [Fact]
        public void BindParameterSet()
        {
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandParameterSetDescriptor(typeof(TestCommandParameterSet), "paramSet", Array.Empty<Attribute>(), new CommandParameterSetMemberDescriptor[]
                    {
                        new CommandParameterSetMemberDescriptor(
                            new CommandOptionDescriptor(typeof(int), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None, null, CommandOptionFlags.None, Array.Empty<Attribute>()),
                            (x, v) => ((TestCommandParameterSet)x).Option0 = (int)v
                        ),
                        new CommandParameterSetMemberDescriptor(
                            new CommandOptionDescriptor(typeof(bool), "option1", Array.Empty<char>(), "", new CoconaDefaultValue(false), null, CommandOptionFlags.None, Array.Empty<Attribute>()),
                            (x, v) => ((TestCommandParameterSet)x).Option1 = (bool)v
                        ),
                        new CommandParameterSetMemberDescriptor(
                            new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                            (x, v) => ((TestCommandParameterSet)x).Arg0 = (string)v
                        )
                    }),
                }
            );

            var commandOptions = new CommandOption[] { new CommandOption(commandDescriptor.Options[0], "123", 0), };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            var paramSet = invokeArgs[0].Should().BeOfType<TestCommandParameterSet>().Subject;
            paramSet.Option0.Should().Be(123);
            paramSet.Option1.Should().BeFalse();
            paramSet.Arg0.Should().Be("argValue0");
        }

        [Fact]
        public void BindParameterSet_Mixed()
        {
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandParameterSetDescriptor(typeof(TestCommandParameterSet), "paramSet", Array.Empty<Attribute>(), new CommandParameterSetMemberDescriptor[]
                    {
                        new CommandParameterSetMemberDescriptor(
                            new CommandOptionDescriptor(typeof(int), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None, null, CommandOptionFlags.None, Array.Empty<Attribute>()),
                            (x, v) => ((TestCommandParameterSet)x).Option0 = (int)v
                        ),
                        new CommandParameterSetMemberDescriptor(
                            new CommandOptionDescriptor(typeof(bool), "option1", Array.Empty<char>(), "", new CoconaDefaultValue(false), null, CommandOptionFlags.None, Array.Empty<Attribute>()),
                            (x, v) => ((TestCommandParameterSet)x).Option1 = (bool)v
                        ),
                        new CommandParameterSetMemberDescriptor(
                            new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                        (x, v) => ((TestCommandParameterSet)x).Arg0 = (string)v
                        )
                    }),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { new CommandOption(commandDescriptor.Options[0], "123", 0), new CommandOption(commandDescriptor.Options[1], "true", 1), };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);

            invokeArgs[0].Should().Be("argValue0");

            var paramSet = invokeArgs[1].Should().BeOfType<TestCommandParameterSet>().Subject;
            paramSet.Option0.Should().Be(123);
            paramSet.Option1.Should().BeTrue();
            paramSet.Arg0.Should().Be("argValue1");

            invokeArgs[2].Should().Be("argValue2");
        }

        class TestCommandParameterSet : ICommandParameterSet
        {
            [Option]
            public int Option0 { get; set; }
            [Option]
            public bool Option1 { get; set; }

            [Argument]
            public string Arg0 { get; set; }
        }


        [Fact]
        public void BindParameterizedParameterSet()
        {
            var commandDescriptor = CreateCommand(new ICommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    new CommandParameterizedParameterSetDescriptor(typeof(TestCommandParameterizedParameterSet), "paramSet", Array.Empty<Attribute>(), new ICommandParameterDescriptor[]
                    {
                        new CommandOptionDescriptor(typeof(int), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None, null, CommandOptionFlags.None, Array.Empty<Attribute>()),
                        new CommandOptionDescriptor(typeof(bool), "option1", Array.Empty<char>(), "", new CoconaDefaultValue(false), null, CommandOptionFlags.None, Array.Empty<Attribute>()),
                        new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                    }),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                }
            );

            var commandOptions = new CommandOption[] { new CommandOption(commandDescriptor.Options[0], "123", 0), new CommandOption(commandDescriptor.Options[1], "true", 1), };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0", 0), new CommandArgument("argValue1", 1), new CommandArgument("argValue2", 2), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);

            invokeArgs[0].Should().Be("argValue0");

            var paramSet = invokeArgs[1].Should().BeOfType<TestCommandParameterizedParameterSet>().Subject;
            paramSet.Option0.Should().Be(123);
            paramSet.Option1.Should().BeTrue();
            paramSet.Arg0.Should().Be("argValue1");

            invokeArgs[2].Should().Be("argValue2");
        }

        class TestCommandParameterizedParameterSet : ICommandParameterSet
        {
            public int Option0 { get; }
            public bool Option1 { get; }
            public string Arg0 { get; }

            public TestCommandParameterizedParameterSet(int option0, bool option1, [Argument] string arg0)
            {
                Option0 = option0;
                Option1 = option1;
                Arg0 = arg0;
            }
        }
    }
}
