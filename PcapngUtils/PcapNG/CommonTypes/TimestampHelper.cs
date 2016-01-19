using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using PcapngUtils.Extensions;

namespace PcapngUtils.PcapNG.CommonTypes
{
    [ToString]    
    public sealed class TimestampHelper
    {

        #region fields && properties
        private static readonly DateTime EpochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        [IgnoreDuringToString]
        public UInt32 TimestampHigh
        {
            get;
            private set;
        }

        [IgnoreDuringToString]
        public UInt32 TimestampLow
        {
            get;
            private set;
        }
        public UInt64 Seconds
        {
            get;
            private set;
        }

        public UInt64 Microseconds
        {
            get;
            private set;
        }

        #endregion

        #region ctor
        public TimestampHelper(byte[] timestampAsByte,bool reverseByteOrder)
        {
            Contract.Requires<ArgumentNullException>(timestampAsByte != null, "timestampAsByte cannot be null");
            Contract.Requires<ArgumentException>(timestampAsByte.Length == 8, "timestamp must have length = 8");           

            TimestampHigh = (BitConverter.ToUInt32(timestampAsByte.Take(4).ToArray(), 0)).ReverseByteOrder(reverseByteOrder);
            TimestampLow = (BitConverter.ToUInt32(timestampAsByte.Skip(4).Take(4).ToArray(), 0)).ReverseByteOrder(reverseByteOrder);            

            long timestamp = ((TimestampHigh * 4294967296) + TimestampLow);
            this.Seconds = (UInt64)(timestamp / 1000000);
            this.Microseconds = (UInt64)(timestamp % 1000000);
        }

        public TimestampHelper(UInt64 seconds, UInt64 microseconds)
        {
            Seconds = seconds;
            Microseconds = microseconds;

            ulong timestamp = seconds * 1000000 + microseconds;
            TimestampHigh = (uint)(timestamp / 4294967296);
            TimestampLow = (uint)(timestamp % 4294967296);            
        }
        #endregion

        #region method
        public DateTime ToUtc()
        {
            ulong ticks = (Microseconds * (TimeSpan.TicksPerMillisecond / 1000)) +
                         (Seconds * TimeSpan.TicksPerSecond);
            return EpochDateTime.AddTicks((long)ticks);
        }

        public byte[] ConvertToByte(bool reverseByteOrder)
        {
            ulong timestamp = Seconds * 1000000 + Microseconds;
            uint timestampHigh =(uint)(timestamp / 4294967296);
            uint timestampLow = (uint)(timestamp % 4294967296);           

            List<byte> ret = new List<byte>();
            ret.AddRange(BitConverter.GetBytes(timestampHigh.ReverseByteOrder(reverseByteOrder)));
            ret.AddRange(BitConverter.GetBytes(timestampLow.ReverseByteOrder(reverseByteOrder)));

            return ret.ToArray();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            TimestampHelper p = (TimestampHelper)obj;
            return (Seconds == p.Seconds) && (Microseconds == p.Microseconds);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
