using MSCore.Util.Logger;
using MSCore.Util.JwtToken;
using ServiceAdapter;
using System.Reflection;
using AuthManager.Contract;
using MSCore.Util.Swagger;
using AuthManager.ApiResult;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllers(v =>
{
    v.Filters.Add<ApiResultFilter>();
    v.Filters.Add<ApiExceptionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region localLogger Register

builder.Logging.AddMSCoreLocalFileLogger(builder.Configuration.GetSection("LocalLog").Get<LoggerSetting>());

#endregion

builder.WebHost.UseMSCoreSwagger<ApiVersion>(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);

#region Consul Register

builder.WebHost.UseServiceAdaptor(builder.Configuration);

#endregion

#region 支持jwt鉴权
builder.WebHost.UseMSCoreJwtTokenClient(builder.Configuration);
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 使用自定义中间件处理Forbidden响应
app.UseMiddleware<ForbiddenMiddleware>();

#region 开启jwt验证中间件
app.UseAuthentication();
#endregion

app.UseAuthorization();


app.MapControllers();





app.Run();
