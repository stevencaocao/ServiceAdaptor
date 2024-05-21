# 1. MSCore介绍

MSCore是一款.net扩展工具集，集成了如下功能：

- 集成EntityFramework，支持mysql、sqlserver、sqlite、达梦、postgresql五种数据库，支持原生sql
- 集成服务日志输出到本地
- 集成定期清理日志功能
- 集成jwt授权和鉴权支持工具
- 自定义swagger支持版本管理

# 2. 使用方法

## 2.1 EntityFramework

Program中添加如下代码

```C#
//注册entityframework
builder.Services.UseMSCoreEFCore<DBContext>("App.Db.Project");
```

appsettings.json

```json
  "App": {
    "Db": {
      /* 数据库设置 */
      "Project": {
        /* 数据库类型，可为  dm mysql mssql sqlite pgsql */
        /*达梦数据库需要创建实例时配置大小写不敏感，直接写sql查询时，需要拼接数据库名称，可从Appsettings中读取数据库名称*/
        ///*连接字符串 */
        //"type": "mssql",
        //"ConnectionString": "Data Source=127.0.0.1,1433;Database=test;UID=sa;PWD=123456;",
        //"type": "dm",
        //"ConnectionString": "Server=10.10.10.59;Port=5236;UID=SYSDBA;PWD=SYSDBA001;Database=test;",
        "type": "mysql",
        "ConnectionString": "Server=10.10.10.59;Port=3306;UID=root;PWD=123456;Database=test;",
        //"type": "sqlite",
        //"ConnectionString": "Data Source=D:\\test.db;",
        //"type": "pgsql",
        //"ConnectionString": "server=127.0.0.1;port=5432;user id=postgres;password=123456;Database=test;"
        /* 备机链接字符串 */
        //"ConnectionStringStandby": "Server=10.10.10.59;Port=5236;UID=SYSDBA;PWD=SYSDBA001;Database=test;"
      }
    }
  },
```

重写DBContext

```C#
 public partial class DBContext : BaseDBContext
 {
     /// <summary>
     /// Key
     /// </summary>
     public override string ConnectionKey { get; set; } = "App.Db.Project";
     public DBContext(DbContextOptions<DBContext> options)
         : base(options)
     {

     }
 }
```

Controller中使用

```C#
private DBContext _dbContext;
//注入dbcontext
public TestController(DBContext dBContext)
{
    _dbContext = dBContext;
}

 [HttpGet("getHospitals")]
 public List<HospitalInfo> GetHospitals()
 {

     return _dbContext.hospitals.ToList();
 }
```



## 2.2 本地日志

Program中添加如下代码

```c#
#region localLogger Register
//添加本地日志文件
builder.Logging.AddMSCoreLocalFileLogger(builder.Configuration.GetSection("LocalLog").Get<LoggerSetting>());
#endregion
```

服务配置文件样例

```json
  //本地日志
  "LocalLog": {
    "Enable": true,
    //日志保留天数
    "SaveDays": 30,
    //日志文件绝对路径，不设置默认放在程序主目录
    "LogFilePath": "",
    //日志文件根据消息等级分别存储 0表示不单独存储，1表示单独存储
    "SingleLevelFile": 1,
    //日志等级
    "LogLevel": "Information"
  },
```

## 2.3 使用jwt鉴权

Program中添加如下代码

```c#

#region 支持jwt鉴权
builder.WebHost.UseMSCoreJwtTokenClient(builder.Configuration);
#endregion

#region 开启jwt验证中间件
// 使用自定义中间件处理Forbidden响应
app.UseMiddleware<ForbiddenMiddleware>();
app.UseAuthentication();
#endregion
```

配置文件

```json
  "JWT": {
    /*签名算法，RsaSha256或者HmacSha256*/
    "AlgorithmsType": "RsaSha256",
    /*缓冲时间*/
    "ClockSkew": 0,
    /*私钥，签名算法是Hmac时，授权和鉴权方都需要，并且需要同样的key*/
    "JwtSecurityKey": "test$RFVnhy6test$RFVnhy6",
    /*过期时间（授权方配置，鉴权方不需要），分钟*/
    "Expires": 10,
    /*刷新令牌过期时间（授权方配置，鉴权方不需要），天*/
    "Refresh_Expires": 30,
    /*有效签发者*/
    "ValidIssuer": "admin",
    /*有效受众*/
    "ValidAudience": "admin",
    /*公钥，授权方式使用RSA签名算法时鉴权方使用，授权方不需要此配置*/
    /*使用RSA签名算法的话，需要先运行授权服务，在程序根目录得到key.public.json，将key复制到下面配置*/
    "publicKey": "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxBocVdDaSnUBTRDQsssE18PTFCeHxFgjJx55igbRcHoyXFwBXSApeagzIDHvShH6ZcpbLhKF4mFeKysmhwJeavNCCGtwQXgLt/bXR4btkqQZXmwWEVqeEStpwQjHJ42TDNtEGR54SDCDyyJBKCy7WgXLK7akkh4jryKPdz8ACN5UtgrOzTkNtgwfNlnn+MxL3TUQCyLwl0vwgkD5dVHvs1H8E/qxG6amyUYOXWGuSN0A/EYJGjWHb+PgF3VzBuGwb8hnV40G+xbOz4ce5z3fxvzM5CCpO4R1RqLjWqarH0eug8reABS2oNi4heCQ7aCVLZkzvX8rVp2oVlmJYgl45wIDAQAB"
  }
```

控制器中调用

```c#
 private readonly IJwtTokenUtils _jwtTokenUtils;
 public AccountController(IConfiguration configuration, IJwtTokenUtils jwtTokenUtils)
 {
     _jwtTokenUtils = jwtTokenUtils;
 }
 
  //生成token，去掉敏感字段信息
 IEnumerable<Claim> claims = new[] { new Claim("role", "admin") };
var view = _jwtTokenUtils.getToken(new { userName = "admin", userCode = "1001" }, claims);
 
 //刷新token
 //IEnumerable<Claim> claims = new[] { new Claim("role", "admin") };
 //var view = _jwtTokenUtils.refresh(refreshModel, user,claims);
```



## 2.4 swagger自定义

Program中添加如下代码

```c#
#region 支持带版本控制的swagger
builder.WebHost.UseMSCoreSwagger<ApiVersion>(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);
#endregion

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseMSCoreSwaggerUI<ApiVersion>();    
}
```

*.csproj文件中增加如下配置，确保输出swagger的xml文件

```c#
//<Project Sdk="Microsoft.NET.Sdk.Web">
//  <PropertyGroup>
	  <GenerateDocumentationFile>True</GenerateDocumentationFile>
//  </PropertyGroup>
//</Project>
```

增加api版本枚举类型

```C#
public enum ApiVersion
{
    V1,
    V2
}
```

Controller中定义版本

```C#
[Route("api/v1/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiVersion.V1))]
public class TestController : ControllerBase
```

appsettings.json中增加swagger标题

```json
  "ProjectName": "ServiceA Test Api",
  "ProjectDescription": "this is test api",
```

