using Cocona.Command.Binder;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Cocona.Internal
{
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
                    var typedArray = Array.CreateInstance(elementType, values.Length);
                    for (var i = 0; i < values.Length; i++)
                    {
                        typedArray.SetValue(converter.ConvertTo(elementType, values[i]), i);
                    }
                    var listT = typeof(List<>).MakeGenericType(elementType);

                    arrayOrEnumerableLike = Activator.CreateInstance(listT, new[] { typedArray })!;
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
    }
}
