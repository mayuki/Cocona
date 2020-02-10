using System;
using System.Collections.Generic;

namespace Cocona.Command
{
    public interface ICoconaCommandProvider
    {
        CommandCollection GetCommandCollection();
    }
}
