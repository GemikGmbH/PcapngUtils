using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using PcapngUtils.Common;
using PcapngUtils.Extensions;

namespace PcapngUtils.PcapNG.BlockTypes
{
    [ToString]    
    public sealed class SimplePacketBlock : AbstractBlock, INgPacket
    {   
        #region IPacket
        public ulong Seconds
        {
            get { return 0; }
        }

        public ulong Microseconds
        {
            get { return 0; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The block type
        /// </summary>
        public override BaseBlock.Types BlockType
        {
            get { return BaseBlock.Types.SimplePacket; }
        }

        /// <summary>
        /// contains information about relations between packet and interface  on which it was captured 
        /// </summary>
        public override int? AssociatedInterfaceId
        {
            get { return 0; }
        }

        /// <summary>
        /// Packet Len: actual length of the packet when it was transmitted on the network. Can be different from captured len if the packet  
        /// has been truncated by the capture process.
        /// </summary>
        public int PacketLength
        {
            get { return _data != null ? _data.Length : 0; }
        }

        private byte[] _data;
        /// <summary>
        /// Packet Data: the data coming from the network, including link-layers headers. The length of this field can be derived from the field  
        /// Block Total Length, present in the Block Header, and it is the minimum value among the SnapLen (present in the Interface Description
        ///  Block) and the Packet Len (present in this header).
        /// </summary>
        [IgnoreDuringToString]
        public byte[] Data
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
        
        #endregion

        #region ctor
        public static SimplePacketBlock Parse(BaseBlock baseBlock, Action<Exception> actionOnException)
        {
            Contract.Requires<ArgumentNullException>(baseBlock != null, "BaseBlock cannot be null");
            //Contract.Requires<ArgumentNullException>(baseBlock.Body != null, "BaseBlock.Body cannot be null");
            Contract.Requires<ArgumentException>(baseBlock.BlockType == BaseBlock.Types.SimplePacket, "Invalid packet type");

            long positionInStream = baseBlock.PositionInStream;
            using (Stream stream = new MemoryStream(baseBlock.Body))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    int packetLength = binaryReader.ReadInt32().ReverseByteOrder(baseBlock.ReverseByteOrder);

                    byte [] data = binaryReader.ReadBytes(packetLength);
                    if (data.Length < packetLength)
                        throw new EndOfStreamException("Unable to read beyond the end of the stream");
                    int remainderLength = packetLength % BaseBlock.AlignmentBoundary;
                    if (remainderLength > 0)
                    {
                        int paddingLength = BaseBlock.AlignmentBoundary - remainderLength;
                        binaryReader.ReadBytes(paddingLength);
                    }
                    SimplePacketBlock simplePacketBlock = new SimplePacketBlock(data, positionInStream);
                    return simplePacketBlock;
                }
            }
        }

        /// <summary>
        /// The Simple Packet Block is a lightweight container for storing the packets coming from the network. Its presence is optional.
        /// A Simple Packet Block is similar to a Packet Block (see Section 3.5), but it is smaller, simpler to process and contains only a 
        /// minimal set of information. This block is preferred to the standard Packet Block when performance or space occupation are 
        /// critical factors, such as in sustained traffic dump applications. A capture file can contain both Packet Blocks and Simple Packet 
        /// Blocks: for example, a capture tool could switch from Packet Blocks to Simple Packet Blocks when the hardware resources become 
        /// critical. The Simple Packet Block does not contain the Interface ID field. Therefore, it must be assumed that all the Simple Packet 
        /// Blocks have been captured on the interface previously specified in the first Interface Description Block.
        /// </summary>       
        public SimplePacketBlock( byte[] data, long positionInStream = 0)
        {
            Contract.Requires<ArgumentNullException>(data != null, "Data cannot be null");     
            this.Data = data;                    
            this.PositionInStream = positionInStream;
        }
        #endregion

        #region method

        public void Comment(string comment)
        {
            throw new NotImplementedException("comment is not supported for simple packets.");
        }
        protected override BaseBlock ConvertToBaseBlock(bool reverseByteOrder, Action<Exception> actionOnException)
        {
            List<byte> body = new List<byte>();
            body.AddRange(BitConverter.GetBytes(Data.Length.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(Data);
            int remainderLength = (BaseBlock.AlignmentBoundary - Data.Length % BaseBlock.AlignmentBoundary) % BaseBlock.AlignmentBoundary;
            for (int i = 0; i < remainderLength; i++)
            {
                body.Add(0);
            }            
            BaseBlock baseBlock = new BaseBlock(BlockType, body.ToArray(), reverseByteOrder, 0);
            return baseBlock;
        }
        #endregion
    }
}
