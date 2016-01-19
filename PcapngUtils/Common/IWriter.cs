using System;

namespace PcapngUtils.Common
{
    public interface IWriter : IDisposable
    {
        object SyncRoot { get; }

        void Close();
        void WritePacket(IPacket packet);

        event CommonDelegates.ExceptionEventDelegate OnExceptionEvent;
    }
}
