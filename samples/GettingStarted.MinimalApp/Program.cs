using Cocona;

namespace CoconaSample.GettingStarted.MinimalApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public void Hello(bool toUpperCase, [Argument]string name)
        {
            Console.WriteLine($"Hello {(toUpperCase ? name.ToUpper() : name)}");
        }
    }
}
