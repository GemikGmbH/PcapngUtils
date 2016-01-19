using System.IO;
using PcapngUtils.PcapNG.BlockTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class SectionHeaderBlockTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SectionHeaderBlock_ConvertToByte_Test(bool reorder)
        {
            SectionHeaderBlock prePacketBlock, postPacketBlock;
            byte[] byteblock = { 10, 13, 13, 10, 136, 0, 0, 0, 77, 60, 43, 26, 1, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 3, 0, 43, 0, 54, 52, 45, 98, 105, 116, 32, 87, 105, 110, 100, 111, 119, 115, 32, 55, 32, 83, 101, 114, 118, 105, 99, 101, 32, 80, 97, 99, 107, 32, 49, 44, 32, 98, 117, 105, 108, 100, 32, 55, 54, 48, 49, 0, 4, 0, 52, 0, 68, 117, 109, 112, 99, 97, 112, 32, 49, 46, 49, 48, 46, 55, 32, 40, 118, 49, 46, 49, 48, 46, 55, 45, 48, 45, 103, 54, 98, 57, 51, 49, 97, 49, 32, 102, 114, 111, 109, 32, 109, 97, 115, 116, 101, 114, 45, 49, 46, 49, 48, 41, 0, 0, 0, 0, 136, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.NotNull(block);
                    prePacketBlock = block as SectionHeaderBlock;
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
                    postPacketBlock = block as SectionHeaderBlock;
                    Assert.NotNull(postPacketBlock);

                    Assert.Equal(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.Equal(prePacketBlock.MagicNumber, postPacketBlock.MagicNumber);
                    Assert.Equal(prePacketBlock.MajorVersion, postPacketBlock.MajorVersion);
                    Assert.Equal(prePacketBlock.MinorVersion, postPacketBlock.MinorVersion);
                    Assert.Equal(prePacketBlock.SectionLength, postPacketBlock.SectionLength);
                    Assert.Equal(prePacketBlock.PositionInStream, postPacketBlock.PositionInStream);
                    Assert.Equal(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                    Assert.Equal(prePacketBlock.Options.Hardware, postPacketBlock.Options.Hardware);
                    Assert.Equal(prePacketBlock.Options.OperatingSystem, postPacketBlock.Options.OperatingSystem);
                    Assert.Equal(prePacketBlock.Options.UserApplication, postPacketBlock.Options.UserApplication);
                }
            }
        }
    }
}
