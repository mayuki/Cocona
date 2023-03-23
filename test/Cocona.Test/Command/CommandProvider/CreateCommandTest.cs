using Cocona.Builder.Metadata;
using Cocona.Command;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Cocona.Test.Command.CommandProvider
{
    public class CreateCommandTest
    {
        [Fact]
        public void Default_NoOptions_NoArguments_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_NoOptions_NoArguments_ReturnInt()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnInt)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnInt));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(int));
        }

        [Fact]
        public void Default_NoOptions_NoArguments_ReturnTaskOfInt()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnTaskOfInt)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnTaskOfInt));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(Task<int>));
        }

        [Fact]
        public void Default_NoOptions_NoArguments_ReturnValueTaskOfInt()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnValueTaskOfInt)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnValueTaskOfInt));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(ValueTask<int>));
        }

        [Fact]
        public void Default_NoOptions_NoArguments_ReturnString()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnString)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_ReturnString));
            cmd.Options.Should().BeEmpty();
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(string));
        }

        [Fact]
        public void Default_ImplicitOptions1_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1WithDefaultValue_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasShortName_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasName_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasNameAndDescription_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_ImplicitOptions1_ExplicitOptions1HasDescription_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Options1_Arguments1_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Options1_Arguments2_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_HasArray_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_WithDefaultValue_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
        public void Default_Arguments_Required_After_Optional()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_Required_After_Optional)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);

            }).Message.Should().Be("The required arguments must be placed before the optional arguments. (Argument: arg2)");
        }

        [Fact]
        public void Default_Arguments_HasDescription_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_HasDescription_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_HasName_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_Arguments_Ordered_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
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
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_HasIgnoreParameter_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.Default_HasIgnoreParameter_NoReturn));
            cmd.Parameters.Should().HaveCount(3);
            cmd.Options.Should().HaveCount(1);
            cmd.Arguments.Should().HaveCount(1);
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Default_TreatBooleanOptionAsDefaultFalse_NoReturn()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_TreatBooleanOptionAsDefaultFalse_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.Default_TreatBooleanOptionAsDefaultFalse_NoReturn));
            cmd.Parameters.Should().HaveCount(1);
            cmd.Options.Should().HaveCount(1);
            cmd.Options[0].DefaultValue.HasValue.Should().BeTrue();
            cmd.Options[0].DefaultValue.Value.Should().Be(false);
            cmd.Arguments.Should().BeEmpty();
            cmd.Aliases.Should().BeEmpty();
            cmd.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Invalid_SameOptionName()
        {
            Assert.Throws<CoconaException>(() => new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Invalid_SameOptionName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default));
        }

        [Fact]
        public void Invalid_SameOptionShortName()
        {
            Assert.Throws<CoconaException>(() => new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Invalid_SameOptionShortName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default));
        }


        [Fact]
        public void DefaultCommandFlags()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn));
            cmd.Flags.Should().NotHaveFlag(CommandFlags.Hidden);
            cmd.Flags.Should().NotHaveFlag(CommandFlags.SubCommandsEntryPoint);
            cmd.Flags.Should().NotHaveFlag(CommandFlags.Primary);
            cmd.Flags.Should().NotHaveFlag(CommandFlags.IgnoreUnknownOptions);
        }

        [Fact]
        public void IgnoreUnknownOptions()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.IgnoreUnknownOptions)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.IgnoreUnknownOptions));
            cmd.Flags.Should().HaveFlag(CommandFlags.IgnoreUnknownOptions);
        }

        [Fact]
        public void IgnoreUnknownOptions_ClassLevel()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest_IgnoreUnknownOptions>(nameof(CommandTest_IgnoreUnknownOptions.IgnoreUnknownOptions)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.IgnoreUnknownOptions));
            cmd.Flags.Should().HaveFlag(CommandFlags.IgnoreUnknownOptions);
        }

        [Fact]
        public void Hidden()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Hidden)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.Hidden));
            cmd.Flags.Should().HaveFlag(CommandFlags.Hidden);
        }

        [Fact]
        public void HasOptionLikeCommands()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.HasOptionLikeCommands)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.HasOptionLikeCommands));
            cmd.OptionLikeCommands.Should().HaveCount(3);
            cmd.OptionLikeCommands[0].Name.Should().Be("foo");
            cmd.OptionLikeCommands[0].Flags.Should().HaveFlag(CommandOptionFlags.OptionLikeCommand);
            cmd.OptionLikeCommands[0].ShortName.Should().BeEmpty();
            cmd.OptionLikeCommands[1].Name.Should().Be("bar");
            cmd.OptionLikeCommands[1].ShortName.Should().Contain('b');
            cmd.OptionLikeCommands[1].Flags.Should().HaveFlag(CommandOptionFlags.OptionLikeCommand);
            cmd.OptionLikeCommands[2].Name.Should().Be("hidden");
            cmd.OptionLikeCommands[2].ShortName.Should().BeEmpty();
            cmd.OptionLikeCommands[2].Flags.Should().HaveFlag(CommandOptionFlags.OptionLikeCommand);
            cmd.OptionLikeCommands[2].Flags.Should().HaveFlag(CommandOptionFlags.Hidden);
        }

        [Fact]
        public void CommandMethodForwardedTo()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandMethodForwardedTo)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default);
            cmd.Name.Should().Be(nameof(CommandTest.CommandMethodForwardedTo));
            cmd.Description.Should().Be("CommandMethodForwardedTo-description");
            cmd.Method.Should().BeSameAs(typeof(CommandTest_CommandMethodForwardedToTarget).GetMethod(nameof(CommandTest_CommandMethodForwardedToTarget.CommandMethodForwardedToTarget)));
            cmd.Parameters.Should().HaveCount(1);
            cmd.ReturnType.Should().Be<int>();
        }

        [Fact]
        public void UseMetadata_CommandName()
        {
            var metadata = new object[]
            {
                new CommandNameMetadata("__CommandName"),
            };
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn)), isSingleCommand: true, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default, metadata);
            cmd.Name.Should().Be("__CommandName");
        }

        [Fact]
        public void UseMetadata_CommandDescription()
        {
            var metadata = new object[]
            {
                new CommandDescriptionMetadata("__CommandDescription"),
            };
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn)), isSingleCommand: true, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default, metadata);
            cmd.Description.Should().Be("__CommandDescription");
        }

        [Fact]
        public void UseMetadata_CommandAliases()
        {
            var metadata = new object[]
            {
                new CommandAliasesMetadata(new [] { "_alias1", "_alias2" }),
            };
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn)), isSingleCommand: true, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default, metadata);
            cmd.Aliases.Should().BeEquivalentTo(new[] { "_alias1", "_alias2" });
        }

        [Fact]
        public void UseMetadata_HiddenAttribute()
        {
            var metadata = new object[]
            {
                new HiddenAttribute(),
            };
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn)), isSingleCommand: true, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default, metadata);
            cmd.IsHidden.Should().BeTrue();
        }

        [Fact]
        public void UseMetadata_CommandAttribute()
        {
            var metadata = new object[]
            {
                new CommandAttribute("__CommandName")
                {
                    Description = "__CommandDescription",
                    Aliases = new [] { "_alias1", "_alias2" },
                }
            };
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.Default_NoOptions_NoArguments_NoReturn)), isSingleCommand: true, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(), default, metadata);
            cmd.Name.Should().Be("__CommandName");
            cmd.Description.Should().Be("__CommandDescription");
            cmd.Aliases.Should().BeEquivalentTo(new[] { "_alias1", "_alias2" });
        }

        [Fact]
        public void NullableAsOptional_Nullable_Enabled()
        {
#nullable enable
            void TestMethod(
                string option0NonNullable,
                int option1NonNullable,
                string? option2Nullable,
                int? option3Nullable,
                bool option4NonNullable,
                bool? option5Nullable,

                [Argument]
                string arg0NonNullable,
                [Argument]
                int arg1NonNullable,
                [Argument]
                string? arg2Nullable,
                [Argument]
                int? arg3Nullable
            )
            { }
#nullable restore
            var metadata = new object[]
            {
                new CommandNameMetadata("TestMethod"),
            };
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(
                new Action<string, int, string?, int?, bool, bool?, string, int, string?, int?>(TestMethod).Method,
                isSingleCommand: true,
                new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(),
                default,
                metadata
            );
            cmd.Name.Should().Be("TestMethod");
            cmd.Options.Should().HaveCount(6);
            // [NotNull] string
            cmd.Options[0].Name.Should().Be("option0NonNullable");
            cmd.Options[0].IsRequired.Should().BeTrue();
            cmd.Options[0].OptionType.Should().Be<string>();
            cmd.Options[0].DefaultValue.HasValue.Should().BeFalse();
            // [NotNull] int
            cmd.Options[1].Name.Should().Be("option1NonNullable");
            cmd.Options[1].IsRequired.Should().BeTrue();
            cmd.Options[1].OptionType.Should().Be<int>();
            cmd.Options[1].DefaultValue.HasValue.Should().BeFalse();
            // [Nullable] string
            cmd.Options[2].Name.Should().Be("option2Nullable");
            cmd.Options[2].IsRequired.Should().BeFalse();
            cmd.Options[2].OptionType.Should().Be<string>();
            cmd.Options[2].DefaultValue.HasValue.Should().BeTrue();
            cmd.Options[2].DefaultValue.Value.Should().Be(null);
            // [Nullable] int?
            cmd.Options[3].Name.Should().Be("option3Nullable");
            cmd.Options[3].IsRequired.Should().BeFalse();
            cmd.Options[3].OptionType.Should().Be<int?>();
            cmd.Options[3].DefaultValue.HasValue.Should().BeTrue();
            cmd.Options[3].DefaultValue.Value.Should().Be(null);
            // [NotNull] bool
            cmd.Options[4].Name.Should().Be("option4NonNullable");
            cmd.Options[4].IsRequired.Should().BeFalse(); // non-nullable bool is optional by default.
            cmd.Options[4].OptionType.Should().Be<bool>();
            cmd.Options[4].DefaultValue.HasValue.Should().BeTrue();
            cmd.Options[4].DefaultValue.Value.Should().Be(false);
            // [Nullable] bool?
            cmd.Options[5].Name.Should().Be("option5Nullable");
            cmd.Options[5].IsRequired.Should().BeFalse();
            cmd.Options[5].OptionType.Should().Be<bool?>();
            cmd.Options[5].DefaultValue.HasValue.Should().BeTrue();
            cmd.Options[5].DefaultValue.Value.Should().Be(null);

            cmd.Arguments.Should().HaveCount(4);
            // [NotNull] string
            cmd.Arguments[0].Name.Should().Be("arg0NonNullable");
            cmd.Arguments[0].IsRequired.Should().BeTrue();
            cmd.Arguments[0].ArgumentType.Should().Be<string>();
            cmd.Arguments[0].DefaultValue.HasValue.Should().BeFalse();
            // [NotNull] int
            cmd.Arguments[1].Name.Should().Be("arg1NonNullable");
            cmd.Arguments[1].IsRequired.Should().BeTrue();
            cmd.Arguments[1].ArgumentType.Should().Be<int>();
            cmd.Arguments[1].DefaultValue.HasValue.Should().BeFalse();
            // [Nullable] string?
            cmd.Arguments[2].Name.Should().Be("arg2Nullable");
            cmd.Arguments[2].IsRequired.Should().BeFalse();
            cmd.Arguments[2].ArgumentType.Should().Be<string>();
            cmd.Arguments[3].DefaultValue.HasValue.Should().BeTrue();
            cmd.Arguments[3].DefaultValue.Value.Should().Be(null);
            // [Nullable] int?
            cmd.Arguments[3].Name.Should().Be("arg3Nullable");
            cmd.Arguments[3].IsRequired.Should().BeFalse();
            cmd.Arguments[3].ArgumentType.Should().Be<int?>();
            cmd.Arguments[3].DefaultValue.HasValue.Should().BeTrue();
            cmd.Arguments[3].DefaultValue.Value.Should().Be(null);
        }

        [Fact]
        public void NullableAsOptional_Nullable_Disabled()
        {
#nullable disable
#pragma warning disable CS8632
            void TestMethod(
                string option0NonNullable, // Unknown
                int option1NonNullable, // NotNull
                string? option2Nullable, // Nullable
                int? option3Nullable, // Nullable
                bool option4NonNullable, // NotNull
                bool? option5Nullable, // Nullable

                [Argument]
                string arg0NonNullable, // Unknown
                [Argument]
                int arg1NonNullable, // NotNull
                [Argument]
                string? arg2Nullable, // Nullable
                [Argument]
                int? arg3Nullable // Nullable
            )
            { }
#pragma warning restore CS8632
#nullable restore
            var metadata = new object[]
            {
                new CommandNameMetadata("TestMethod"),
            };
            var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(
                new Action<string, int, string?, int?, bool, bool?, string, int, string?, int?>(TestMethod).Method,
                isSingleCommand: true,
                new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(),
                default,
                metadata
            );
            cmd.Name.Should().Be("TestMethod");
            cmd.Options.Should().HaveCount(6);
            cmd.Options[0].Name.Should().Be("option0NonNullable");
            cmd.Options[0].IsRequired.Should().BeTrue(); // Unknown
            cmd.Options[0].OptionType.Should().Be<string>();
            cmd.Options[1].Name.Should().Be("option1NonNullable");
            cmd.Options[1].IsRequired.Should().BeTrue(); // NotNull
            cmd.Options[1].OptionType.Should().Be<int>();
            cmd.Options[2].Name.Should().Be("option2Nullable");
            cmd.Options[2].IsRequired.Should().BeFalse(); // Nullable
            cmd.Options[2].OptionType.Should().Be<string>();
            cmd.Options[3].Name.Should().Be("option3Nullable");
            cmd.Options[3].IsRequired.Should().BeFalse(); // Nullable
            cmd.Options[3].OptionType.Should().Be<int?>();
            cmd.Options[4].Name.Should().Be("option4NonNullable");
            cmd.Options[4].IsRequired.Should().BeFalse(); // non-nullable bool is optional by default.
            cmd.Options[4].OptionType.Should().Be<bool>();
            cmd.Options[5].Name.Should().Be("option5Nullable");
            cmd.Options[5].IsRequired.Should().BeFalse(); // Nullable
            cmd.Options[5].OptionType.Should().Be<bool?>();
            cmd.Arguments.Should().HaveCount(4);
            cmd.Arguments[0].Name.Should().Be("arg0NonNullable");
            cmd.Arguments[0].IsRequired.Should().BeTrue(); // Unknown
            cmd.Arguments[0].ArgumentType.Should().Be<string>();
            cmd.Arguments[1].Name.Should().Be("arg1NonNullable");
            cmd.Arguments[1].IsRequired.Should().BeTrue(); // NotNull
            cmd.Arguments[1].ArgumentType.Should().Be<int>();
            cmd.Arguments[2].Name.Should().Be("arg2Nullable");
            cmd.Arguments[2].IsRequired.Should().BeFalse(); // Nullable
            cmd.Arguments[2].ArgumentType.Should().Be<string>();
            cmd.Arguments[3].Name.Should().Be("arg3Nullable");
            cmd.Arguments[3].IsRequired.Should().BeFalse(); // Nullable
            cmd.Arguments[3].ArgumentType.Should().Be<int?>();
        }

        [Fact]
        public void NullableAsOptional_ParameterSet_CanNotBe_Nullable()
        {
#nullable enable
            void TestMethod(ParameterSetTest_Parameterized? parameterSetTestParameterized)
            { }
#nullable restore
            var metadata = new object[]
            {
                new CommandNameMetadata("TestMethod"),
            };
            Assert.Throws<NotSupportedException>(() =>
            {
                var cmd = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(
                    new Action<ParameterSetTest_Parameterized?>(TestMethod).Method,
                    isSingleCommand: true,
                    new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(),
                    default,
                    metadata
                );
            });
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
            public void Default_Arguments_Required_After_Optional([Argument(Order = -10)]int? arg0, [Argument(Order = -20)]int arg1, [Argument(Order = 0)]int arg2) { } // arg1(Required), arg0(Optional), arg2(Required)
            public void Default_HasIgnoreParameter_NoReturn(string name, [Ignore]string ignored, [Argument]string arg0) { }
            public void Default_HasIgnoreValueTypeParameter_NoReturn(string name, [Ignore]int ignored, [Argument]string arg0) { }
            public void Default_TreatBooleanOptionAsDefaultFalse_NoReturn(bool flag0) { }

            public void Invalid_SameOptionName([Option("option")]string option0, [Option("option")]int option1) { }
            public void Invalid_SameOptionShortName([Option('o')]string option0, [Option('o')]int option1) { }

            [Hidden]
            public void Hidden() { }

            [IgnoreUnknownOptions]
            public void IgnoreUnknownOptions() { }

            [OptionLikeCommand("foo", new char[] { }, typeof(CommandTest), nameof(Default_NoOptions_NoArguments_NoReturn))]
            [OptionLikeCommand("bar", new char[] { 'b' }, typeof(CommandTest), nameof(Default_NoOptions_NoArguments_NoReturn))]
            [OptionLikeCommand("hidden", new char[] {}, typeof(CommandTest), nameof(Hidden))]
            public void HasOptionLikeCommands() { }

            [Command(Description = "CommandMethodForwardedTo-description")]
            [CommandMethodForwardedTo(typeof(CommandTest_CommandMethodForwardedToTarget), nameof(CommandTest_CommandMethodForwardedToTarget.CommandMethodForwardedToTarget))]
            public void CommandMethodForwardedTo() => throw new NotSupportedException();

            [Command("<>")]
            public void InvalidCommandName() => throw new NotSupportedException();
        }

        [IgnoreUnknownOptions]
        public class CommandTest_IgnoreUnknownOptions
        {
            public void IgnoreUnknownOptions() { }
        }

        public class CommandTest_CommandMethodForwardedToTarget
        {
            public int CommandMethodForwardedToTarget([Argument]string arg0) => 0;
        }

        public record ParameterSetTest_Parameterized(int ValueA, string ValueB) : ICommandParameterSet;
    }
}
