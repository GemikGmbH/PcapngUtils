using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using PcapngUtils.Extensions;
using PcapngUtils.PcapNG.CommonTypes;
using PcapngUtils.PcapNG.OptionTypes;

namespace PcapngUtils.PcapNG.BlockTypes
{
    [ToString]      
    public sealed class InterfaceStatisticsBlock:AbstractBlock
    {
        #region Properties
        /// <summary>
        /// The block type
        /// </summary>
        public override BaseBlock.Types BlockType
        {
            get { return BaseBlock.Types.InterfaceStatistics; }
        }
        
        /// <summary>
        /// Interface ID: it specifies the interface these statistics refers to; the correct interface will be the one whose Interface 
        /// Description Block (within the current Section of the file) is identified by same number (see Section 3.2) of this field. 
        /// Please note: in former versions of this document, this field was 16 bits only. As this differs from its usage in other places 
        /// of this doc and as this block was not used "in the wild" before (as to the knowledge of the authors), it seems reasonable to 
        /// change it to 32 bits!
        /// </summary>
        public int InterfaceId
        {
            get;
            set;
        }

        /// <summary>
        /// contains information about relations between packet and interface  on which it was captured 
        /// </summary>
        public override int? AssociatedInterfaceId
        {
            get { return InterfaceId; }
        }

        /// <summary>
        /// Timestamp: time this statistics refers to. The format of the timestamp is the same already defined in the Enhanced Packet 
        /// Timestamp (High) and Timestamp (Low): high and low 32-bits of a 64-bit quantity representing the timestamp. The timestamp is a 
        /// single 64-bit unsigned integer representing the number of units since 1/1/1970. The way to interpret this field is specified by the 
        /// 'if_tsresol' option (see Figure 9) of the Interface Description block referenced by this packet. Please note that differently 
        /// from the libpcap file format, timestamps are not saved as two 32-bit values accounting for the seconds and microseconds since 
        /// 1/1/1970. They are saved as a single 64-bit quantity saved as two 32-bit words.
        /// </summary>
        public TimestampHelper Timestamp
        {
            get;
            set;
        }

        private InterfaceStatisticsOption _options;
        /// <summary>
        /// optional fields. Optional fields can be used to insert some information that may be useful when reading data, but that is not 
        /// really needed for packet processing. Therefore, each tool can either read the content of the optional fields (if any), 
        /// or skip some of them or even all at once.
        /// </summary>
        public InterfaceStatisticsOption Options
        {
            get
            {
                return _options;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Options cannot be null");
                _options = value;
            }
        }
        #endregion

        #region ctor
        public static InterfaceStatisticsBlock Parse(BaseBlock baseBlock, Action<Exception> actionOnException)
        {
            Contract.Requires<ArgumentNullException>(baseBlock != null, "BaseBlock cannot be null");
            //Contract.Requires<ArgumentNullException>(baseBlock.Body != null, "BaseBlock.Body cannot be null");
            Contract.Requires<ArgumentException>(baseBlock.BlockType == BaseBlock.Types.InterfaceStatistics, "Invalid packet type");    

            long positionInStream = baseBlock.PositionInStream;
            using (Stream stream = new MemoryStream(baseBlock.Body))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    int interfaceId = binaryReader.ReadInt32().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    byte[] timestamp = binaryReader.ReadBytes(8);
                    if (timestamp.Length < 8)
                        throw new EndOfStreamException("Unable to read beyond the end of the stream");
                    TimestampHelper timestampHelper = new TimestampHelper(timestamp, baseBlock.ReverseByteOrder);
                    InterfaceStatisticsOption options = InterfaceStatisticsOption.Parse(binaryReader, baseBlock.ReverseByteOrder, actionOnException);
                    InterfaceStatisticsBlock statisticBlock = new InterfaceStatisticsBlock(interfaceId, timestampHelper, options, positionInStream);
                    return statisticBlock;
                }
            }
        }

        /// <summary>
        /// The Interface Statistics Block contains the capture statistics for a given interface and it is optional. The statistics are referred 
        /// to the interface defined in the current Section identified by the Interface ID field. An Interface Statistics Block is normally 
        /// placed at the end of the file, but no assumptions can be taken about its position - it can even appear multiple times for the same 
        /// interface.
        /// </summary>        
        public InterfaceStatisticsBlock(int interfaceId, TimestampHelper timestamp, InterfaceStatisticsOption options, long positionInStream = 0)
        {
            Contract.Requires<ArgumentNullException>(timestamp != null, "Timestamp cannot be null");
            Contract.Requires<ArgumentNullException>(options != null, "Options cannot be null");
           
            this.InterfaceId = interfaceId;
            this.Timestamp = timestamp;              
            this._options = options;
            this.PositionInStream = positionInStream;
        }
        #endregion;

        #region method
        protected override BaseBlock ConvertToBaseBlock(bool reverseByteOrder, Action<Exception> actionOnException)
        {
            List<byte> body = new List<byte>();
            body.AddRange(BitConverter.GetBytes(InterfaceId.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(Timestamp.ConvertToByte(reverseByteOrder));            
            body.AddRange(Options.ConvertToByte(reverseByteOrder, actionOnException));
            BaseBlock baseBlock = new BaseBlock(BlockType, body.ToArray(), reverseByteOrder, 0);
            return baseBlock;
        }
        #endregion
    }
}
