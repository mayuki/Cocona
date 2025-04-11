using WildCoco.Application;
using WildCoco.Command.Dispatcher;
using WildCoco.CommandLine;
using WildCoco.Internal;
using WildCoco.Resources;

namespace WildCoco.Command;

public class WildCocoBootstrapper : IWildCocoBootstrapper
{
    private readonly IWildCocoCommandLineArgumentProvider _commandLineArgumentProvider;
    private readonly IWildCocoCommandProvider _commandProvider;
    private readonly IWildCocoCommandResolver _commandResolver;
    private readonly IWildCocoCommandDispatcher _dispatcher;
    private readonly IWildCocoConsoleProvider _console;

    private CommandCollection? _commandCollection;
    public WildCocoBootstrapper(
        IWildCocoCommandLineArgumentProvider commandLineArgumentProvider,
        IWildCocoCommandProvider commandProvider,
        IWildCocoCommandResolver commandResolver,
        IWildCocoCommandDispatcher dispatcher,
        IWildCocoConsoleProvider console
    )
    {
        _commandLineArgumentProvider = commandLineArgumentProvider;
        _commandProvider = commandProvider;
        _commandResolver = commandResolver;
        _dispatcher = dispatcher;
        _console = console;
    }

    public void Initialize()
    {
        _commandCollection = _commandProvider.GetCommandCollection();
    }

    public async ValueTask<int> RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            var resolved = _commandResolver.ParseAndResolve(_commandCollection ?? throw new WildCocoException("Call Initialize before RunAsync"), _commandLineArgumentProvider.GetArguments());
            return await _dispatcher.DispatchAsync(resolved, cancellationToken);
        }
        catch (CommandNotFoundException cmdNotFoundEx)
        {
            if (string.IsNullOrWhiteSpace(cmdNotFoundEx.Command))
            {
                _console.Error.WriteLine(string.Format(Strings.Dispatcher_Error_CommandNotFound, cmdNotFoundEx.Message));
            }
            else
            {
                _console.Error.WriteLine(string.Format(Strings.Dispatcher_Error_NotACommand, cmdNotFoundEx.Command));
            }

            var similarCommands = cmdNotFoundEx.ImplementedCommands.All
                .Where(x => !x.IsHidden && Levenshtein.GetDistance(cmdNotFoundEx.Command.ToLowerInvariant(), x.Name.ToLowerInvariant()) < 3).ToArray();
            if (similarCommands.Any())
            {
                _console.Error.WriteLine();
                _console.Error.WriteLine(Strings.Dispatcher_Error_SimilarCommands);
                foreach (var c in similarCommands)
                {
                    _console.Error.WriteLine($"  {c.Name}");
                }
            }

            return 1;
        }
    }
}
