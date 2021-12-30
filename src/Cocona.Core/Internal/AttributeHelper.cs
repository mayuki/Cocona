using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cocona.Internal
{
    internal static class AttributeHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (T1?, T2?) GetAttributes<T1, T2>(IReadOnlyList<object> attrs)
            where T1 : Attribute
            where T2 : Attribute
        {
            var attr1 = default(T1);
            var attr2 = default(T2);
            for (var i = 0; i < attrs.Count; i++)
            {
                var attr = attrs[i];
                switch (attr)
                {
                    case T1 typedAttr:
                        attr1 = typedAttr;
                        break;
                    case T2 typedAttr:
                        attr2 = typedAttr;
                        break;
                }
            }

            return (attr1, attr2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (T1?, T2?, T3?) GetAttributes<T1, T2, T3>(IReadOnlyList<object> attrs)
            where T1 : Attribute
            where T2 : Attribute
            where T3 : Attribute
        {
            var attr1 = default(T1);
            var attr2 = default(T2);
            var attr3 = default(T3);
            for (var i = 0; i < attrs.Count; i++)
            {
                var attr = attrs[i];
                switch (attr)
                {
                    case T1 typedAttr:
                        attr1 = typedAttr;
                        break;
                    case T2 typedAttr:
                        attr2 = typedAttr;
                        break;
                    case T3 typedAttr:
                        attr3 = typedAttr;
                        break;
                }
            }

            return (attr1, attr2, attr3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (T1?, T2?, T3?, T4?) GetAttributes<T1, T2, T3, T4>(IReadOnlyList<object> attrs)
            where T1 : Attribute
            where T2 : Attribute
            where T3 : Attribute
            where T4 : Attribute
        {
            var attr1 = default(T1);
            var attr2 = default(T2);
            var attr3 = default(T3);
            var attr4 = default(T4);
            for (var i = 0; i < attrs.Count; i++)
            {
                var attr = attrs[i];
                switch (attr)
                {
                    case T1 typedAttr:
                        attr1 = typedAttr;
                        break;
                    case T2 typedAttr:
                        attr2 = typedAttr;
                        break;
                    case T3 typedAttr:
                        attr3 = typedAttr;
                        break;
                    case T4 typedAttr:
                        attr4 = typedAttr;
                        break;
                }
            }

            return (attr1, attr2, attr3, attr4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (T1?, T2?, T3?, T4?, T5?) GetAttributes<T1, T2, T3, T4, T5>(IReadOnlyList<object> attrs)
            where T1 : Attribute
            where T2 : Attribute
            where T3 : Attribute
            where T4 : Attribute
            where T5 : Attribute
        {
            var attr1 = default(T1);
            var attr2 = default(T2);
            var attr3 = default(T3);
            var attr4 = default(T4);
            var attr5 = default(T5);
            for (var i = 0; i < attrs.Count; i++)
            {
                var attr = attrs[i];
                switch (attr)
                {
                    case T1 typedAttr:
                        attr1 = typedAttr;
                        break;
                    case T2 typedAttr:
                        attr2 = typedAttr;
                        break;
                    case T3 typedAttr:
                        attr3 = typedAttr;
                        break;
                    case T4 typedAttr:
                        attr4 = typedAttr;
                        break;
                    case T5 typedAttr:
                        attr5 = typedAttr;
                        break;
                }
            }

            return (attr1, attr2, attr3, attr4, attr5);
        }
    }
}
