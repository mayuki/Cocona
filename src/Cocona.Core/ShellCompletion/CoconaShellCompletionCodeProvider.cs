using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cocona.Command;
using Cocona.ShellCompletion.Candidate;
using Cocona.ShellCompletion.Generators;

namespace Cocona.ShellCompletion
{
    /// <summary>
    /// Default implementation of <see cref="ICoconaShellCompletionCodeProvider"/>
    /// </summary>
    public class CoconaShellCompletionCodeProvider : ICoconaShellCompletionCodeProvider
    {
        private readonly ICoconaShellCompletionCodeGenerator[] _providers;

        public IEnumerable<string> SupportedTargets { get; }

        public bool CanHandle(string target)
            => _providers.Any(x => x.Targets.Contains(target));

        public CoconaShellCompletionCodeProvider(IEnumerable<ICoconaShellCompletionCodeGenerator> providers)
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
}
