using System;
using System.IO;
using System.Linq;
using PcapngUtils.Common;
using PcapngUtils.Extensions;
using PcapngUtils.Pcap;
using PcapngUtils.PcapNG;
using PcapngUtils.PcapNG.BlockTypes;

namespace PcapngUtils.Utilities
{
    /// <summary>
    /// Gets Arbitrary number of pcap files and merge them into a pcapng file
    /// </summary>
    public class PcapMerger : IDisposable
    {

        private class Selector:IDisposable
        {
            private readonly IReader _reader;
            private IPacket _stash;

            public HeaderWithInterfacesDescriptions Header { get; private set; }
            private int Index { get; set; }
            public int Count { get; set; }

            public Selector(Stream stream, int index)
            {
                _reader=new PcapReader(stream);
                Header = _reader.GetPcapNgHeader().First();
                Index = index;
            }

            public Selector(string path, int index)
            {
                _reader = new PcapReader(path);
                Header = _reader.GetPcapNgHeader().First();
                Index = index;
            }

            public IPacket Peek()
            {
                if(_stash!= null)
                    return _stash;
                
                if (_reader.EndOfStream)
                    return null;

                Count++;
                var packet = _reader.Read();
                var ngPacket = EnchantedPacketBlock.CreateEnchantedPacketFromIPacket(packet,e=> { throw e; });
                ngPacket.InterfaceID = Index;
                _stash = ngPacket;

                return _stash;
            }

            public IPacket Take()
            {
                if (_stash == null)
                    Peek();

                var temp = _stash;
                _stash = null;

                return temp;
            }

            public void Dispose()
            {
                _reader.Dispose();
            }
        }

        private readonly Selector[] _selectors;
        private readonly Stream _output;
        private readonly bool _pathMode;

        public PcapMerger(string path,params string[] paths)
        {
            _output = File.OpenWrite(path);
            _pathMode = true;

            _selectors = new Selector[paths.Length];

            for (var i = 0; i < paths.Length; i++)
                _selectors[i] = new Selector(paths[i], i);
        }

        public PcapMerger(Stream stream, params Stream[] streams)
        {
            _output = stream;
            _pathMode = false;

            _selectors = new Selector[streams.Length];

            for(var i=0;i<streams.Length;i++)
                _selectors[i]=new Selector(streams[i],i);
        }

        public void Merge()
        {
            var headers = _selectors.Select(s => s.Header).ToList();
            var header = headers.MergeNgHeaders();

            var writer = new PcapNgWriter(_output, header);

            while (true)
            {
                var smallest = _selectors.OrderBy(s =>
                {
                    var p = s.Peek();
                    return p == null ? ulong.MaxValue : p.Seconds;
                }).ThenBy(s =>
                {
                    var p = s.Peek();
                    return p == null ? ulong.MaxValue : p.Nanoseconds;
                }).First();

                var packet = smallest.Take();
                if (packet == null)
                    return;

                writer.WritePacket(packet);
            }
        }

        public int Count()
        {
            return _selectors.Sum(s => s.Count);
        }

        public void Dispose()
        {
            if(_pathMode)
                _output.Dispose();
            
            foreach (var stream in _selectors)
                stream.Dispose();
        }
    }
}
