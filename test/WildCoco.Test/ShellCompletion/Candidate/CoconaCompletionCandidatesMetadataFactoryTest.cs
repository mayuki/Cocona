using WildCoco.Command;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Candidate.Providers;
using WildCoco.Command;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Candidate.Providers;

namespace WildCoco.Test.ShellCompletion.Candidate;

public class WildCocoCompletionCandidatesMetadataFactoryTest
{
    [Fact]
    public void GetMetadata_Option_Default()
    {
        var factory = new WildCocoCompletionCandidatesMetadataFactory();
        var option = new CommandOptionDescriptor(
            typeof(int),
            "test",
            Array.Empty<char>(),
            "",
            WildCocoDefaultValue.None,
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
        var factory = new WildCocoCompletionCandidatesMetadataFactory();
        var option = new CommandOptionDescriptor(
            typeof(TestEnum),
            "test",
            Array.Empty<char>(),
            "",
            WildCocoDefaultValue.None,
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
