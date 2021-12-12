using System.Text;
using WeihanLi.Common.Compressor;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.CompressorTest;

public class DataCompressorTest
{
    [Fact]
    public void NullCompressorTest()
    {
        var compressor = new NullDataCompressor();
        var bytes = Encoding.UTF8.GetBytes("Compressor Test");
        var compressedBytes = compressor.Compress(bytes);
        Assert.True(compressedBytes.SequenceEqual(bytes));
        var decompressedBytes = compressor.Decompress(compressedBytes);
        Assert.True(decompressedBytes.SequenceEqual(bytes));
    }

    [Fact]
    public async Task NullCompressorAsyncTest()
    {
        var compressor = new NullDataCompressor();
        var bytes = Encoding.UTF8.GetBytes("Compressor Test");
        var compressedBytes = await compressor.CompressAsync(bytes);
        Assert.True(compressedBytes.SequenceEqual(bytes));
        var decompressedBytes = await compressor.DecompressAsync(compressedBytes);
        Assert.True(decompressedBytes.SequenceEqual(bytes));
    }

    [Theory]
    //[InlineData(10)]
    [InlineData(256)]
    [InlineData(257)]
    [InlineData(500)]
    [InlineData(100_000)]
    public void GZipCompressorTest(int len)
    {
        var compressor = new GZipDataCompressor();
        var str = new string('a', len);
        var bytes = Encoding.UTF8.GetBytes(str);
        var compressedBytes = compressor.Compress(bytes);
        var decompressedBytes = compressor.Decompress(compressedBytes);
        var text = Encoding.UTF8.GetString(decompressedBytes);
        Assert.Equal(str, text);
    }

    [Theory]
    [InlineData(256)]
    [InlineData(257)]
    [InlineData(500)]
    [InlineData(100_000)]
    public async Task GZipCompressorAsyncTest(int len)
    {
        var compressor = new GZipDataCompressor();
        var str = new string('a', len);
        var bytes = Encoding.UTF8.GetBytes(str);
        var compressedBytes = await compressor.CompressAsync(bytes);
        var decompressedBytes = await compressor.DecompressAsync(compressedBytes);
        var text = Encoding.UTF8.GetString(decompressedBytes);
        Assert.Equal(str, text);

        Assert.Equal(str, compressedBytes.DecompressGZipString());
    }
}
