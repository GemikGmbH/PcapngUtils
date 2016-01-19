using System.Diagnostics.Contracts;
using System.IO;
using PcapngUtils.PcapNG.CommonTypes;
using PcapngUtils.PcapNG.OptionTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class InterfaceStatisticsOptionTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [ContractVerification(false)]
        public static void InterfaceStatisticsOption_ConvertToByte_Test(bool reorder)
        {
            InterfaceStatisticsOption preOption = new InterfaceStatisticsOption();
            InterfaceStatisticsOption postOption;
            preOption.Comment = "Test Comment";
            preOption.DeliveredToUser = 25;
            preOption.EndTime = new TimestampHelper(new byte[] { 1, 0, 0, 0, 2, 0, 0, 0 }, false);
            preOption.StartTime = new TimestampHelper(new byte[] { 1, 0, 0, 3, 2, 0, 0, 4 }, false);
            preOption.FilterAccept = 30;
            preOption.InterfaceDrop = 35;
            preOption.InterfaceReceived = 40;
            preOption.SystemDrop = 45;

            byte[] preOptionByte = preOption.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(preOptionByte))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postOption = InterfaceStatisticsOption.Parse(binaryReader, reorder, null);
                }
            }

            Assert.NotNull(postOption);
            Assert.Equal(preOption.Comment, postOption.Comment);
            Assert.Equal(preOption.DeliveredToUser, postOption.DeliveredToUser);
            Assert.Equal(preOption.EndTime.Seconds, postOption.EndTime.Seconds);
            Assert.Equal(preOption.EndTime.Microseconds, postOption.EndTime.Microseconds);
            Assert.Equal(preOption.StartTime.Seconds, postOption.StartTime.Seconds);
            Assert.Equal(preOption.StartTime.Microseconds, postOption.StartTime.Microseconds);
            Assert.Equal(preOption.FilterAccept, postOption.FilterAccept);
            Assert.Equal(preOption.InterfaceDrop, postOption.InterfaceDrop);
            Assert.Equal(preOption.InterfaceReceived, postOption.InterfaceReceived);
            Assert.Equal(preOption.SystemDrop, postOption.SystemDrop);
        }
    }
}
