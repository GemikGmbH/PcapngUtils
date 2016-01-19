using System.Diagnostics.Contracts;
using System.IO;
using PcapngUtils.PcapNG.OptionTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class SectionHeaderOptionTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [ContractVerification(false)]
        public static void SectionHeaderOption_ConvertToByte_Test(bool reorder)
        {
            SectionHeaderOption preOption = new SectionHeaderOption();
            SectionHeaderOption postOption;
            preOption.Comment = "Test Comment";
            preOption.Hardware = "x86 Personal Computer";
            preOption.OperatingSystem = "Windows 7";
            preOption.UserApplication = "PcapngUtils";
            byte[] preOptionByte = preOption.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(preOptionByte))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postOption = SectionHeaderOption.Parse(binaryReader, reorder, null);
                }
            }

            Assert.NotNull(postOption);
            Assert.Equal(preOption.Comment, postOption.Comment);
            Assert.Equal(preOption.Hardware, postOption.Hardware);
            Assert.Equal(preOption.OperatingSystem, postOption.OperatingSystem);
            Assert.Equal(preOption.UserApplication, postOption.UserApplication);
        }
    }
}
