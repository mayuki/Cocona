using System.Diagnostics;
using System.Reflection;

namespace Cocona.Application
{
    public class CoconaApplicationMetadataProvider : ICoconaApplicationMetadataProvider
    {
        public string GetExecutableName()
        {
            return Process.GetCurrentProcess().ProcessName;
        }

        public string GetDescription()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            return entryAssembly?.GetCustomAttribute<AssemblyTitleAttribute>()?.Title
                ?? string.Empty;
        }

        public string GetProductName()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            return entryAssembly?.GetCustomAttribute<AssemblyProductAttribute>()?.Product
                ?? entryAssembly?.FullName
                    ?? string.Empty;
        }

        public string GetVersion()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            return entryAssembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? entryAssembly?.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                    ?? "1.0.0.0";
        }
    }
}
