using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Internal;
using FluentAssertions;
using Xunit;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

namespace Cocona.Test.Internal
{
    public class NullabilityInfoContextHelperTest
    {
        [Fact]
        public void ValueType_Nullable()
        {
            var helper = new NullabilityInfoContextHelper();
            Action<int?> a = (int? value) => { };
            helper.GetNullabilityState(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.Nullable);
        }

        [Fact]
        public void ValueType_NotNull()
        {
            var helper = new NullabilityInfoContextHelper();
            Action<int> a = (int value) => { };
            helper.GetNullabilityState(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.NotNull);
        }

        [Fact]
        public void NullableReferenceType_Enable_Nullable()
        {
#nullable enable
            var helper = new NullabilityInfoContextHelper();
            Action<string?> a = (string? value) => { };
            helper.GetNullabilityState(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.Nullable);
#nullable restore
        }

        [Fact]
        public void NullableReferenceType_Enable_NotNull()
        {
#nullable enable
            var helper = new NullabilityInfoContextHelper();
            Action<string> a = (string value) => { };
            helper.GetNullabilityState(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.NotNull);
#nullable restore
        }

        [Fact]
        public void NullableReferenceType_Disable_Nullable()
        {
#nullable disable
            var helper = new NullabilityInfoContextHelper();
            Action<string?> a = (string? value) => { };
            helper.GetNullabilityState(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.Nullable);
#nullable restore
        }

        [Fact]
        public void NullableReferenceType_Disable_NotNull()
        {
#nullable disable
            var helper = new NullabilityInfoContextHelper();
            Action<string> a = (string value) => { };
            helper.GetNullabilityState(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.Unknown);
#nullable restore
        }
    }
}
