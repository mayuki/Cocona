using System.Reflection;

namespace Cocona.Builder
{
    public class OptionLikeDelegateCommandData : ICommandData
    {
        public MethodInfo Method { get; }
        public object? Target { get; }
        public IReadOnlyList<char> ShortNames { get; }
        public IReadOnlyList<object> Metadata { get; }

        public OptionLikeDelegateCommandData(IReadOnlyList<char> shortNames, MethodInfo methodInfo, object? target, IReadOnlyList<object> metadata)
        {
            Method = methodInfo;
            Target = target;
            ShortNames = shortNames;
            Metadata = metadata;
        }
    }

    public class OptionLikeDelegateCommandDataSource : ICommandDataSource
    {
        private readonly Delegate _delegate;
        private readonly IReadOnlyList<char> _shortNames;
        private readonly IReadOnlyList<Action<ICommandBuilder>> _conventions;
        private readonly IReadOnlyList<object> _baseMetadata;

        public OptionLikeDelegateCommandDataSource(Delegate @delegate, IReadOnlyList<char> shortNames, IReadOnlyList<Action<ICommandBuilder>> conventions, IReadOnlyList<object> baseMetadata)
        {
            _delegate = @delegate;
            _shortNames = shortNames;
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
            public IReadOnlyList<char> ShortNames { get; }
            public IList<object> Metadata { get; } = new List<object>();

            public CommandBuilder(OptionLikeDelegateCommandDataSource source)
            {
                Method = source._delegate.Method;
                Target = source._delegate.Target;
                ShortNames = source._shortNames;
                Metadata = new List<object>();
            }

            public ICommandData Build()
            {
                return new OptionLikeDelegateCommandData(ShortNames, Method, Target, Metadata.ToArray());
            }
        }
    }
}
