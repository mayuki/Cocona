using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cocona.Command;
using FluentAssertions;
using Xunit;

#pragma warning disable 169
#pragma warning disable 649

namespace Cocona.Test.Command.CommandProvider
{
    public class CommandParameterSetTest
    {
        [Fact]
        public void ParameterSet()
        {
            var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ParameterSet)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());

            command.Parameters.Should().HaveCount(1);
            command.Options.Should().NotBeEmpty();
            command.Arguments.Should().NotBeEmpty();
            command.Options[0].Name.Should().Be("BooleanFlag");
            command.Options[1].Name.Should().Be("BooleanFlagImplicit");
            command.Arguments[0].Name.Should().Be("Argument");
        }

        [Fact]
        public void ParameterSetMustNotBeOption()
        {
            Assert.Throws<CoconaException>(() =>
            {
                var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ParameterSet_Option)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
            });
        }
        [Fact]
        public void ParameterSetMustNotBeArgument()
        {
            Assert.Throws<CoconaException>(() =>
            {
                var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ParameterSet_Argument)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
            });
        }

        [Fact]
        public void Duplicated_Option_Option()
        {
            Assert.Throws<CoconaException>(() =>
            {
                var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.OptionWithParameterSet_Duplicated)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
            });
        }

        [Fact]
        public void Duplicated_Option_Argument()
        {
            // Arguments can be declared multiple times.
            var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ArgumentWithParameterSet_Duplicated)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
        }

        //[Fact]
        //public void Record()
        //{
        //    var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ParameterSet_Record)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());

        //    command.Parameters.Should().HaveCount(1);
        //    command.Options.Should().NotBeEmpty();
        //    command.Arguments.Should().NotBeEmpty();
        //    command.Options[0].Name.Should().Be("BooleanFlag");
        //    command.Arguments[0].Name.Should().Be("Argument");
        //}

        [Fact]
        public void ClassIsNotMarkedAsParameterSet()
        {
            var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ParameterSet_NotICommandParameterSet)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
            command.Parameters[0].Should().NotBeOfType<CommandParameterSetDescriptor>();
        }

        [Fact]
        public void DefaultValue()
        {
            var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ParameterSet_DefaultValue)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
            command.Parameters[0].Should().BeOfType<CommandParameterSetDescriptor>();

            var paramSetDesc = ((CommandParameterSetDescriptor)command.Parameters[0]);
            
            // The boolean property is treated as optional flag implicitly.
            var paramDesc0 = (CommandOptionDescriptor)paramSetDesc.MemberDescriptors[0].ParameterDescriptor;
            paramDesc0.Name.Should().Be("BooleanOption");
            paramDesc0.DefaultValue.HasValue.Should().BeTrue();
            paramDesc0.DefaultValue.Value.Should().Be(false);

            var paramDesc1 = (CommandOptionDescriptor)paramSetDesc.MemberDescriptors[1].ParameterDescriptor;
            paramDesc1.Name.Should().Be("StringOption");
            paramDesc1.DefaultValue.HasValue.Should().BeTrue();
            paramDesc1.DefaultValue.Value.Should().Be("DefaultValue");

            var paramDesc2 = (CommandOptionDescriptor)paramSetDesc.MemberDescriptors[2].ParameterDescriptor;
            paramDesc2.Name.Should().Be("StringOption_WithoutHasDefaultValue");
            paramDesc2.DefaultValue.HasValue.Should().BeFalse();
        }

        [Fact]
        public void HasNoParameterlessConstructor()
        {
            Assert.Throws<CoconaException>(() =>
            {
                var command = new CoconaCommandProvider(Array.Empty<Type>()).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.ParameterSet_NoParameterlessConstructor)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
            });
        }

        class CommandTest
        {
            public void ParameterSet(TestParameterSet paramSet) { }
            public void ParameterSet_Option([Option] TestParameterSet paramSet) { }
            public void ParameterSet_Argument([Argument] TestParameterSet paramSet) { }
            public void OptionWithParameterSet(string option0, TestParameterSet paramSet) { }
            public void OptionWithParameterSet_Option(string option0, [Option] TestParameterSet paramSet) { }
            public void OptionWithParameterSet_Argument(string option0, [Argument] TestParameterSet paramSet) { }

            public void OptionWithParameterSet_Duplicated([Option] bool BooleanFlag, TestParameterSet paramSet) { }
            public void ArgumentWithParameterSet_Duplicated([Argument] string Argument, TestParameterSet paramSet) { }

            public void ParameterSet_Record(TestParameterSet_Record paramSet) { }
            public void ParameterSet_DefaultValue(TestParameterSet_DefaultValue paramSet) { }
            public void ParameterSet_NotICommandParameterSet(TestParameterSet_NotICommandParameterSet paramSet) { }
            public void ParameterSet_NoParameterlessConstructor(TestParameterSet_NoParameterlessConstructor paramSet) { }
        }

        class TestParameterSet_NotICommandParameterSet
        { }

        class TestParameterSet : ICommandParameterSet
        {
            [Option]
            public bool BooleanFlag { get; set; }
            public bool BooleanFlagImplicit { get; set; }
            [Argument]
            public string Argument { get; set; }
        }

        class TestParameterSet_Option_Prop_GetterOnly : ICommandParameterSet
        {
            [Option]
            public bool BooleanFlag { get; }
        }
        class TestParameterSet_Option_Prop_InitSetter : ICommandParameterSet
        {
            [Option]
            public bool BooleanFlag { get; init; }
        }
        class TestParameterSet_Option_Prop_PrivateSetter : ICommandParameterSet
        {
            [Option]
            public bool BooleanFlag { get; private set; }
        }
        class TestParameterSet_Option_Field : ICommandParameterSet
        {
            [Option] public bool BooleanFlag;
        }
        class TestParameterSet_Option_Field_Private : ICommandParameterSet
        {
            [Option] private bool BooleanFlag;
        }

        record TestParameterSet_Record([property: Option]bool BooleanFlag, [property: Argument] string Argument) : ICommandParameterSet;

        class TestParameterSet_DefaultValue : ICommandParameterSet
        {
            public bool BooleanOption { get; set; }
            [Option, HasDefaultValue]
            public string StringOption { get; set; } = "DefaultValue";
            [Option]
            public string StringOption_WithoutHasDefaultValue { get; set; } = "DefaultValue";
        }
        class TestParameterSet_NoParameterlessConstructor : ICommandParameterSet
        {
            [Option]
            public bool BooleanFlag { get; set; }

            public TestParameterSet_NoParameterlessConstructor(bool booleanFlag)
            {
                BooleanFlag = booleanFlag;
            }
        }

        private static MethodInfo GetMethod<T>(string methodName)
        {
            return typeof(T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public)!;
        }

    }
}
