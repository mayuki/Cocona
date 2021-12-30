using System;
using System.Collections.Generic;

namespace Cocona.Builder
{
    public class CommandTypeConventionBuilder
    {
        private readonly List<Action<ICommandBuilder>> _conventions;

        public CommandTypeConventionBuilder(List<Action<ICommandBuilder>> conventions)
        {
            _conventions = conventions;
        }

        public void Add(Action<ICommandBuilder> convention)
        {
            _conventions.Add(convention);
        }
    }
}
