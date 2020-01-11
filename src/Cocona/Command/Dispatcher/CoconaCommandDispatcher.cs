using Cocona.Command.Binder;
using Cocona.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher
{
    public interface ICoconaCommandDispatcher
    {
        ValueTask<int> DispatchAsync(string[] args);
    }

    public class CoconaCommandDispatcher : ICoconaCommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICoconaCommandProvider _commandProvider;
        private readonly ICoconaParameterBinder _parameterBinder;
        private readonly ICoconaCommandLineParser _commandLineParser;

        public CoconaCommandDispatcher(
            IServiceProvider serviceProvider,
            ICoconaCommandProvider commandProvider,
            ICoconaParameterBinder parameterBinder,
            ICoconaCommandLineParser commandLineParser
        )
        {
            _serviceProvider = serviceProvider;
            _commandProvider = commandProvider;
            _parameterBinder = parameterBinder;
            _commandLineParser = commandLineParser;
        }

        public ValueTask<int> DispatchAsync(string[] args)
        {
            var commandCollection = _commandProvider.GetCommandCollection();
            if (commandCollection.All.Count > 1)
            {
                // multi-commands hosted style
                if (args.Length > 0)
                {
                    var matchedCommand = commandCollection.All
                        .FirstOrDefault(x =>
                            string.Compare(x.Name, args[0], StringComparison.OrdinalIgnoreCase) == 0 ||
                            x.Aliases.Any(y => string.Compare(y, args[0], StringComparison.OrdinalIgnoreCase) == 0)
                        );

                    if (matchedCommand != null)
                    {
                        return DispatchAsyncCore(args.Skip(1).ToArray(), matchedCommand);
                    }
                }
            }
            else
            {
                // single-command style
                var matchedCommand = commandCollection.All[0];
                if (matchedCommand != null)
                {
                    return DispatchAsyncCore(args, matchedCommand);
                }
            }

            throw new Exception("CommandNotImplemented");
        }

        private async ValueTask<int> DispatchAsyncCore(string[] args, CommandDescriptor command)
        {
            var parsedCommandLine = _commandLineParser.ParseCommand(args, command.Options, command.Arguments);

            var invokeArgs = _parameterBinder.Bind(command, parsedCommandLine.Options, parsedCommandLine.Arguments);
            var commandInstance = ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, command.CommandType);
            var result = command.Method.Invoke(commandInstance, invokeArgs);

            if (result is Task<int> taskOfInt)
            {
                var exitCode = await taskOfInt.ConfigureAwait(false);
                return exitCode;
            }
            else if (result is ValueTask<int> valueTaskOfInt)
            {
                var exitCode = await valueTaskOfInt.ConfigureAwait(false);
                return exitCode;
            }
            else if (result is int exitCode)
            {
                return exitCode;
            }

            return 0;
        }
    }
}
