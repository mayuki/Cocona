using System;
using System.Threading.Tasks;
using Cocona;
using Cocona.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoconaSample.InAction.CommandFilter
{
    [SampleCommandFilter("Class")]
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        // Example:
        //     [SampleCommandFilter(Class)]
        //         [SampleCommandFilter(Method)]
        //             [SampleCommandFilterWithDI]
        //                 Hello (Command)
        [SampleCommandFilter("Method")]
        [SampleCommandFilterWithDI]
        public void Hello()
        {
            Console.WriteLine($"Hello Konnichiwa");
        }
    }

    class SampleCommandFilterAttribute : CommandFilterAttribute
    {
        private readonly string _label;

        public SampleCommandFilterAttribute(string label)
        {
            _label = label;
        }

        public override async ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
        {
            Console.WriteLine($"[SampleCommandFilter({_label})] Before Command: {ctx.Command.Name}");
            try
            {
                return await next(ctx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SampleCommandFilter({_label})] Exception: {ex.GetType().FullName}: {ex.Message}");
                throw;
            }
            finally
            {
                Console.WriteLine($"[SampleCommandFilter({_label})] End Command: {ctx.Command.Name}");
            }
        }
    }

    class SampleCommandFilterWithDIAttribute : Attribute, IFilterFactory
    {
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return ActivatorUtilities.CreateInstance<SampleCommandFilterWithDI>(serviceProvider);
        }
    }

    class SampleCommandFilterWithDI : ICommandFilter
    {
        private readonly ILogger _logger;

        public SampleCommandFilterWithDI(ILogger<SampleCommandFilterWithDI> logger)
        {
            _logger = logger;
        }

        public async ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
        {
            _logger.LogInformation($"[SampleCommandFilterWithDI] Before Command: {ctx.Command.Name}");
            try
            {
                return await next(ctx);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"[SampleCommandFilterWithDI] Exception: {ex.GetType().FullName}: {ex.Message}");
                throw;
            }
            finally
            {
                _logger.LogInformation($"[SampleCommandFilterWithDI] End Command: {ctx.Command.Name}");
            }
        }
    }
}
