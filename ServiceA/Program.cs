using MSCore.EntityFramework;
using ServiceA.BASE;
using ServiceAdapter;
using MSCore.Util.Logger;
using System.Reflection;
using MSCore.Util;
using MSCore.Util.JwtToken;
using MSCore.Util.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.UseMSCoreEFCore<DBContext>("App.Db.Project");

builder.Services.AddScoped<IUnitOfWork, EFUnitOfWork>();

#region localLogger Register

builder.Logging.AddMSCoreLocalFileLogger(builder.Configuration.GetSection("LocalLog").Get<LoggerSetting>());

#endregion

#region Consul Register

builder.WebHost.UseServiceAdaptor(builder.Configuration);

#endregion

#region ֧��jwt��Ȩ
//builder.WebHost.UseJwtToken(builder.Configuration);
//builder.WebHost.UseMSCoreJwtToken(builder.Configuration);
#endregion

#region ֧�ִ��汾���Ƶ�swagger
builder.WebHost.UseMSCoreSwagger<ApiVersion>(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseMSCoreSwaggerUI<ApiVersion>();    
}

#region ����jwt��֤�м��
app.UseAuthentication();
#endregion

app.UseAuthorization();

app.MapControllers();





app.Run();
