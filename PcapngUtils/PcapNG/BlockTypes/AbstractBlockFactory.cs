using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace PcapngUtils.PcapNG.BlockTypes
{          
    public static class AbstractBlockFactory
    {
        #region method
        public static AbstractBlock ReadNextBlock(BinaryReader binaryReader, bool bytesReorder, Action<Exception> actionOnException)
        {
            Contract.Requires<ArgumentNullException>(binaryReader != null, "binaryReader cannot be null");
            try
            {
                BaseBlock baseblock = new BaseBlock(binaryReader, bytesReorder);
                AbstractBlock block = null; ;
                switch (baseblock.BlockType)
                {
                    case BaseBlock.Types.SectionHeader:
                        block = SectionHeaderBlock.Parse(baseblock, actionOnException);  
                        break;
                    case BaseBlock.Types.InterfaceDescription:
                        block = InterfaceDescriptionBlock.Parse(baseblock, actionOnException);                        
                        break;
                    case BaseBlock.Types.Packet:
                        block = PacketBlock.Parse(baseblock, actionOnException);
                        break;
                    case BaseBlock.Types.SimplePacket:                             
                        block = SimplePacketBlock.Parse(baseblock, actionOnException);   
                        break;
                    case BaseBlock.Types.NameResolution:
                        block = NameResolutionBlock.Parse(baseblock, actionOnException);                         
                        break;
                    case BaseBlock.Types.InterfaceStatistics:
                        block = InterfaceStatisticsBlock.Parse(baseblock, actionOnException);
                        break;
                    case BaseBlock.Types.EnhancedPacket:
                        block = EnchantedPacketBlock.Parse(baseblock, actionOnException);
                        break;
                    default:                             
                        break;
                }
                return block;
            }
            catch(Exception exc)
            {
                actionOnException(exc);
                return null;
            }

        }
        #endregion
    }
}
