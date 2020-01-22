using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cocona.Command;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.Command.BuiltIn
{
    public class ToCommandCaseTest
    {
        [Fact]
        public void DisableCommandNameConversion()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>(), enableConvertCommandNameToLowerCase: false).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
            cmd.Name.Should().Be("CommandName");
            cmd.Options[0].Name.Should().Be("dryRun");
        }
        [Fact]
        public void EnableCommandNameConversion()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>(), enableConvertCommandNameToLowerCase: true).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
            cmd.Name.Should().Be("command-name");
            cmd.Options[0].Name.Should().Be("dryRun");
        }
        [Fact]
        public void DisableOptionNameConversion()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>(), enableConvertOptionNameToLowerCase: false).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
            cmd.Name.Should().Be("CommandName");
            cmd.Options[0].Name.Should().Be("dryRun");
        }
        [Fact]
        public void EnableOptionNameConversion()
        {
            var cmd = new CoconaCommandProvider(Array.Empty<Type>(), enableConvertOptionNameToLowerCase: true).CreateCommand(GetMethod<CommandTest>(nameof(CommandTest.CommandName)), false, new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>());
            cmd.Name.Should().Be("CommandName");
            cmd.Options[0].Name.Should().Be("dry-run");
        }

        class CommandTest
        {
            public void CommandName(bool dryRun)
            {
            }
        }

        private static MethodInfo GetMethod<T>(string methodName)
        {
            return typeof(T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        }

        [Fact]
        public void NoProcess()
        {
            CoconaCommandProvider.ToCommandCase("foo").Should().Be("foo");
        }

        [Fact]
        public void ToKebabCase()
        {
            CoconaCommandProvider.ToCommandCase("fooBar").Should().Be("foo-bar");
        }

        [Fact]
        public void ToKebabCase_FollowingUpperCase()
        {
            CoconaCommandProvider.ToCommandCase("enableUI").Should().Be("enable-ui");
        }

        [Fact]
        public void ToKebabCase_FollowingUpperCase_2()
        {
            CoconaCommandProvider.ToCommandCase("enableUIWindow").Should().Be("enable-uiwindow");
        }

        [Fact]
        public void ToKebabCase_ManyUpperCase()
        {
            CoconaCommandProvider.ToCommandCase("BooleanTrueByDefault").Should().Be("boolean-true-by-default");
        }

        [Fact]
        public void UpperCase()
        {
            CoconaCommandProvider.ToCommandCase("Enable").Should().Be("enable");
        }

        [Fact]
        public void Number()
        {
            CoconaCommandProvider.ToCommandCase("option0").Should().Be("option0");
        }
    }
}
