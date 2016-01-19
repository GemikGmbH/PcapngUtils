using System;

namespace PcapngUtils.Common
{
    public interface IWriter : IDisposable
    {
        void Close();
        void WritePacket(IPacket packet);

        event CommonDelegates.ExceptionEventDelegate OnExceptionEvent;
    }
}
