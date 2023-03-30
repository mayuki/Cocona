using Cocona;

namespace CoconaSample.InAction.ParameterSet;

class Program
{
    static void Main(string[] args)
    {
        CoconaApp.Run<Program>(args);
    }

    /// <summary>
    /// Define a set of Options and Arguments that are common to multiple commands.
    /// </summary>
    public record CommonParameters(
        [Option('t', Description = "Specifies the remote host to connect.")]
        string Host,
        [Option('p', Description = "Port to connect to on the remote host.")]
        int Port,
        [Option('u', Description = "Specifies the user to log in as on the remote host.")]
        string User = "root",
        [Option('f', Description = "Perform without user confirmation.")]
        bool Force = false
    ) : ICommandParameterSet;

    public void Add(CommonParameters commonParams, [Argument] string from, [Argument] string to)
    {
        Console.WriteLine($"Add: {commonParams.User}@{commonParams.Host}:{commonParams.Port} {(commonParams.Force ? " (Force)" : "")}");
        Console.WriteLine($"{from} -> {to}");
    }

    public void Update(CommonParameters commonParams, [Option('r', Description = "Traverse recursively to perform.")] bool recursive, [Argument] string path)
    {
        Console.WriteLine($"Update: {commonParams.User}@{commonParams.Host}:{commonParams.Port} {(commonParams.Force ? " (Force)" : "")}");
        Console.WriteLine($"{path}{(recursive ? " (Recursive)" : "")}");
    }
}