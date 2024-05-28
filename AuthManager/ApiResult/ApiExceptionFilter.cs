using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AuthManager.ApiResult
{
    /// <summary>
    /// 异常处理
    /// </summary>
    public class ApiExceptionFilter : IExceptionFilter
    {
        void IExceptionFilter.OnException(ExceptionContext context)
        {
            bool skipApiResult = false;
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(SkipApiResultAttribute)))
            {
                skipApiResult = true;
            }

            if (context.Exception is FirstArgsException)
            {
                FirstArgsException argsException = (FirstArgsException)context.Exception;
                JsonResult objectResult = new JsonResult(new
                {
                    code = argsException.code.getCodeInt(),
                    msg = argsException.getMessage(),
                });
                if (!skipApiResult)
                {
                    objectResult = new JsonResult(new ApiResult<FirstArgsException>(argsException));
                }
                if (argsException.code == ErrorCode.UNAUTHORIZED)
                {
                    context.HttpContext.Response.StatusCode = 401;
                }
                context.Result = objectResult;
            }
            else if (context.Exception is FirstException)
            {
                FirstException argsException = (FirstException)context.Exception;
                JsonResult objectResult = new JsonResult(new
                {
                    code = argsException.code.getCodeInt(),
                    msg = argsException.getMessage(),
                });
                if (!skipApiResult)
                {
                    objectResult = new JsonResult(new ApiResult<FirstException>(argsException));
                }
                if (argsException.code == ErrorCode.UNAUTHORIZED)
                {
                    context.HttpContext.Response.StatusCode = 401;
                }
                context.Result = objectResult;
            }
            else if (context.Exception is Exception)
            {
                Exception argsException = context.Exception;
                JsonResult objectResult = new JsonResult(new
                {
                    code = 500,
                    msg = argsException.Message,
                    stackTrace = argsException.ToString()
                });
                if (!skipApiResult)
                {
                    objectResult = new JsonResult(new ApiResult<object>(argsException));
                }
                context.Result = objectResult;
            }
        }
    }
}
