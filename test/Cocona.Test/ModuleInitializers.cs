using System.Globalization;
using System.Runtime.CompilerServices;

namespace Cocona.Test
{
    public class ModuleInitializers
    {
        [ModuleInitializer]
        public static void EnforceCurrentCulture()
        {
            // Ignore the culture of the user's environment and use InvariantCulture.
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        }
    }
}
