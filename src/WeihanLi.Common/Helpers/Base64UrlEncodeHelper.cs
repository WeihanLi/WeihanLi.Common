using System.Text;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class Base64UrlEncodeHelper
{
    private const char Base64PadCharacter = '=';
    private const string DoubleBase64PadCharacter = "==";
    private const char Base64Character62 = '+';
    private const char Base64Character63 = '/';
    private const char Base64UrlCharacter62 = '-';
    private const char Base64UrlCharacter63 = '_';

    /// <summary>
    /// The following functions perform base64url encoding which differs from regular base64 encoding as follows
    /// * padding is skipped so the pad character '=' doesn't have to be percent encoded
    /// * the 62nd and 63rd regular base64 encoding characters ('+' and '/') are replace with ('-' and '_')
    /// The changes make the encoding alphabet file and URL safe.
    /// </summary>
    /// <param name="arg">string to encode.</param>
    /// <returns>Base64Url encoding of the UTF8 bytes.</returns>
    public static string Encode(string arg)
    {
        if (arg == null)
        {
            throw new ArgumentNullException(nameof(arg));
        }
        return Encode(arg.GetBytes());
    }

    /// <summary>
    /// Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64-url digits. Parameters specify
    /// the subset as an offset in the input array, and the number of elements in the array to convert.
    /// </summary>
    /// <param name="inArray">An array of 8-bit unsigned integers.</param>
    /// <param name="length">An offset in inArray.</param>
    /// <param name="offset">The number of elements of inArray to convert.</param>
    /// <returns>The string representation in base 64 url encoding of length elements of inArray, starting at position offset.</returns>
    /// <exception cref="T:System.ArgumentNullException">'inArray' is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">offset or length is negative OR offset plus length is greater than the length of inArray.</exception>
    public static string Encode(byte[] inArray, int offset, int length)
    {
        if (inArray == null)
        {
            throw new ArgumentNullException(nameof(inArray));
        }
        return Convert.ToBase64String(inArray, offset, length).Split(Base64PadCharacter)[0].Replace(Base64Character62, Base64UrlCharacter62).Replace(Base64Character63, Base64UrlCharacter63);
    }

    /// <summary>
    /// Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64-url digits. Parameters specify
    /// the subset as an offset in the input array, and the number of elements in the array to convert.
    /// </summary>
    /// <param name="inArray">An array of 8-bit unsigned integers.</param>
    /// <returns>The string representation in base 64 url encoding of length elements of inArray, starting at position offset.</returns>
    /// <exception cref="T:System.ArgumentNullException">'inArray' is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">offset or length is negative OR offset plus length is greater than the length of inArray.</exception>
    public static string Encode(byte[] inArray)
    {
        if (inArray == null)
        {
            throw new ArgumentNullException(nameof(inArray));
        }
        return Convert.ToBase64String(inArray, 0, inArray.Length).Split(Base64PadCharacter)[0].Replace(Base64Character62, Base64UrlCharacter62).Replace(Base64Character63, Base64UrlCharacter63);
    }

    /// <summary>
    /// Converts the specified string, which encodes binary data as base-64-url digits, to an equivalent 8-bit unsigned integer array.</summary>
    /// <param name="str">base64Url encoded string.</param>
    /// <returns>UTF8 bytes.</returns>
    public static byte[] DecodeBytes(string str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }
        str = str.Replace(Base64UrlCharacter62, Base64Character62);
        str = str.Replace(Base64UrlCharacter63, Base64Character63);
        switch (str.Length % 4)
        {
            case 0:
                return Convert.FromBase64String(str);

            case 2:
                str += DoubleBase64PadCharacter;
                goto case 0;
            case 3:
                str += Base64PadCharacter.ToString();
                goto case 0;
            default:
                throw new FormatException($"IDX10400: Unable to decode: '{str}' as Base64url encoded string.");
        }
    }

    /// <summary>Decodes the string from Base64UrlEncoded to UTF8.</summary>
    /// <param name="arg">string to decode.</param>
    /// <returns>UTF8 string.</returns>
    public static string Decode(string arg)
    {
        return Encoding.UTF8.GetString(DecodeBytes(arg));
    }
}
