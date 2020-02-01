using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Application
{
    public interface ICoconaAppContextAccessor
    {
        CoconaAppContext? Current { get; set; }
    }

    public class CoconaAppContextAccessor : ICoconaAppContextAccessor
    {
        public CoconaAppContext? Current { get; set; }
    }
}
