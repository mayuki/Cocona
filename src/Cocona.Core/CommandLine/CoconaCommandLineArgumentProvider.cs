namespace Cocona.CommandLine
{
    public class CoconaCommandLineArgumentProvider : ICoconaCommandLineArgumentProvider
    {
        private readonly string[] _args;

        public CoconaCommandLineArgumentProvider(string[] args)
        {
            _args = args;
        }

        public string[] GetArguments()
            => _args;
    }
}
