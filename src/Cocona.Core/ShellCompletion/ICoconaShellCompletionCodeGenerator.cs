using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cocona.Command;

namespace Cocona.ShellCompletion
{
    public interface ICoconaShellCompletionCodeGenerator
    {
        IEnumerable<string> SupportedTargets { get; }
        bool CanHandle(string target);
        void Generate(string target, TextWriter writer, CommandCollection commandCollection);
    }

    public class CoconaShellCompletionCodeGenerator : ICoconaShellCompletionCodeGenerator
    {
        private readonly ICoconaShellCompletionCodeProvider[] _providers;

        public IEnumerable<string> SupportedTargets { get; }

        public bool CanHandle(string target)
            => _providers.Any(x => x.Targets.Contains(target));

        public CoconaShellCompletionCodeGenerator(IEnumerable<ICoconaShellCompletionCodeProvider> providers)
        {
            _providers = providers?.ToArray() ?? throw new ArgumentNullException(nameof(providers));
            SupportedTargets = _providers.SelectMany(xs => xs.Targets).ToArray();
        }

        public void Generate(string target, TextWriter writer, CommandCollection commandCollection)
        {
            var provider = _providers.First(x => x.Targets.Contains(target));
            provider.Generate(writer, commandCollection);
        }
    }
}
