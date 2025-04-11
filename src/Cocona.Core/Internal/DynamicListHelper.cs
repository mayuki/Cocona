using Cocona.Command.Binder;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Cocona.Internal;

internal static class DynamicListHelper
{
    /// <summary>
    /// Indicates whether the specified type is <see cref="Array"/> or <see cref="List{T}"/> or enumerable-like.
    /// </summary>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static bool IsArrayOrEnumerableLike(Type valueType)
    {
        if (valueType.IsGenericType)
        {
            // Any<T>
            var openGenericType = valueType.GetGenericTypeDefinition();

            // List<T> (== IList<T>, IReadOnlyList<T>, ICollection<T>, IEnumerable<T>)
            if (openGenericType == typeof(List<>) ||
                openGenericType == typeof(IList<>) ||
                openGenericType == typeof(IReadOnlyList<>) ||
                openGenericType == typeof(ICollection<>) ||
                openGenericType == typeof(IEnumerable<>))
            {
                return true;
            }
            else if (HasSuitableConstructor(valueType))
            {
                return true;
            }
        }
        else if (valueType.IsArray)
        {
            // T[]
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets a type of a list or array element.
    /// </summary>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static Type GetElementType(Type valueType)
    {
        if (IsArrayOrEnumerableLike(valueType))
        {
            if (valueType.IsArray)
            {
                return valueType.GetElementType()!;
            }
            else
            {
                return valueType.GetGenericArguments()[0];
            }
        }

        return valueType;
    }

    /// <summary>
    /// Create an array or list instance from the values. A return value indicates the array or list instance has created or not.
    /// </summary>
    /// <param name="valueType"></param>
    /// <param name="values"></param>
    /// <param name="converter"></param>
    /// <param name="arrayOrEnumerableLike"></param>
    /// <returns></returns>
    public static bool TryCreateArrayOrEnumerableLike(Type valueType, string?[] values, ICoconaValueConverter converter, [NotNullWhen(true)] out object? arrayOrEnumerableLike)
    {
        if (valueType.IsGenericType)
        {
            // Any<T>
            var openGenericType = valueType.GetGenericTypeDefinition();
            var elementType = valueType.GetGenericArguments()[0];

            // List<T> (== IList<T>, IReadOnlyList<T>, ICollection<T>, IEnumerable<T>)
            if (openGenericType == typeof(List<>) ||
                openGenericType == typeof(IList<>) ||
                openGenericType == typeof(IReadOnlyList<>) ||
                openGenericType == typeof(ICollection<>) ||
                openGenericType == typeof(IEnumerable<>))
            {
                var typedArray = MakeTypedArrayOfValues(values, converter, elementType);
                var listT = typeof(List<>).MakeGenericType(elementType);

                arrayOrEnumerableLike = Activator.CreateInstance(listT, new[] { typedArray })!;
                return true;
            }
            if (TryGetSuitableConstructor(valueType, elementType, out var ctor))
            {
                var typedArray = MakeTypedArrayOfValues(values, converter, elementType);
                arrayOrEnumerableLike = ctor.Invoke(new object?[] { typedArray });
                return true;
            }
        }
        else if (valueType.IsArray)
        {
            // T[]
            var elementType = valueType.GetElementType()!;
            var typedArray = Array.CreateInstance(elementType, values.Length);
            for (var i = 0; i < values.Length; i++)
            {
                typedArray.SetValue(converter.ConvertTo(elementType, values[i]), i);
            }

            arrayOrEnumerableLike = typedArray;
            return true;
        }

        arrayOrEnumerableLike = null;
        return false;
    }

    private static Array MakeTypedArrayOfValues(string?[] values, ICoconaValueConverter converter, Type elementType)
    {
        if (elementType == typeof(string)) return values;
        var typedArray = Array.CreateInstance(elementType, values.Length);
        for (var i = 0; i < values.Length; i++)
        {
            typedArray.SetValue(converter.ConvertTo(elementType, values[i]), i);
        }

        return typedArray;
    }

    private static bool TryGetSuitableConstructor(Type valueType, Type elementType, [NotNullWhen(true)] out ConstructorInfo? foundConstructor)
    {
        if (valueType.IsInterface || valueType.IsAbstract || valueType == typeof(string))
        {
            foundConstructor = default;
            return false;
        }
        var constructors = valueType.GetConstructors();
        var validConstructors = new List<(ConstructorInfo Constructor, Type ParameterType)>();
        foreach (var constructor in constructors)
        {
            var parameterInfos = constructor.GetParameters();
            if (parameterInfos.Length != 1)
            {
                continue;
            }
            var singleParameterType = parameterInfos[0].ParameterType;
            if (singleParameterType.IsGenericType 
                && singleParameterType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IEnumerable<>))
                && singleParameterType.GenericTypeArguments[0] == elementType)
            {
                foundConstructor = constructor;
                return true;
            }
            else if (singleParameterType.IsArray && singleParameterType.GetElementType() == elementType)
            {
                foundConstructor = constructor;
                return true;
            }
        }
        foundConstructor = null;
        return false;
    }
    private static bool HasSuitableConstructor(Type valueType)
    {
        if (valueType.IsInterface || valueType.IsAbstract || valueType == typeof(string))
        {
            return false;
        }
        var constructors = valueType.GetConstructors();
        var validConstructors = new List<(ConstructorInfo Constructor, Type ParameterType)>();
        foreach (var constructor in constructors)
        {
            var parameterInfos = constructor.GetParameters();
            if (parameterInfos.Length != 1)
            {
                continue;
            }
            var singleParameterType = parameterInfos[0].ParameterType;
            if (singleParameterType.IsGenericType && singleParameterType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IEnumerable<>)))
            {
                return true;
            }
            else if (singleParameterType.IsArray)
            {
                return true;
            }
        }
        return false;
    }
}
