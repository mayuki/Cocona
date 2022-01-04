using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
