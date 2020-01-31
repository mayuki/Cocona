using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Filters
{
    public interface IOrderedFilter
    {
        int Order { get; }
    }
}
