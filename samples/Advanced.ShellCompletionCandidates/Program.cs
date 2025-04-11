using WildCoco.CommandLine;
using WildCoco.ShellCompletion.Candidate;
using WildCoco;
using WildCoco.CommandLine;
using WildCoco.ShellCompletion.Candidate;

namespace WildCocoSample.Advanced.ShellCompletionCandidates;

class Program
{
    static void Main(string[] args)
    {
        WildCocoApp.Run<Program>(args, options =>
        {
            options.EnableShellCompletionSupport = true;
        });
    }

    public void Inline([CompletionCandidates(new [] { "Foo", "Bar", "Baz" })] string name)
    {
        Console.WriteLine($"Hello {name}!");
    }

    public void Static([CompletionCandidates(typeof(StaticMemberNameProvider))] string[] name)
    {
        Console.WriteLine($"Hello {name}!");
    }

    public void OnTheFly([CompletionCandidates(typeof(OnTheFlyMemberNameProvider))] string name, [CompletionCandidates(new [] { "TrySail", "Sphere" })]string group)
    {
        Console.WriteLine($"Hello {name}!");
    }
}

class StaticMemberNameProvider : IWildCocoCompletionStaticCandidatesProvider
{
    private readonly CompletionCandidateResult _results = CompletionCandidateResult.Keywords(
        new CompletionCandidateValue("Karen", ""),
        new CompletionCandidateValue("Alice", ""),
        new CompletionCandidateValue("Shino", ""),
        new CompletionCandidateValue("Yoko", ""),
        new CompletionCandidateValue("Aya", "")
    );

    public CompletionCandidateResult GetCandidates(WildCocoCompletionCandidatesMetadata metadata) => _results;
}

class OnTheFlyMemberNameProvider : IWildCocoCompletionOnTheFlyCandidatesProvider
{
    private readonly CompletionCandidateValue[] _resultsSphere =
    {
        new CompletionCandidateValue("Aki", "Aki Toyosaki"),
        new CompletionCandidateValue("Minako", "Minako Kotobuki"),
        new CompletionCandidateValue("Haruka", "Haruka Tomatsu"),
        new CompletionCandidateValue("Ayahi", "Ayahi Takagaki")
    };
    private readonly CompletionCandidateValue[] _resultsTrySail =
    {
        new CompletionCandidateValue("Sora", "Sora Amamiya"),
        new CompletionCandidateValue("Momo", "Momo Asakura"),
        new CompletionCandidateValue("Shiina", "Shiina Natsukawa")
    };

    public IReadOnlyList<CompletionCandidateValue> GetCandidates(WildCocoCompletionCandidatesMetadata metadata, ParsedCommandLine parsedCommandLine)
        => parsedCommandLine.Options.Any(x => x.Option.Name == "group" && string.Equals(x.Value, "trysail", StringComparison.OrdinalIgnoreCase))
            ? _resultsTrySail
            : _resultsSphere;
}
