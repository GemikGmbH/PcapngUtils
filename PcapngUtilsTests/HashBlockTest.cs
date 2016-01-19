using System.Linq;
using System.Text;
using PcapngUtils.PcapNG.CommonTypes;
using Xunit;

namespace PcapngUtilsTests
{
    public class HashBlockTest
    {
        [Fact]
        public void HashBlock_InvalidEnum_Test()
        {
            string md5Hash = "f59b7efafd800e27b47a488d30615c73";
            byte[] utf8Hash = Encoding.UTF8.GetBytes(md5Hash);
            byte[] test = { 5 };
            test = test.Concat(utf8Hash).ToArray();

            HashBlock hashBlock = new HashBlock(test);
            Assert.Equal(hashBlock.Algorithm, HashBlock.HashAlgorithm.Invalid);
        }

        [Fact]
        public static void HashBlock_Md5Test_Test()
        {
            string md5Hash = "f59b7efafd800e27b47a488d30615c73";
            byte[] utf8Hash = Encoding.UTF8.GetBytes(md5Hash);
            byte[] test = { (byte)HashBlock.HashAlgorithm.Md5 };
            test = test.Concat(utf8Hash).ToArray();

            HashBlock hashBlock = new HashBlock(test);
            Assert.Equal(hashBlock.Algorithm, HashBlock.HashAlgorithm.Md5);
            Assert.Equal(hashBlock.StringValue, md5Hash);
        }
    }
}
