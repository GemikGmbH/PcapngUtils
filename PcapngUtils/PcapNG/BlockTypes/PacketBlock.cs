﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using PcapngUtils.Common;
using PcapngUtils.Extensions;
using PcapngUtils.PcapNG.CommonTypes;
using PcapngUtils.PcapNG.OptionTypes;

namespace PcapngUtils.PcapNG.BlockTypes
{
    [ToString]          
    public sealed class PacketBlock:AbstractBlock,INgPacket
    {
        #region IPacket
        public ulong Seconds
        {
            get { return Timestamp.Seconds; }
        }

        public ulong Microseconds
        {
            get { return Timestamp.Microseconds; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The block type
        /// </summary>
        public override BaseBlock.Types BlockType
        {
            get { return BaseBlock.Types.Packet; }
        }  
       
        /// <summary>
        /// Interface ID: iit specifies the interface this packet comes from; the correct interface will be the one whose Interface Description
        ///  Block (within the current Section of the file) is identified by the same number (see Section 3.2) of this field.
        /// </summary>
        public Int16 InterfaceId
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
        /// Drops Count: a local drop counter. It specifies the number of packets lost (by the interface and the operating system) between 
        /// this packet and the preceding one. The value xFFFF (in hexadecimal) is reserved for those systems in which this information is 
        /// not available.
        /// </summary>
        public Int16 DropCount
        {
            get;
            set;
        }

        /// <summary>
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

        /// <summary>
        /// Captured Len: number of bytes captured from the packet (i.e. the length of the Packet Data field). It will be the minimum value 
        /// among the actual Packet Length and the snapshot length (SnapLen defined in Figure 9). The value of this field does not include 
        /// the padding bytes added at the end of the Packet Data field to align the Packet Data Field to a 32-bit boundary
        /// </summary>
        public int CapturedLength
        {
            get { return _data!= null ? _data.Length : 0; }            
        }

        /// <summary>
        /// Packet Len: actual length of the packet when it was transmitted on the network. Can be different from Captured Len if the user 
        /// wants only a snapshot of the packet.
        /// </summary>
        public int PacketLength
        {
            get;
            set;
        }

        private byte[] _data;
        /// <summary>
        /// Packet Data: the data coming from the network, including link-layer headers. The format of the link-layer headers depends on the LinkType field specified in the Interface Description Block 
        /// </summary>
        [IgnoreDuringToString]
        public byte [] Data
        {
            get
            {
                return _data;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Data cannot be null");
                _data = value;
            }
        }

        private PacketOption _options;
        /// <summary>
        /// optional fields. Optional fields can be used to insert some information that may be useful when reading data, but that is not 
        /// really needed for packet processing. Therefore, each tool can either read the content of the optional fields (if any), 
        /// or skip some of them or even all at once.
        /// </summary>
        public PacketOption Options
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
        public static PacketBlock Parse(BaseBlock baseBlock, Action<Exception> actionOnException)
        {
            Contract.Requires<ArgumentNullException>(baseBlock != null, "BaseBlock cannot be null");
            //Contract.Requires<ArgumentNullException>(baseBlock.Body != null, "BaseBlock.Body cannot be null");
            Contract.Requires<ArgumentException>(baseBlock.BlockType == BaseBlock.Types.Packet, "Invalid packet type");    

            long positionInStream = baseBlock.PositionInStream;
            using (Stream stream = new MemoryStream(baseBlock.Body))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    short interfaceId = binaryReader.ReadInt16().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    short dropCount = binaryReader.ReadInt16().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    byte[] timestamp = binaryReader.ReadBytes(8);
                    if (timestamp.Length < 8)
                        throw new EndOfStreamException("Unable to read beyond the end of the stream");
                    TimestampHelper timestampHelper = new TimestampHelper(timestamp, baseBlock.ReverseByteOrder);
                    int capturedLength = binaryReader.ReadInt32().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    int packetLength = binaryReader.ReadInt32().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    byte[] data = binaryReader.ReadBytes(capturedLength);
                    if (data.Length < capturedLength)
                        throw new EndOfStreamException("Unable to read beyond the end of the stream");
                    int remainderLength = capturedLength % BaseBlock.AlignmentBoundary;
                    if (remainderLength > 0)
                    {
                        int paddingLength = BaseBlock.AlignmentBoundary - remainderLength;
                        binaryReader.ReadBytes(paddingLength);
                    }
                    PacketOption options = PacketOption.Parse(binaryReader, baseBlock.ReverseByteOrder, actionOnException);
                    PacketBlock packetBlock = new PacketBlock(interfaceId, timestampHelper, data, packetLength, options, positionInStream);
                    return packetBlock;
                }
            }
        }
  
        /// <summary>
        /// The Packet Block is marked obsolete, better use the Enhanced Packet Block instead!
        /// A Packet Block is the standard container for storing the packets coming from the network. The Packet Block is optional because 
        /// packets can be stored either by means of this block or the Simple Packet Block, which can be used to speed up dump generation.
        /// </summary>          
        public PacketBlock(short interfaceId, TimestampHelper timestamp,  byte[] data, int packetLength, PacketOption options, long positionInStream = 0)
        {
            Contract.Requires<ArgumentNullException>(timestamp != null, "Timestamp cannot be null");
            Contract.Requires<ArgumentNullException>(options != null, "Options cannot be null");
            Contract.Requires<ArgumentNullException>(data != null, "Data cannot be null");
            
            this.InterfaceId = interfaceId;
            this.Timestamp = timestamp;
            this.PacketLength = packetLength;
            this.Data = data;
            this._options = options;
            this.PositionInStream = positionInStream;
        }
        #endregion

        #region method
        protected override BaseBlock ConvertToBaseBlock(bool reverseByteOrder, Action<Exception> actionOnException)
        {
            List<byte> body = new List<byte>();
            body.AddRange(BitConverter.GetBytes(InterfaceId.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(BitConverter.GetBytes(DropCount.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(Timestamp.ConvertToByte(reverseByteOrder));
            body.AddRange(BitConverter.GetBytes(Data.Length.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(BitConverter.GetBytes(PacketLength.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(Data);
            int remainderLength = (BaseBlock.AlignmentBoundary - Data.Length % BaseBlock.AlignmentBoundary) % BaseBlock.AlignmentBoundary;
            for (int i = 0; i < remainderLength; i++)
            {
                body.Add(0);
            }
            body.AddRange(Options.ConvertToByte(reverseByteOrder, actionOnException));
            BaseBlock baseBlock = new BaseBlock(this.BlockType, body.ToArray(), reverseByteOrder, 0);
            return baseBlock;
        }

        public void Comment(string comment)
        {
            _options.Comment = comment;
        }
        #endregion
    }
}
