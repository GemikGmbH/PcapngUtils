using System.IO;
using PcapngUtils.PcapNG.BlockTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class SimplePacketBlockTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SimplePacketBlock_ConvertToByte_Test(bool reorder)
        {
            SimplePacketBlock prePacketBlock, postPacketBlock;
            byte[] byteblock = { 3, 0, 0, 0, 139, 0, 0, 0, 123, 0, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 151, 143, 0, 243, 59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 208, 241, 255, 191, 127, 0, 0, 0, 208, 79, 17, 243, 59, 0, 0, 0, 96, 5, 0, 243, 59, 0, 0, 0, 252, 6, 0, 243, 59, 0, 0, 0, 96, 2, 0, 243, 59, 0, 0, 0, 88, 6, 64, 0, 0, 0, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 104, 83, 17, 243, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 139, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.NotNull(block);
                    prePacketBlock = block as SimplePacketBlock;
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
                    postPacketBlock = block as SimplePacketBlock;
                    Assert.NotNull(postPacketBlock);

                    Assert.Equal(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.Equal(prePacketBlock.Data, postPacketBlock.Data);
                }
            }
        }
    }
}
