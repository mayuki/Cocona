using System.Collections.Generic;
using System.IO;
using Cocona.Command;
using Cocona.ShellCompletion.Candidate;

namespace Cocona.ShellCompletion.Generators
{
    /// <summary>
    /// Generates the shell code for the completion.
    /// </summary>
    public interface ICoconaShellCompletionCodeGenerator
    {
        /// <summary>
        /// Gets the shell names that supported by the generator.
        /// </summary>
        IReadOnlyList<string> Targets { get; }

        /// <summary>
        /// Generates a shell code for the completion.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to be output.</param>
        /// <param name="commandCollection">The <see cref="CommandCollection"/>.</param>
        void Generate(TextWriter writer, CommandCollection commandCollection);

        /// <summary>
        /// Generates completion candidates for shell on the fly.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to be output.</param>
        /// <param name="values">The on-the-fly candidates.</param>
        void GenerateOnTheFlyCandidates(TextWriter writer, IReadOnlyList<CompletionCandidateValue> values);
    }
}
