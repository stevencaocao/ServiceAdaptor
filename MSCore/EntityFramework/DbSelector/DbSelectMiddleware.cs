using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MSCore.EntityFramework.DbSelector
{
    public class DbSelectMiddleware<DbContext> where DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly RequestDelegate _next;

        public DbSelectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DbContext dbContext)
        {
            dbContext.LoadDBContext(context.Request.Method);

            await _next(context);
        }
    }


}
