using System;
using System.Collections.Generic;
using System.IO;
using Cocona.Application;
using Cocona.Command;
using Cocona.ShellCompletion.Candidate;
using Cocona.ShellCompletion.Generators;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.ShellCompletion
{
    public class BashCoconaShellCompletionCodeGeneratorTest
    {
        [Fact]
        public void Generate()
        {
            var provider = new BashCoconaShellCompletionCodeGenerator(new CoconaApplicationMetadataProvider(), new TestCompletionCandidates());
            provider.Targets.Should().BeEquivalentTo("bash");

            var writer = new StringWriter();
            provider.Generate(writer, new CommandCollection(Array.Empty<CommandDescriptor>()));
        }

        class TestCompletionCandidates : ICoconaCompletionCandidates
        {
            public StaticCompletionCandidates GetStaticCandidatesFromOption(CommandOptionDescriptor option)
            {
                return new StaticCompletionCandidates(CompletionCandidateResult.Default);
            }

            public StaticCompletionCandidates GetStaticCandidatesFromArgument(CommandArgumentDescriptor argument)
            {
                return new StaticCompletionCandidates(CompletionCandidateResult.Default);
            }

            public IReadOnlyList<CompletionCandidateValue> GetOnTheFlyCandidates(string paramName, IReadOnlyList<string> arguments, int curPos, string? candidateHint)
            {
                throw new NotImplementedException();
            }
        }
    }
}
