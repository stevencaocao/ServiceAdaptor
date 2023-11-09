
using Gateway;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Nacos;
using Ocelot.Provider.Polly;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles();
app.UseStaticFiles();
if (!string.IsNullOrEmpty(serviceCenterType))
    app.UseOcelot().Wait();

app.UseAuthorization();
app.MapControllers();

app.Run();
