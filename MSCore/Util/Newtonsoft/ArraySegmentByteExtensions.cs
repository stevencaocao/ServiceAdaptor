using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace MSCore.Util.Newtonsoft
{
    public static partial class ArraySegmentByteExtensions
    {
        public static readonly ArraySegment<byte> Null = default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasData<T>(this ArraySegment<T> seg)
        {
            return seg != null && seg.Array != null && seg.Count > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<T> Slice<T>(this ArraySegment<T> seg, int Offset, int? count = null)
        {
            return new ArraySegment<T>(seg.Array, seg.Offset + Offset, count ?? seg.Count - Offset);
        }





        #region ArraySegmentByte <--> String

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ArraySegmentByteToString(this ArraySegment<byte> data, Encoding encoding = null)
        {
            if (null == data || data.Array == null) return null;
            ReadOnlySpan<byte> span = data;
            return span.SpanToString(encoding);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> StringToArraySegmentByte(this string data)
        {
            return null == data ? Null : data.StringToBytes().BytesToArraySegmentByte();
        }
        #endregion


        #region ArraySegmentByte <--> bytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ArraySegmentByteToBytes(this ArraySegment<byte> data)
        {
            if (null == data) return null;


            var bytes = new byte[data.Count];
            if (data.Count > 0)
            {
                data.CopyTo(bytes);
            }
            return bytes;
        }


        #endregion


        #region ArraySegmentByte <--> Int32

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ArraySegmentByteToInt32(this ArraySegment<byte> data, int startIndex = 0)
        {
            return BitConverter.ToInt32(data.Array, data.Offset + startIndex);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> Int32ToArraySegmentByte(this int data)
        {
            return BitConverter.GetBytes(data).BytesToArraySegmentByte();
        }
        #endregion


        #region ArraySegmentByte <--> Int64

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ArraySegmentByteToInt64(this ArraySegment<byte> data, int startIndex = 0)
        {
            return BitConverter.ToInt64(data.Array, data.Offset + startIndex);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> Int64ToArraySegmentByte(this long data)
        {
            return BitConverter.GetBytes(data).BytesToArraySegmentByte();
        }
        #endregion

    }
}
