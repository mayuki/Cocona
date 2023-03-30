namespace Cocona.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class InitializeOnStartupFilterAttribute : Attribute, IFilterFactory
    {
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }

    public class InitializeOnStartupFilter : ICommandFilter, IOrderedFilter
    {
        public int Order => int.MinValue;

        public InitializeOnStartupFilter()
        { }

        public ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
        {
            return next(ctx);
        }
    }
}
