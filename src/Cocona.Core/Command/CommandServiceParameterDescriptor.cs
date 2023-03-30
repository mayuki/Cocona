using System.Diagnostics;

namespace Cocona.Command;

[DebuggerDisplay("Service: {Name,nq}")]
public class CommandServiceParameterDescriptor : ICommandParameterDescriptor
{
    public Type ServiceType { get; }
    public string Name { get; }
    public IReadOnlyList<Attribute> ParameterAttributes { get; } = Array.Empty<Attribute>();

    public CommandServiceParameterDescriptor(Type serviceType, string name)
    {
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}