using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.CommandLine;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
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

            return new CoconaParameterBinder(services.BuildServiceProvider(), new CoconaValueConverter());
        }

        [Fact]
        public void BindArguments()
        {
            // void Test(string arg0, string arg1, string arg2);
            // new [] { "argValue0", "argValue1", "argValue2" }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), new CommandArgument("argValue2"), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs.Should().Equal("argValue0", "argValue1", "argValue2");
        }

        [Fact]
        public void BindArguments_Ordered()
        {
            // void Test([Argument(Order = 5)]string arg0, [Argument(Order = 4)]string arg1, [Argument(Order = 3)]string arg2);
            // new [] { "argValue0", "argValue1", "argValue2" }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 5, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 4, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 3, "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), new CommandArgument("argValue2"), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs.Should().Equal("argValue2", "argValue1", "argValue0");
        }

        [Fact]
        public void BindArguments_Insufficient()
        {
            // void Test([Argument]string arg0, [Argument]string arg1, [Argument]string arg2);
            // new [] { "argValue0", "argValue1" }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), };

            Assert.Throws<Exception>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
        }

        [Fact]
        public void BindArguments_Insufficient_DefaultValue()
        {
            // void Test([Argument]string arg0, [Argument]string arg1, [Argument]string arg2 = "hello");
            // new [] { "argValue0", "argValue1" }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", new CoconaDefaultValue("hello")),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs.Should().Equal("argValue0", "argValue1", "hello");
        }

        [Fact]
        public void BindService()
        {
            // void Test([FromService]MyService arg0);
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandServiceParameterDescriptor(typeof(MyService)),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), new CommandArgument("argValue2"), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(1);
            invokeArgs[0].Should().BeOfType<MyService>();
        }

        [Fact]
        public void BindService_Argument()
        {
            // void Test([FromService]MyService arg0);
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandServiceParameterDescriptor(typeof(MyService)),
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), new CommandArgument("argValue2"), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(2);
            invokeArgs[0].Should().BeOfType<MyService>();
            invokeArgs[1].Should().Be("argValue0");
        }
    }
}
