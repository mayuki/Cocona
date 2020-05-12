using System;
using System.Collections.Generic;
using System.Text;
using Cocona.Command;
using Cocona.ShellCompletion.Candidate;
using Cocona.ShellCompletion.Candidate.Providers;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.ShellCompletion.Candidate
{
    public class StaticKeywordsCompletionCandidatesProviderTest
    {
        [Fact]
        public void NoAttribute()
        {
            var argument = new CommandArgumentDescriptor(
                typeof(int),
                "test",
                0,
                "",
                CoconaDefaultValue.None,
                new Attribute[]
                {
                }
            );
            var metadata = new CoconaCompletionCandidatesMetadata(
                CompletionCandidateType.Provider,
                typeof(StaticKeywordsCompletionCandidatesProvider),
                argument
            );

            var provider = new StaticKeywordsCompletionCandidatesProvider();
            var candidates = provider.GetCandidates(metadata);
            candidates.ResultType.Should().Be(CompletionCandidateResultType.Default);
            candidates.Values.Should().BeEmpty();
        }

        [Fact]
        public void HasAttribute()
        {
            var argument = new CommandArgumentDescriptor(
                typeof(int),
                "test",
                0,
                "",
                CoconaDefaultValue.None,
                new Attribute[]
                {
                    new CompletionCandidatesAttribute(new [] { "Alice", "Karen" }),
                }
            );
            var metadata = new CoconaCompletionCandidatesMetadata(
                CompletionCandidateType.Provider,
                typeof(StaticKeywordsCompletionCandidatesProvider),
                argument
            );

            var provider = new StaticKeywordsCompletionCandidatesProvider();
            var candidates = provider.GetCandidates(metadata);
            candidates.ResultType.Should().Be(CompletionCandidateResultType.Keywords);
            candidates.Values.Should().HaveCount(2);
            candidates.Values[0].Value.Should().Be("Alice");
            candidates.Values[1].Value.Should().Be("Karen");
        }
    }
}
