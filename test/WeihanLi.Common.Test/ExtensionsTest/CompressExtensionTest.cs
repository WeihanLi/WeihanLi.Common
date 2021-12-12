using System.Text;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest;

public class CompressExtensionTest
{
    [Fact]
    public void CompressStringTest()
    {
        var str = new string('1', 10000);
        var compressBytes = str.CompressGZip();
        Assert.Equal(str, Encoding.UTF8.GetString(compressBytes.DecompressGZip()));
        var compressStr = str.GetBytes().CompressGZipString();
        Assert.Equal(compressStr, Encoding.UTF8.GetString(compressBytes));
        Assert.Equal(str, Encoding.UTF8.GetString(compressBytes.DecompressGZip()));
        Assert.Equal(str, compressBytes.DecompressGZipString());
    }
}
