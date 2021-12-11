using JetBrains.Annotations;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

/// <summary>
/// CompressionExtension
/// </summary>
public static class CompressionExtension
{
    /// <summary>
    ///     A string extension method that compress the given string to GZip byte array.
    /// </summary>
    /// <param name="this">The stringToCompress to act on.</param>
    /// <returns>The string compressed into a GZip byte array.</returns>
    public static byte[] CompressGZip([NotNull] this string @this)
        => @this.CompressGZip(Encoding.UTF8);

    /// <summary>
    ///     A string extension method that compress the given string to GZip byte array.
    /// </summary>
    /// <param name="this">The stringToCompress to act on.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>The string compressed into a GZip byte array.</returns>
    public static byte[] CompressGZip([NotNull] this string @this, Encoding encoding) => encoding.GetBytes(@this).CompressGZip();

    /// <summary>
    ///     A byteArray extension method that compress the given byte array to GZip byte array.
    /// </summary>
    /// <param name="bytes">The stringToCompress to act on.</param>
    /// <returns>The string compressed into a GZip byte array.</returns>
    public static byte[] CompressGZip([NotNull] this byte[] bytes)
    {
        using var memoryStream = new MemoryStream();
        using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        {
            zipStream.Write(bytes);
        }
        return memoryStream.ToArray();
    }

    public static async Task<byte[]> CompressGZipAsync([NotNull] this byte[] bytes)
    {
        using var memoryStream = new MemoryStream();
        using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        {
            await zipStream.WriteAsync(bytes);
        }
        return memoryStream.ToArray();
    }

    public static byte[] CompressGZip([NotNull] this Stream stream)
    {
        using var memoryStream = new MemoryStream();
        using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        {
            stream.CopyTo(zipStream);
        }
        return memoryStream.ToArray();
    }

    public static async Task<byte[]> CompressGZipAsync([NotNull] this Stream stream)
    {
        using var memoryStream = new MemoryStream();
        using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        {
            await stream.CopyToAsync(zipStream);
        }
        return memoryStream.ToArray();
    }

    /// <summary>
    ///     A byte[] extension method that decompress the byte array gzip to string.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>The byte array gzip to string.</returns>
    public static byte[] DecompressGZip([NotNull] this byte[] @this)
    {
        using var memoryStream = new MemoryStream(@this);
        return memoryStream.DecompressGZip();
    }

    public static async Task<byte[]> DecompressGZipAsync([NotNull] this byte[] @this)
    {
        using var memoryStream = new MemoryStream(@this);
        return await memoryStream.DecompressGZipAsync();
    }

    public static byte[] DecompressGZip([NotNull] this Stream stream)
    {
        using var outStream = new MemoryStream();
        using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
        {
            zipStream.CopyTo(outStream);
        }
        return outStream.GetBuffer();
    }

    public static async Task<byte[]> DecompressGZipAsync([NotNull] this Stream stream)
    {
        using var outStream = new MemoryStream();
        using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
        {
            await zipStream.CopyToAsync(outStream);
        }
        return outStream.GetBuffer();
    }

    public static string CompressGZipString([NotNull] this byte[] bytes) => bytes.CompressGZipString(Encoding.UTF8);

    public static string CompressGZipString([NotNull] this byte[] bytes, Encoding encoding)
    {
        using var memoryStream = new MemoryStream();
        using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        {
            zipStream.Write(bytes);
        }
        return encoding.GetString(memoryStream.ToArray());
    }

    public static string DecompressGZipString([NotNull] this byte[] bytes) =>
        bytes.DecompressGZipString(Encoding.UTF8);

    public static string DecompressGZipString([NotNull] this byte[] bytes, Encoding encoding) => encoding.GetString(bytes.DecompressGZip());

    /// <summary>
    ///     A FileInfo extension method that creates a zip file.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    public static void CreateGZip([NotNull] this FileInfo @this)
    {
        using var originalFileStream = @this.OpenRead();
        using var compressedFileStream = File.Create(@this.FullName + ".gz");
        using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressionStream);
    }

    /// <summary>
    ///     A FileInfo extension method that creates a zip file.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destination">Destination for the zip.</param>
    public static void CreateGZip([NotNull] this FileInfo @this, string destination)
    {
        using var originalFileStream = @this.OpenRead();
        using var compressedFileStream = File.Create(destination);
        using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressionStream);
    }

    /// <summary>
    ///     A FileInfo extension method that creates a zip file.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destination">Destination for the zip.</param>
    public static void CreateGZip([NotNull] this FileInfo @this, FileInfo destination)
    {
        using var originalFileStream = @this.OpenRead();
        using var compressedFileStream = File.Create(destination.FullName);
        using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressionStream);
    }

    /// <summary>
    ///     A FileInfo extension method that extracts the g zip to directory described by
    ///     @this.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    public static void ExtractGZipToDirectory([NotNull] this FileInfo @this)
    {
        using var originalFileStream = @this.OpenRead();
        var newFileName = Path.GetFileNameWithoutExtension(@this.FullName);

        using var decompressedFileStream = File.Create(newFileName);
        using var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress);
        decompressionStream.CopyTo(decompressedFileStream);
    }

    /// <summary>
    ///     A FileInfo extension method that extracts the g zip to directory described by
    ///     @this.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destination">Destination for the.</param>
    public static void ExtractGZipToDirectory([NotNull] this FileInfo @this, string destination)
    {
        using var originalFileStream = @this.OpenRead();
        using var compressedFileStream = File.Create(destination);
        using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressionStream);
    }

    /// <summary>
    ///     A FileInfo extension method that extracts the g zip to directory described by
    ///     @this.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destination">Destination for the.</param>
    public static void ExtractGZipToDirectory([NotNull] this FileInfo @this, FileInfo destination)
    {
        using var originalFileStream = @this.OpenRead();
        using var compressedFileStream = File.Create(destination.FullName);
        using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressionStream);
    }

    /// <summary>Opens a zip archive at the specified path and in the specified mode.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="mode">
    ///     One of the enumeration values that specifies the actions that are allowed
    ///     on the entries in the opened archive.
    /// </param>
    /// <returns>A ZipArchive.</returns>
    public static ZipArchive OpenZipFile([NotNull] this FileInfo @this, ZipArchiveMode mode)
    {
        return ZipFile.Open(@this.FullName, mode);
    }

    /// <summary>Opens a zip archive at the specified path and in the specified mode.</summary>
    /// <param name="this">
    ///     The path to the archive to open, specified as a relative or absolute
    ///     path. A relative path is interpreted as relative to the current working directory.
    /// </param>
    /// <param name="mode">
    ///     One of the enumeration values that specifies the actions that are allowed
    ///     on the entries in the opened archive.
    /// </param>
    /// <param name="entryNameEncoding">
    ///     The encoding to use when reading or writing entry names in
    ///     this archive. Specify a value for this parameter only when an encoding is required for
    ///     interoperability with zip archive tools and libraries that do not support UTF-8 encoding for
    ///     entry names.
    /// </param>
    /// <returns>A ZipArchive.</returns>
    public static ZipArchive OpenZipFile([NotNull] this FileInfo @this, ZipArchiveMode mode, Encoding entryNameEncoding)
    {
        return ZipFile.Open(@this.FullName, mode, entryNameEncoding);
    }

    /// <summary>
    ///     The path to the archive to open, specified as a relative or absolute path. A relative path is interpreted as
    ///     relative to the current working directory.
    /// </summary>
    /// <param name="this">
    ///     The path to the archive to open, specified as a relative or absolute path. A relative path is
    ///     interpreted as relative to the current working directory.
    /// </param>
    /// <returns>The opened zip archive.</returns>
    public static ZipArchive OpenReadZipFile([NotNull] this FileInfo @this)
    {
        return ZipFile.OpenRead(@this.FullName);
    }

    /// <summary>
    ///     Extracts all the files in the specified zip archive to a directory on the file system
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destinationDirectoryName">
    ///     The path to the directory in which to place the
    ///     extracted files, specified as a relative or absolute path. A relative path is interpreted as
    ///     relative to the current working directory.
    /// </param>
    public static void ExtractZipFileToDirectory([NotNull] this FileInfo @this, string destinationDirectoryName)
    {
        ZipFile.ExtractToDirectory(@this.FullName, destinationDirectoryName);
    }

    /// <summary>
    ///     Extracts all the files in the specified zip archive to a directory on the file system and uses the specified
    ///     character encoding for entry names.
    /// </summary>
    /// <param name="this">The path to the archive that is to be extracted.</param>
    /// <param name="destinationDirectoryName">
    ///     The path to the directory in which to place the extracted files, specified as a
    ///     relative or absolute path. A relative path is interpreted as relative to the current working directory.
    /// </param>
    /// <param name="entryNameEncoding">
    ///     The encoding to use when reading or writing entry names in this archive. Specify a
    ///     value for this parameter only when an encoding is required for interoperability with zip archive tools and
    ///     libraries that do not support UTF-8 encoding for entry names.
    /// </param>
    public static void ExtractZipFileToDirectory([NotNull] this FileInfo @this, string destinationDirectoryName, Encoding entryNameEncoding)
    {
        ZipFile.ExtractToDirectory(@this.FullName, destinationDirectoryName, entryNameEncoding);
    }

    /// <summary>Extracts all the files in the specified zip archive to a directory on the file system.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destinationDirectory">Pathname of the destination directory.</param>
    public static void ExtractZipFileToDirectory([NotNull] this FileInfo @this, DirectoryInfo destinationDirectory)
    {
        ZipFile.ExtractToDirectory(@this.FullName, destinationDirectory.FullName);
    }

    /// <summary>
    ///     Extracts all the files in the specified zip archive to a directory on the file system
    ///     and uses the specified character encoding for entry names.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destinationDirectory">Pathname of the destination directory.</param>
    /// <param name="entryNameEncoding">
    ///     The encoding to use when reading or writing entry names in
    ///     this archive. Specify a value for this parameter only when an encoding is required for
    ///     interoperability with zip archive tools and libraries that do not support UTF-8 encoding for
    ///     entry names.
    /// </param>
    public static void ExtractZipFileToDirectory([NotNull] this FileInfo @this, DirectoryInfo destinationDirectory, Encoding entryNameEncoding)
    {
        ZipFile.ExtractToDirectory(@this.FullName, destinationDirectory.FullName, entryNameEncoding);
    }

    /// <summary>
    ///     Creates a zip archive that contains the files and directories from the specified
    ///     directory.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destinationArchiveFileName">
    ///     The path of the archive to be created, specified as a
    ///     relative or absolute path. A relative path is interpreted as relative to the current working
    ///     directory.
    /// </param>
    public static void CreateZipFile([NotNull] this DirectoryInfo @this, string destinationArchiveFileName)
    {
        ZipFile.CreateFromDirectory(@this.FullName, destinationArchiveFileName);
    }

    /// <summary>
    ///     Creates a zip archive that contains the files and directories from the specified
    ///     directory, uses the specified compression level, and optionally includes the base directory.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destinationArchiveFileName">
    ///     The path of the archive to be created, specified as a
    ///     relative or absolute path. A relative path is interpreted as relative to the current working
    ///     directory.
    /// </param>
    /// <param name="compressionLevel">
    ///     One of the enumeration values that indicates whether to
    ///     emphasize speed or compression effectiveness when creating the entry.
    /// </param>
    /// <param name="includeBaseDirectory">
    ///     true to include the directory name from
    ///     sourceDirectoryName at the root of the archive; false to include only the contents of the
    ///     directory.
    /// </param>
    public static void CreateZipFile([NotNull] this DirectoryInfo @this, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory)
    {
        ZipFile.CreateFromDirectory(@this.FullName, destinationArchiveFileName, compressionLevel, includeBaseDirectory);
    }

    /// <summary>
    ///     Creates a zip archive that contains the files and directories from the specified directory, uses the specified
    ///     compression level and character encoding for entry names, and optionally includes the base directory.
    /// </summary>
    /// <param name="this">
    ///     The path to the directory to be archived, specified as a relative or absolute path. A relative path
    ///     is interpreted as relative to the current working directory.
    /// </param>
    /// <param name="destinationArchiveFileName">
    ///     The path of the archive to be created, specified as a relative or absolute
    ///     path. A relative path is interpreted as relative to the current working directory.
    /// </param>
    /// <param name="compressionLevel">
    ///     One of the enumeration values that indicates whether to emphasize speed or compression
    ///     effectiveness when creating the entry.
    /// </param>
    /// <param name="includeBaseDirectory">
    ///     true to include the directory name from sourceDirectoryName at the root of the
    ///     archive; false to include only the contents of the directory.
    /// </param>
    /// <param name="entryNameEncoding">
    ///     The encoding to use when reading or writing entry names in this archive. Specify a
    ///     value for this parameter only when an encoding is required for interoperability with zip archive tools and
    ///     libraries that do not support UTF-8 encoding for entry names.
    /// </param>
    public static void CreateZipFile([NotNull] this DirectoryInfo @this, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding)
    {
        ZipFile.CreateFromDirectory(@this.FullName, destinationArchiveFileName, compressionLevel, includeBaseDirectory, entryNameEncoding);
    }

    /// <summary>
    ///     Creates a zip archive that contains the files and directories from the specified
    ///     directory.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destinationArchiveFile">
    ///     The path of the archive to be created, specified as a
    ///     relative or absolute path. A relative path is interpreted as relative to the current working
    ///     directory.
    /// </param>
    public static void CreateZipFile([NotNull] this DirectoryInfo @this, FileInfo destinationArchiveFile)
    {
        ZipFile.CreateFromDirectory(@this.FullName, destinationArchiveFile.FullName);
    }

    /// <summary>
    ///     Creates a zip archive that contains the files and directories from the specified
    ///     directory, uses the specified compression level, and optionally includes the base directory.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destinationArchiveFile">
    ///     The path of the archive to be created, specified as a
    ///     relative or absolute path. A relative path is interpreted as relative to the current working
    ///     directory.
    /// </param>
    /// <param name="compressionLevel">
    ///     One of the enumeration values that indicates whether to
    ///     emphasize speed or compression effectiveness when creating the entry.
    /// </param>
    /// <param name="includeBaseDirectory">
    ///     true to include the directory name from
    ///     sourceDirectoryName at the root of the archive; false to include only the contents of the
    ///     directory.
    /// </param>
    public static void CreateZipFile([NotNull] this DirectoryInfo @this, FileInfo destinationArchiveFile, CompressionLevel compressionLevel, bool includeBaseDirectory)
    {
        ZipFile.CreateFromDirectory(@this.FullName, destinationArchiveFile.FullName, compressionLevel, includeBaseDirectory);
    }

    /// <summary>
    ///     Creates a zip archive that contains the files and directories from the specified
    ///     directory, uses the specified compression level and character encoding for entry names, and
    ///     optionally includes the base directory.
    /// </summary>
    /// <param name="this">
    ///     The path to the directory to be archived, specified as a relative or
    ///     absolute path. A relative path is interpreted as relative to the current working directory.
    /// </param>
    /// <param name="destinationArchiveFile">
    ///     The path of the archive to be created, specified as a
    ///     relative or absolute path. A relative path is interpreted as relative to the current working
    ///     directory.
    /// </param>
    /// <param name="compressionLevel">
    ///     One of the enumeration values that indicates whether to
    ///     emphasize speed or compression effectiveness when creating the entry.
    /// </param>
    /// <param name="includeBaseDirectory">
    ///     true to include the directory name from
    ///     sourceDirectoryName at the root of the archive; false to include only the contents of the
    ///     directory.
    /// </param>
    /// <param name="entryNameEncoding">
    ///     The encoding to use when reading or writing entry names in
    ///     this archive. Specify a value for this parameter only when an encoding is required for
    ///     interoperability with zip archive tools and libraries that do not support UTF-8 encoding for
    ///     entry names.
    /// </param>
    public static void CreateZipFile([NotNull] this DirectoryInfo @this, FileInfo destinationArchiveFile, CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding entryNameEncoding)
    {
        ZipFile.CreateFromDirectory(@this.FullName, destinationArchiveFile.FullName, compressionLevel, includeBaseDirectory, entryNameEncoding);
    }
}
