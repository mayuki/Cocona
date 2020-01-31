using System;

namespace Cocona.Filters
{
    /// <summary>
    /// An interface for filter which can create an instance of an executable command filter.
    /// </summary>
    public interface IFilterFactory
    {
        IFilterMetadata CreateInstance(IServiceProvider serviceProvider);
    }
}