using System;
using PcapngUtils.Extensions;
using Xunit;

namespace PcapngUtilsTests
{
    public class ReverseByteOrderExtensionTest
    {
        [Fact]
        public void ReverseByteOrderExtension_UInt16_Test()
        {
            UInt16 origin = 0xABCD;
            UInt16 test = origin.ReverseByteOrder(false);
            Assert.Equal(test, origin);
            test = origin.ReverseByteOrder(true);
            Assert.Equal(test, 0xCDAB);
        }

        [Fact]
        public void ReverseByteOrderExtension_UInt32_Test()
        {
            UInt32 origin = 0xABCDEF01;
            UInt32 test = origin.ReverseByteOrder(false);
            Assert.Equal(test, origin);
            test = origin.ReverseByteOrder(true);
            Assert.Equal(test, 0x01EFCDABU);
        }

        [Fact]
        public void ReverseByteOrderExtension_UInt64_Test()
        {
            UInt64 origin = 0x0123456789ABCDEF;
            UInt64 test = origin.ReverseByteOrder(false);
            Assert.Equal(test, origin);
            test = origin.ReverseByteOrder(true);
            Assert.Equal(test, 0xEFCDAB8967452301);
        }

        [Fact]
        public void ReverseByteOrderExtension_Int16_Test()
        {
            Int16 origin = 0x3210;
            Int16 test = origin.ReverseByteOrder(false);
            Assert.Equal(test, origin);
            test = origin.ReverseByteOrder(true);
            Assert.Equal(test, 0x1032);
        }

        [Fact]
        public void ReverseByteOrderExtension_Int32_Test()
        {
            Int32 origin = 0x76543210;
            Int32 test = origin.ReverseByteOrder(false);
            Assert.Equal(test, origin);
            test = origin.ReverseByteOrder(true);
            Assert.Equal(test, 0x10325476);
        }

        [Fact]
        public void ReverseByteOrderExtension_Int64_Test()
        {
            Int64 origin = 0x7654321076543210;
            Int64 test = origin.ReverseByteOrder(false);
            Assert.Equal(test, origin);
            test = origin.ReverseByteOrder(true);
            Assert.Equal(test, 0x1032547610325476);
        }
    }
}