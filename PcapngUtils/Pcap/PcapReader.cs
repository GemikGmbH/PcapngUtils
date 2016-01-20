using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using PcapngUtils.Common;
using PcapngUtils.Extensions;

namespace PcapngUtils.Pcap
{         
    public sealed class PcapReader : Disposable, IReader
    {
        #region event & delegate
        public event CommonDelegates.ExceptionEventDelegate OnExceptionEvent; 
        private void OnException(Exception exception)
        {
            Contract.Requires<ArgumentNullException>(exception != null, "exception cannot be null or empty");
            CommonDelegates.ExceptionEventDelegate handler = OnExceptionEvent;
            if (handler != null)
                handler(this, exception); 
            else
                ExceptionDispatchInfo.Capture(exception).Throw();
        }

        public event CommonDelegates.ReadPacketEventDelegate OnReadPacketEvent;
        private void OnReadPacket(IPacket packet)
        {
            Contract.Requires<ArgumentNullException>(Header != null, "Header cannot be null");
            Contract.Requires<ArgumentNullException>(packet != null, "packet cannot be null");
            CommonDelegates.ReadPacketEventDelegate handler = OnReadPacketEvent;
            if (handler != null)
                handler(Header, packet);
        }
        #endregion

        #region fields & properties
        private Stream _stream;
        private BinaryReader _binaryReader;
        public SectionHeader Header { get; private set; }
        private readonly object _syncRoot = new object();
        private long _basePosition;
        public bool EndOfStream { get { return _binaryReader.BaseStream.Position >= _binaryReader.BaseStream.Length; } }
        public object SyncRoot { get { return _syncRoot; } }
        #endregion

        #region ctor
        public PcapReader(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(File.Exists(path), "file must exists");

            Initialize(new FileStream(path, FileMode.Open));
        }

        public PcapReader(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null, "stream cannot be null");

            Initialize(s);
        }

        private void Initialize(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null, "stream cannot be null");
            Contract.Requires<Exception>(stream.CanRead == true, "cannot read stream");

            this._stream = stream;
            _binaryReader = new BinaryReader(stream);
            Header = SectionHeader.Parse(_binaryReader);
            _basePosition = _binaryReader.BaseStream.Position;
            Rewind();
        }
        #endregion

        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Read all packet from a stream. After read any packet event OnReadPacketEvent is called.
        /// Function is NOT asynchronous! (blocking thread). If you want abort it, use CancellationToken
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void ReadPackets(CancellationToken cancellationToken)
        {
            PcapPacket packet;
            while (_binaryReader.BaseStream.Position < _binaryReader.BaseStream.Length && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    lock (_syncRoot)
                        packet = ReadPcap();
                    OnReadPacket(packet);
                }
                catch(Exception exc)
                {
                    OnException(exc);
                }
            }
        }

        public IPacket Read()
        {
            return ReadPcap();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PcapPacket ReadPcap()
        {
            var position = _binaryReader.BaseStream.Position;
            var secs = _binaryReader.ReadUInt32().ReverseByteOrder(Header.ReverseByteOrder);
            var usecs = _binaryReader.ReadUInt32().ReverseByteOrder(Header.ReverseByteOrder);
            if (Header.NanoSecondResolution)
                usecs = usecs / 1000;
            var caplen = _binaryReader.ReadUInt32().ReverseByteOrder(Header.ReverseByteOrder);
            var len = _binaryReader.ReadUInt32().ReverseByteOrder(Header.ReverseByteOrder);

            var data = _binaryReader.ReadBytes((int)caplen);
            if (data.Length < caplen)
                throw new EndOfStreamException("Unable to read beyond the end of the stream");

            var packet = new PcapPacket((UInt64)secs, (UInt64)usecs, data, position);
            return packet;
        }

        public void Seek(long position)
        {
            _binaryReader.BaseStream.Position = position;
        }

        /// <summary>
        /// rewind to the beginning of the stream 
        /// </summary>
        private void Rewind()
        {
            Contract.Requires<ArgumentNullException>(Header != null, "Header cannot be null");
            lock (_syncRoot)
            {
                _binaryReader.BaseStream.Position = this._basePosition;
            }
        }

        #region IDisposable Members
        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (_binaryReader != null)
                _binaryReader.Close();
            if (_stream != null)
                _stream.Close();
        }

        #endregion

    }

       
}
