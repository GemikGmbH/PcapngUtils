using System.IO;
using PcapngUtils.Pcap;
using Xunit;

namespace PcapngUtilsTests
{
    public class SectionHeaderTest
    {
        [Fact]
        public void SectionHeader_ConvertToByte_Test()
        {
            SectionHeader pre = SectionHeader.CreateEmptyHeader(false, false);
            using (MemoryStream stream = new MemoryStream(pre.ConvertToByte()))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    SectionHeader post = SectionHeader.Parse(br);
                    Assert.Equal(pre.MagicNumber, post.MagicNumber);
                    Assert.Equal(pre.ReverseByteOrder, post.ReverseByteOrder);
                    Assert.Equal(pre.MajorVersion, post.MajorVersion);
                    Assert.Equal(pre.MinorVersion, post.MinorVersion);
                    Assert.Equal(pre.LinkType, post.LinkType);
                    Assert.Equal(pre.MaximumCaptureLength, post.MaximumCaptureLength);
                    Assert.Equal(pre.NanoSecondResolution, post.NanoSecondResolution);
                    Assert.Equal(pre.SignificantFigures, post.SignificantFigures);
                    Assert.Equal(pre.TimezoneOffset, post.TimezoneOffset);
                }
            }

        }
    }
}
