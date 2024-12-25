using System;
namespace MSCore.EntityFramework.Entity
{
    public class BaseEntity
    {

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        ///// <summary>
        ///// 是否删除
        ///// </summary>
        //public virtual bool IsDelete { get; set; }
    }
}
