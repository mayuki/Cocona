using System;
using System.Collections.Generic;
using System.Text;
using ConsoleAppFramework;

namespace Cocona.Benchmark.External.Commands
{
    public class ConsoleAppFrameworkCommand : ConsoleAppBase
    {
        public void Execute(
            [global::ConsoleAppFramework.Option("s")]
            string? str,
            [global::ConsoleAppFramework.Option("i")]
            int intOption,
            [global::ConsoleAppFramework.Option("b")]
            bool boolOption)
        {
        }
    }
}
