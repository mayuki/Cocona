using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Services;

namespace Cocona.Benchmark.External.Commands
{
    [CliFx.Attributes.Command]
    public class CliFxCommand : CliFx.ICommand
    {
        [CommandOption("str", 's')]
        public string? StrOption { get; set; }

        [CommandOption("int", 'i')]
        public int IntOption { get; set; }

        [CommandOption("bool", 'b')]
        public bool BoolOption { get; set; }

        public Task ExecuteAsync(IConsole console) => Task.CompletedTask;
    }
}
