using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MSCore.Util.Logger;
using System.Threading.Tasks;

namespace MSCore.Util.JwtToken
{
    public class ForbiddenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ForbiddenMiddleware> _logger;

        public ForbiddenMiddleware(RequestDelegate next, ILogger<ForbiddenMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                // 自定义403处理逻辑
                _logger.LogWarning("Forbidden response detected. Custom logic executed.");
                LoggerHelper.LogWarning("Forbidden response detected. Custom logic executed.");

                // 例如：返回自定义的JSON响应
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\":\"Access forbidden\"}");
            }
        }
    }
}
