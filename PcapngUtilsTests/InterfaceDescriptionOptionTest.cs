using System.Diagnostics.Contracts;
using System.IO;
using System.Net.NetworkInformation;
using PcapngUtils.PcapNG.OptionTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class IPAddress_v6_Test
    {
        [Fact]
        public void InterfaceDescriptionOption_IPAddress_v6_Test()
        {
            byte[] preTab = new byte[] { 0x20, 0x01, 0x0d, 0xb8, 0x85, 0xa3, 0x08, 0xd3, 0x13, 0x19, 0x8a, 0x2e, 0x03, 0x70, 0x73, 0x44, 0x40 };
            InterfaceDescriptionOption.IpAddressV6 address = new InterfaceDescriptionOption.IpAddressV6(preTab);
            Assert.NotNull(address);
            Assert.Equal(address.Address, "2001:0db8:85a3:08d3:1319:8a2e:0370:7344");
            Assert.Equal(address.PrefixLength, 64);
            Assert.Equal(address.ToString(), "2001:0db8:85a3:08d3:1319:8a2e:0370:7344/64");
            byte[] postTab = address.ConvertToByte();
            Assert.Equal(preTab, postTab);
        }
    }

    public class IpAddressV4Test
    {
        [Fact]
        public void InterfaceDescriptionOption_IPAddress_v4_Test()
        {
            byte[] preTab = new byte[] { 192, 168, 0, 1, 255, 255, 255, 0 };
            InterfaceDescriptionOption.IpAddressV4 address = new InterfaceDescriptionOption.IpAddressV4(preTab);
            Assert.NotNull(address);
            Assert.Equal(address.Address, "192.168.0.1");
            Assert.Equal(address.Mask, "255.255.255.0");
            Assert.Equal(address.ToString(), "192.168.0.1 255.255.255.0");
            byte[] postTab = address.ConvertToByte();
            Assert.Equal(preTab, postTab);
        }
    }

    public class InterfaceDescriptionOptionTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [ContractVerification(false)]
        public void InterfaceDescriptionOption_ConvertToByte_Test(bool reorder)
        {
            InterfaceDescriptionOption preOption = new InterfaceDescriptionOption();
            InterfaceDescriptionOption postOption;
            preOption.Comment = "Test Comment";
            preOption.Description = "Test Description";
            preOption.EuiAddress = new byte[] { 0x00, 0x0A, 0xE6, 0xFF, 0xFE, 0x3E, 0xFD, 0xE1 };
            preOption.Filter = new byte[] { 5, 6, 7, 8 };
            preOption.FrameCheckSequence = 255;
            preOption.IPv4Address = new InterfaceDescriptionOption.IpAddressV4(new byte[] { 127, 0, 0, 1, 255, 255, 255, 0 });
            preOption.IPv6Address = new InterfaceDescriptionOption.IpAddressV6(new byte[] { 0x20, 0x01, 0x0d, 0x0db, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x14, 0x28, 0x57, 0xab, 0x40 });
            preOption.MacAddress = new PhysicalAddress(new byte[] { 0x00, 0x0A, 0xE6, 0x3E, 0xFD, 0xE1 });
            preOption.Name = "Test Name";
            preOption.OperatingSystem = "Test OperatingSystem";
            preOption.Speed = 12345678;
            preOption.TimeOffsetSeconds = 1234;
            preOption.TimestampResolution = 6;
            preOption.TimeZone = 1;


            byte[] preOptionByte = preOption.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(preOptionByte))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postOption = InterfaceDescriptionOption.Parse(binaryReader, reorder, null);
                }
            }

            Assert.NotNull(postOption);
            Assert.Equal(preOption.Comment, postOption.Comment);
            Assert.Equal(preOption.Description, postOption.Description);
            Assert.Equal(preOption.EuiAddress, postOption.EuiAddress);
            Assert.Equal(preOption.Filter, postOption.Filter);
            Assert.Equal(preOption.FrameCheckSequence, postOption.FrameCheckSequence);
            Assert.Equal(preOption.IPv4Address, postOption.IPv4Address);
            Assert.Equal(preOption.IPv6Address, postOption.IPv6Address);
            Assert.Equal(preOption.MacAddress, postOption.MacAddress);
            Assert.Equal(preOption.Name, postOption.Name);
            Assert.Equal(preOption.OperatingSystem, postOption.OperatingSystem);
            Assert.Equal(preOption.Speed, postOption.Speed);
            Assert.Equal(preOption.TimeOffsetSeconds, postOption.TimeOffsetSeconds);
            Assert.Equal(preOption.TimestampResolution, postOption.TimestampResolution);
            Assert.Equal(preOption.TimeZone, postOption.TimeZone);

        }
    }
}
