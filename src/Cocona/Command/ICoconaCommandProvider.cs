using System;

namespace Cocona.Command
{
    public interface ICoconaCommandProvider
    {
        CommandCollection GetCommandCollection(Type[] targetTypes);
    }
}
