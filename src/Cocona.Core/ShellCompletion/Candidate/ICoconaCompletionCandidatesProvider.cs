using System;
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
        CompletionCandidateResult GetCandidates(CoconaCompletionCandidatesMetadata metadata, ParsedCommandLine parsedCommandLine);
    }
}
