using PcapngUtils.PcapNG.BlockTypes;

namespace PcapngUtils.Common
{
    public interface INgPacket:IPacket
    {
        BaseBlock.Types BlockType { get; }
        int? AssociatedInterfaceId { get; }
        long PositionInStream { get; }
        int PacketLength { get; }

        void Comment(string comment);
    }
}
