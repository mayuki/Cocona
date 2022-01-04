using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Builder;

namespace Cocona.Lite.Builder.Internal
{
    internal class CoconaLiteAppHostOptions
    {
        public Action<ICoconaCommandsBuilder>? ConfigureApplication { get; set; }
    }
}
