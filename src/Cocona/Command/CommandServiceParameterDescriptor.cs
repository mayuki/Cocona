using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cocona.Command
{
    [DebuggerDisplay("Service: {Name,nq}")]
    public class CommandServiceParameterDescriptor : CommandParameterDescriptor
    {
        public Type ServiceType { get; }

        public CommandServiceParameterDescriptor(Type serviceType)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        }
    }
}
