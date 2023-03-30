using Cocona.Internal;

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

        [Fact]
        public void Compat_ValueType_Nullable()
        {
            var helper = new NullabilityInfoContextHelper();
            Action<int?> a = (int? value) => { };
            helper.GetNullabilityState_Compat(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.Nullable);
        }

        [Fact]
        public void Compat_ValueType_NotNull()
        {
            var helper = new NullabilityInfoContextHelper();
            Action<int> a = (int value) => { };
            helper.GetNullabilityState_Compat(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.NotNull);
        }


        [Fact]
        public void Compat_NullableReferenceType_Enable_Nullable()
        {
#nullable enable
            var helper = new NullabilityInfoContextHelper();
            Action<string?> a = (string? value) => { };
            helper.GetNullabilityState_Compat(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.Nullable);
#nullable restore
        }

        [Fact]
        public void Compat_NullableReferenceType_Enable_Nullable_Array()
        {
#nullable enable
            var helper = new NullabilityInfoContextHelper();
            Action<string[]?> a = (string[]? value) => { };
            helper.GetNullabilityState_Compat(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.Nullable);
#nullable restore
        }

        [Fact]
        public void Compat_NullableReferenceType_Enable_NotNull()
        {
#nullable enable
            var helper = new NullabilityInfoContextHelper();
            Action<string> a = (string value) => { };
            helper.GetNullabilityState_Compat(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.NotNull);
#nullable restore
        }

        [Fact]
        public void Compat_NullableReferenceType_Disable_Nullable_NullableContextAttribute()
        {
#nullable disable
            var helper = new NullabilityInfoContextHelper();
            Action<string?> a = /*[NullableContext(0x2)]*/(string? value) => { };
            helper.GetNullabilityState_Compat(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.Nullable);
#nullable restore
        }

        [Fact]
        public void Compat_NullableReferenceType_Disable_Nullable_NullableAttribute()
        {
#nullable disable
            var helper = new NullabilityInfoContextHelper();
            Action<string?, string> a = (/*[Nullable(0x2)]*/string? value, string valueB) => { };
            helper.GetNullabilityState_Compat(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.Nullable);
#nullable restore
        }

        [Fact]
        public void Compat_NullableReferenceType_Disable_NotNull()
        {
#nullable disable
            var helper = new NullabilityInfoContextHelper();
            Action<string> a = (string value) => { };
            helper.GetNullabilityState_Compat(a.Method.GetParameters()[0]).Should().Be(NullabilityInfoContextHelper.NullabilityState.Unknown);
#nullable restore
        }
    }
}
