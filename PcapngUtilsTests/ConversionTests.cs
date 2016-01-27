using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using PcapngUtils;
using PcapngUtils.Common;
using PcapngUtils.Extensions;
using PcapngUtils.Pcap;
using PcapngUtils.PcapNG;
using PcapngUtils.PcapNG.BlockTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class ConversionTests
    {
        public static IEnumerable<string[][]> MultipleInterfaceData 
        {
            get
            {
                return new List<string[][]>
                {
                    new[]
                    {
                        new[]
                        {
                            @"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\small0.pcap",
                            @"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\small1.pcap",
                            @"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\small2.pcap",
                            @"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\small3.pcap"
                        }
                    }
                };
            }
        }
        [Theory]
        [MemberData("MultipleInterfaceData")]
        public void Pcap_To_PcapNg_MultipleInterfaces(params string[] paths)
        {
            var headers = new List<HeaderWithInterfacesDescriptions>();
            var packets = new List<IPacket>();

            for (var i=0;i<paths.Length;i++)
            {
                using (var reader = ReaderFactory.GetReader(paths[i]))
                {
                    var header = reader.GetPcapNgHeader();
                    //header[0].Header.AssociatedInterfaceId = i;
                    //header[0].InterfaceDescriptions[0].AssociatedInterfaceId = i;

                    headers.Add(header[0]);
                    while (!reader.EndOfStream)
                    {
                        var origPacket = reader.Read();
                        var newPacket = EnchantedPacketBlock.CreateEnchantedPacketFromIPacket(origPacket,e=>
                        {
                            throw e;
                        });

                        newPacket.InterfaceID = i;
                        packets.Add(newPacket);
                    }
                }
            }

            var finalHeader = headers.MergeNgHeaders();

            var testPath = paths.First() + ".tv";
            using (var writer = new PcapNgWriter(testPath, finalHeader))
            {
                foreach(var p in packets)
                    writer.WritePacket(p);
            }

            throw new Exception("unfortunately for now we should check the file manually.");
        }

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
