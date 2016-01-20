using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using PcapngUtils.Common;
using PcapngUtils.PcapNG.BlockTypes;

namespace PcapngUtils.PcapNG
{ 
    public sealed class PcapNgReader : Disposable, IReader
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
            Contract.Requires<ArgumentNullException>(packet != null, "packet cannot be null");
            CommonDelegates.ReadPacketEventDelegate handler = OnReadPacketEvent;
            if (handler != null)
                handler(this._headersWithInterface.Last(), packet);
        }

        
        #endregion

        #region fields && properties
        private BinaryReader _binaryReader;
        private Stream _stream;
        private long _basePosition;
        private bool _reverseByteOrder;

        private List<HeaderWithInterfacesDescriptions> _headersWithInterface = new List<HeaderWithInterfacesDescriptions>();
        public IList<HeaderWithInterfacesDescriptions> HeadersWithInterfaceDescriptions
        {
            get { return _headersWithInterface.AsReadOnly(); }
        }

        private readonly object _syncRoot = new object();

        public object SyncRoot { get { return _syncRoot; } }
        public bool EndOfStream { get { return _binaryReader.BaseStream.Position >= _binaryReader.BaseStream.Length; } }
        #endregion

        #region ctor
        public PcapNgReader(string path, bool swapBytes)             
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(File.Exists(path), "file must exists");

            Initialize(new FileStream(path, FileMode.Open), swapBytes);
        }

        public PcapNgReader(Stream stream, bool reverseByteOrder)             
        {
            Contract.Requires<ArgumentNullException>(stream != null, "stream cannot be null");

            Initialize(stream, reverseByteOrder);
        }

        private void Initialize(Stream stream, bool reverseByteOrder)
        {
            Contract.Requires<ArgumentNullException>(stream != null, "stream cannot be null");
            Contract.Requires<Exception>(stream.CanRead == true, "cannot read stream");
            Action<Exception> reThrowException = (exc) => 
            { 
                ExceptionDispatchInfo.Capture(exc).Throw(); 
            };
            this._reverseByteOrder = reverseByteOrder;
            this._stream = stream;   
            _binaryReader = new BinaryReader(stream);
            List<KeyValuePair<SectionHeaderBlock, List<InterfaceDescriptionBlock>>> preHeadersWithInterface = new List<KeyValuePair<SectionHeaderBlock, List<InterfaceDescriptionBlock>>>(); 
            while (_binaryReader.BaseStream.Position < _binaryReader.BaseStream.Length && _basePosition == 0)
            {
                AbstractBlock block = AbstractBlockFactory.ReadNextBlock(_binaryReader, this._reverseByteOrder, reThrowException);
                if (block == null )
                    break;

                switch (block.BlockType)
                {
                    case BaseBlock.Types.SectionHeader:
                        if (block is SectionHeaderBlock)
                        {
                            SectionHeaderBlock headerBlock = block as SectionHeaderBlock;
                            preHeadersWithInterface.Add(new KeyValuePair<SectionHeaderBlock,List<InterfaceDescriptionBlock>>(headerBlock,new List<InterfaceDescriptionBlock>()));
                        }
                        break;
                    case BaseBlock.Types.InterfaceDescription:
                        if (block is InterfaceDescriptionBlock)
                        {
                            InterfaceDescriptionBlock interfaceBlock = block as InterfaceDescriptionBlock;
                            if (preHeadersWithInterface.Any())
                            {
                                preHeadersWithInterface.Last().Value.Add(interfaceBlock);
                            }
                            else
                            {
                                throw new Exception(string.Format("[PcapNgReader.Initialize] stream must contains SectionHeaderBlock before any InterfaceDescriptionBlock"));
                            }
                        }
                        break;
                    default:
                        _basePosition = block.PositionInStream;
                        break;
                }     
            }
            if (_basePosition <= 0)
                _basePosition = _binaryReader.BaseStream.Position;

            if(!preHeadersWithInterface.Any() )
                throw new ArgumentException(string.Format("[PcapNgReader.Initialize] Stream don't contains any SectionHeaderBlock"));

            if(!(from item in preHeadersWithInterface where (item.Value.Any()) select item).Any())
                throw new ArgumentException(string.Format("[PcapNgReader.Initialize] Stream don't contains any InterfaceDescriptionBlock"));               

            _headersWithInterface = (from item in preHeadersWithInterface 
                                                where (item.Value.Any()) 
                                                select item)
                                                .Select(x => new HeaderWithInterfacesDescriptions(x.Key, x.Value))
                                                .ToList(); 
   
            Rewind();
        }
        #endregion

        #region methods
        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// rewind to the beginning of the stream 
        /// </summary>
        private void Rewind()
        {
            lock (_syncRoot)
            {
                _binaryReader.BaseStream.Position = _basePosition;
            }
        }

        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(_binaryReader != null)
                _binaryReader.Close();
            if (_stream != null)
                _stream.Close();
        }

        public void Seek(long position)
        {
            _binaryReader.BaseStream.Position = position;
        }

        public void ReadPackets(CancellationToken cancellationToken)
        {
            AbstractBlock block;
            long prevPosition = 0;
            while (_binaryReader.BaseStream.Position < _binaryReader.BaseStream.Length && !cancellationToken.IsCancellationRequested)
            {                   
                try
                {
                    lock (_syncRoot)
                    {
                        prevPosition = _binaryReader.BaseStream.Position;
                        block = AbstractBlockFactory.ReadNextBlock(_binaryReader, this._reverseByteOrder, OnException);
                    }

                    if (block == null)
                        throw new Exception(string.Format("[ReadPackets] AbstractBlockFactory cannot read packet on position {0}", prevPosition));

                    switch(block.BlockType)
                    {
                        case BaseBlock.Types.EnhancedPacket:
                            {
                                EnchantedPacketBlock enchantedBlock = block as EnchantedPacketBlock;
                                if (enchantedBlock == null)
                                    throw new Exception(string.Format("[ReadPackets] system cannot cast block to EnchantedPacketBlock. Block start on position: {0}.", prevPosition));
                                else
                                    OnReadPacket(enchantedBlock);
                            }
                            break;
                        case BaseBlock.Types.Packet:
                            {
                                PacketBlock packetBlock = block as PacketBlock;
                                if (packetBlock == null)
                                    throw new Exception(string.Format("[ReadPackets] system cannot cast block to PacketBlock. Block start on position: {0}.", prevPosition));
                                else
                                    OnReadPacket(packetBlock);
                            }
                            break;
                        case BaseBlock.Types.SimplePacket:
                            {
                                SimplePacketBlock simpleBlock = block as SimplePacketBlock;
                                if (simpleBlock == null)
                                    throw new Exception(string.Format("[ReadPackets] system cannot cast block to SimplePacketBlock. Block start on position: {0}.", prevPosition));
                                else
                                    OnReadPacket(simpleBlock);
                            }
                            break;
                        default:
                            break;
                    } 
                }
                catch (Exception exc)
                {
                    OnException(exc);
                    lock (_syncRoot)
                    {
                        if (prevPosition == _binaryReader.BaseStream.Position)
                            break;
                    }
                    continue;
                }
            }
        }

        public INgPacket ReadPcap()
        {
            var block = AbstractBlockFactory.ReadNextBlock(_binaryReader, _reverseByteOrder, OnException);

            switch (block.BlockType)
            {
                case BaseBlock.Types.EnhancedPacket:
                {
                    var enchantedBlock = block as EnchantedPacketBlock;
                    return enchantedBlock;
                }
                case BaseBlock.Types.Packet:
                {
                    var packetBlock = block as PacketBlock;
                    return packetBlock;
                }
                case BaseBlock.Types.SimplePacket:
                {
                    var simpleBlock = block as SimplePacketBlock;
                    return simpleBlock;
                }
            }
            throw new Exception("failed to read next packet.");
        }

        public IPacket Read()
        {
            return ReadPcap();
        }

        #endregion
    }
}
