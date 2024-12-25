using MSCore.EntityFramework.Entity;
using System;

namespace MSCore.Util.Logger.OpLog
{
    /// <summary>
    /// 记录日志模型
    /// </summary>
    public class MOpLog : BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 来源IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 模块路径
        /// </summary>
        public string ModulePath { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 操作人工号
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 操作窗口名称
        /// </summary>
        public string BelongWindow { get; set; }

        /// <summary>
        /// 所属业务模块
        /// </summary>
        public string BelongModule { get; set; }

        /// <summary>
        /// 所属功能
        /// </summary>
        public string BelongFun { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string OperateContent { get; set; }


    }
}
