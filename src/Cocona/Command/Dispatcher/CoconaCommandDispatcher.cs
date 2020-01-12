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
                    var commandName = args[0];
                    var matchedCommand = commandCollection.All
                        .FirstOrDefault(x =>
                            string.Compare(x.Name, commandName, StringComparison.OrdinalIgnoreCase) == 0 ||
                            x.Aliases.Any(y => string.Compare(y, args[0], StringComparison.OrdinalIgnoreCase) == 0)
                        );

                    if (matchedCommand == null)
                    {
                        throw new CommandNotFoundException(
                            commandName,
                            commandCollection,
                            $"The specified command '{commandName}' was not found."
                        );
                    }

                    return DispatchAsyncCore(args.Skip(1).ToArray(), matchedCommand);
                }
                else
                {
                    // Use default command (NOTE: The default command must have no argument.)
                    throw new NotImplementedException();
                }
            }
            else
            {
                // single-command style
                if (commandCollection.All.Any())
                {
                    return DispatchAsyncCore(args, commandCollection.All[0]);
                }
            }

            throw new CommandNotFoundException(
                string.Empty,
                commandCollection,
                $"No commands are implemented yet."
            );
        }

        private async ValueTask<int> DispatchAsyncCore(string[] args, CommandDescriptor command)
        {
            var parsedCommandLine = _commandLineParser.ParseCommand(args, command.Options, command.Arguments);
            if (parsedCommandLine.UnknownOptions.Any()) throw new Exception("UnknownOption:"+parsedCommandLine.UnknownOptions[0]); // TOOD: Exception type

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
