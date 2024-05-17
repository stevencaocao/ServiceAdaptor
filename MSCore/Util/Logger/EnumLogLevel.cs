using System.ComponentModel;

namespace MSCore.Util.Logger
{
    public enum EnumLogLevel
    {
        //
        // 摘要:
        //     调试
        [Description("调试")]
        Debug,
        //
        // 摘要:
        //     消息
        [Description("消息")]
        Info,
        //
        // 摘要:
        //     警告
        [Description("警告")]
        Warning,
        //
        // 摘要:
        //     错误
        [Description("错误")]
        Error,
        //
        // 摘要:
        //     致命
        [Description("致命")]
        Fatal
    }
}
