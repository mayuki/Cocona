using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Builder
{
    public class SubCommandData : ICommandData
    {
        public IReadOnlyList<object> Metadata { get; }
        public IReadOnlyList<ICommandData> Children { get; }

        public SubCommandData(IReadOnlyList<ICommandData> children, IReadOnlyList<object> metadata)
        {
            Children = children;
            Metadata = metadata;
        }
    }

    public class SubCommandsDataSource : ICommandDataSource
    {
        private readonly IReadOnlyList<ICommandData> _subCommands;
        private readonly IReadOnlyList<Action<ICommandBuilder>> _conventions;
        private readonly IReadOnlyList<object> _baseMetadata;

        public SubCommandsDataSource(IReadOnlyList<ICommandData> subCommands, IReadOnlyList<Action<ICommandBuilder>> conventions, IReadOnlyList<object> baseMetadata)
        {
            _subCommands = subCommands;
            _conventions = conventions;
            _baseMetadata = baseMetadata;
        }

        public ICommandData Build()
        {
            var builder = new CommandBuilder(this);

            foreach (var item in _baseMetadata)
            {
                builder.Metadata.Add(item);
            }

            foreach (var convention in _conventions)
            {
                convention(builder);
            }

            return builder.Build();
        }

        class CommandBuilder : ICommandBuilder
        {
            public object? Target { get; set; }
            public IList<object> Metadata { get; } = new List<object>();
            public IReadOnlyList<ICommandData> SubCommands { get; set; }

            public CommandBuilder(SubCommandsDataSource source)
            {
                Metadata = new List<object>();
                SubCommands = source._subCommands;
            }

            public ICommandData Build()
            {
                return new SubCommandData(SubCommands, Metadata.ToArray());
            }
        }
    }
}
