# WeihanLi.Common

## Build status

[![WeihanLi.Common Latest Stable](https://img.shields.io/nuget/v/WeihanLi.Common.svg)](https://www.nuget.org/packages/WeihanLi.Common/)

[![WeihanLi.Common Latest](https://img.shields.io/nuget/vpre/WeihanLi.Common)](https://www.nuget.org/packages/WeihanLi.Common/absoluteLatest)

[![Pipelines Build Status](https://weihanli.visualstudio.com/Pipelines/_apis/build/status/WeihanLi.WeihanLi.Common?branchName=dev)](https://weihanli.visualstudio.com/Pipelines/_build/latest?definitionId=16&branchName=dev)

[![Github Build Status](https://github.com/WeihanLi/WeihanLi.Common/workflows/dotnetcore/badge.svg?branch=dev)](https://github.com/WeihanLi/WeihanLi.Common/actions?query=workflow%3Adotnetcore+branch%3Adev)

## Intro

基于 .netstandard2.0 的 .NET 常用帮助类，扩展方法等，基础类库

## Packages

与这个 Repository 相关的 nuget 包：

- [WeihanLi.Common](https://www.nuget.org/packages/WeihanLi.Common) 基础组件
- [WeihanLi.Common.Aspect.Castle](https://www.nuget.org/packages/WeihanLi.Common.Aspect.Castle/)  基于 Castle 的 AOP 扩展
- [WeihanLi.Common.Aspect.AspectCore](https://www.nuget.org/packages/WeihanLi.Common.Aspect.Castle/)  基于 AspectCore 的 AOP 扩展（`CreateProxyWithTarget` 不支持 class)
- [WeihanLi.Data](https://www.nuget.org/packages/WeihanLi.Data) 数据库扩展
- [WeihanLi.Common.Logging.Log4Net](https://www.nuget.org/packages/WeihanLi.Common.Logging.Log4Net) 日志 log4net 扩展
- [WeihanLi.Common.Logging.Serilog](https://www.nuget.org/packages/WeihanLi.Common.Logging.Serilog) 日志 serilog 扩展

## Features

- Dependence Injection(类比微软依赖注入框架自定义实现的依赖注入框架)
- Fluent Aspects -- AOP implemented(基于动态代理实现的 AOP 框架)
- Event Related(EventBus/EventQueue/EventStore)
- Logging Framework(结合log4net/serilog/微软日志框架实现的日志框架)
- Dapper-like Ado.Net extensions(类似 Dapper 的 Ado.Net 扩展)
- TOTP implement(TOTP算法实现)
- and more ...

## Contact

Contact me if you need: <weihanli@outlook.com>
