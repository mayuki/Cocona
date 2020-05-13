using System.Collections.Generic;
using System.IO;
using Cocona.Command;
using Cocona.ShellCompletion.Candidate;

namespace Cocona.ShellCompletion.Generators
{
    public interface ICoconaShellCompletionCodeGenerator
    {
        IReadOnlyList<string> Targets { get; }
        void Generate(TextWriter writer, CommandCollection commandCollection);
        void GenerateOnTheFlyCandidates(TextWriter writer, IReadOnlyList<CompletionCandidateValue> values);
    }
}
