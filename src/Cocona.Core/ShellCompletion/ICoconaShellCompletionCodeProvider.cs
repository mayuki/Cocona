using System.Collections.Generic;
using System.IO;
using Cocona.Command;
using Cocona.ShellCompletion.Candidate;

namespace Cocona.ShellCompletion
{
    /// <summary>
    /// Provides to generate shell completion code and candidates.
    /// </summary>
    public interface ICoconaShellCompletionCodeProvider
    {
        /// <summary>
        /// Gets the shell names supported by the provider.
        /// </summary>
        IEnumerable<string> SupportedTargets { get; }

        /// <summary>
        /// Gets the value that indicates whether the provider can handle.
        /// </summary>
        /// <param name="target">The target shell name</param>
        /// <returns></returns>
        bool CanHandle(string target);

        /// <summary>
        /// Generates a shell completion support code for the target shell.
        /// </summary>
        /// <param name="target">The target shell name</param>
        /// <param name="writer">The <see cref="TextWriter"/> which to the code is written</param>
        /// <param name="commandCollection">The <see cref="CommandCollection"/></param>
        void Generate(string target, TextWriter writer, CommandCollection commandCollection);

        /// <summary>
        /// Generates formatted candidates for the target shell.
        /// </summary>
        /// <param name="target">The target shell name</param>
        /// <param name="writer">The <see cref="TextWriter"/> which to the code is written</param>
        /// <param name="values">The candidates</param>
        void GenerateOnTheFlyCandidates(string target, TextWriter writer, IReadOnlyList<CompletionCandidateValue> values);
    }
}
