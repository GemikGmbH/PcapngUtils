using System.IO;
using PcapngUtils.PcapNG.BlockTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class InterfaceStatisticsBlockTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InterfaceStatisticsBlock_ConvertToByte_Test(bool reorder)
        {
            InterfaceStatisticsBlock preStatisticBlock, postStatisticBlock;
            byte[] byteblock = { 5, 0, 0, 0, 108, 0, 0, 0, 1, 0, 0, 0, 34, 18, 5, 0, 87, 234, 56, 202, 1, 0, 28, 0, 67, 111, 117, 110, 116, 101, 114, 115, 32, 112, 114, 111, 118, 105, 100, 101, 100, 32, 98, 121, 32, 100, 117, 109, 112, 99, 97, 112, 2, 0, 8, 0, 34, 18, 5, 0, 36, 137, 18, 202, 3, 0, 8, 0, 34, 18, 5, 0, 87, 234, 56, 202, 4, 0, 8, 0, 56, 0, 0, 0, 0, 0, 0, 0, 5, 0, 8, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 108, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.NotNull(block);
                    preStatisticBlock = block as InterfaceStatisticsBlock;
                    Assert.NotNull(preStatisticBlock);
                    byteblock = preStatisticBlock.ConvertToByte(reorder, null);
                }
            }
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, reorder, null);
                    Assert.NotNull(block);
                    postStatisticBlock = block as InterfaceStatisticsBlock;
                    Assert.NotNull(postStatisticBlock);

                    Assert.Equal(preStatisticBlock.BlockType, postStatisticBlock.BlockType);
                    Assert.Equal(preStatisticBlock.InterfaceId, postStatisticBlock.InterfaceId);
                    Assert.Equal(preStatisticBlock.Timestamp, postStatisticBlock.Timestamp);
                    Assert.Equal(preStatisticBlock.Options.Comment, postStatisticBlock.Options.Comment);
                    Assert.Equal(preStatisticBlock.Options.DeliveredToUser, postStatisticBlock.Options.DeliveredToUser);
                    Assert.Equal(preStatisticBlock.Options.EndTime, postStatisticBlock.Options.EndTime);
                    Assert.Equal(preStatisticBlock.Options.StartTime, postStatisticBlock.Options.StartTime);
                    Assert.Equal(preStatisticBlock.Options.FilterAccept, postStatisticBlock.Options.FilterAccept);
                    Assert.Equal(preStatisticBlock.Options.InterfaceDrop, postStatisticBlock.Options.InterfaceDrop);
                    Assert.Equal(preStatisticBlock.Options.InterfaceReceived, postStatisticBlock.Options.InterfaceReceived);
                    Assert.Equal(preStatisticBlock.Options.SystemDrop, postStatisticBlock.Options.SystemDrop);
                }
            }
        }
    }
}
