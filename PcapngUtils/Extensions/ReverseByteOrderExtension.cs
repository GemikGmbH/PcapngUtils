using System;
using System.Runtime.CompilerServices;

namespace PcapngUtils.Extensions
{    
    public static class ReverseByteOrderExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 ReverseByteOrder(this UInt32 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            return (value << 24) |
                   ((value << 8) & 0x00ff0000) |
                   ((value >> 8) & 0x0000ff00) |
                   ((value >> 24) & 0x000000ff);

            //value = (value >> 16) | (value << 16);
            //return ((value & 0xFF00FF00) >> 8) | ((value & 0x00FF00FF) << 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 ReverseByteOrder(this Int32 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            return (value << 24) |
                   ((value << 8) & 0x00ff0000) |
                   ((value >> 8) & 0x0000ff00) |
                   ((value >> 24) & 0x000000ff);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 ReverseByteOrder(this UInt16 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            return (ushort)((value << 8) | ((value >> 8) & 0x00ff));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 ReverseByteOrder(this Int16 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)                
                return value;

            return (short)((value << 8) | ((value >> 8) & 0x00ff));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 ReverseByteOrder(this UInt64 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            return ((0x00000000000000FF) & (value >> 56)
                    | (0x000000000000FF00) & (value >> 40)
                    | (0x0000000000FF0000) & (value >> 24)
                    | (0x00000000FF000000) & (value >> 8)
                    | (0x000000FF00000000) & (value << 8)
                    | (0x0000FF0000000000) & (value << 24)
                    | (0x00FF000000000000) & (value << 40)
                    | (0xFF00000000000000) & (value << 56));

            //value = (value >> 32) | (value << 32);
            //value = ((value & 0xFFFF0000FFFF0000) >> 16) | ((value & 0x0000FFFF0000FFFF) << 16);
            //return ((value & 0xFF00FF00FF00FF00) >> 8) | ((value & 0x00FF00FF00FF00FF) << 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 ReverseByteOrder(this Int64 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;

            return ((0x00000000000000FF) & (value >> 56)
                    | (0x000000000000FF00) & (value >> 40)
                    | (0x0000000000FF0000) & (value >> 24)
                    | (0x00000000FF000000) & (value >> 8)
                    | (0x000000FF00000000) & (value << 8)
                    | (0x0000FF0000000000) & (value << 24)
                    | (0x00FF000000000000) & (value << 40)
                    | (long) ((0xFF00000000000000) & ((ulong) (value << 56))));

            //value = (value >> 32) | (value << 32);
            //value = ((value & 0xFFFF0000FFFF0000) >> 16) | ((value & 0x0000FFFF0000FFFF) << 16);
            //return ((value & 0xFF00FF00FF00FF00) >> 8) | ((value & 0x00FF00FF00FF00FF) << 8);
        }
    }
}
