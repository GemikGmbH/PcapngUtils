using System.IO;
using PcapngUtils.PcapNG.BlockTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class PacketBlockTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PacketBlock_ConvertToByte_Test(bool reorder)
        {
            PacketBlock prePacketBlock, postPacketBlock;
            byte[] byteblock = { 2, 0, 0, 0, 167, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 123, 0, 0, 0, 232, 3, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 151, 143, 0, 243, 59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 208, 241, 255, 191, 127, 0, 0, 0, 208, 79, 17, 243, 59, 0, 0, 0, 96, 5, 0, 243, 59, 0, 0, 0, 252, 6, 0, 243, 59, 0, 0, 0, 96, 2, 0, 243, 59, 0, 0, 0, 88, 6, 64, 0, 0, 0, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 104, 83, 17, 243, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 167, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.NotNull(block);
                    prePacketBlock = block as PacketBlock;
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
                    postPacketBlock = block as PacketBlock;
                    Assert.NotNull(postPacketBlock);

                    Assert.Equal(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.Equal(prePacketBlock.Data, postPacketBlock.Data);
                    Assert.Equal(prePacketBlock.InterfaceId, postPacketBlock.InterfaceId);
                    Assert.Equal(prePacketBlock.DropCount, postPacketBlock.DropCount);
                    Assert.Equal(prePacketBlock.Microseconds, postPacketBlock.Microseconds);
                    Assert.Equal(prePacketBlock.PacketLength, postPacketBlock.PacketLength);
                    Assert.Equal(prePacketBlock.PositionInStream, postPacketBlock.PositionInStream);
                    Assert.Equal(prePacketBlock.Seconds, postPacketBlock.Seconds);
                    Assert.Equal(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                    Assert.Equal(prePacketBlock.Options.Hash, postPacketBlock.Options.Hash);
                    Assert.Equal(prePacketBlock.Options.PacketFlag, postPacketBlock.Options.PacketFlag);
                }
            }
        }
    }
}
