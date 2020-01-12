using Cocona.Command;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cocona.Test.Command.CommandProvider
{
    public class CreateCommandTest
    {
        [Fact]
        public void Default_NoOptions_NoArguments_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_NoOptions_NoArguments_ReturnInt()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnInt)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnInt));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(int));
        }

        [Fact]
        public void Default_NoOptions_NoArguments_ReturnTaskOfInt()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnTaskOfInt)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnTaskOfInt));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(Task<int>));
        }

        [Fact]
        public void Default_NoOptions_NoArguments_ReturnValueTaskOfInt()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnValueTaskOfInt)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnValueTaskOfInt));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(ValueTask<int>));
        }

        [Fact]
        public void Default_NoOptions_NoArguments_ReturnString()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnString)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnString));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(string));
        }

        [Fact]
        public void Default_ImplicitOptions1_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_ImplicitOptions1_NoReturn));
            cmd.Options.Should().HaveCount(1);
            cmd.Options[0].Name.Should().Be("option0");
            cmd.Options[0].OptionType.Should().Be(typeof(string));
            cmd.Options[0].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[0].IsRequired.Should().BeTrue();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_ImplicitOptions1WithDefaultValue_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1WithDefaultValue_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_ImplicitOptions1WithDefaultValue_NoReturn));
            cmd.Options.Should().HaveCount(3);
            cmd.Options[0].Name.Should().Be("option0");
            cmd.Options[0].OptionType.Should().Be(typeof(string));
            cmd.Options[0].DefaultValue.HasValue.Should().BeTrue();
            cmd.Options[0].DefaultValue.Value.Should().Be("Hello");
            cmd.Options[0].IsRequired.Should().BeFalse();
            cmd.Options[1].Name.Should().Be("option1");
            cmd.Options[1].OptionType.Should().Be(typeof(int));
            cmd.Options[1].DefaultValue.HasValue.Should().BeTrue();
            cmd.Options[1].DefaultValue.Value.Should().Be(int.MaxValue);
            cmd.Options[1].IsRequired.Should().BeFalse();
            cmd.Options[2].Name.Should().Be("option2");
            cmd.Options[2].OptionType.Should().Be(typeof(bool?));
            cmd.Options[2].DefaultValue.HasValue.Should().BeTrue();
            cmd.Options[2].DefaultValue.Value.Should().Be(null);
            cmd.Options[2].IsRequired.Should().BeFalse();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_ImplicitOptions1_ExplicitOptions1_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1_NoReturn));
            cmd.Options.Should().HaveCount(2);
            cmd.Options[0].Name.Should().Be("option0");
            cmd.Options[0].OptionType.Should().Be(typeof(string));
            cmd.Options[0].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[0].IsRequired.Should().BeTrue();
            cmd.Options[1].Name.Should().Be("option1");
            cmd.Options[1].OptionType.Should().Be(typeof(int));
            cmd.Options[1].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[1].IsRequired.Should().BeTrue();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_ImplicitOptions1_ExplicitOptions1HasShortName_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasShortName_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasShortName_NoReturn));
            cmd.Options.Should().HaveCount(2);
            cmd.Options[0].Name.Should().Be("option0");
            cmd.Options[0].OptionType.Should().Be(typeof(string));
            cmd.Options[0].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[0].IsRequired.Should().BeTrue();
            cmd.Options[1].Name.Should().Be("option1");
            cmd.Options[1].ShortName.Should().Contain('o');
            cmd.Options[1].OptionType.Should().Be(typeof(int));
            cmd.Options[1].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[1].IsRequired.Should().BeTrue();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_ImplicitOptions1_ExplicitOptions1HasName_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasName_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasName_NoReturn));
            cmd.Options.Should().HaveCount(2);
            cmd.Options[0].Name.Should().Be("option0");
            cmd.Options[0].OptionType.Should().Be(typeof(string));
            cmd.Options[0].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[0].IsRequired.Should().BeTrue();
            cmd.Options[1].Name.Should().Be("yet_another_option1");
            cmd.Options[1].ShortName.Should().BeEmpty();
            cmd.Options[1].OptionType.Should().Be(typeof(int));
            cmd.Options[1].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[1].IsRequired.Should().BeTrue();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_ImplicitOptions1_ExplicitOptions1HasNameAndDescription_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasNameAndDescription_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasNameAndDescription_NoReturn));
            cmd.Options.Should().HaveCount(2);
            cmd.Options[0].Name.Should().Be("option0");
            cmd.Options[0].OptionType.Should().Be(typeof(string));
            cmd.Options[0].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[0].IsRequired.Should().BeTrue();
            cmd.Options[1].Name.Should().Be("yet_another_option1");
            cmd.Options[1].ShortName.Should().BeEmpty();
            cmd.Options[1].OptionType.Should().Be(typeof(int));
            cmd.Options[1].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[1].IsRequired.Should().BeTrue();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_ImplicitOptions1_ExplicitOptions1HasDescription_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasDescription_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasDescription_NoReturn));
            cmd.Options.Should().HaveCount(2);
            cmd.Options[0].Name.Should().Be("option0");
            cmd.Options[0].OptionType.Should().Be(typeof(string));
            cmd.Options[0].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[0].IsRequired.Should().BeTrue();
            cmd.Options[1].Name.Should().Be("option1");
            cmd.Options[1].Description.Should().Be("this is option1");
            cmd.Options[1].ShortName.Should().BeEmpty();
            cmd.Options[1].OptionType.Should().Be(typeof(int));
            cmd.Options[1].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[1].IsRequired.Should().BeTrue();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_Options1_Arguments1_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Options1_Arguments1_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_Options1_Arguments1_NoReturn));
            cmd.Options.Should().HaveCount(1);
            cmd.Options[0].Name.Should().Be("option0");
            cmd.Options[0].OptionType.Should().Be(typeof(string));
            cmd.Options[0].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[0].IsRequired.Should().BeTrue();
            cmd.Arguments.Should().HaveCount(1);
            cmd.Arguments[0].Name.Should().Be("arg0");
            cmd.Arguments[0].Order.Should().Be(0);
            cmd.Arguments[0].ArgumentType.Should().Be(typeof(int));
            cmd.Arguments[0].IsRequired.Should().BeTrue();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_Options1_Arguments2_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Options1_Arguments2_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_Options1_Arguments2_NoReturn));
            cmd.Options.Should().HaveCount(1);
            cmd.Options[0].Name.Should().Be("option0");
            cmd.Options[0].OptionType.Should().Be(typeof(string));
            cmd.Options[0].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[0].IsRequired.Should().BeTrue();
            cmd.Arguments.Should().HaveCount(2);
            cmd.Arguments[0].Name.Should().Be("arg0");
            cmd.Arguments[0].Order.Should().Be(0);
            cmd.Arguments[0].ArgumentType.Should().Be(typeof(int));
            cmd.Arguments[0].IsRequired.Should().BeTrue();
            cmd.Arguments[1].Name.Should().Be("arg1");
            cmd.Arguments[1].Order.Should().Be(1);
            cmd.Arguments[1].ArgumentType.Should().Be(typeof(string));
            cmd.Arguments[1].IsRequired.Should().BeTrue();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_Arguments_HasArray_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_HasArray_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_Arguments_HasArray_NoReturn));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().HaveCount(2);
            cmd.Arguments[0].Name.Should().Be("arg0");
            cmd.Arguments[0].Order.Should().Be(0);
            cmd.Arguments[0].ArgumentType.Should().Be(typeof(int));
            cmd.Arguments[0].IsRequired.Should().BeTrue();
            cmd.Arguments[1].Name.Should().Be("arg1");
            cmd.Arguments[1].Order.Should().Be(1);
            cmd.Arguments[1].ArgumentType.Should().Be(typeof(string[]));
            cmd.Arguments[1].IsEnumerableLike.Should().BeTrue();
            cmd.Arguments[1].IsRequired.Should().BeTrue();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_Arguments_WithDefaultValue_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_WithDefaultValue_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_Arguments_WithDefaultValue_NoReturn));
            cmd.Options.Should().HaveCount(1);
            cmd.Options[0].Name.Should().Be("option0");
            cmd.Options[0].OptionType.Should().Be(typeof(string));
            cmd.Options[0].DefaultValue.HasValue.Should().BeFalse();
            cmd.Options[0].IsRequired.Should().BeTrue();
            cmd.Arguments.Should().HaveCount(2);
            cmd.Arguments[0].Name.Should().Be("arg0");
            cmd.Arguments[0].Order.Should().Be(0);
            cmd.Arguments[0].ArgumentType.Should().Be(typeof(int));
            cmd.Arguments[0].DefaultValue.Value.Should().Be(12345);
            cmd.Arguments[0].IsRequired.Should().BeFalse();
            cmd.Arguments[1].Name.Should().Be("arg1");
            cmd.Arguments[1].Order.Should().Be(1);
            cmd.Arguments[1].ArgumentType.Should().Be(typeof(string));
            cmd.Arguments[1].IsEnumerableLike.Should().BeFalse();
            cmd.Arguments[1].DefaultValue.Value.Should().BeNull();
            cmd.Arguments[1].IsRequired.Should().BeFalse();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_Arguments_HasDescription_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_HasDescription_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_Arguments_HasDescription_NoReturn));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().HaveCount(1);
            cmd.Arguments[0].Name.Should().Be("arg0");
            cmd.Arguments[0].Description.Should().Be("arg no.0");
            cmd.Arguments[0].Order.Should().Be(0);
            cmd.Arguments[0].ArgumentType.Should().Be(typeof(int));
            cmd.Arguments[0].IsRequired.Should().BeTrue();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_Arguments_HasName_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_HasName_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_Arguments_HasName_NoReturn));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().HaveCount(1);
            cmd.Arguments[0].Name.Should().Be("yet_another_arg0");
            cmd.Arguments[0].Order.Should().Be(0);
            cmd.Arguments[0].ArgumentType.Should().Be(typeof(int));
            cmd.Arguments[0].IsRequired.Should().BeTrue();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_Arguments_Ordered_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_Ordered_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_Arguments_Ordered_NoReturn));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().HaveCount(2);
            cmd.Arguments[0].Name.Should().Be("arg0");
            cmd.Arguments[0].Order.Should().Be(5);
            cmd.Arguments[0].ArgumentType.Should().Be(typeof(int[]));
            cmd.Arguments[0].IsRequired.Should().BeTrue();
            cmd.Arguments[1].Name.Should().Be("arg1");
            cmd.Arguments[1].Order.Should().Be(int.MinValue);
            cmd.Arguments[1].ArgumentType.Should().Be(typeof(string[]));
            cmd.Arguments[1].IsRequired.Should().BeTrue();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_HasIgnoreParameter_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_HasIgnoreParameter_NoReturn)), false);
            cmd.Name.Should().Be(nameof(CommandTest.Default_HasIgnoreParameter_NoReturn));
            cmd.Parameters.Should().HaveCount(3);
            cmd.Options.Should().HaveCount(1);
            cmd.Arguments.Should().HaveCount(1);
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Invalid_SameOptionName()
        {
            Assert.Throws<CoconaException>(() => new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Invalid_SameOptionName)), false));
        }

        [Fact]
        public void Invalid_SameOptionShortName()
        {
            Assert.Throws<CoconaException>(() => new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Invalid_SameOptionShortName)), false));
        }

        private static MethodInfo GetMethod<T>(string methodName)
        {
            return typeof(T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        }

        public class CommandTest
        {
            public void Default_NoOptions_NoArguments_NoReturn() { }
            public int Default_NoOptions_NoArguments_ReturnInt() => 12345;
            public Task<int> Default_NoOptions_NoArguments_ReturnTaskOfInt() => Task.FromResult(54321);
            public ValueTask<int> Default_NoOptions_NoArguments_ReturnValueTaskOfInt() => new ValueTask<int>(54321);
            public string Default_NoOptions_NoArguments_ReturnString() => string.Empty;
            public void Default_ImplicitOptions1_NoReturn(string option0) { }
            public void Default_ImplicitOptions1WithDefaultValue_NoReturn(string option0 = "Hello", int option1 = int.MaxValue, bool? option2 = null) { }
            public void Default_ImplicitOptions1_ExplicitOptions1_NoReturn(string option0, [Option]int option1) { }
            public void Default_ImplicitOptions1_ExplicitOptions1HasShortName_NoReturn(string option0, [Option('o')]int option1) { }
            public void Default_ImplicitOptions1_ExplicitOptions1HasName_NoReturn(string option0, [Option("yet_another_option1")]int option1) { }
            public void Default_ImplicitOptions1_ExplicitOptions1HasNameAndDescription_NoReturn(string option0, [Option("yet_another_option1", Description = "this is option1")]int option1) { }
            public void Default_ImplicitOptions1_ExplicitOptions1HasDescription_NoReturn(string option0, [Option(Description = "this is option1")]int option1) { }
            public void Default_Options1_Arguments1_NoReturn(string option0, [Argument]int arg0) { }
            public void Default_Options1_Arguments2_NoReturn(string option0, [Argument]int arg0, [Argument]string arg1) { }
            public void Default_Arguments_HasArray_NoReturn([Argument]int arg0, [Argument]string[] arg1) { }
            public void Default_Arguments_WithDefaultValue_NoReturn(string option0, [Argument]int arg0 = 12345, [Argument]string arg1 = null) { }
            public void Default_Arguments_HasDescription_NoReturn([Argument(Description = "arg no.0")]int arg0) { }
            public void Default_Arguments_HasName_NoReturn([Argument("yet_another_arg0")]int arg0) { }
            public void Default_Arguments_Ordered_NoReturn([Argument(Order = 5)]int[] arg0, [Argument(Order = int.MinValue)]string[] arg1) { }
            public void Default_HasIgnoreParameter_NoReturn(string name, [Ignore]string ignored, [Argument]string arg0) { }
            public void Default_HasIgnoreValueTypeParameter_NoReturn(string name, [Ignore]int ignored, [Argument]string arg0) { }

            public void Invalid_SameOptionName([Option("option")]string option0, [Option("option")]int option1) { }
            public void Invalid_SameOptionShortName([Option('o')]string option0, [Option('o')]int option1) { }
        }

    }
}
