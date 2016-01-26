using System;
using System.Collections.Generic;
using System.Linq;
using PcapngUtils.Common;
using PcapngUtils.Pcap;
using PcapngUtils.PcapNG;
using PcapngUtils.PcapNG.BlockTypes;
using PcapngUtils.PcapNG.OptionTypes;

namespace PcapngUtils.Extensions
{
    public static class HeaderExtensions
    {
        public static SectionHeader GetPcapHeader(this IReader reader)
        {
            var pcapReader = reader as PcapReader;
            if (pcapReader != null)
                return pcapReader.Header;

            var pcapNgReader = (PcapNgReader)reader;
            
            var header = pcapNgReader.HeadersWithInterfaceDescriptions;
            if(header.Count!=1)
                throw new NotSupportedException("pcap header conversion can only work with 1 header");

            if(header[0].InterfaceDescriptions.Count!=1)
                throw new NotSupportedException("pcap header conversion can only work with 1 interface");

            var interfaceDesc = header[0].InterfaceDescriptions[0];

            var sectionHeader =
                new SectionHeader(
                    magicNumber:
                        header[0].Header.MagicNumber == SectionHeaderBlock.MagicNumbers.Identical
                            ? SectionHeader.MagicNumbers.NanosecondIdentical
                            : SectionHeader.MagicNumbers.NanosecondSwapped, linkType: interfaceDesc.LinkType,
                    snaplen: (uint) interfaceDesc.SnapLength);

            return sectionHeader;
        }

        public static List<HeaderWithInterfacesDescriptions> GetPcapNgHeader(this IReader reader)
        {
            var pcapNgReader = reader as PcapNgReader;
            if (pcapNgReader != null)
                return pcapNgReader.HeadersWithInterfaceDescriptions.ToList();

            var pcapReader = (PcapReader) reader;

            var headerOptions = pcapReader.Header;
            var header = SectionHeaderBlock.GetEmptyHeader(false/*headerOptions.ReverseByteOrder*/);
            var interfaceDesc = new InterfaceDescriptionBlock(headerOptions.LinkType,
                (int) headerOptions.MaximumCaptureLength, new InterfaceDescriptionOption());

            return new List<HeaderWithInterfacesDescriptions>
            {
                new HeaderWithInterfacesDescriptions(header, new List<InterfaceDescriptionBlock> {interfaceDesc})
            };
        }
    }
}
