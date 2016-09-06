using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using PcapngUtils.Common;
using PcapngUtils.PcapNG.BlockTypes;

namespace PcapngUtils.PcapNG
{
    public sealed class PcapNgWriter : Disposable, IWriter
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

        private List<HeaderWithInterfacesDescriptions> _headersWithInterface = new List<HeaderWithInterfacesDescriptions>();
        public IList<HeaderWithInterfacesDescriptions> HeadersWithInterfaces
        {
            get { return _headersWithInterface.AsReadOnly(); }
        }

        private readonly object _syncRoot = new object();

        public object SyncRoot { get { return _syncRoot; } }

        public long Position { get { return _stream.Position; } }
        #endregion

        #region ctor
        public PcapNgWriter(string path, bool reverseByteOrder = false)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(!File.Exists(path), "file exists");
            HeaderWithInterfacesDescriptions header = HeaderWithInterfacesDescriptions.CreateEmptyHeadeWithInterfacesDescriptions(reverseByteOrder);
            Initialize(new FileStream(path, FileMode.Create), new List<HeaderWithInterfacesDescriptions>(){header}) ;
        }

        public PcapNgWriter(Stream stream, bool reverseByteOrder = false)
        {
            Contract.Requires<ArgumentNullException>(stream != null && stream.CanWrite, "stream cannot be null and should be writable");
            HeaderWithInterfacesDescriptions header = HeaderWithInterfacesDescriptions.CreateEmptyHeadeWithInterfacesDescriptions(reverseByteOrder);
            Initialize(stream, new List<HeaderWithInterfacesDescriptions>() { header });
        }  

        public PcapNgWriter(string path, List<HeaderWithInterfacesDescriptions> headersWithInterface)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(!File.Exists(path), "file exists");

            Contract.Requires<ArgumentNullException>(headersWithInterface != null, "headersWithInterface list cannot be null");

            Contract.Requires<ArgumentException>(headersWithInterface.Count >= 1, "headersWithInterface list is empty");

            Initialize(new FileStream(path, FileMode.Create), headersWithInterface);
        }

        public PcapNgWriter(Stream stream, List<HeaderWithInterfacesDescriptions> headersWithInterface)
        {
            Contract.Requires<ArgumentNullException>(stream != null && stream.CanWrite, "stream cannot be null and should be writable");
            Contract.Requires<ArgumentNullException>(headersWithInterface != null, "headersWithInterface list cannot be null");
            Contract.Requires<ArgumentException>(headersWithInterface.Count >= 1, "headersWithInterface list is empty");

            Initialize(stream, headersWithInterface);
        }

        private void Initialize(Stream stream, List<HeaderWithInterfacesDescriptions> headersWithInterface)
         {                     
             Contract.Requires<ArgumentNullException>(stream != null, "stream cannot be null");
             Contract.Requires<Exception>(stream.CanWrite == true, "Cannot write to stream");
             Contract.Requires<ArgumentNullException>(headersWithInterface != null, "headersWithInterface list cannot be null");

             Contract.Requires<ArgumentException>(headersWithInterface.Count >= 1, "headersWithInterface list is empty");

             this._headersWithInterface = headersWithInterface;
             this._stream = stream;
             _binaryWriter = new BinaryWriter(stream);
             Action<Exception> reThrowException = (exc) =>
             {
                 ExceptionDispatchInfo.Capture(exc).Throw();
             };
             foreach (var header in headersWithInterface)
             {
                 _binaryWriter.Write(header.ConvertToByte(header.Header.ReverseByteOrder, reThrowException));          
             }
               
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
                AbstractBlock abstractBlock =null;
                if (packet is AbstractBlock)
                {
                    abstractBlock = packet as AbstractBlock;                     
                }
                else
                {
                    abstractBlock = EnchantedPacketBlock.CreateEnchantedPacketFromIPacket(packet, OnException);
                }

                HeaderWithInterfacesDescriptions header = this.HeadersWithInterfaces.Last();
                byte[] data = abstractBlock.ConvertToByte(header.Header.ReverseByteOrder, OnException);

                if (abstractBlock.AssociatedInterfaceId.HasValue)
                {
                    if (abstractBlock.AssociatedInterfaceId.Value >= header.InterfaceDescriptions.Count)
                    {
                        throw new ArgumentOutOfRangeException(string.Format("[PcapNGWriter.WritePacket] Packet interface ID: {0} is greater than InterfaceDescriptions count: {1}", abstractBlock.AssociatedInterfaceId.Value, header.InterfaceDescriptions.Count));
                    }
                    int maxLength = header.InterfaceDescriptions[abstractBlock.AssociatedInterfaceId.Value].SnapLength;
                    if (data.Length > maxLength)
                    {
                        throw new ArgumentOutOfRangeException(string.Format("[PcapNGWriter.WritePacket] block length: {0} is greater than MaximumCaptureLength: {1}",data.Length,maxLength));
                            
                    }
                }
                lock (_syncRoot)
                {
                    _binaryWriter.Write(data);
                }
            }
            catch (Exception exc)
            {
                OnException(exc);
            }
        }

        public void WriteHeaderWithInterfacesDescriptions(HeaderWithInterfacesDescriptions headersWithInterface)
        {
            Contract.Requires<ArgumentNullException>(headersWithInterface != null, "headersWithInterface  cannot be null");
            Contract.Requires<ArgumentNullException>(headersWithInterface.Header != null, "headersWithInterface.Header  cannot be null");

            byte [] data = headersWithInterface.ConvertToByte(headersWithInterface.Header.ReverseByteOrder, OnException);
            try
            {
                //lock (_syncRoot)
                //{
                    _binaryWriter.Write(data);
               // }
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
