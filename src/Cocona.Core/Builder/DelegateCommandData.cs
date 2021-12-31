using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cocona.Builder
{
    public class DelegateCommandData : ICommandData
    {
        public MethodInfo Method { get; }
        public object? Target { get; }
        public IReadOnlyList<object> Metadata { get; }

        public DelegateCommandData(MethodInfo methodInfo, object? target, IReadOnlyList<object> metadata)
        {
            Method = methodInfo;
            Target = target;
            Metadata = metadata;
        }
    }

    public class DelegateCommandDataSource : ICommandDataSource
    {
        private readonly Delegate _delegate;
        private readonly IReadOnlyList<Action<ICommandBuilder>> _conventions;
        private readonly IReadOnlyList<object> _baseMetadata;

        public DelegateCommandDataSource(Delegate @delegate, IReadOnlyList<Action<ICommandBuilder>> conventions, IReadOnlyList<object> baseMetadata)
        {
            _delegate = @delegate;
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

            foreach (var item in _delegate.Method.GetCustomAttributes(inherit: true))
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
            public MethodInfo Method { get; }
            public object? Target { get; }
            public IList<object> Metadata { get; } = new List<object>();

            public CommandBuilder(DelegateCommandDataSource source)
            {
                Method = source._delegate.Method;
                Target = source._delegate.Target;
                Metadata = new List<object>();
            }

            public ICommandData Build()
            {
                return new DelegateCommandData(Method, Target, Metadata.ToArray());
            }
        }
    }
}
