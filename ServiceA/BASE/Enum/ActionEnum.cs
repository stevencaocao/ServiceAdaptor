namespace Base.Enum
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum ActionEnum
    {
        Add = 1,
        Edit = 2,
        Delete = 3,
        ChangePassword = 4,
        DownLoad = 5
    }

    /// <summary>
    /// 操作结果
    /// </summary>
    public enum OperatorResultEnum
    {
        success = 0,
        error = 1,
        exception = 2
    }

    /// <summary>
    /// 请求方法
    /// </summary>
    public enum RequestMethodTypeEnum
    {
        GET = 0,
        POST = 1
    }
}
