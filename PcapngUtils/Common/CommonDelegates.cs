using System;

namespace PcapngUtils.Common
{
    public class CommonDelegates
    {
        public delegate void ExceptionEventDelegate(object sender, Exception exception);

        public delegate void ReadPacketEventDelegate(object context, IPacket packet);
    }
}
