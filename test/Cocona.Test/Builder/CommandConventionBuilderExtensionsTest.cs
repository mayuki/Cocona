using System;
using System.Collections.Generic;
using Cocona.Builder;
using Cocona.Builder.Metadata;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.Builder
{
    public class CommandConventionBuilderExtensionsTest
    {
        [Fact]
        public void WithName()
        {
            var conventions = new List<Action<ICommandBuilder>>();
            var conventionBuilder = new CommandConventionBuilder(conventions);
            var dataSource = new DelegateCommandDataSource(TestMethod, conventions);

            conventionBuilder.WithName("Hello");

            var data = dataSource.Build();
            data.Metadata[0].Should().BeOfType<CommandNameMetadata>().Subject.Name.Should().Be("Hello");
        }

        [Fact]
        public void WithDescription()
        {
            var conventions = new List<Action<ICommandBuilder>>();
            var conventionBuilder = new CommandConventionBuilder(conventions);
            var dataSource = new DelegateCommandDataSource(TestMethod, conventions);

            conventionBuilder.WithDescription("HelloDescription");

            var data = dataSource.Build();
            data.Metadata[0].Should().BeOfType<CommandDescriptionMetadata>().Subject.Description.Should().Be("HelloDescription");
        }

        [Fact]
        public void WithAliases()
        {
            var conventions = new List<Action<ICommandBuilder>>();
            var conventionBuilder = new CommandConventionBuilder(conventions);
            var dataSource = new DelegateCommandDataSource(TestMethod, conventions);

            conventionBuilder.WithAliases("foo", "bar");

            var data = dataSource.Build();
            data.Metadata[0].Should().BeOfType<CommandAliasesMetadata>().Subject.Aliases.Should().BeEquivalentTo(new [] { "foo", "bar" });
        }

        void TestMethod() => Console.WriteLine("Hello");
    }
}
