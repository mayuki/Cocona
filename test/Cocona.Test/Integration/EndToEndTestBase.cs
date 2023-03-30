#if COCONA_LITE
using CoconaApp = Cocona.CoconaLiteApp;
using CoconaAppOptions = Cocona.CoconaLiteAppOptions;
#endif

namespace Cocona.Test.Integration;

public enum RunBuilderMode
{
    CreateBuilder,
    CreateHostBuilder,
    Shortcut,
}

[Collection("End to End")] // NOTE: Test cases use `Console` and does not run in parallel.
public abstract class EndToEndTestBase
{
#if !NET5_0_OR_GREATER
    static EndToEndTestBase()
    {
        ModuleInitializers.EnforceCurrentCulture();
    }
#endif

    protected (string StandardOut, string StandardError, int ExitCode) Run(string[] args, Action<string[]> action)
    {
        var stdOutWriter = new StringWriter();
        var stdErrWriter = new StringWriter();

        Console.SetOut(stdOutWriter);
        Console.SetError(stdErrWriter);

        action(args);

        return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
    }

    protected async Task<(string StandardOut, string StandardError, int ExitCode)> RunAsync(string[] args, Func<string[], Task> action)
    {
        var stdOutWriter = new StringWriter();
        var stdErrWriter = new StringWriter();

        Console.SetOut(stdOutWriter);
        Console.SetError(stdErrWriter);

        await action(args);

        return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
    }

    protected (string StandardOut, string StandardError, int ExitCode) Run<T>(RunBuilderMode mode, string[] args, Action<CoconaAppOptions>? configureOptions = null)
    {
        var stdOutWriter = new StringWriter();
        var stdErrWriter = new StringWriter();

        Console.SetOut(stdOutWriter);
        Console.SetError(stdErrWriter);

        switch (mode)
        {
            case RunBuilderMode.CreateBuilder:
                var builder = CoconaApp.CreateBuilder(args, configureOptions);
                var app = builder.Build();
                app.AddCommands<T>();

                app.Run();
                break;
            case RunBuilderMode.CreateHostBuilder:
                CoconaApp.CreateHostBuilder()
                    .Run<T>(args, configureOptions);
                break;
            case RunBuilderMode.Shortcut:
                CoconaApp.Run<T>(args, configureOptions);
                break;
        }

        return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
    }

    protected async Task<(string StandardOut, string StandardError, int ExitCode)> RunAsync<T>(RunBuilderMode mode, string[] args, CancellationToken cancellationToken)
    {
        var stdOutWriter = new StringWriter();
        var stdErrWriter = new StringWriter();

        Console.SetOut(stdOutWriter);
        Console.SetError(stdErrWriter);

        switch (mode)
        {
            case RunBuilderMode.CreateBuilder:
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommands<T>();

                await app.RunAsync(cancellationToken);
                break;
            case RunBuilderMode.CreateHostBuilder:
                await CoconaApp.CreateHostBuilder()
                    .RunAsync<T>(args, cancellationToken: cancellationToken);
                break;
            case RunBuilderMode.Shortcut:
                await CoconaApp.RunAsync<T>(args, cancellationToken: cancellationToken);
                break;
        }

        return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
    }

    protected (string StandardOut, string StandardError, int ExitCode) Run(RunBuilderMode mode, string[] args, Type[] types)
    {
        var stdOutWriter = new StringWriter();
        var stdErrWriter = new StringWriter();

        Console.SetOut(stdOutWriter);
        Console.SetError(stdErrWriter);

        switch (mode)
        {
            case RunBuilderMode.CreateBuilder:
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                foreach (var type in types)
                {
                    app.AddCommands(type);
                }

                app.Run();
                break;
            case RunBuilderMode.CreateHostBuilder:
                CoconaApp.CreateHostBuilder()
                    .Run(args, types);
                break;
            case RunBuilderMode.Shortcut:
                CoconaApp.Run(args, types);
                break;
        }

        return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
    }

    protected async Task<(string StandardOut, string StandardError, int ExitCode)> RunAsync<T>(RunBuilderMode mode, string[] args, Type[] types, CancellationToken cancellationToken)
    {
        var stdOutWriter = new StringWriter();
        var stdErrWriter = new StringWriter();

        Console.SetOut(stdOutWriter);
        Console.SetError(stdErrWriter);

        switch (mode)
        {
            case RunBuilderMode.CreateBuilder:
                var builder = CoconaApp.CreateBuilder(args);
                var app = builder.Build();
                foreach (var type in types)
                {
                    app.AddCommands(type);
                }

                await app.RunAsync(cancellationToken);
                break;
            case RunBuilderMode.CreateHostBuilder:
                await CoconaApp.CreateHostBuilder()
                    .RunAsync(args, types, cancellationToken: cancellationToken);
                break;
            case RunBuilderMode.Shortcut:
                await CoconaApp.RunAsync(args, types, cancellationToken: cancellationToken);
                break;
        }

        return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
    }
}
