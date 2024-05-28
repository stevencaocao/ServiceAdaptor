namespace AuthManager.ApiResult
{
    /// <summary>
    /// 跳过api统一返回值的格式
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class SkipApiResultAttribute : Attribute
    {
    }
}
