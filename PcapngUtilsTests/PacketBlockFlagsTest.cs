using PcapngUtils.PcapNG.CommonTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class PacketBlockFlagsTest
    {
        [Fact]
        public void PacketBlockFlags_Inbound_Test()
        {
            PacketBlockFlags packetFlagInbound = new PacketBlockFlags(511);
            Assert.True(packetFlagInbound.Inbound);

            PacketBlockFlags packetFlagNoInbound = new PacketBlockFlags(512);
            Assert.False(packetFlagNoInbound.Inbound);
        }

        [Fact]
        public void PacketBlockFlags_Outbound_Test()
        {
            PacketBlockFlags packetFlagOutbound = new PacketBlockFlags(254);
            Assert.True(packetFlagOutbound.Outbound);

            PacketBlockFlags packetFlagNoOutbound = new PacketBlockFlags(253);
            Assert.False(packetFlagNoOutbound.Outbound);
        }

        [Fact]
        public void PacketBlockFlags_Unicast_Test()
        {
            PacketBlockFlags packetFlagUnicast = new PacketBlockFlags(255);
            Assert.True(packetFlagUnicast.Unicast);

            PacketBlockFlags packetFlagNoUnicast = new PacketBlockFlags(128);
            Assert.False(packetFlagNoUnicast.Unicast);
        }

        [Fact]
        public void PacketBlockFlags_Multicast_Test()
        {
            PacketBlockFlags packetFlagMulticast = new PacketBlockFlags(255);
            Assert.True(packetFlagMulticast.Multicast);

            PacketBlockFlags packetFlagNoMulticast = new PacketBlockFlags(128);
            Assert.False(packetFlagNoMulticast.Multicast);
        }

        [Fact]
        public void PacketBlockFlags_Broadcast_Test()
        {
            PacketBlockFlags packetFlagBroadcast = new PacketBlockFlags(255);
            Assert.True(packetFlagBroadcast.Broadcast);

            PacketBlockFlags packetFlagNoBroadcast = new PacketBlockFlags(128);
            Assert.False(packetFlagNoBroadcast.Broadcast);
        }

        [Fact]
        public void PacketBlockFlags_Promisious_Test()
        {
            PacketBlockFlags packetFlagPromisious = new PacketBlockFlags(255);
            Assert.True(packetFlagPromisious.Promiscuous);

            PacketBlockFlags packetFlagNoPromisious = new PacketBlockFlags(128);
            Assert.False(packetFlagNoPromisious.Promiscuous);
        }

        [Fact]
        public void PacketBlockFlags_FCSLength_Test()
        {
            PacketBlockFlags packetFlagFcsLength = new PacketBlockFlags(480);
            Assert.True(packetFlagFcsLength.FcsLength);

            PacketBlockFlags packetFlagNoFcsLength = new PacketBlockFlags(128);
            Assert.False(packetFlagNoFcsLength.FcsLength);
        }

        [Fact]
        public void PacketBlockFlags_CrcError_Test()
        {
            PacketBlockFlags packetCrcError = new PacketBlockFlags(0xFF000000);
            Assert.True(packetCrcError.CrcError);

            PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
            Assert.False(packetFlagNoError.CrcError);
        }

        [Fact]
        public void PacketBlockFlags_PacketTooLongError_Test()
        {
            PacketBlockFlags packetTooLongError = new PacketBlockFlags(0xFF000000);
            Assert.True(packetTooLongError.PacketTooLongError);

            PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
            Assert.False(packetFlagNoError.PacketTooLongError);
        }

        [Fact]
        public void PacketBlockFlags_PacketTooShortError_Test()
        {
            PacketBlockFlags packetTooShortError = new PacketBlockFlags(0xFF000000);
            Assert.True(packetTooShortError.PacketTooShortError);

            PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
            Assert.False(packetFlagNoError.PacketTooShortError);
        }

        [Fact]
        public void PacketBlockFlags_WrongInterFrameGapError_Test()
        {
            PacketBlockFlags packetWrongInterFrameGapError = new PacketBlockFlags(0xFF000000);
            Assert.True(packetWrongInterFrameGapError.WrongInterFrameGapError);

            PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
            Assert.False(packetFlagNoError.WrongInterFrameGapError);
        }

        [Fact]
        public void PacketBlockFlags_UnalignedFrameError_Test()
        {
            PacketBlockFlags packetUnalignedFrameError = new PacketBlockFlags(0xFF000000);
            Assert.True(packetUnalignedFrameError.UnalignedFrameError);

            PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
            Assert.False(packetFlagNoError.UnalignedFrameError);
        }

        [Fact]
        public void PacketBlockFlags_tartFrameDelimiterError_Test()
        {
            PacketBlockFlags packetStartFrameDelimiterError = new PacketBlockFlags(0xFF000000);
            Assert.True(packetStartFrameDelimiterError.StartFrameDelimiterError);

            PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
            Assert.False(packetFlagNoError.StartFrameDelimiterError);
        }

        [Fact]
        public void PacketBlockFlags_PreambleError_Test()
        {
            PacketBlockFlags packetPreambleError = new PacketBlockFlags(0xFF000000);
            Assert.True(packetPreambleError.PreambleError);

            PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
            Assert.False(packetFlagNoError.PreambleError);
        }

        [Fact]
        public void PacketBlockFlags_SymbolError_Test()
        {
            PacketBlockFlags packetSymbolError = new PacketBlockFlags(0xFF000000);
            Assert.True(packetSymbolError.SymbolError);

            PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
            Assert.False(packetFlagNoError.SymbolError);
        }
    }
}
