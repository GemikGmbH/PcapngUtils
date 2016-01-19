using System;
using System.Diagnostics.Contracts;
using System.IO;
using PcapngUtils.Common;
using PcapngUtils.Pcap;
using PcapngUtils.PcapNG;
using PcapngUtils.PcapNG.BlockTypes;

namespace PcapngUtils
{
    public enum PcapFileType
    {
        PcapMicroPrecision,
        PcapNanoPrecision,
        PcapNg
    }

    public sealed class ReaderFactory
    {
        #region fields && properties

        #endregion

        #region methods

        
        public static IReader GetReader(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(File.Exists(path), "file must exists");
            
            var mask = FindMask(path);

            bool swapped;
            var mode = FindPcapFileType(mask, out swapped);

            switch (mode)
            {
                case PcapFileType.PcapMicroPrecision:
                case PcapFileType.PcapNanoPrecision:
                {
                    var reader = new PcapReader(path);
                    return reader;
                }
                case PcapFileType.PcapNg:
                {
                    var reader = new PcapNgReader(path, swapped);
                    return reader;
                }
                default:
                    throw new ArgumentException(
                        string.Format("[IReaderFactory.GetReader] file {0} is not PCAP/PCAPNG file", path));
            }
        }

        public static IReader GetReader(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null && stream.Length > 0,
                "data stream cannot be null or empty");
            var defaultPosition = stream.Position;

            var mask = FindMask(stream);
            stream.Position = defaultPosition;

            bool swapped;
            var mode = FindPcapFileType(mask, out swapped);

            switch (mode)
            {
                case PcapFileType.PcapMicroPrecision:
                case PcapFileType.PcapNanoPrecision:
                {
                    var reader = new PcapReader(stream);
                    return reader;
                }
                case PcapFileType.PcapNg:
                {
                    var reader = new PcapNgReader(stream, swapped);
                    return reader;
                }
                default:
                    throw new ArgumentException(
                        string.Format("[IReaderFactory.GetReader] stream is not PCAP/PCAPNG file"));
            }
        }

        public static PcapFileType GetPcapFileType(Stream stream, out bool swapped)
        {
            Contract.Requires<ArgumentNullException>(stream != null && stream.Length > 0,
                "data stream cannot be null or empty");
            var defaultPosition = stream.Position;

            var mask = FindMask(stream);
            stream.Position = defaultPosition;

            return FindPcapFileType(mask, out swapped);
        }

        public static PcapFileType GetPcapFileType(string path, out bool swapped)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(File.Exists(path), "file must exists");

            var mask = FindMask(path);
            return FindPcapFileType(mask,out swapped);
        }

        private static uint FindMask(Stream stream)
        {
            var binaryReader = new BinaryReader(stream);

            if (binaryReader.BaseStream.Length < 12)
                throw new ArgumentException(string.Format("[IReaderFactory.GetReader] stream is too short "));

            var mask = binaryReader.ReadUInt32();
            if (mask == (uint)BaseBlock.Types.SectionHeader)
            {
                binaryReader.ReadUInt32();
                mask = binaryReader.ReadUInt32();
            }

            return mask;
        }

        private static uint FindMask(string path)
        {
            var mask = 0U;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                mask = FindMask(stream);
            }
            return mask;
        }

        private static PcapFileType FindPcapFileType(uint mask, out bool swapped)
        {
            switch (mask)
            {
                case (uint)SectionHeader.MagicNumbers.MicrosecondSwapped:
                    swapped = true;
                    return PcapFileType.PcapMicroPrecision;
                case (uint)SectionHeader.MagicNumbers.NanosecondSwapped:
                    swapped = true;
                    return PcapFileType.PcapNanoPrecision;
                case (uint)SectionHeader.MagicNumbers.MicrosecondIdentical:
                    swapped = false;
                    return PcapFileType.PcapMicroPrecision;
                case (uint)SectionHeader.MagicNumbers.NanosecondIdentical:
                    swapped = false;
                    return PcapFileType.PcapNanoPrecision;
                case (uint)SectionHeaderBlock.MagicNumbers.Identical:
                    swapped = false;
                    return PcapFileType.PcapNg;
                case (uint)SectionHeaderBlock.MagicNumbers.Swapped:
                    swapped = true;
                    return PcapFileType.PcapNg;
                default:
                    throw new ArgumentException(string.Format("[IReaderFactory.GetReader] file is not PCAP/PCAPNG file"));
            }
        }
        #endregion
    }
}
