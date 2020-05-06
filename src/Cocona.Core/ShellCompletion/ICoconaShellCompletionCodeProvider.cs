using System.Collections.Generic;
using System.IO;
using System.Text;
using Cocona.Command;

namespace Cocona.ShellCompletion
{
    public interface ICoconaShellCompletionCodeProvider
    {
        IReadOnlyList<string> Targets { get; }
        void Generate(TextWriter writer, CommandCollection commandCollection);
    }
}
