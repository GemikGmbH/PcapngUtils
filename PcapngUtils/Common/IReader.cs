using System;
using System.Threading;

namespace PcapngUtils.Common
{
    public interface IReader : IDisposable 
    {
        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        void Close(); 
        event CommonDelegates.ExceptionEventDelegate OnExceptionEvent;
        event CommonDelegates.ReadPacketEventDelegate OnReadPacketEvent;

        /// <summary>
        /// Read all packet from a stream. After read any packet event OnReadPacketEvent is called.
        /// Function is NOT asynchronous! (blocking thread). If you want abort it, use CancellationToken
        /// </summary>
        /// <param name="cancellationToken"></param>
        void ReadPackets(CancellationToken cancellationToken);

        /// <summary>
        /// Reads next packet in the stream.
        /// </summary>
        /// <returns>Returns last fetched packet</returns>
        IPacket Read();
        void Seek(long position);

        bool EndOfStream { get; }
        object SyncRoot { get; }
    }
}
