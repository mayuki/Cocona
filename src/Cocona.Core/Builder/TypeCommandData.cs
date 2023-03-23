using System;
using System.Collections.Generic;
using System.Linq;

namespace Cocona.Builder
{
    public class TypeCommandData : ICommandData
    {
        public Type Type { get; }
        public IReadOnlyList<object> Metadata { get; }

        public TypeCommandData(Type type, IReadOnlyList<object> metadata)
        {
            Type = type;
            Metadata = metadata;
        }
    }

    public class TypeCommandDataSource : ICommandDataSource
    {
        private readonly Type _type;
        private readonly IReadOnlyList<Action<ICommandBuilder>> _conventions;
        private readonly IReadOnlyList<object> _baseMetadata;

        public TypeCommandDataSource(Type type, IReadOnlyList<Action<ICommandBuilder>> conventions, IReadOnlyList<object> baseMetadata)
        {
            _type = type;
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

            foreach (var item in _type.GetCustomAttributes(inherit: true))
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
            public Type Type { get; set; }
            public IList<object> Metadata { get; } = new List<object>();

            public CommandBuilder(TypeCommandDataSource source)
            {
                Metadata = new List<object>();
                Type = source._type;
            }

            public ICommandData Build()
            {
                return new TypeCommandData(Type, Metadata.ToArray());
            }
        }
    }
}
