using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.SqlServer.Server;
using PcapngUtils.Common;
using PcapngUtils.Pcap;
using Xunit;
using Xunit.Abstractions;

namespace PcapngUtilsTests
{
    public class PcapReaderTest
    {

        private ITestOutputHelper _helper;
        public PcapReaderTest(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        [Theory]
        [InlineData(20)]
        [InlineData(170)]
        [InlineData(200)]
        public void PcapReader_IncompletedFileStream_Test(int maxLength)
        {
            Assert.Throws<EndOfStreamException>(() =>
            {
                byte[] data =
                {
                    212, 195, 178, 161, 2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 0, 0, 1, 0, 0, 0,
                    89, 92, 28, 85, 58, 246, 7, 0, 124, 0, 0, 0, 124, 0, 0, 0, 68, 109, 87, 125, 40, 18, 192, 74, 0,
                    154, 76, 44, 8, 0, 69, 0, 0, 110, 100, 55, 0, 0, 117, 17, 76, 144, 37, 157, 173, 13, 192, 168, 1,
                    101, 130, 165, 130, 165, 0, 90, 107, 107, 0, 25, 137, 153, 119, 253, 219, 183, 207, 74, 89, 213,
                    110, 239, 3, 75, 110, 227, 57, 128, 86, 105, 94, 91, 40, 2, 126, 2, 227, 250, 106, 221, 113, 98,
                    211, 229, 10, 134, 44, 193, 245, 77, 75, 238, 69, 78, 16, 195, 254, 113, 224, 43, 130, 205, 115,
                    131, 90, 245, 238, 164, 68, 27, 45, 26, 73, 234, 87, 155, 38, 207, 55, 185, 252, 116, 214, 9, 21,
                    191, 90, 47, 72, 237, 89, 92, 28, 85, 238, 252, 7, 0, 124, 0, 0, 0, 124, 0, 0, 0, 192, 74, 0,
                    154, 76, 44, 68, 109, 87, 125, 40, 18, 8, 0, 69, 0, 0, 110, 86, 139
                };
                data = data.Take(maxLength).ToArray();
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (PcapReader reader = new PcapReader(stream))
                    {
                        reader.OnReadPacketEvent += (context, packet) =>
                        {
                            IPacket ipacket = packet;
                        };
                        reader.OnExceptionEvent += (sender, exc) =>
                        {
                            ExceptionDispatchInfo.Capture(exc).Throw();
                        };
                        reader.ReadPackets(new CancellationToken());
                        var a = reader.Header;
                    }

                }
            });
        }

        [Theory]
        [InlineData(20)]
        [InlineData(170)]
        [InlineData(200)]
        public void PcapReader_Sequential_Test(int datalen)
        {
            Assert.Throws<EndOfStreamException>(() =>
            {
                byte[] data =
                {
                    212, 195, 178, 161, 2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 0, 0, 1, 0, 0, 0,
                    89, 92, 28, 85, 58, 246, 7, 0, 124, 0, 0, 0, 124, 0, 0, 0, 68, 109, 87, 125, 40, 18, 192, 74, 0,
                    154, 76, 44, 8, 0, 69, 0, 0, 110, 100, 55, 0, 0, 117, 17, 76, 144, 37, 157, 173, 13, 192, 168, 1,
                    101, 130, 165, 130, 165, 0, 90, 107, 107, 0, 25, 137, 153, 119, 253, 219, 183, 207, 74, 89, 213,
                    110, 239, 3, 75, 110, 227, 57, 128, 86, 105, 94, 91, 40, 2, 126, 2, 227, 250, 106, 221, 113, 98,
                    211, 229, 10, 134, 44, 193, 245, 77, 75, 238, 69, 78, 16, 195, 254, 113, 224, 43, 130, 205, 115,
                    131, 90, 245, 238, 164, 68, 27, 45, 26, 73, 234, 87, 155, 38, 207, 55, 185, 252, 116, 214, 9, 21,
                    191, 90, 47, 72, 237, 89, 92, 28, 85, 238, 252, 7, 0, 124, 0, 0, 0, 124, 0, 0, 0, 192, 74, 0,
                    154, 76, 44, 68, 109, 87, 125, 40, 18, 8, 0, 69, 0, 0, 110, 86, 139
                };

                data = data.Take(datalen).ToArray();

                using (var stream = new MemoryStream(data))
                using (var reader = new PcapReader(stream))
                    while (!reader.EndOfStream)
                    {
                        var packet = reader.ReadPcap();
                        Assert.NotNull(packet);
                    }


            });
        }

        [Theory]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Gtp0.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Gtp1.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map0.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map1.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Dia0.pcap")]
        public void PcapReader_Validate(string path)
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

        private static byte[] Md5(string path)
        {
            using (var md5 = MD5.Create())
            using (var fs = new FileStream(path, FileMode.Open))
                return md5.ComputeHash(fs);
        }

        [Theory]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Gtp0.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Gtp1.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map0.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map1.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Dia0.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\10million.pcap")]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PcapReader_Perf(string path)
        {
            var testPath = path + ".tp";

            var watch = new Stopwatch();

            watch.Start();
            using (var reader = new PcapReader(path))
            using (var writer = new PcapWriter(testPath, reader.Header))
            {
                while (!reader.EndOfStream)
                {
                    var packet = reader.ReadPcap();
                    writer.WritePacket(packet);
                }
            }
            watch.Stop();
            _helper.WriteLine("'{0}' took '{1}' ms to rewrite.", path, watch.ElapsedMilliseconds);

            File.Delete(testPath);
        }

        [Theory]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Gtp0.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Gtp1.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map0.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Map1.pcap")]
        [InlineData(@"C:\Users\Hesenpai\Desktop\Gemik2\TestFiles\Dia0.pcap")]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PcapReader_Mem_Perf(string path)
        {
            var msSrc = new MemoryStream();
            var msDst = new MemoryStream();

            using(var fs=new FileStream(path,FileMode.Open))
                Copy(fs,msSrc);

            msSrc.Position = 0;

            var watch = new Stopwatch();

            watch.Start();
            using (var reader = new PcapReader(msSrc))
            using (var writer = new PcapWriter(msDst, reader.Header))
            {
                while (!reader.EndOfStream)
                {
                    var packet = reader.ReadPcap();
                    writer.WritePacket(packet);
                }
            }
            watch.Stop();
            _helper.WriteLine("'{0}' took '{1}' ms to rewrite.", path, watch.ElapsedMilliseconds);
        }

        private static void Copy(Stream src,Stream dst)
        {
            var buffer = new byte[32768];

            while (true)
            {
                var readCount = src.Read(buffer, 0, buffer.Length);
                if (readCount == 0)
                    return;

                dst.Write(buffer,0,readCount);
            }
        }
    }
}
