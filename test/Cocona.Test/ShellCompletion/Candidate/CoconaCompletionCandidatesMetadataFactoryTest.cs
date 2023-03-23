using System;
using Cocona.Command;
using Cocona.ShellCompletion.Candidate;
using Cocona.ShellCompletion.Candidate.Providers;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.ShellCompletion.Candidate
{
    public class CoconaCompletionCandidatesMetadataFactoryTest
    {
        [Fact]
        public void GetMetadata_Option_Default()
        {
            var factory = new CoconaCompletionCandidatesMetadataFactory();
            var option = new CommandOptionDescriptor(
                typeof(int),
                "test",
                Array.Empty<char>(),
                "",
                CoconaDefaultValue.None,
                string.Empty,
                CommandOptionFlags.None,
                new Attribute[]{  }
            );

            var metadata = factory.GetMetadata(option);
            metadata.CandidateType.Should().Be(CompletionCandidateType.Default);
            metadata.CandidatesProviderType.Should().Be<StaticCompletionCandidatesProvider>();
        }

        [Fact]
        public void GetMetadata_Option_Enum()
        {
            var factory = new CoconaCompletionCandidatesMetadataFactory();
            var option = new CommandOptionDescriptor(
                typeof(TestEnum),
                "test",
                Array.Empty<char>(),
                "",
                CoconaDefaultValue.None,
                string.Empty,
                CommandOptionFlags.None,
                new Attribute[] { }
            );

            var metadata = factory.GetMetadata(option);
            metadata.CandidateType.Should().Be(CompletionCandidateType.Provider);
            metadata.CandidatesProviderType.Should().Be<EnumCompletionCandidatesProvider>();
        }

        enum TestEnum
        {
            Alice,
            Karen
        }
    }
}
