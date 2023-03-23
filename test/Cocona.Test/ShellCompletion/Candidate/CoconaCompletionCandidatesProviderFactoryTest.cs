#nullable enable
using System;
using System.Collections.Generic;
using Cocona.Application;
using Cocona.Command;
using Cocona.CommandLine;
using Cocona.ShellCompletion.Candidate;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cocona.Test.ShellCompletion.Candidate
{
    public class CoconaCompletionCandidatesProviderFactoryTest
    {
        [Fact]
        public void Static()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var activator = new CoconaInstanceActivator();

            var option = new CommandOptionDescriptor(
                typeof(int),
                "test",
                Array.Empty<char>(),
                "",
                CoconaDefaultValue.None,
                string.Empty,
                CommandOptionFlags.None,
                new Attribute[] { }
            );
            var factory = new CoconaCompletionCandidatesProviderFactory(serviceProvider, activator);
            var metadata = new CoconaCompletionCandidatesMetadata(
                CompletionCandidateType.Provider,
                typeof(TestStaticProvider),
                option
            );
            var provider = factory.CreateStaticProvider(metadata);
            provider.Should().BeOfType<TestStaticProvider>();
        }

        [Fact]
        public void OnTheFly()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var activator = new CoconaInstanceActivator();

            var option = new CommandOptionDescriptor(
                typeof(int),
                "test",
                Array.Empty<char>(),
                "",
                CoconaDefaultValue.None,
                string.Empty,
                CommandOptionFlags.None,
                new Attribute[] { }
            );
            var factory = new CoconaCompletionCandidatesProviderFactory(serviceProvider, activator);
            var metadata = new CoconaCompletionCandidatesMetadata(
                CompletionCandidateType.Provider,
                typeof(TestOnTheFlyProvider),
                option
            );
            var provider = factory.CreateOnTheFlyProvider(metadata);
            provider.Should().BeOfType<TestOnTheFlyProvider>();
        }

        private class TestStaticProvider : ICoconaCompletionStaticCandidatesProvider
        {
            public CompletionCandidateResult GetCandidates(CoconaCompletionCandidatesMetadata metadata)
            {
                throw new NotImplementedException();
            }
        }

        private class TestOnTheFlyProvider : ICoconaCompletionOnTheFlyCandidatesProvider
        {
            public IReadOnlyList<CompletionCandidateValue> GetCandidates(CoconaCompletionCandidatesMetadata metadata, ParsedCommandLine parsedCommandLine)
            {
                throw new NotImplementedException();
            }
        }
    }
}
