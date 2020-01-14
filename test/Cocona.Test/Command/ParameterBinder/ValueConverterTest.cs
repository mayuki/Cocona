using Cocona.Command.Binder;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cocona.Test.Command.ParameterBinder
{
    public class ValueConverterTest
    {
        [Fact]
        public void CanNotConvertType()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CoconaValueConverter().ConvertTo(typeof(int), "hello"));
        }
    }
}
