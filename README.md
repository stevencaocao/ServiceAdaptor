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