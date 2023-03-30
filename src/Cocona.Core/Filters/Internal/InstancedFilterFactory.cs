namespace Cocona.Filters.Internal
{
    internal class InstancedFilterFactory : IFilterFactory, IOrderedFilter
    {
        private readonly IFilterMetadata _instance;

        public InstancedFilterFactory(IFilterMetadata instance, int order)
        {
            _instance = instance;
            Order = order;
        }

        public int Order { get; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
            => _instance;
    }
}