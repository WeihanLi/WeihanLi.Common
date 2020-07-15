using System;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class EncoderTest
    {
        [Fact]
        public void Base62EncodeTest()
        {
            Base62Encoder.Encode(Guid.NewGuid());
            Base62Encoder.Encode(DateTime.UtcNow.Ticks);
            Base62Encoder.Encode("xxxxxxxxx");
            //Base62Encoder.Encode("你好", Encoding.UTF8); // not supported
        }

        [Fact]
        public void Base62GuidTest()
        {
            var guid = Guid.NewGuid();
            var encodedText = Base62Encoder.Encode(guid);
            var decodeGuid = Base62Encoder.DecodeGuid(encodedText);
            Assert.Equal(guid, decodeGuid);
        }

        [Theory]
        [InlineData(12345)]
        [InlineData(-12345)]
        [InlineData(123)]
        [InlineData(-123)]
        public void Base62LongTest(long num)
        {
            var encodedText = Base62Encoder.Encode(num);
            var decodeNum = Base62Encoder.DecodeLong(encodedText);
            Assert.Equal(num, decodeNum);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("Alice")]
        public void Base62StringTest(string str)
        {
            var encodedText = Base62Encoder.Encode(str);
            var decodeStr = Base62Encoder.DecodeString(encodedText);
            Assert.Equal(str, decodeStr);
        }

        [Fact]
        public void Base36EncodeTest()
        {
            Base36Encoder.Encode(Guid.NewGuid());
            Base36Encoder.Encode(DateTime.UtcNow.Ticks);
            Base36Encoder.Encode("xxxxxxxxx");
            //Base36Encoder.Encode("你好", Encoding.UTF8); // not supported
        }

        [Fact]
        public void Base36GuidTest()
        {
            var guid = Guid.NewGuid();
            var encodedText = Base36Encoder.Encode(guid);
            var decodeGuid = Base36Encoder.DecodeGuid(encodedText);
            Assert.Equal(guid, decodeGuid);
        }

        [Theory]
        [InlineData(12345)]
        [InlineData(-12345)]
        [InlineData(123)]
        [InlineData(-123)]
        public void Base36LongTest(long num)
        {
            var encodedText = Base36Encoder.Encode(num);
            var decodeNum = Base36Encoder.DecodeLong(encodedText);
            Assert.Equal(num, decodeNum);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("Alice")]
        public void Base36StringTest(string str)
        {
            var encodedText = Base36Encoder.Encode(str);
            var decodeStr = Base36Encoder.DecodeString(encodedText);
            Assert.Equal(str, decodeStr, ignoreCase: true);
        }
    }
}
