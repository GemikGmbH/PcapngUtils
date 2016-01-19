using System;
using PcapngUtils.PcapNG.CommonTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class TimestampHelperTest
    {
        [Fact]
        public void TimestampHelper_Simple_Test()
        {
            byte[] testData = { 1, 0, 0, 0, 1, 0, 0, 0 };
            uint timestampHigh = BitConverter.ToUInt32(testData, 0);
            TimestampHelper timestamp = new TimestampHelper(testData, false);
            Assert.Equal(timestamp.TimestampHigh, 1U);
            Assert.Equal(timestamp.TimestampLow, 1U);
            Assert.Equal(timestamp.Seconds, 4294UL);
            Assert.Equal(timestamp.Microseconds, 967297UL);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TimestampHelper_ConvertToByte_Test(bool reorder)
        {
            byte[] preData = { 1, 0, 0, 0, 1, 0, 0, 0 };

            TimestampHelper preTimestamp = new TimestampHelper(preData, false);
            byte[] postData = preTimestamp.ConvertToByte(reorder);
            TimestampHelper postTimestamp = new TimestampHelper(postData, reorder);
            Assert.Equal(preTimestamp, postTimestamp);
        }
    }
}
