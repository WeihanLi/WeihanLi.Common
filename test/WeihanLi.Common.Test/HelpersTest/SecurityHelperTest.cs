using System.Text;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class SecurityHelperTest
    {
        #region IsSafeSqlString

        [Fact]
        public void IsSafeSqlStringNullOrEmptyTest()
        {
            // no strict
            Assert.True(SecurityHelper.IsSafeSqlString("", false));
            Assert.True(SecurityHelper.IsSafeSqlString(null, false));

            // strict
            Assert.True(SecurityHelper.IsSafeSqlString(""));
            Assert.True(SecurityHelper.IsSafeSqlString(null));
        }

        [Fact]
        public void IsSafeSqlStringBlackTest()
        {
            Assert.False(SecurityHelper.IsSafeSqlString("drop table test", false));
            Assert.False(SecurityHelper.IsSafeSqlString("drop table test"));

            Assert.True(SecurityHelper.IsSafeSqlString(";ll,ss", false));
            Assert.False(SecurityHelper.IsSafeSqlString(";ll,ss"));
        }

        [Theory]
        [InlineData("abcde")]
        [InlineData("12345")]
        public void IsSafeSqlStringWhiteTest(string testString)
        {
            Assert.True(SecurityHelper.IsSafeSqlString(testString));
        }

        #endregion IsSafeSqlString

        #region GenerateRandomCode

        [Theory]
        [InlineData(6)]
        public void GenerateRandomCodeLengthTest(int length)
        {
            Assert.Equal(SecurityHelper.GenerateRandomCode(length).Length,length);
        }

        [Fact]
        public void GenerateRandomCodeContentTest()
        {
            Assert.Matches("^([0-9]*)$", SecurityHelper.GenerateRandomCode(4,true));
            Assert.Matches("^([0-9a-z]*)$", SecurityHelper.GenerateRandomCode(4));
        }
        #endregion

        #region Hash

        [Theory]
        [InlineData("12345")]
        public void HashStringTest(string str)
        {
            // MD5
            // Hash result test
            Assert.Equal(HashHelper.GetHashedString(HashType.MD5, str),SecurityHelper.MD5_Encrypt(str));
            // case
            Assert.Equal(HashHelper.GetHashedString(HashType.MD5, str,true), SecurityHelper.MD5_Encrypt(str,true));
            // Encoding test
            Assert.Equal(HashHelper.GetHashedString(HashType.MD5, str),HashHelper.GetHashedString(HashType.MD5, str,Encoding.UTF8));
            // SHA1
            // Hash result test
            Assert.Equal(HashHelper.GetHashedString(HashType.SHA1, str), SecurityHelper.SHA1_Encrypt(str));
            // case
            Assert.Equal(HashHelper.GetHashedString(HashType.SHA1, str, true), SecurityHelper.SHA1_Encrypt(str, true));
            // Encoding test
            Assert.Equal(HashHelper.GetHashedString(HashType.SHA1, str), HashHelper.GetHashedString(HashType.SHA1, str, Encoding.UTF8));
            // SHA256
            // Hash result test
            Assert.Equal(HashHelper.GetHashedString(HashType.SHA256, str), SecurityHelper.SHA256_Encrypt(str));
            // case
            Assert.Equal(HashHelper.GetHashedString(HashType.SHA256, str, true), SecurityHelper.SHA256_Encrypt(str, true));
            // Encoding test
            Assert.Equal(HashHelper.GetHashedString(HashType.SHA256, str), HashHelper.GetHashedString(HashType.SHA256, str, Encoding.UTF8));
            // SHA384
            // Encoding test
            Assert.Equal(HashHelper.GetHashedString(HashType.SHA384, str), HashHelper.GetHashedString(HashType.SHA384, str, Encoding.UTF8));
            // SHA512
            // Hash result test
            Assert.Equal(HashHelper.GetHashedString(HashType.SHA512, str), SecurityHelper.SHA512_Encrypt(str));
            // case
            Assert.Equal(HashHelper.GetHashedString(HashType.SHA512, str, true), SecurityHelper.SHA512_Encrypt(str, true));
            // Encoding test
            Assert.Equal(HashHelper.GetHashedString(HashType.SHA512, str), HashHelper.GetHashedString(HashType.SHA512, str, Encoding.UTF8));
        }

        [Fact]
        public void HashNullOrEmptyTest()
        {
            Assert.Equal(SecurityHelper.MD5_Encrypt(null),"");
            Assert.Equal(SecurityHelper.MD5_Encrypt(null),HashHelper.GetHashedString(HashType.MD5, null));
            Assert.Equal(SecurityHelper.MD5_Encrypt(""), "");
            Assert.Equal(SecurityHelper.MD5_Encrypt(""), HashHelper.GetHashedString(HashType.MD5, ""));
        }
        #endregion
    }
}