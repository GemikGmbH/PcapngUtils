using System.IO;
using PcapngUtils.PcapNG.BlockTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class EnchantedPacketBlockTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void EnchantedPacketBlock_ConvertToByte_Test(bool reorder)
        {
            EnchantedPacketBlock prePacketBlock, postPacketBlock;
            byte[] byteblock = { 6, 0, 0, 0, 132, 0, 0, 0, 0, 0, 0, 0, 12, 191, 4, 0, 118, 176, 176, 8, 98, 0, 0, 0, 98, 0, 0, 0, 0, 0, 94, 0, 1, 177, 0, 33, 40, 5, 41, 186, 8, 0, 69, 0, 0, 84, 48, 167, 0, 0, 255, 1, 3, 72, 192, 168, 177, 160, 10, 64, 11, 49, 8, 0, 10, 251, 67, 168, 0, 0, 79, 161, 27, 41, 0, 2, 83, 141, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 0, 0, 132, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.NotNull(block);
                    prePacketBlock = block as EnchantedPacketBlock;
                    Assert.NotNull(prePacketBlock);
                    byteblock = prePacketBlock.ConvertToByte(reorder, null);
                }
            }
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, reorder, null);
                    Assert.NotNull(block);
                    postPacketBlock = block as EnchantedPacketBlock;
                    Assert.NotNull(postPacketBlock);

                    Assert.Equal(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.Equal(prePacketBlock.Data, postPacketBlock.Data);
                    Assert.Equal(prePacketBlock.InterfaceID, postPacketBlock.InterfaceID);
                    Assert.Equal(prePacketBlock.Microseconds, postPacketBlock.Microseconds);
                    Assert.Equal(prePacketBlock.PacketLength, postPacketBlock.PacketLength);
                    Assert.Equal(prePacketBlock.PositionInStream, postPacketBlock.PositionInStream);
                    Assert.Equal(prePacketBlock.Seconds, postPacketBlock.Seconds);
                    Assert.Equal(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                    Assert.Equal(prePacketBlock.Options.DropCount, postPacketBlock.Options.DropCount);
                    Assert.Equal(prePacketBlock.Options.Hash, postPacketBlock.Options.Hash);
                    Assert.Equal(prePacketBlock.Options.PacketFlag, postPacketBlock.Options.PacketFlag);
                }
            }
        }
    }
}
