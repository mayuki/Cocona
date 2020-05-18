using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocona.ShellCompletion.Candidate.Providers
{
    public sealed class StaticKeywordsCompletionCandidatesProvider : ICoconaCompletionStaticCandidatesProvider
    {
        public CompletionCandidateResult GetCandidates(CoconaCompletionCandidatesMetadata metadata)
        {
            var attr = metadata.ParameterAttributes.OfType<CompletionCandidatesAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return CompletionCandidateResult.Keywords(((ICoconaCompletionCandidatesStaticKeywords)attr).Candidates);
            }

            return CompletionCandidateResult.Default;
        }
    }

    internal interface ICoconaCompletionCandidatesStaticKeywords
    {
        IEnumerable<CompletionCandidateValue> Candidates { get; }
    }
}
