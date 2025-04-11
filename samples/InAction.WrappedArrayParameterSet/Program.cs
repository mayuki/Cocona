using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cocona;

namespace CoconaSample.InAction.WrappedArrayParameterSet;

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
        [Option('i', Description = "Items to process")]
        WrappedArray<WrappedItem> Items
    ) : ICommandParameterSet;

    public class WrappedArray<T> : IEnumerable<T>
    {
        private readonly T[] _items;

        public WrappedArray(T[] items) {
            _items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }

    [TypeConverter(typeof(WrappedItemConverter))]
    public record WrappedItem(int Index, string First3Letters, string Rest);

    public class WrappedItemConverter : TypeConverter
    {
        private static int _counter;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                return new WrappedItem(_counter++, s.Substring(0, 3), s.Substring(3));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public void Add(CommonParameters commonParams, [Argument] string from, [Argument] string to)
    {
        Console.WriteLine($"Add: {commonParams} {string.Join(", ", commonParams.Items)}");
        Console.WriteLine($"{from} -> {to}");
    }

    public void Update(CommonParameters commonParams, [Option('r', Description = "Traverse recursively to perform.")] bool recursive, [Argument] string path)
    {
        Console.WriteLine($"Update: {commonParams} {string.Join(", ", commonParams.Items)}");
        Console.WriteLine($"{path}{(recursive ? " (Recursive)" : "")}");
    }
}
