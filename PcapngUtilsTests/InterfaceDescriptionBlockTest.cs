using System.IO;
using PcapngUtils.PcapNG.BlockTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class InterfaceDescriptionBlockTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InterfaceDescriptionBlock_ConvertToByte_Test(bool reorder)
        {
            InterfaceDescriptionBlock prePacketBlock, postPacketBlock;
            byte[] byteblock = { 1, 0, 0, 0, 136, 0, 0, 0, 1, 0, 0, 0, 255, 255, 0, 0, 2, 0, 50, 0, 92, 68, 101, 118, 105, 99, 101, 92, 78, 80, 70, 95, 123, 68, 65, 51, 70, 56, 65, 55, 54, 45, 55, 49, 55, 69, 45, 52, 69, 65, 55, 45, 57, 69, 68, 53, 45, 48, 51, 57, 56, 68, 68, 69, 57, 67, 49, 55, 69, 125, 0, 0, 9, 0, 1, 0, 6, 0, 0, 0, 12, 0, 43, 0, 54, 52, 45, 98, 105, 116, 32, 87, 105, 110, 100, 111, 119, 115, 32, 55, 32, 83, 101, 114, 118, 105, 99, 101, 32, 80, 97, 99, 107, 32, 49, 44, 32, 98, 117, 105, 108, 100, 32, 55, 54, 48, 49, 0, 0, 0, 0, 0, 136, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.NotNull(block);
                    prePacketBlock = block as InterfaceDescriptionBlock;
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
                    postPacketBlock = block as InterfaceDescriptionBlock;
                    Assert.NotNull(postPacketBlock);

                    Assert.Equal(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.Equal(prePacketBlock.LinkType, postPacketBlock.LinkType);
                    Assert.Equal(prePacketBlock.SnapLength, postPacketBlock.SnapLength);
                    Assert.Equal(prePacketBlock.PositionInStream, postPacketBlock.PositionInStream);

                    Assert.Equal(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                    Assert.Equal(prePacketBlock.Options.Description, postPacketBlock.Options.Description);
                    Assert.Equal(prePacketBlock.Options.EuiAddress, postPacketBlock.Options.EuiAddress);
                    Assert.Equal(prePacketBlock.Options.Filter, postPacketBlock.Options.Filter);
                    Assert.Equal(prePacketBlock.Options.FrameCheckSequence, postPacketBlock.Options.FrameCheckSequence);
                    Assert.Equal(prePacketBlock.Options.IPv4Address, postPacketBlock.Options.IPv4Address);
                    Assert.Equal(prePacketBlock.Options.IPv6Address, postPacketBlock.Options.IPv6Address);
                    Assert.Equal(prePacketBlock.Options.MacAddress, postPacketBlock.Options.MacAddress);
                    Assert.Equal(prePacketBlock.Options.Name, postPacketBlock.Options.Name);
                    Assert.Equal(prePacketBlock.Options.OperatingSystem, postPacketBlock.Options.OperatingSystem);
                    Assert.Equal(prePacketBlock.Options.Speed, postPacketBlock.Options.Speed);
                    Assert.Equal(prePacketBlock.Options.TimeOffsetSeconds, postPacketBlock.Options.TimeOffsetSeconds);
                    Assert.Equal(prePacketBlock.Options.TimestampResolution, postPacketBlock.Options.TimestampResolution);
                    Assert.Equal(prePacketBlock.Options.TimeZone, postPacketBlock.Options.TimeZone);
                }
            }
        }
    }
}
