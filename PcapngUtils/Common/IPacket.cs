using System;

namespace PcapngUtils.Common
{
    public interface IPacket
    {
        UInt64 Seconds {get;}
        UInt64 Microseconds{get;}
        UInt64 Nanoseconds { get; }
        byte[] Data { get; }
    }
}
