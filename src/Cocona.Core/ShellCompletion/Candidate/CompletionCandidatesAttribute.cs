using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocona.ShellCompletion.Candidate.Providers;

namespace Cocona.ShellCompletion.Candidate
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class CompletionCandidatesAttribute : Attribute, ICoconaCompletionCandidatesStaticKeywords
    {
        private readonly CompletionCandidateValue[] _candidates;

        public CompletionCandidateType CandidateType { get; }
        public Type CandidatesProviderType { get; }
        IEnumerable<CompletionCandidateValue> ICoconaCompletionCandidatesStaticKeywords.Candidates => _candidates;

        public CompletionCandidatesAttribute(CompletionCandidateType candidateType)
        {
            switch (candidateType)
            {
                case CompletionCandidateType.Default:
                case CompletionCandidateType.File:
                case CompletionCandidateType.Directory:
                    break;
                default:
                    throw new InvalidOperationException($"CompletionCandidateType '{candidateType}' requires a custom provider type.");
            }

            _candidates = Array.Empty<CompletionCandidateValue>();
            CandidateType = candidateType;
            CandidatesProviderType = typeof(StaticCompletionCandidatesProvider);
        }

        public CompletionCandidatesAttribute(Type typeOfProvider)
        {
            if (!typeof(ICoconaCompletionOnTheFlyCandidatesProvider).IsAssignableFrom(typeOfProvider) &&
                !typeof(ICoconaCompletionStaticCandidatesProvider).IsAssignableFrom(typeOfProvider))
            {
                throw new ArgumentException($"The type '{typeOfProvider.FullName}' must implement ICoconaCompletionStaticCandidatesProvider or ICoconaCompletionOnTheFlyCandidatesProvider.");
            }

            CandidateType = CompletionCandidateType.Provider;
            _candidates = Array.Empty<CompletionCandidateValue>();
            CandidatesProviderType = typeOfProvider;
        }

        public CompletionCandidatesAttribute(string[] candidates)
        {
            CandidateType = CompletionCandidateType.Provider;
            CandidatesProviderType = typeof(StaticKeywordsCompletionCandidatesProvider);
            _candidates = candidates?.Select(x => new CompletionCandidateValue(x, string.Empty)).ToArray() ?? throw new ArgumentNullException(nameof(candidates));
        }
    }
}
