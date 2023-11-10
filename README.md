# 1. ServiceAdaptor简介

ServiceAdaptor是基于服务中心consul、Nacos的适配器

ServiceAdaptor集成了如下功能：

- 注册注册服务到consul

- 注册服务到Nacos

- 自定义服务日志输出
- 封装了服务间相互调用

# 2. 架构说明

## 2.1 服务网关

服务网关Gateway基于Ocelot，对外提供http服务，支持静态资源（html、js）等，具体如下：

例：http://127.0.0.1:80/index.html                               127.0.0.1:80对应网关的IP和端口

其他服务站点都通过网关对外提供服务，具体方式如下：

例：http://127.0.0.1:80/ServiceA/api/Test/index         127.0.0.1:80对应网关的IP和端口，ServiceA是服务站点的服务名称，api/Test/index 是具体的接口地址

## 2.2 服务站点

服务站点就是常说的每一个微服务，分布式提供应用层服务。

例：ServiceA、ServiceB

# 3. 快速搭建微服务步骤

1、创建.netcore项目，nuget上添加ServiceAdapter包

2、Program中添加如下代码

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

3、服务配置文件样例

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

4、服务之间调用示例

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

