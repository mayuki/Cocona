using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Application
{
    public interface ICoconaServiceProviderIsService
    {
        bool IsService(Type t);
    }

    public class NullCoconaServiceProviderIsService : ICoconaServiceProviderIsService
    {
        public static ICoconaServiceProviderIsService Instance { get; } = new NullCoconaServiceProviderIsService();
        public bool IsService(Type t) => false;
    }
}
