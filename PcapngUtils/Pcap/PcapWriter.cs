using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.ExceptionServices;
using PcapngUtils.Common;
using PcapngUtils.Extensions;

namespace PcapngUtils.Pcap
{
    public sealed class PcapWriter : Disposable, IWriter
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
        #endregion

        #region fields & properties
        private Stream _stream;
        private BinaryWriter _binaryWriter;
        private SectionHeader _header;
        private readonly object _syncRoot = new object();

        public object SyncRoot { get { return _syncRoot; } }

        public long Position { get { return _stream.Position; } }

        #endregion

        #region ctor
        public PcapWriter(string path, bool nanoseconds = false, bool reverseByteOrder = false)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(!File.Exists(path), "file exists");
            SectionHeader sh = SectionHeader.CreateEmptyHeader(nanoseconds, reverseByteOrder);
            Initialize(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, 32768), sh);
        }

        public PcapWriter(Stream stream, bool nanoseconds = false, bool reverseByteOrder = false)
        {
            Contract.Requires<ArgumentNullException>(stream!=null && stream.CanWrite, "stream cannot be null and should be writable");

            SectionHeader sh = SectionHeader.CreateEmptyHeader(nanoseconds, reverseByteOrder);
            Initialize(stream, sh);
        }

        public PcapWriter(string path, SectionHeader header)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(!File.Exists(path), "file exists");
            Contract.Requires<ArgumentNullException>(header!=null, "SectionHeader cannot be null");
            
            Initialize(new FileStream(path, FileMode.Create),header);
        }

        public PcapWriter(Stream stream, SectionHeader header)
        {
            Contract.Requires<ArgumentNullException>(stream != null && stream.CanWrite, "stream cannot be null and should be writable");
            Contract.Requires<ArgumentNullException>(header != null, "SectionHeader cannot be null");

            Initialize(stream, header);
        }

         private void Initialize(Stream stream, SectionHeader header)
         {                     
             Contract.Requires<ArgumentNullException>(stream != null, "stream cannot be null");
             Contract.Requires<Exception>(stream.CanWrite == true, "Cannot write to stream");
             Contract.Requires<ArgumentNullException>(header != null, "header cannot be null");
             this._header = header;              
             this._stream = stream;
             _binaryWriter = new BinaryWriter(stream);
             _binaryWriter.Write(header.ConvertToByte());            
         }
        #endregion

         /// <summary>
         /// Close stream, dispose members
         /// </summary>
        public void Close()
        {
            Dispose();
        }

        public void WritePacket(IPacket packet)
        {
            try
            {
                uint secs = (uint) packet.Seconds;
                uint lsecs = _header.NanoSecondResolution ? (uint) packet.Nanoseconds : (uint) packet.Microseconds;
                uint caplen = (uint) packet.Data.Length;
                uint len = (uint) packet.Data.Length;
                byte[] data = packet.Data;

                var size = data.Length + 16;
                if (size > _header.MaximumCaptureLength)
                    throw new ArgumentOutOfRangeException(
                        string.Format(
                            "[PcapWriter.WritePacket] packet length: {0} is greater than MaximumCaptureLength: {1}",
                            size, _header.MaximumCaptureLength));

                _binaryWriter.Write(secs.ReverseByteOrder(_header.ReverseByteOrder));
                _binaryWriter.Write(lsecs.ReverseByteOrder(_header.ReverseByteOrder));
                _binaryWriter.Write(caplen.ReverseByteOrder(_header.ReverseByteOrder));
                _binaryWriter.Write(len.ReverseByteOrder(_header.ReverseByteOrder));
                _binaryWriter.Write(data);
            }
            catch (Exception exc)
            {
                OnException(exc);
            }
        }

        public void WriteUnsafe(uint secs,uint lsecs,byte[] data)
        {
            try
            {
                var caplen = (uint)data.Length;
                var len = caplen;

                _binaryWriter.Write(secs);
                _binaryWriter.Write(lsecs);
                _binaryWriter.Write(caplen);
                _binaryWriter.Write(len);
                _binaryWriter.Write(data);
            }
            catch (Exception exc)
            {
                OnException(exc);
            }
        } 
        #region IDisposable Members
        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (_binaryWriter != null)
                _binaryWriter.Close();
            if (_stream != null)
                _stream.Close();
        }

        #endregion      
    }  
}
