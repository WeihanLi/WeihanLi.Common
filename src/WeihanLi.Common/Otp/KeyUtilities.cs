using System;

namespace WeihanLi.Common.Otp
{
    /// <summary>
    /// Some helper methods to perform common key functions
    /// </summary>
    internal class KeyUtilities
    {
        /// <summary>
        /// Overwrite potentially sensitive data with random junk
        /// </summary>
        /// <remarks>
        /// Warning!
        ///
        /// This isn't foolproof by any means.  The garbage collector could have moved the actual
        /// location in memory to another location during a collection cycle and left the old data in place
        /// simply marking it as available.  We can't control this or even detect it.
        /// This method is simply a good faith effort to limit the exposure of sensitive data in memory as much as possible
        /// </remarks>
        internal static void Destroy(byte[] sensitiveData)
        {
            if (sensitiveData == null)
                throw new ArgumentNullException(nameof(sensitiveData));
            new Random().NextBytes(sensitiveData);
        }

        /// <summary>
        /// converts a long into a big endian byte array.
        /// </summary>
        /// <remarks>
        /// RFC 4226 specifies big endian as the method for converting the counter to data to hash.
        /// </remarks>
        internal static byte[] GetBigEndianBytes(long input)
        {
            // Since .net uses little endian numbers, we need to reverse the byte order to get big endian.
            var data = BitConverter.GetBytes(input);
            Array.Reverse(data);
            return data;
        }

        /// <summary>
        /// converts an int into a big endian byte array.
        /// </summary>
        /// <remarks>
        /// RFC 4226 specifies big endian as the method for converting the counter to data to hash.
        /// </remarks>
        internal static byte[] GetBigEndianBytes(int input)
        {
            // Since .net uses little endian numbers, we need to reverse the byte order to get big endian.
            var data = BitConverter.GetBytes(input);
            Array.Reverse(data);
            return data;
        }
    }
}
