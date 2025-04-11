using WildCoco.Command;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Generators;

namespace WildCoco.ShellCompletion;

/// <summary>
/// Default implementation of <see cref="IWildCocoShellCompletionCodeProvider"/>
/// </summary>
public class WildCocoShellCompletionCodeProvider : IWildCocoShellCompletionCodeProvider
{
    private readonly IWildCocoShellCompletionCodeGenerator[] _providers;

    public IEnumerable<string> SupportedTargets { get; }

    public bool CanHandle(string target)
        => _providers.Any(x => x.Targets.Contains(target));

    public WildCocoShellCompletionCodeProvider(IEnumerable<IWildCocoShellCompletionCodeGenerator> providers)
    {
        _providers = providers?.ToArray() ?? throw new ArgumentNullException(nameof(providers));
        SupportedTargets = _providers.SelectMany(xs => xs.Targets).ToArray();
    }

    public void Generate(string target, TextWriter writer, CommandCollection commandCollection)
    {
        var provider = _providers.First(x => x.Targets.Contains(target));
        provider.Generate(writer, commandCollection);
    }

    public void GenerateOnTheFlyCandidates(string target, TextWriter writer, IReadOnlyList<CompletionCandidateValue> values)
    {
        var provider = _providers.First(x => x.Targets.Contains(target));
        provider.GenerateOnTheFlyCandidates(writer, values);
    }
}
