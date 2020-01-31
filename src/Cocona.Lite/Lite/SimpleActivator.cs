using System;
using System.Linq;
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
                parameterValues[i] = serviceProvider.GetService(bestMatchCtorParameters[i].ParameterType);
            }

            return bestMatchCtor.Invoke(parameterValues);
        }
    }
}
