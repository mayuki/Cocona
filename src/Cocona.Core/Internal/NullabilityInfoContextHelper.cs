using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Internal
{
    internal class NullabilityInfoContextHelper
    {
        // see: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.nullabilitystate?view=net-6.0
        public enum NullabilityState
        {
            Unknown = 0,
            NotNull = 1,
            Nullable = 2,
        }

#if NET6_0_OR_GREATER
        private readonly NullabilityInfoContext _context = new NullabilityInfoContext();

        public NullabilityState GetNullabilityState(ParameterInfo parameterInfo)
        {
            return (NullabilityState)_context.Create(parameterInfo).ReadState;
        }
#else
        public NullabilityState GetNullabilityState(ParameterInfo parameterInfo)
        {
            foreach (var attr in parameterInfo.GetCustomAttributesData())
            {
                if (attr.AttributeType is { Namespace: "System.Runtime.CompilerServices", Name: "NullableAttribute" })
                {
                    return (NullabilityState)((byte)attr.ConstructorArguments[0].Value!);
                }
            }

            return NullabilityState.Unknown;
        }
#endif
    }
}
