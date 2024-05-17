using ServiceAdapter.JwtToken;
using MSCore.EntityFramework.Extend;
using MSCore.EntityFramework;
using ServiceA.BASE;
using ServiceAdapter;
using MSCore.Util.Logger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.UseEntityFrameworkCore<DBContext>("App.Db.Project");

builder.Services.AddScoped<IUnitOfWork, EFUnitOfWork>();

#region localLogger Register

builder.Logging.AddLocalFileLogger(builder.Configuration.GetSection("LocalLog").Get<LoggerSetting>());

#endregion

#region Consul Register

builder.WebHost.UseServiceAdaptor(builder.Configuration);

#endregion

#region 支持jwt鉴权
builder.WebHost.UseJwtToken(builder.Configuration);
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region 开启jwt验证中间件
app.UseAuthentication();
#endregion

app.UseAuthorization();

app.MapControllers();





app.Run();
