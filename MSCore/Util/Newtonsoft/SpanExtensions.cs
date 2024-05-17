﻿using MSCore.Util.Common;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace MSCore.Util.Newtonsoft
{
    public static partial class SpanExtensions
    {


        #region SpanToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SpanToString(this ReadOnlySpan<byte> data, Encoding encoding = null)
        {
            return Serialization_Newtonsoft.Instance.SpanToString(data, encoding);
        }

        #endregion







    }
}
