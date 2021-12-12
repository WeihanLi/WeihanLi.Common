using System.Numerics;
using System.Text;

namespace WeihanLi.Common.Helpers;

public static class Base62Encoder
{
    private const string Charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string Encode(Guid guid)
    {
        var bytes = guid.ToByteArray().ToList();

        if (BitConverter.IsLittleEndian)
            bytes.Add(0);
        else
            bytes.Insert(0, 0);

        return Encode(bytes.ToArray());
    }

    public static string Encode(string text)
    {
        var bytes = Encoding.ASCII.GetBytes(text);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return Encode(bytes);
    }

    public static string Encode(long num)
    {
        var bytes = BitConverter.GetBytes(num).ToList();

        if (BitConverter.IsLittleEndian)
            bytes.Add(0);
        else
            bytes.Insert(0, 0);

        return Encode(bytes.ToArray());
    }

    private static string Encode(byte[] bytes)
    {
        var result = string.Empty;

        var bi = new BigInteger(bytes);

        do
        {
            result = Charset[(int)(bi % 62)] + result;
            bi /= 62;
        } while (bi > 0);

        return result;
    }

    public static string DecodeString(string codeStr)
    {
        var bytes = Decode(codeStr);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return Encoding.ASCII.GetString(bytes);
    }

    public static Guid DecodeGuid(string codeStr)
    {
        var bytes = Decode(codeStr).ToList();

        if (bytes.Count > 16)
        {
            if (BitConverter.IsLittleEndian)
            {
                bytes.RemoveAt(bytes.Count - 1);
            }
            else
            {
                bytes.RemoveAt(0);
            }
        }
        else if (bytes.Count < 16)
        {
            bytes.AddRange(Enumerable.Repeat((byte)0, 16 - bytes.Count));
        }

        return new Guid(bytes.ToArray());
    }

    public static long DecodeLong(string codeStr)
    {
        var bytes = Decode(codeStr).ToList();
        if (bytes.Count > 8)
        {
            if (BitConverter.IsLittleEndian)
            {
                bytes.RemoveAt(bytes.Count - 1);
            }
            else
            {
                bytes.RemoveAt(0);
            }
        }
        else if (bytes.Count < 8)
        {
            bytes.AddRange(Enumerable.Repeat((byte)0, 8 - bytes.Count));
        }
        return BitConverter.ToInt64(bytes.ToArray(), 0);
    }

    private static byte[] Decode(string codedStr)
    {
        var result = new BigInteger(0);
        var len = codedStr.Length;

        for (var i = 0; i < len; i++)
        {
            var ch = codedStr[i];
            var num = Charset.IndexOf(ch);
            result += num * BigInteger.Pow(62, len - i - 1);
        }

        return result.ToByteArray();
    }
}

public static class Base36Encoder
{
    private const string Charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Encode(Guid guid)
    {
        var bytes = guid.ToByteArray().ToList();

        if (BitConverter.IsLittleEndian)
            bytes.Add(0);
        else
            bytes.Insert(0, 0);

        return Encode(bytes.ToArray());
    }

    public static string Encode(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var bytes = Encoding.ASCII.GetBytes(text.ToUpper());

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return Encode(bytes);
    }

    public static string Encode(long num)
    {
        var bytes = BitConverter.GetBytes(num).ToList();

        if (BitConverter.IsLittleEndian)
            bytes.Add(0);
        else
            bytes.Insert(0, 0);

        return Encode(bytes.ToArray());
    }

    private static string Encode(byte[] bytes)
    {
        var result = string.Empty;

        var bi = new BigInteger(bytes);
        do
        {
            result = Charset[(int)(bi % 36)] + result;
            bi = bi / 36;
        } while (bi > 0);

        return result;
    }

    public static string DecodeString(string codeStr)
    {
        if (string.IsNullOrEmpty(codeStr))
            return string.Empty;

        var bytes = Decode(codeStr);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return Encoding.ASCII.GetString(bytes);
    }

    public static Guid DecodeGuid(string codeStr)
    {
        var bytes = Decode(codeStr).ToList();

        if (bytes.Count > 16)
        {
            if (BitConverter.IsLittleEndian)
            {
                bytes.RemoveAt(bytes.Count - 1);
            }
            else
            {
                bytes.RemoveAt(0);
            }
        }
        else if (bytes.Count < 16)
        {
            bytes.AddRange(Enumerable.Repeat((byte)0, 16 - bytes.Count));
        }

        return new Guid(bytes.ToArray());
    }

    public static long DecodeLong(string codeStr)
    {
        var bytes = Decode(codeStr).ToList();
        if (bytes.Count > 8)
        {
            if (BitConverter.IsLittleEndian)
            {
                bytes.RemoveAt(bytes.Count - 1);
            }
            else
            {
                bytes.RemoveAt(0);
            }
        }
        else if (bytes.Count < 8)
        {
            bytes.AddRange(Enumerable.Repeat((byte)0, 8 - bytes.Count));
        }
        return BitConverter.ToInt64(bytes.ToArray(), 0);
    }

    private static byte[] Decode(string codedStr)
    {
        if (string.IsNullOrEmpty(codedStr))
        {
            return Array.Empty<byte>();
        }

        var result = new BigInteger(0);
        var len = codedStr.Length;

        for (var i = 0; i < len; i++)
        {
            var ch = codedStr[i];
            var num = Charset.IndexOf(ch);
            result += num * BigInteger.Pow(36, len - i - 1);
        }

        var bytes = result.ToByteArray();

        return bytes;
    }
}
