using System;
using System.Runtime.CompilerServices;

namespace PcapngUtils.Extensions
{    
    public static class ReverseByteOrderExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe UInt32 ReverseByteOrder(this UInt32 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            UInt32 dst;

            var pValue =(byte*) &value;
            var pDst =(byte*) &dst;

            pDst[0] = pValue[3];
            pDst[1] = pValue[2];
            pDst[2] = pValue[1];
            pDst[3] = pValue[0];

            return dst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Int32 ReverseByteOrder(this Int32 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            Int32 dst;

            var pValue = (byte*)&value;
            var pDst = (byte*)&dst;

            pDst[0] = pValue[3];
            pDst[1] = pValue[2];
            pDst[2] = pValue[1];
            pDst[3] = pValue[0];

            return dst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe UInt16 ReverseByteOrder(this UInt16 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            UInt16 dst;

            var pValue = (byte*)&value;
            var pDst = (byte*)&dst;

            pDst[0] = pValue[1];
            pDst[1] = pValue[0];

            return dst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Int16 ReverseByteOrder(this Int16 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            Int16 dst;

            var pValue = (byte*)&value;
            var pDst = (byte*)&dst;

            pDst[0] = pValue[1];
            pDst[1] = pValue[0];

            return dst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe UInt64 ReverseByteOrder(this UInt64 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            UInt64 dst;

            var pValue = (byte*)&value;
            var pDst = (byte*)&dst;

            pDst[0] = pValue[7];
            pDst[1] = pValue[6];
            pDst[2] = pValue[5];
            pDst[3] = pValue[4];
            pDst[4] = pValue[3];
            pDst[5] = pValue[2];
            pDst[6] = pValue[1];
            pDst[7] = pValue[0];

            return dst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Int64 ReverseByteOrder(this Int64 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            Int64 dst;

            var pValue = (byte*)&value;
            var pDst = (byte*)&dst;

            pDst[0] = pValue[7];
            pDst[1] = pValue[6];
            pDst[2] = pValue[5];
            pDst[3] = pValue[4];
            pDst[4] = pValue[3];
            pDst[5] = pValue[2];
            pDst[6] = pValue[1];
            pDst[7] = pValue[0];

            return dst;
        }
    }
}
