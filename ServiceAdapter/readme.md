# 1. ServiceAdapter简介

ServiceAdapter是基于服务中心consul、Nacos的适配器，无代码入侵，一句代码即可集成

ServiceAdapter集成了如下功能：

- 注册注册服务到consul
- 注册服务到Nacos
- 集成服务日志输出到本地
- 封装了服务间相互调用

# 2. 使用方法

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
   "LogFilePath": ""
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

