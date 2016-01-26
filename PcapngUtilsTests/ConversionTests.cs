using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using PcapngUtils;
using PcapngUtils.Extensions;
using PcapngUtils.Pcap;
using PcapngUtils.PcapNG;
using Xunit;

namespace PcapngUtilsTests
{
    public class ConversionTests
    {
        [Theory]
        //[InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Gtp0.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Gtp1.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map0.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map1.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map2.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Dia0.pcap")]
        public void Pcap_To_Pcap(string path)
        {
            var testPath = path + ".tv";
            using (var reader = new PcapReader(path))
            using (var writer = new PcapWriter(testPath, reader.Header))
            {
                while (!reader.EndOfStream)
                {
                    var packet = reader.ReadPcap();
                    writer.WritePacket(packet);
                }
            }

            var orig = Md5(path);
            var test = Md5(testPath);

            var res = test.SequenceEqual(orig);
            if (res)
            {
                File.Delete(testPath);
                Assert.True(res);
            }
            else
                throw new Exception(
                    "check files manually using hex comparer or something, files may not be binary same but equal.");
        }

        [Theory]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\http.littleendian.ntar")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\http.bigendian.ntar")]
        public void PcapNg_To_PcapNg(string path)
        {
            var testPath = path + ".tv";

            bool swapped;
            ReaderFactory.GetPcapFileType(path, out swapped);

            using (var reader = new PcapNgReader(path,swapped))
            using (var writer = new PcapNgWriter(testPath, reader.HeadersWithInterfaceDescriptions.ToList()))
            {
                while (!reader.EndOfStream)
                {
                    var packet = reader.ReadPcap();
                    writer.WritePacket(packet);
                }
            }

            var orig = Md5(path);
            var test = Md5(testPath);

            var res = test.SequenceEqual(orig);
            if (res)
            {
                File.Delete(testPath);
                Assert.True(res);
            }
            else
                throw new Exception(
                    "check files manually using hex comparer or something, files may not be binary same but equal.");
        }


        [Theory]
        //[InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Gtp1.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map0.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map1.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map2.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Dia0.pcap")]
        public void Pcap_To_PcapNg_ToPcap(string path)
        {
            SectionHeader origHeader;
            var ms = new MemoryStream();
            var testPath = path + ".tv";
            using (var reader = ReaderFactory.GetReader(path))
            {
                origHeader = reader.GetPcapHeader();
                var writer = new PcapNgWriter(ms, reader.GetPcapNgHeader());
                while (!reader.EndOfStream)
                {
                    var packet = reader.Read();
                    writer.WritePacket(packet);
                }
            }

            ms.Position = 0;
            using (var reader = ReaderFactory.GetReader(ms))
            using (var writer = new PcapWriter(testPath, origHeader/*reader.GetPcapHeader()*/)) //our converted headers are always in nanoseconds.
            {
                while (!reader.EndOfStream)
                {
                    var packet = reader.Read();
                    writer.WritePacket(packet);
                }
            }

            var orig = Md5(path);
            var test = Md5(testPath);

            var res = test.SequenceEqual(orig);
            if (res)
            {
                File.Delete(testPath);
                Assert.True(res);
            }
            else
                throw new Exception(
                    "check files manually using hex comparer or something, files may not be binary same but equal."); 
        }

        private static byte[] Md5(string path)
        {
            using (var md5 = MD5.Create())
            using (var fs = new FileStream(path, FileMode.Open))
                return md5.ComputeHash(fs);
        }
    }
}
