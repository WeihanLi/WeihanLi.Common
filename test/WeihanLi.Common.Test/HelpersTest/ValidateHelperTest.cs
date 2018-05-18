using System;
using System.Collections.Generic;
using System.Text;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest
{
    public class ValidateHelperTest
    {
        [Fact]
        public void IsEmailTest()
        {
            Assert.False(ValidateHelper.IsEmail(null));
            Assert.False(ValidateHelper.IsEmail(""));
            Assert.False(ValidateHelper.IsEmail("123abc"));
            Assert.False(ValidateHelper.IsEmail("abc@"));
            Assert.True(ValidateHelper.IsEmail("abc@outlook.com"));
        }

        [Fact]
        public void IsMobileTest()
        {
            Assert.False(ValidateHelper.IsMobile(null));
            Assert.False(ValidateHelper.IsMobile(""));
            Assert.False(ValidateHelper.IsMobile("123abc"));
            Assert.False(ValidateHelper.IsMobile("abcdefghijk"));
            Assert.True(ValidateHelper.IsMobile("13245678901"));
        }
    }
}
