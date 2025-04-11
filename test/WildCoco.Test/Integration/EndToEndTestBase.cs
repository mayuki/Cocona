using WildCoco;
#if WildCoco_LITE
using WildCocoApp = WildCoco.WildCocoLiteApp;
using WildCocoAppOptions = WildCoco.WildCocoLiteAppOptions;
#endif

namespace WildCoco.Test.Integration;

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

    protected (string StandardOut, string StandardError, int ExitCode) Run<T>(RunBuilderMode mode, string[] args, Action<WildCocoAppOptions>? configureOptions = null)
    {
        var stdOutWriter = new StringWriter();
        var stdErrWriter = new StringWriter();

        Console.SetOut(stdOutWriter);
        Console.SetError(stdErrWriter);

        switch (mode)
        {
            case RunBuilderMode.CreateBuilder:
                var builder = WildCocoApp.CreateBuilder(args, configureOptions);
                var app = builder.Build();
                app.AddCommands<T>();

                app.Run();
                break;
            case RunBuilderMode.CreateHostBuilder:
                WildCocoApp.CreateHostBuilder()
                    .Run<T>(args, configureOptions);
                break;
            case RunBuilderMode.Shortcut:
                WildCocoApp.Run<T>(args, configureOptions);
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
                var builder = WildCocoApp.CreateBuilder(args);
                var app = builder.Build();
                app.AddCommands<T>();

                await app.RunAsync(cancellationToken);
                break;
            case RunBuilderMode.CreateHostBuilder:
                await WildCocoApp.CreateHostBuilder()
                    .RunAsync<T>(args, cancellationToken: cancellationToken);
                break;
            case RunBuilderMode.Shortcut:
                await WildCocoApp.RunAsync<T>(args, cancellationToken: cancellationToken);
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
                var builder = WildCocoApp.CreateBuilder(args);
                var app = builder.Build();
                foreach (var type in types)
                {
                    app.AddCommands(type);
                }

                app.Run();
                break;
            case RunBuilderMode.CreateHostBuilder:
                WildCocoApp.CreateHostBuilder()
                    .Run(args, types);
                break;
            case RunBuilderMode.Shortcut:
                WildCocoApp.Run(args, types);
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
                var builder = WildCocoApp.CreateBuilder(args);
                var app = builder.Build();
                foreach (var type in types)
                {
                    app.AddCommands(type);
                }

                await app.RunAsync(cancellationToken);
                break;
            case RunBuilderMode.CreateHostBuilder:
                await WildCocoApp.CreateHostBuilder()
                    .RunAsync(args, types, cancellationToken: cancellationToken);
                break;
            case RunBuilderMode.Shortcut:
                await WildCocoApp.RunAsync(args, types, cancellationToken: cancellationToken);
                break;
        }

        return (stdOutWriter.ToString(), stdErrWriter.ToString(), Environment.ExitCode);
    }
}
