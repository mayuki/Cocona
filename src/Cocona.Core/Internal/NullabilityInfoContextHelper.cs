using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            return GetNullabilityState_Compat(parameterInfo);
        }
#endif

        internal NullabilityState GetNullabilityState_Compat(ParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType.IsValueType)
            {
                if (parameterInfo.ParameterType.IsConstructedGenericType && parameterInfo.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return NullabilityState.Nullable;
                }
                else
                {
                    return NullabilityState.NotNull;
                }
            }

            // NullableAttribute on the parameter.
            foreach (var attr in parameterInfo.GetCustomAttributesData())
            {
                if (attr.AttributeType is { Namespace: "System.Runtime.CompilerServices", Name: "NullableAttribute" })
                {
                    return (NullabilityState)GetFirstByte(attr.ConstructorArguments[0].Value!);
                }
            }

            // NullableContextAttribute on the member
            foreach (var attr in parameterInfo.Member.GetCustomAttributesData())
            {
                if (attr.AttributeType is { Namespace: "System.Runtime.CompilerServices", Name: "NullableContextAttribute" })
                {
                    return (NullabilityState)GetFirstByte(attr.ConstructorArguments[0].Value!);
                }
            }

            // NullableContextAttribute on the declaring type
            if (parameterInfo.Member.DeclaringType is not null)
            {
                foreach (var attr in parameterInfo.Member.DeclaringType.GetCustomAttributesData())
                {
                    if (attr.AttributeType is { Namespace: "System.Runtime.CompilerServices", Name: "NullableContextAttribute" })
                    {
                        return (NullabilityState)GetFirstByte(attr.ConstructorArguments[0].Value!);
                    }
                }
            }

            return NullabilityState.Unknown;

            static byte GetFirstByte(object? attrCtorArg)
            {
                return attrCtorArg switch
                {
                    byte x => x,
                    IReadOnlyCollection<CustomAttributeTypedArgument> x => (byte)x.First().Value!,
                    _ => throw new NotSupportedException(),
                };
            }
        }
    }
}
