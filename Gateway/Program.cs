
using Gateway;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Nacos;
using Ocelot.Provider.Polly;
using ServiceAdapter.https;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var serviceCenterType = builder.Configuration.GetSection(ConstantsKey.ServiceCenterType).Value;
if (serviceCenterType == ConstantsKey.Nacos)
{
    builder.Services.AddOcelot().AddNacosDiscovery().AddPolly().AddCacheManager(x => x.WithDictionaryHandle());
}
else if (serviceCenterType == ConstantsKey.Consul)
{
    var ocelotBuilder = builder.Services.AddOcelot().AddConsul().AddPolly().AddCacheManager(x => x.WithDictionaryHandle());
    if (builder.Configuration.GetValue<int>(ConstantsKey.SaveConsulConfig) == 1)
    {
        // Store the configuration in consul
        ocelotBuilder.AddConfigStoredInConsul();
    }
}

#region 支持https
Console.WriteLine("args.lengh:" + args.Length + "\n" + JsonConvert.SerializeObject(args));
builder.WebHost.UseHttps(builder.Configuration, args);
#endregion

var app = builder.Build();

ConfigurationBuilder jsonBuilder = new ConfigurationBuilder();
jsonBuilder.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "contentTypeMap.json"), optional: true, reloadOnChange: true);
IConfigurationRoot mimeConfig = jsonBuilder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    ContentTypeProvider = new FileExtensionContentTypeProvider(mimeConfig.AsEnumerable().ToDictionary(x => x.Key, x => x.Value))
});

if (!string.IsNullOrEmpty(serviceCenterType))
    app.UseOcelot().Wait();

#region 开启https中间件
if (Convert.ToString(builder.Configuration["https:Enable"]).ToLower().Trim() == "true")
    app.UseHttpsRedirection();
#endregion

app.UseAuthorization();
app.MapControllers();

app.Run();
