using System.IO;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IOExtension
    {
        /// <summary>
        /// 把字节数组全部写入当前流
        /// </summary>
        /// <param name="this">当前流</param>
        /// <param name="byteArray">要写入的字节数组</param>
        /// <returns></returns>
        public static Stream Write([NotNull] this Stream @this, [NotNull]byte[] byteArray)
        {
            @this.Write(byteArray, 0, byteArray.Length);
            return @this;
        }

        /// <summary>
        /// 把字节数组全部写入当前流
        /// </summary>
        /// <param name="this">当前流</param>
        /// <param name="byteArray">要写入的字节数组</param>
        /// <returns></returns>
        public static async Task<Stream> WriteAsync([NotNull] this Stream @this, [NotNull]byte[] byteArray)
        {
            await @this.WriteAsync(byteArray, 0, byteArray.Length);
            return @this;
        }

        /// <summary>
        /// 将一个Stream添加到另外一个Stream中
        /// </summary>
        /// <param name="this">当前Stream</param>
        /// <param name="stream">stream</param>
        /// <returns></returns>
        public static Stream Append([NotNull]this Stream @this, Stream stream)
        {
            @this.Write(stream.ToByteArray());
            return @this;
        }

        /// <summary>
        /// 将一个Stream添加到另外一个Stream中
        /// </summary>
        /// <param name="this">当前Stream</param>
        /// <param name="stream">stream</param>
        /// <returns></returns>
        public static async Task<Stream> AppendAsync([NotNull]this Stream @this, Stream stream)
        {
            await @this.WriteAsync(await stream.ToByteArrayAsync());
            return @this;
        }

        /// <summary>
        ///     A Stream extension method that converts the Stream to a byte array.
        /// </summary>
        /// <param name="this">The Stream to act on.</param>
        /// <returns>The Stream as a byte[].</returns>
        public static byte[] ToByteArray([NotNull]this Stream @this)
        {
            using (var ms = new MemoryStream())
            {
                @this.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        ///     A Stream extension method that converts the Stream to a byte array.
        /// </summary>
        /// <param name="this">The Stream to act on.</param>
        /// <returns>The Stream as a byte[].</returns>
        public static async Task<byte[]> ToByteArrayAsync([NotNull]this Stream @this)
        {
            using (var ms = new MemoryStream())
            {
                await @this.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        ///     A Stream extension method that reads a stream to the end.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>
        ///     The rest of the stream as a string, from the current position to the end. If the current position is at the
        ///     end of the stream, returns an empty string ("").
        /// </returns>
        public static string ReadToEnd([NotNull]this Stream @this)
        {
            using (var sr = new StreamReader(@this, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        ///     A Stream extension method that reads a stream to the end.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>
        ///     The rest of the stream as a string, from the current position to the end. If the current position is at the
        ///     end of the stream, returns an empty string ("").
        /// </returns>
        public static async Task<string> ReadToEndAsync([NotNull]this Stream @this)
        {
            using (var sr = new StreamReader(@this, Encoding.UTF8))
            {
                return await sr.ReadToEndAsync();
            }
        }

        /// <summary>
        ///     A Stream extension method that reads a stream to the end.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        ///     The rest of the stream as a string, from the current position to the end. If the current position is at the
        ///     end of the stream, returns an empty string ("").
        /// </returns>
        public static string ReadToEnd([NotNull]this Stream @this, Encoding encoding)
        {
            using (var sr = new StreamReader(@this, encoding))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        ///     A Stream extension method that reads a stream to the end.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        ///     The rest of the stream as a string, from the current position to the end. If the current position is at the
        ///     end of the stream, returns an empty string ("").
        /// </returns>
        public static async Task<string> ReadToEndAsync([NotNull]this Stream @this, Encoding encoding)
        {
            using (var sr = new StreamReader(@this, encoding))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}
