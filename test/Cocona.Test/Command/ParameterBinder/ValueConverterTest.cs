using Cocona.Command.Binder;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.Command.ParameterBinder
{
    public class ValueConverterTest
    {
        [Fact]
        public void CanNotConvertType_Int32()
        {
            var ex = Assert.Throws<FormatException>(() => new CoconaValueConverter().ConvertTo(typeof(int), "hello"));
        }

        [Fact]
        public void StringToString()
        {
            new CoconaValueConverter().ConvertTo(typeof(string), "hello").Should().Be("hello");
        }

        [Fact]
        public void Boolean_True()
        {
            new CoconaValueConverter().ConvertTo(typeof(bool), "trUe").Should().Be(true);
        }

        [Fact]
        public void Boolean_False()
        {
            new CoconaValueConverter().ConvertTo(typeof(bool), "faLse").Should().Be(false);
        }

        [Fact]
        public void Boolean_False_UnknownValue()
        {
            new CoconaValueConverter().ConvertTo(typeof(bool), "unknown").Should().Be(false);
        }

        [Fact]
        public void Boolean_Int32()
        {
            new CoconaValueConverter().ConvertTo(typeof(int), "12345").Should().Be(12345);
        }
    }
}
