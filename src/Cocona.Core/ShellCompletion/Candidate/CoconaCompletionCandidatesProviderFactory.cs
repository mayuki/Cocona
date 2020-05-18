using System;
using Cocona.Application;

namespace Cocona.ShellCompletion.Candidate
{
    public class CoconaCompletionCandidatesProviderFactory : ICoconaCompletionCandidatesProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICoconaInstanceActivator _activator;

        public CoconaCompletionCandidatesProviderFactory(IServiceProvider serviceProvider, ICoconaInstanceActivator activator)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _activator = activator ?? throw new ArgumentNullException(nameof(activator));
        }

        public ICoconaCompletionStaticCandidatesProvider CreateStaticProvider(CoconaCompletionCandidatesMetadata metadata)
        {
            if (!(typeof(ICoconaCompletionStaticCandidatesProvider).IsAssignableFrom(metadata.CandidatesProviderType)))
            {
                throw new InvalidOperationException($"Type '{metadata.CandidatesProviderType.FullName}' doesn't implement ICoconaCompletionStaticCandidatesProvider.");
            }

            return (ICoconaCompletionStaticCandidatesProvider)_activator.GetServiceOrCreateInstance(_serviceProvider, metadata.CandidatesProviderType!)!;
        }


        public ICoconaCompletionOnTheFlyCandidatesProvider CreateOnTheFlyProvider(CoconaCompletionCandidatesMetadata metadata)
        {
            if (!(typeof(ICoconaCompletionOnTheFlyCandidatesProvider).IsAssignableFrom(metadata.CandidatesProviderType)))
            {
                throw new InvalidOperationException($"Type '{metadata.CandidatesProviderType.FullName}' doesn't implement ICoconaCompletionOnTheFlyCandidatesProvider.");
            }

            return (ICoconaCompletionOnTheFlyCandidatesProvider)_activator.GetServiceOrCreateInstance(_serviceProvider, metadata.CandidatesProviderType!)!;
        }
    }
}
