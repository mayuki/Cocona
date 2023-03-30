using Cocona;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace CoconaSample.InAction.ExitCode;

class Program
{
    // Example:
    //    on PowerShell
    //        PS> dotnet run -- return
    //        PS> $LASTEXITCODE
    //        123
    //    on bash:
    //        $ dotnet run -- return
    //        $ echo $?
    //        123
    static void Main(string[] args)
    {
        CoconaApp.Run<Program>(args);
    }

    public int Return()
    {
        return 123; // ExitCode: 123
    }

    public async Task<int> ReturnAsync()
    {
        return 456; // ExitCode: 456
    }

    public async ValueTask<int> ReturnValueTaskAsync()
    {
        return 1; // ExitCode: 1
    }

    public void Throw()
    {
        Foo();

        void Foo()
        {
            Bar();
        }

        void Bar()
        {
            Baz();
        }

        void Baz()
        {
            // When a user-code throws a `CommandExitedException`, Cocona handles as a non-local exit.

            throw new CommandExitedException(128); // ExitCode: 128
            // throw new CommandExitedException("Error Message", 1);
        }
    }
}