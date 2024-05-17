﻿using System.Runtime.CompilerServices;

namespace MSCore.Util.Newtonsoft
{
    public static partial class ObjectExtensions
    {

        #region IsValueTypeOrStringType
        /// <summary>
        /// Gets a value indicating whether the data is a value type or string type
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValueTypeOrStringType(this object data)
        {
            if (data == null)
            {
                return false;
                //throw new ArgumentNullException(nameof(data));
            }
            return data.GetType().TypeIsValueTypeOrStringType();
        }
        #endregion


        #region Convert

        /// <summary>
        /// 若Type为Nullable类型（例如 long?）则转换为对应的值类型(例如long)，否则直接转换。（亦可为Enum类型）
        /// 若转换失败，会返回default(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Convert<T>(this object value)
        {
            if (value == null)
            {
                return default;
                //throw new ArgumentNullException(nameof(value));
            }
            return (T)System.Convert.ChangeType(value, typeof(T).GetUnderlyingTypeIfNullable());
        }


        /// <summary>
        /// 若为Nullable类型（例如 long?）则转换为对应的值类型(例如long)，否则直接转换。（亦可为Enum类型）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Convert(this object value, System.Type type)
        {
            if (value == null)
            {
                return null;
                //throw new ArgumentNullException(nameof(value));
            }
            return System.Convert.ChangeType(value, type.GetUnderlyingTypeIfNullable());
        }
        #endregion




    }
}
