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
            // Arguments: new [] { "argValue0", "argValue1", "argValue2" }
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
            // Arguments: new [] { "argValue0", "argValue1", "argValue2" }
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
            // Arguments: new [] { "argValue0", "argValue1" }
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
            // Arguments: new [] { "argValue0", "argValue1" }
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
        public void BindArguments_Array_First()
        {
            // void Test([Argument]string[] arg0, [Argument]string arg1, [Argument]string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "arg0", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), new CommandArgument("argValue2"), new CommandArgument("argValue3"), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs.Should().Equal(new[] { "argValue0", "argValue1", }, "argValue2", "argValue3");
        }

        [Fact]
        public void BindArguments_Array_Middle()
        {
            // void Test([Argument]string arg0, [Argument]string[] arg1, [Argument]string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string[]), "arg1", 1, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), new CommandArgument("argValue2"), new CommandArgument("argValue3"), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs.Should().Equal("argValue0", new[] { "argValue1", "argValue2", }, "argValue3");
        }

        [Fact]
        public void BindArguments_Array_Last()
        {
            // void Test([Argument]string arg0, [Argument]string arg1, [Argument]string[] arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string[]), "arg2", 2, "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), new CommandArgument("argValue2"), new CommandArgument("argValue3"), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs.Should().Equal("argValue0", "argValue1", new[] { "argValue2", "argValue3", });
        }

        [Fact]
        public void BindArguments_Array_Multiple_Invalid()
        {
            // void Test([Argument]string arg0, [Argument]string[] arg1, [Argument]string[] arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string[]), "arg1", 1, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string[]), "arg2", 2, "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), new CommandArgument("argValue2"), new CommandArgument("argValue3"), };

            Assert.Throws<Exception>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
        }

        [Fact]
        public void BindArguments_Enumerable_Multiple_Invalid()
        {
            // void Test([Argument]string arg0, [Argument]string[] arg1, [Argument]IReadOnlyList<string> arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string), "arg0", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string[]), "arg1", 1, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(IReadOnlyList<string>), "arg2", 2, "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), new CommandArgument("argValue2"), new CommandArgument("argValue3"), };

            Assert.Throws<Exception>(() => CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments));
        }

        [Fact]
        public void BindArguments_List()
        {
            // void Test([Argument]string[] arg0, [Argument]string arg1, [Argument]string arg2);
            // Arguments: new [] { "argValue0", "argValue1", "argValue2", "argValue3" }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(List<string>), "arg0", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg1", 1, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "arg2", 2, "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[] { };
            var commandArguments = new CommandArgument[] { new CommandArgument("argValue0"), new CommandArgument("argValue1"), new CommandArgument("argValue2"), new CommandArgument("argValue3"), };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(3);
            invokeArgs.Should().Equal(new List<string>() { "argValue0", "argValue1", }, "argValue2", "argValue3");
        }

        [Fact]
        public void BindOptions()
        {
            // void Test([Option]string option0, [Option]bool option1);
            // Options: --option0=foo --option1
            // Arguments: new [] { }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(string), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                    new CommandOptionDescriptor(typeof(bool), "option1", Array.Empty<char>(), "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo"),
                new CommandOption(commandDescriptor.Options[1], "true"),
            };
            var commandArguments = new CommandArgument[] { };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(2);
            invokeArgs[0].Should().Be("foo");
            invokeArgs[1].Should().Be(true);
        }

        [Fact]
        public void BindOptions_DefaultValue_1()
        {
            // void Test([Option]string option0, [Option]bool option1 = false);
            // Options: --option0=foo
            // Arguments: new [] { }
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(string), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                    new CommandOptionDescriptor(typeof(bool), "option1", Array.Empty<char>(), "", new CoconaDefaultValue(false)),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo"),
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
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(string), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                    new CommandOptionDescriptor(typeof(bool), "option1", Array.Empty<char>(), "", new CoconaDefaultValue(false)),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo"),
                new CommandOption(commandDescriptor.Options[1], "true"),
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
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(string[]), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo"),
                new CommandOption(commandDescriptor.Options[0], "bar"),
                new CommandOption(commandDescriptor.Options[0], "baz"),
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
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(List<string>), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo"),
                new CommandOption(commandDescriptor.Options[0], "bar"),
                new CommandOption(commandDescriptor.Options[0], "baz"),
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
            var commandDescriptor = new CommandDescriptor(
                typeof(BindParameterTest),
                "Test",
                Array.Empty<string>(),
                "",
                new CommandParameterDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(IReadOnlyList<string>), "option0", Array.Empty<char>(), "", CoconaDefaultValue.None),
                },
                typeof(void)
            );

            var commandOptions = new CommandOption[]
            {
                new CommandOption(commandDescriptor.Options[0], "foo"),
                new CommandOption(commandDescriptor.Options[0], "bar"),
                new CommandOption(commandDescriptor.Options[0], "baz"),
            };
            var commandArguments = new CommandArgument[] { };

            var invokeArgs = CreateCoconaParameterBinder().Bind(commandDescriptor, commandOptions, commandArguments);
            invokeArgs.Should().NotBeNull();
            invokeArgs.Should().HaveCount(1);
            invokeArgs[0].As<IReadOnlyList<string>>().Should().HaveCount(3);
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
