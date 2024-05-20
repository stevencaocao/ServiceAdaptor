# 1. ServiceAdapter简介

ServiceAdapter是基于服务中心consul、Nacos的适配器，无代码入侵，一行代码即可集成

ServiceAdapter集成了如下功能：

- 注册注册服务到consul
- 注册服务到Nacos
- 集成服务日志输出到本地
- 封装了服务间相互调用
- 集成jwt支持工具
- 集成https支持工具

# 2. 使用方法

## 2.1 注册微服务和本地日志

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
   "SaveDays": 30,
   //日志文件绝对路径，不设置默认放在程序主目录
   "LogFilePath": "",
   //日志文件根据消息等级分别存储 0表示不单独存储，1表示单独存储
   "SingleLevelFile": 0
 },
  "ProjectName": "ServiceA Test Api",
  "ProjectDescription": "AAAA",
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

## 2.2 服务之间调用示例

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

## 2.3 使用jwt鉴权

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
    /*私钥，签名算法是Hmac时，授权和鉴权方都需要，并且需要同样的key*/
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



## 2.4 gateway使用https

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

