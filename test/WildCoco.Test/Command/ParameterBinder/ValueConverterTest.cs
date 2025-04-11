using WildCoco.Command.Binder;
using WildCoco.Command.Binder;

namespace WildCoco.Test.Command.ParameterBinder;

public class ValueConverterTest
{
    internal class FileInfoComparer : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo? x, FileInfo? y)
        {
            return x?.Name == y?.Name;
        }

        public int GetHashCode(FileInfo obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class DirectoryInfoComparer : IEqualityComparer<DirectoryInfo>
    {
        public bool Equals(DirectoryInfo? x, DirectoryInfo? y)
        {
            return x?.Name == y?.Name;
        }

        public int GetHashCode(DirectoryInfo obj)
        {
            return obj.GetHashCode();
        }
    }

    [Fact]
    public void CanNotConvertType_Int32()
    {
        var ex = Assert.Throws<FormatException>(() => new WildCocoValueConverter().ConvertTo(typeof(int), "hello"));
    }

    [Fact]
    public void StringToString()
    {
        new WildCocoValueConverter().ConvertTo(typeof(string), "hello").Should().Be("hello");
    }

    [Fact]
    public void Boolean_True()
    {
        new WildCocoValueConverter().ConvertTo(typeof(bool), "trUe").Should().Be(true);
    }

    [Fact]
    public void Boolean_False()
    {
        new WildCocoValueConverter().ConvertTo(typeof(bool), "faLse").Should().Be(false);
    }

    [Fact]
    public void Boolean_False_UnknownValue()
    {
        new WildCocoValueConverter().ConvertTo(typeof(bool), "unknown").Should().Be(false);
    }

    [Fact]
    public void Boolean_Int32()
    {
        new WildCocoValueConverter().ConvertTo(typeof(int), "12345").Should().Be(12345);
    }

    [Fact]
    public void DirectoryInfo_Null()
    {
        new WildCocoValueConverter().ConvertTo(typeof(FileInfo), null).Should().Be(null);
    }

    [Fact]
    public void DirectoryInfo_String()
    {
        new WildCocoValueConverter().ConvertTo(typeof(DirectoryInfo), "somedirname").Should()
            .Be(new DirectoryInfo("somedirname"), new DirectoryInfoComparer());
    }

    [Fact]
    public void FileInfo_Null()
    {
        new WildCocoValueConverter().ConvertTo(typeof(FileInfo), null).Should().Be(null);
    }

    [Fact]
    public void FileInfo_String()
    {
        new WildCocoValueConverter().ConvertTo(typeof(FileInfo), "somefilename").Should()
            .Be(new FileInfo("somefilename"), new FileInfoComparer());
    }
}
