using System;
using System.Collections.Generic;
using Cocona.Internal;

namespace Cocona.Builder
{
    /// <summary>
    /// Builds conventions that will be used for customization of CommandBuilder instances.
    /// </summary>
    public class CommandConventionBuilder
    {
        private readonly List<Action<ICommandBuilder>> _conventions;

        public CommandConventionBuilder(List<Action<ICommandBuilder>> conventions)
        {
            _conventions = conventions;
        }

        public void Add(Action<ICommandBuilder> convention)
        {
            ThrowHelper.ThrowIfNull(convention);

            _conventions.Add(convention);
        }
    }
}
