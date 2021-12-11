using JetBrains.Annotations;
using System.Threading.Tasks;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Compressor;

/// <summary>
/// DataCompressor
/// </summary>
public interface IDataCompressor
{
    /// <summary>
    /// compress data
    /// </summary>
    /// <param name="sourceData">source data</param>
    /// <returns>compressed data</returns>
    [NotNull]
    byte[] Compress([NotNull] byte[] sourceData);

    /// <summary>
    /// compress data async
    /// </summary>
    /// <param name="sourceData">source data</param>
    /// <returns>compressed data</returns>
    [NotNull]
    Task<byte[]> CompressAsync([NotNull] byte[] sourceData);

    /// <summary>
    /// decompress compressed data
    /// </summary>
    /// <param name="compressedData">compressed data</param>
    /// <returns>source data</returns>
    [NotNull]
    byte[] Decompress([NotNull] byte[] compressedData);

    /// <summary>
    /// decompress compressed data async
    /// </summary>
    /// <param name="compressedData">compressed data</param>
    /// <returns>source data</returns>
    [NotNull]
    Task<byte[]> DecompressAsync([NotNull] byte[] compressedData);
}

/// <summary>
/// NullDataCompressor
/// do nothing compress
/// </summary>
public sealed class NullDataCompressor : IDataCompressor
{
    public byte[] Compress(byte[] sourceData)
    {
        return sourceData;
    }

    public Task<byte[]> CompressAsync(byte[] sourceData)
    {
        return Task.FromResult(sourceData);
    }

    public byte[] Decompress(byte[] compressedData)
    {
        return compressedData;
    }

    public Task<byte[]> DecompressAsync(byte[] compressedData)
    {
        return Task.FromResult(compressedData);
    }
}

/// <summary>
/// GZipDataCompressor
/// </summary>
public class GZipDataCompressor : IDataCompressor
{
    public byte[] Compress(byte[] sourceData)
    {
        return sourceData.CompressGZip();
    }

    public Task<byte[]> CompressAsync(byte[] sourceData)
    {
        return sourceData.CompressGZipAsync();
    }

    public byte[] Decompress(byte[] compressedData)
    {
        return compressedData.DecompressGZip();
    }

    public Task<byte[]> DecompressAsync(byte[] compressedData)
    {
        return compressedData.DecompressGZipAsync();
    }
}
