using System.Reflection;

namespace Cocona.Builder
{
    public class TypeMethodCommandData : ICommandData
    {
        public MethodInfo Method { get; }
        public object? Target { get; }
        public bool IsImplicitCommand { get; }
        public IReadOnlyList<object> Metadata { get; }

        public TypeMethodCommandData(MethodInfo methodInfo, object? target, bool isImplicitCommand, IReadOnlyList<object> metadata)
        {
            Method = methodInfo;
            Target = target;
            IsImplicitCommand = isImplicitCommand;
            Metadata = metadata;
        }
    }
}
