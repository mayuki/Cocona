using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Builder;

namespace Cocona.Builder.Internal
{
    internal class CoconaAppHostOptions
    {
        public Action<ICoconaCommandsBuilder>? ConfigureApplication { get; set; }
    }
}
