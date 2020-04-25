# Package Release Notes

## WeihanLi.Common

.netstandard2.0 based also support for netfx4.5 common helpers and extensions

### [WeihanLi.Common 1.0.34](https://www.nuget.org/packages/WeihanLi.Common/1.0.34)

- add `PipelineBuilder` to create pipeline easily
- update `TotpHelper`, fix bug when the code starts with `0`

### [WeihanLi.Common 1.0.33](https://www.nuget.org/packages/WeihanLi.Common/1.0.33)

- update `ServiceCollectionDependencyResolver` to fix generic scoped service resolve
- add `ServiceContainerDependencyResolver`
- update `DataExtension`/`DbConnectionExtension`/`DbCommandExtension` fix #9 

### [WeihanLi.Common 1.0.32](https://www.nuget.org/packages/WeihanLi.Common/1.0.32)

- update di extensions, update `GetExportedTypes` with `GetTypes`
- add interface type filter for `RegisterAssemblyTypesAsImplementedInterfaces`/`RegisterTypeAsImplementedInterfaces`
- add `ActivatorHelper.CreateInstance<T>(params object[] parameters)`
- update `TotpHelper`, add check for null salt, disable backward step moving

### [WeihanLi.Common 1.0.31](https://www.nuget.org/packages/WeihanLi.Common/1.0.31)

- update `AsyncLock`, add `Lock`
- update `DependencyResolver`
- add `StringHelper.ToPascalCase`/`StringHelper.ToCamelCase`
- add `UnitOfWork`/`DelegateLoggerProvider`/`TotpOptions`
- rename `JsonToType` => `JsonToObject`(breaking change)

### [WeihanLi.Common 1.0.30](https://www.nuget.org/packages/WeihanLi.Common/1.0.30)

- update `ExpressionExtension.And`/`ExpressionExtension.Or`
- add `SqlExpressionVisitor`
- add `RegisterModule` extension for `IServiceCollection`
- add extensions for `IServiceContainerBuilder`

### [WeihanLi.Common 1.0.29](https://www.nuget.org/packages/WeihanLi.Common/1.0.29)

- add di extension
- add `StringExtension.TrimStart(this string str, string start)` extension

### [WeihanLi.Common 1.0.28](https://www.nuget.org/packages/WeihanLi.Common/1.0.28)

- refact logging
- update `JsonResultModel`/`JsonResultStatus` => `ResultModel`/`ResultStatus`
- add `NetHelper.IsPrivateIP`/`IPNetwork`
- add httpHeader parameters for `HttpHelper.HttpPostFile`

### [WeihanLi.Common 1.0.26](https://www.nuget.org/packages/WeihanLi.Common/1.0.26)

- update event, add async support for publish and subscribe/unsubscribe
- update di, add `ServiceContainerBuilder`/`ServiceContainerModule` to register service
- update cron, export `CronExpression`
- add `ArrayHelper.Empty` for net45,update `ConfigurationHelper` for netstandard2.0

### [WeihanLi.Common 1.0.25](https://www.nuget.org/packages/WeihanLi.Common/1.0.25)

- add `ValidateResultModel`
- update `PagedListModel`
- add build-in di support

### [WeihanLi.Common 1.0.24.3](https://www.nuget.org/packages/WeihanLi.Common/1.0.24.3)

- add `NetHelper.GetRandomPort`
- add `TaskHelper.CompletedTask`
- optimize `ToByteArray(this Stream @this)`/`ToByteArrayAsync(this Stream @this)`
- add `ExpressionExtension.True<T>`/`ExpressionExtension.False<T>`

### [WeihanLi.Common 1.0.24.2](https://www.nuget.org/packages/WeihanLi.Common/1.0.24.2)

- optimize `CronHelper` and `PagedListModel`

### [WeihanLi.Common 1.0.24](https://www.nuget.org/packages/WeihanLi.Common/1.0.24)

- add logging filter
- add `Distinct<T>(Func<T, T, bool> compareFunc)` extension

### [WeihanLi.Common 1.0.23.8](https://www.nuget.org/packages/WeihanLi.Common/1.0.23.8)

- add `CronHelper`/`ConcurrentSet`
- update `EventStoreInMemory`
- update logging

### [WeihanLi.Common 1.0.23.7](https://www.nuget.org/packages/WeihanLi.Common/1.0.23.7)

- add `PeriodBatching`
- update `GetValueGetter`/`GetValueSetter`

### [WeihanLi.Common 1.0.23.6](https://www.nuget.org/packages/WeihanLi.Common/1.0.23.6)

- remove `IsEmpty` for `IEventStore`
- fix `EventStore` `RemoveSubscribtion` bug

### [WeihanLi.Common 1.0.23.4](https://www.nuget.org/packages/WeihanLi.Common/1.0.23.4)

- expose `ApplicationHelper.AppRoot`
- add `IEventBus`/`IPAddressConverter`/`IPEndPointConverter`/`FuncExtension`
- update `HttpClientExtensions`/`SecurityHelper`

### [WeihanLi.Common 1.0.22](https://www.nuget.org/packages/WeihanLi.Common/1.0.22)

- add `DateTimeFormatConverter`
- add `LeftJoin` extension linq method for `IEnumerable`
- add `ActivatorHelper` to create instance
- update `Newtonsoft.Json` package version
- update `JsonResultModel`
- update `Pagedlistmodel`, fix json serialize bug

### [WeihanLi.Common 1.0.21](https://www.nuget.org/packages/WeihanLi.Common/1.0.21)

- add `CancellationToken` support for Repository and DataExtensions async operations
- optimize DataExtension remove `new()` LIMIT

### [WeihanLi.Common 1.0.20](https://www.nuget.org/packages/WeihanLi.Common/1.0.20)

- add [sourceLink](https://github.com/dotnet/sourcelink) support
- add back `SetDependencyResolver(IServiceProvider serviceProvider)` for netstandard2.0
- remove `System.Configuration.ConfigurationManager` dependency for netstandard2.0
- refact `HttpRequester`, add `HttpClientHttpRequester`/`WebRequestHttpRequester`
- add `NoProxyHttpClientHandler`

### [WeihanLi.Common 1.0.19](https://www.nuget.org/packages/WeihanLi.Common/1.0.19)

- Update `IDataSerializer` method `Desrializer` => `Deserialize`
- update HttpRequester
- update logging extensions

### [WeihanLi.Common 1.0.17](https://www.nuget.org/packages/WeihanLi.Common/1.0.17)

- Add `IDataCompressor`/`NullDataCompressor`
- Update `RetryHelper`/`TotpHelper`

### [WeihanLi.Common 1.0.15](https://www.nuget.org/packages/WeihanLi.Common/1.0.15)

- update Repository
- update `AsyncLock`

### [WeihanLi.Common 1.0.14](https://www.nuget.org/packages/WeihanLi.Common/1.0.14)

- update DataExtension, add support for DbConnection Extension select/fetch int
- refact `HttpRequestClient` => `HttpRequester` with fluent api
- add TotpHelper/ObjectId/ObjectIdGenerator

### [WeihanLi.Common 1.0.12](https://www.nuget.org/packages/WeihanLi.Common/1.0.12)

- refact logging and remove package dependency for log4net, move log4net related to WeihanLi.Common.Logging.Log4Net
- update repository, add support for columns mapping

## WeihanLi.Common.Logging.Log4Net

### [WeihanLi.Common.Logging.Log4Net 1.0.28](https://www.nuget.org/packages/WeihanLi.Common.Logging.Log4Net/1.0.28)

- refact logging

### [WeihanLi.Common.Logging.Log4Net 1.0.23.8](https://www.nuget.org/packages/WeihanLi.Common.Logging.Log4Net/1.0.23.8)

- add `ILoggingBuilder` extension for log4net

### [WeihanLi.Common.Logging.Log4Net 1.0.23.7](https://www.nuget.org/packages/WeihanLi.Common.Logging.Log4Net/1.0.23.7)

- add lock for ConfigInit, fix concurrent LogInit bug

### [WeihanLi.Common.Logging.Log4Net 1.0.22.6](https://www.nuget.org/packages/WeihanLi.Common.Logging.Log4Net/1.0.22.6)

- add [sourceLink](https://github.com/dotnet/sourcelink) support
- Optimize `ElasticSearchAppender`, add support for none rolling date index, update LogFields, fix Content-Type warning
- Optimize Log4Net for netframework, add support for `LogManager.GetLogger(loggerName)`/`LogManager.GetLogger(loggerType)`

### [WeihanLi.Common.Logging.Log4Net 1.0.17](https://www.nuget.org/packages/WeihanLi.Common.Logging.Log4Net/1.0.17)

- Update `ElasticSearchAppender` index format, update DateTime => `DateTime.UtcNow`
- Remove `PackageCopyToOutput="true"` to fix some bug when you change the default config file

### [WeihanLi.Common.Logging.Log4Net 1.0.13](https://www.nuget.org/packages/WeihanLi.Common.Logging.Log4Net/1.0.13)

- Add `Log4NetHelper` to use log4net inpendently easily

## WeihanLi.Data

### [WeihanLi.Data 1.0.21](https://www.nuget.org/packages/WeihanLi.Data/1.0.21)

- add [sourceLink](https://github.com/dotnet/sourcelink) support
