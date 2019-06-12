# Package Release Notes

## WeihanLi.Common

.netstandard2.0 based also support for netfx4.5 common helpers and extensions

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

### [WeihanLi.Common.Logging.Log4Net 1.0.17](https://www.nuget.org/packages/WeihanLi.Common.Logging.Log4Net/1.0.17)

- Update `ElasticSearchAppender` index format, update DateTime => `DateTime.UtcNow`
- Remove `PackageCopyToOutput="true"` to fix some bug when you change the default config file

### [WeihanLi.Common.Logging.Log4Net 1.0.13](https://www.nuget.org/packages/WeihanLi.Common.Logging.Log4Net/1.0.13)

- AddLog4NetHelper to use log4net inpendently easily