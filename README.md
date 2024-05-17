# 1. ServiceAdaptor简介

ServiceAdaptor是基于服务中心consul、Nacos的适配器

ServiceAdaptor集成了如下功能：

- 注册注册服务到consul
- 注册服务到Nacos
- 自定义服务日志输出
- 封装了服务间相互调用

# 2. MSCore说明

## 2.1 ORM框架集成

MSCore实现ORM框架entityframework的支持，集成sqlserver、mysql、sqlite、达梦、postgresql数据库驱动。

1、达梦数据库驱动需要自行发布私有源依赖包DmProvider.1.1.0.20739和Microsoft.EntityFrameworkCore.Dm.3.1.0.20604

2、达梦数据库安装实例时需注意设置大小写不敏感，否则需要字段命名和C#实体类命名规则一致（或者C#字段加注解指定一致名称）

3、postgresql大小写敏感，需要字段命名规则和C#一致（或者C#字段加注解指定一致名称）

4、使用实例在ServiceA中，使用时，在配置文件中指定数据库类型和连接字符串，如下：

```C#
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
      //"type": "mysql",
      //"ConnectionString": "Server=10.10.10.59;Port=3306;UID=root;PWD=123456;Database=test;",
      //"type": "sqlite",
      //"ConnectionString": "Data Source=D:\\test.db;",
      "type": "pgsql",
      "ConnectionString": "server=127.0.0.1;port=5432;user id=postgres;password=123456;Database=test;"
    }
  }
```

## 2.2 日志集成

Program中添加如下代码

```C#
#region localLogger Register
//添加本地日志文件
builder.Logging.AddLocalFileLogger(builder.Configuration.GetSection("LocalLog").Get<LoggerSetting>());
#endregion
```

服务配置文件样例

```C#
 //本地日志
  "LocalLog": {
    "Enable": true,
    //日志保留天数
    "SaveDays": 1,
    //日志文件绝对路径，不设置默认放在程序主目录
    "LogFilePath": "",
    //日志文件根据消息等级分别存储 0表示不单独存储，1表示单独存储
    "SingleLevelFile": 1,
    "LogLevel": "Information"
  },
```



# 3. 架构说明

## 3.1 服务网关

服务网关Gateway基于Ocelot，对外提供http服务，支持静态资源（html、js）等，具体如下：

例：http://127.0.0.1:80/index.html                               127.0.0.1:80对应网关的IP和端口

其他服务站点都通过网关对外提供服务，具体方式如下：

例：http://127.0.0.1:80/ServiceA/api/Test/index         127.0.0.1:80对应网关的IP和端口，ServiceA是服务站点的服务名称，api/Test/index 是具体的接口地址

## 3.2 服务站点

服务站点就是常说的每一个微服务，分布式提供应用层服务。

例：ServiceA、ServiceB

# 4. 快速搭建微服务步骤

1、创建.netcore项目，nuget上添加ServiceAdapter包

## 4.1 注册微服务和本地日志

Program中添加如下代码

```c#
#region localLogger Register
//添加本地日志文件
builder.Logging.AddLocalFileLogger(builder.Configuration.GetSection("LocalLog").Get<LoggerSetting>());
#endregion

#region Consul Register
//注册到注册中心
builder.WebHost.UseServiceAdaptor(builder.Configuration);
#endregion

```

服务配置文件样例

```json
 //本地日志
  "LocalLog": {
    "Enable": true,
    //日志保留天数
    "SaveDays": 1,
    //日志文件绝对路径，不设置默认放在程序主目录
    "LogFilePath": "",
    //日志文件根据消息等级分别存储 0表示不单独存储，1表示单独存储
    "SingleLevelFile": 1,
    "LogLevel": "Information"
  },

 //注册微服务consul配置
 "servicecenter": {
   "consul": {
     "Enable": false,
     "ConsulEndpoint": "http://127.0.0.1:8500",
     "ServiceHost": "127.0.0.1",
     "ServicePort": "5228",
     "ServiceName": "ServiceA",
     "GroupName": "ServiceA",
     "Tags": "1"
   },
   "nacos": {
     "Enable": true,
     "Namespace": "public",
     "ServiceName": "ServiceA",
     "EndPoint": "",
     "ServerAddresses": [ "http://localhost:8848" ],
     "DefaultTimeOut": 10000,
     "ListenInterval": 1000,
     "GroupName": "DEFAULT_GROUP",
     "ClusterName": "DEFAULT",
     "Ip": "",
     "PreferredNetworks": "",
     "Port": 0,
     "Weight": 100,
     "RegisterEnabled": true,
     "InstanceEnabled": true,
     "Ephemeral": true,
     "Secure": false,
     "AccessKey": "",
     "SecretKey": "",
     "UserName": "",
     "Password": "",
     "ConfigUseRpc": true,
     "NamingUseRpc": true,
     "NamingLoadCacheAtStart": ""
   }
 }
```

## 4.2 服务之间调用示例

```c#
#region Get

public static async Task<string> GetTestServiceByServiceB()
{
    return await ApiClient.RequestApiAsync<string>("/ServiceB/api/Test/index2", null, ApiClient.Get);
}

#endregion



#region Post

public static async Task<UserInfo> PostTestServiceByServiceB(UserInfo userInfo)
{
    UserInfo userInfo= UserInfo() { name = "张三", age = 18, roles = new List<string>(),remark="" }
    return await ApiClient.RequestApiAsync<UserInfo>("/ServiceB/api/Test/index3", userInfo, ApiClient.Post);
}

#endregion
```

## 4.3 使用jwt鉴权

Program中添加如下代码

```c#
#region 支持jwt鉴权
//开启jwt鉴权
builder.WebHost.UseJwtToken(builder.Configuration);
#endregion

#region 开启jwt验证中间件
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
    /*私钥，签名算法时Hmac时，授权和鉴权方都需要，并且需要同样的key*/
    "JwtSecurityKey": "jiuyun$RFVnhy6jiuyun$RFVnhy6",
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
 var view = _jwtTokenUtils.getToken(new { userName = "admin", userCode = "1001", roles = "用户" });
 
 //刷新token
 //var view = _jwtTokenUtils.refresh(refreshModel, user);
```



## 4.4 gateway使用https

Program中添加如下代码

```c#
#region 支持https
builder.WebHost.UseHttps(builder.Configuration, args);
#endregion

#region 开启https中间件
if (Convert.ToString(builder.Configuration["https:Enable"]).ToLower().Trim() == "true")
    app.UseHttpsRedirection();
#endregion
```

配置文件

```json
  "https": {
     //是否开启https
    "Enable": true,
     //默认https端口
    "defaultPort": 443,
    "Certificate": {
       //证书，程序当前目录下
      "Path": "server.pfx",
       //证书密码
      "Password": "123456"
    }
  },
```

