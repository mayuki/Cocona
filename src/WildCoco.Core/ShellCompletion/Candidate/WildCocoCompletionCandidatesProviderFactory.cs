using WildCoco.Application;

namespace WildCoco.ShellCompletion.Candidate;

public class WildCocoCompletionCandidatesProviderFactory : IWildCocoCompletionCandidatesProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IWildCocoInstanceActivator _activator;

    public WildCocoCompletionCandidatesProviderFactory(IServiceProvider serviceProvider, IWildCocoInstanceActivator activator)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _activator = activator ?? throw new ArgumentNullException(nameof(activator));
    }

    public IWildCocoCompletionStaticCandidatesProvider CreateStaticProvider(WildCocoCompletionCandidatesMetadata metadata)
    {
        if (!(typeof(IWildCocoCompletionStaticCandidatesProvider).IsAssignableFrom(metadata.CandidatesProviderType)))
        {
            throw new InvalidOperationException($"Type '{metadata.CandidatesProviderType.FullName}' doesn't implement IWildCocoCompletionStaticCandidatesProvider.");
        }

        return (IWildCocoCompletionStaticCandidatesProvider)_activator.GetServiceOrCreateInstance(_serviceProvider, metadata.CandidatesProviderType!)!;
    }


    public IWildCocoCompletionOnTheFlyCandidatesProvider CreateOnTheFlyProvider(WildCocoCompletionCandidatesMetadata metadata)
    {
        if (!(typeof(IWildCocoCompletionOnTheFlyCandidatesProvider).IsAssignableFrom(metadata.CandidatesProviderType)))
        {
            throw new InvalidOperationException($"Type '{metadata.CandidatesProviderType.FullName}' doesn't implement IWildCocoCompletionOnTheFlyCandidatesProvider.");
        }

        return (IWildCocoCompletionOnTheFlyCandidatesProvider)_activator.GetServiceOrCreateInstance(_serviceProvider, metadata.CandidatesProviderType!)!;
    }
}
