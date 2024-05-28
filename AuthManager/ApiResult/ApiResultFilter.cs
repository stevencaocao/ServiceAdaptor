using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthManager.ApiResult
{
    /// <summary>
    /// 统一返回值格式
    /// </summary>
    public class ApiResultFilter : IResultFilter
    {
        void IResultFilter.OnResultExecuted(ResultExecutedContext context)
        {

        }
        void IResultFilter.OnResultExecuting(ResultExecutingContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(SkipApiResultAttribute)))
            {
                // 如果操作方法有SkipFilterAttribute特性，则跳过过滤器的执行
                return;
            }
            object? data = null;
            if (context.Result is ObjectResult objectResult)
            {
                data = objectResult.Value;
            }
            var result = new ApiResult<object>(data);
            //返回结果之前
            context.Result = new JsonResult(result);
            if (data != null && result.Code == 401)
            {
                context.HttpContext.Response.StatusCode = 401;
            }
        }
    }

}
