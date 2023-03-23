using System;
using System.Reflection;

namespace Cocona.Lite
{
    public static class SimpleActivator
    {
        public static object CreateInstance(IServiceProvider serviceProvider, Type type)
        {
            var ctors = type.GetConstructors();
            var bestMatchCtor = default(ConstructorInfo);
            var bestMatchCtorParameters = Array.Empty<ParameterInfo>();
            for (var i = 0; i < ctors.Length; i++)
            {
                var ctor = ctors[i];
                var ctorParameters = ctor.GetParameters();

                if (bestMatchCtor == null)
                {
                    bestMatchCtor = ctor;
                    bestMatchCtorParameters = ctorParameters;
                }
                else if (bestMatchCtorParameters.Length < ctorParameters.Length)
                {
                    bestMatchCtor = ctor;
                    bestMatchCtorParameters = ctorParameters;
                }
            }
            if (bestMatchCtor == null) throw new InvalidOperationException($"Type '{type.FullName}' has no constructor.");
            var parameterValues = new object[bestMatchCtorParameters.Length];

            for (var i = 0; i < bestMatchCtorParameters.Length; i++)
            {
                var value = serviceProvider.GetService(bestMatchCtorParameters[i].ParameterType);
                parameterValues[i] = value ?? throw new InvalidOperationException($"Unable to resolve service for type '{bestMatchCtorParameters[i].ParameterType.FullName}' while attempting to activate '{type.FullName}'");
            }

            return bestMatchCtor.Invoke(parameterValues);
        }
    }
}
