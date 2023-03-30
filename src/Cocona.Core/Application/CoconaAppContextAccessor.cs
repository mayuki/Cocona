namespace Cocona.Application
{
    public interface ICoconaAppContextAccessor
    {
        CoconaAppContext? Current { get; set; }
    }

    public class CoconaAppContextAccessor : ICoconaAppContextAccessor
    {
        public CoconaAppContext? Current { get; set; }
    }
}
