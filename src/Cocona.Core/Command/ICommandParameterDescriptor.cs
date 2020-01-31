using System;
using System.Collections.Generic;

namespace Cocona.Command
{
    public interface ICommandParameterDescriptor
    {
        string Name { get; }
        IReadOnlyList<Attribute> ParameterAttributes { get; }
    }
}
