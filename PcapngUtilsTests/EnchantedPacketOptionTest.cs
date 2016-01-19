using System.Diagnostics.Contracts;
using System.IO;
using PcapngUtils.PcapNG.CommonTypes;
using PcapngUtils.PcapNG.OptionTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class EnchantedPacketOptionTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [ContractVerification(false)]
        public static void EnchantedPacketOption_ConvertToByte_Test(bool reorder)
        {
            EnchantedPacketOption preOption = new EnchantedPacketOption();
            EnchantedPacketOption postOption;
            preOption.Comment = "Test Comment";
            preOption.DropCount = 25;
            byte[] md5Hash = { 3, 87, 248, 225, 163, 56, 121, 102, 219, 226, 164, 68, 165, 51, 9, 177, 59 };
            preOption.Hash = new HashBlock(md5Hash);
            preOption.PacketFlag = new PacketBlockFlags(0xFF000000);
            byte[] preOptionByte = preOption.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(preOptionByte))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postOption = EnchantedPacketOption.Parse(binaryReader, reorder, null);
                }
            }

            Assert.NotNull(postOption);
            Assert.Equal(preOption.Comment, postOption.Comment);
            Assert.Equal(preOption.DropCount, postOption.DropCount);
            Assert.Equal(preOption.Hash.Algorithm, postOption.Hash.Algorithm);
            Assert.Equal(preOption.Hash.Value, postOption.Hash.Value);
            Assert.Equal(preOption.PacketFlag.Flag, postOption.PacketFlag.Flag);
        }
    }
}
