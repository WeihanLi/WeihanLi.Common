using System.Security.Cryptography;
using System.Text;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public class SecurityHelperTest
{
    #region GenerateRandomCode

    [Theory]
    [InlineData(6)]
    public void GenerateRandomCodeLengthTest(int length)
    {
        Assert.Equal(SecurityHelper.GenerateRandomCode(length).Length, length);
    }

    [Fact]
    public void GenerateRandomCodeContentTest()
    {
        Assert.Matches("^([0-9]*)$", SecurityHelper.GenerateRandomCode(4, true));
        Assert.Matches("^([0-9a-z]*)$", SecurityHelper.GenerateRandomCode(4));
    }

    #endregion GenerateRandomCode

    #region Hash

    [Theory]
    [InlineData("12345")]
    public void HashStringTest(string str)
    {
        // MD5
        // Hash result test
        Assert.Equal(HashHelper.GetHashedString(HashType.MD5, str), SecurityHelper.MD5(str));
        // case
        Assert.Equal(HashHelper.GetHashedString(HashType.MD5, str, true), SecurityHelper.MD5(str, true));
        // Encoding test
        Assert.Equal(HashHelper.GetHashedString(HashType.MD5, str), HashHelper.GetHashedString(HashType.MD5, str, Encoding.UTF8));
        // SHA1
        // Hash result test
        Assert.Equal(HashHelper.GetHashedString(HashType.SHA1, str), SecurityHelper.SHA1(str));
        // case
        Assert.Equal(HashHelper.GetHashedString(HashType.SHA1, str, true), SecurityHelper.SHA1(str, true));
        // Encoding test
        Assert.Equal(HashHelper.GetHashedString(HashType.SHA1, str), HashHelper.GetHashedString(HashType.SHA1, str, Encoding.UTF8));

        // Encoding test
        Assert.Equal(HashHelper.GetHashedString(HashType.SHA256, str), HashHelper.GetHashedString(HashType.SHA256, str, Encoding.UTF8));

        // Encoding test
        Assert.Equal(HashHelper.GetHashedString(HashType.SHA512, str), HashHelper.GetHashedString(HashType.SHA512, str, Encoding.UTF8));
    }

    [Fact]
    public void HashNullOrEmptyTest()
    {
        Assert.Equal("", SecurityHelper.MD5(""));
        Assert.Equal(SecurityHelper.MD5(""), HashHelper.GetHashedString(HashType.MD5, ""));
    }

    #endregion Hash

    [Theory]
    [InlineData("Hello World")]
    [InlineData("Amazing .NET")]
    public void AesEncrypt(string input)
    {
        var key = "1234567890ABCDEF";
        
        var encrypted = SecurityHelper.AesEncrypt(input, key);
        Assert.NotNull(encrypted);
        Assert.NotEmpty(encrypted);

        var decrypted = SecurityHelper.AesDecrypt(encrypted, key);
        Assert.Equal(input, decrypted);
    }
    
    [Theory]
    [InlineData("Hello World")]
    [InlineData("Amazing .NET")]
    public void AesEncryptWithIV(string input)
    {
        var key = "1234567890ABCDEF";
        var iv = SecurityHelper.GenerateRandomCode(16, true);
        
        var encrypted = SecurityHelper.AesEncrypt(input, key, iv);
        Assert.NotNull(encrypted);
        Assert.NotEmpty(encrypted);

        var decrypted = SecurityHelper.AesDecrypt(encrypted, key, iv);
        Assert.Equal(input, decrypted);
    }
    
    [Theory]
    [InlineData("Test1234", "287E494548B152837EC742DDE76F0A4F")]
    public void AesEncryptExpect(string input, string expectedEncrypted)
    {
        var key = "1234567890ABCDEF";
        
        var encrypted = SecurityHelper.AesEncrypt(input, key);
        Assert.NotNull(encrypted);
        Assert.NotEmpty(encrypted);
        Assert.Equal(expectedEncrypted, encrypted);

        var decrypted = SecurityHelper.AesDecrypt(encrypted, key);
        Assert.Equal(input, decrypted);
    }
    
    [Theory]
    [InlineData("Test1234", "119CBC45E61827B448D704070EDA0A81")]
    public void AesEncryptWithIVExpect(string input, string expectedEncrypted)
    {
        var key = "1234567890ABCDEF";
        
        var encrypted = SecurityHelper.AesEncrypt(input, key, key);
        Assert.NotNull(encrypted);
        Assert.NotEmpty(encrypted);
        Assert.Equal(expectedEncrypted, encrypted);

        var decrypted = SecurityHelper.AesDecrypt(encrypted, key, key);
        Assert.Equal(input, decrypted);
    }
}
