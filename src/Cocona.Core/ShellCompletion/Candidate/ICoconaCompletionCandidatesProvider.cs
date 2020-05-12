using System;
using System.Collections.Generic;
using System.Text;
using Cocona.Command;
using Cocona.CommandLine;

namespace Cocona.ShellCompletion.Candidate
{
    public interface ICoconaCompletionStaticCandidatesProvider
    {
        CompletionCandidateResult GetCandidates(CoconaCompletionCandidatesMetadata metadata);
    }

    public interface ICoconaCompletionOnTheFlyCandidatesProvider
    {
        IReadOnlyList<CompletionCandidateValue> GetCandidates(CoconaCompletionCandidatesMetadata metadata, ParsedCommandLine parsedCommandLine);
    }
}
