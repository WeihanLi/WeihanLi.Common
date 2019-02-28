# WeihanLi.Common.Logging.Log4Net [![WeihanLi.Common.Logging.Log4Net](https://img.shields.io/nuget/v/WeihanLi.Common.Logging.Log4Net.svg)](https://www.nuget.org/packages/WeihanLi.Common.Logging.Log4Net/)

## Intro

- log4net extensions

  - log4net ElasticSearchAppender
  - Log4NetHelper

- `Log4NetLoggerProvider`for Microsoft.Extensions.Logging

- `Log4NetLogHelperProvider` for WeihanLi.Common.Logging

## Use log4net only

1. log4net init

    ``` csharp
    Log4NetHelper.LogInit(); // log4net.config for default

    Log4NetHelper.LogInit(log4netConfigFilePath);
    ```

2. GetLogger

    ``` csharp
    var genericLogger = Log4NetHelper.GetLogger<ClassName>();

    var typeLogger = Log4NetHelper.GetLogger(typeof(Program));

    var logger = Log4NetHelper.GetLogger("LoggerName");
    ```

3. Log message

    ``` csharp
    logger.Debug(msg);
    logger.Info(msg);
    logger.Info(msg, exception);
    logger.Warn(msg);
    logger.Warn(msg, exception);
    logger.Error(msg);
    logger.Error(msg, exception);
    logger.Fatal(msg);
    logger.Fatal(msg, exception);
    ```

4. log4net config sample

``` xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
  </configSections>
  <log4net>
    <appender name="RollingLogFileAppender" type="log4net.Appender.RollingFileAppender">
      <file type="log4net.Util.PatternString" value="./Log/systemLog.%date{yyyy-MM-dd}.log" />
      <appendToFile value="true" />
      <encoding value="unicodeFFFE" />
      <rollingStyle value="Date" />
      <datePattern value="yyyyMMdd" />
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger - %message%newline" />
      </layout>
    </appender>
    <appender name="AdoNetAppender" type="log4net.Appender.AdoNetAppender">
      <bufferSize value="100" />
      <connectionType value="System.Data.SqlClient.SqlConnection, System.Data" />
      <connectionString value="data source=.;initial catalog=WebLog;integrated security=false;persist security info=True;User ID=[***];Password=[***]" />
      <commandText value="INSERT INTO [dbo].[TestLog]([Date],[Thread],[Level],[Logger],[Message],[Exception]) VALUES(@log_date, @thread, @log_level, @logger, @message, @exception)" />
      <parameter>
        <parameterName value="@log_date" />
        <dbType value="DateTime" />
        <layout type="log4net.Layout.RawTimeStampLayout" />
      </parameter>
      <parameter>
        <parameterName value="@thread" />
        <dbType value="String" />
        <size value="255" />
        <layout type="log4net.Layout.PatternLayout">
          <conversionPattern value="%thread" />
        </layout>
      </parameter>
      <parameter>
        <parameterName value="@log_level" />
        <dbType value="String" />
        <size value="50" />
        <layout type="log4net.Layout.PatternLayout">
          <conversionPattern value="%level" />
        </layout>
      </parameter>
      <parameter>
        <parameterName value="@logger" />
        <dbType value="String" />
        <size value="255" />
        <layout type="log4net.Layout.PatternLayout">
          <conversionPattern value="%logger" />
        </layout>
      </parameter>
      <parameter>
        <parameterName value="@message" />
        <dbType value="String" />
        <size value="4000" />
        <layout type="log4net.Layout.PatternLayout">
          <conversionPattern value="%message" />
        </layout>
      </parameter>
      <parameter>
        <parameterName value="@exception" />
        <dbType value="String" />
        <size value="2000" />
        <layout type="log4net.Layout.ExceptionLayout" />
      </parameter>
      <filter type="log4net.Filter.LevelRangeFilter">
        <!--<param name="LevelMin" value="DEBUG" />
        <param name="LevelMax" value="ERROR" />-->
        <param name="LevelMin" value="WARN" />
      </filter>
    </appender>
    <appender name="TraceAppender" type="log4net.Appender.TraceAppender">
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" />
      </layout>
    </appender>
    <appender name="AspNetTraceAppender" type="log4net.Appender.AspNetTraceAppender">
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" />
      </layout>
    </appender>
    <appender name="ConsoleAppender" type="log4net.Appender.ConsoleAppender">
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger - %message%newline" />
      </layout>
    </appender>
    <appender name="ColoredConsoleAppender" type="log4net.Appender.ColoredConsoleAppender">
      <mapping>
        <level value="ERROR" />
        <foreColor value="White" />
        <backColor value="Red, HighIntensity" />
      </mapping>
      <mapping>
        <level value="DEBUG" />
        <backColor value="Green" />
      </mapping>
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger - %message%newline" />
      </layout>
    </appender>
    <appender name="SmtpAppender" type="log4net.Appender.SmtpAppender">
      <authentication value="Basic" />
      <username value="ben121011@126.com" />
      <password value="*******" />
      <to value="w***@outlook.com" />
      <from value="ben121011@126.com" />
      <subject value="logging message test" />
      <smtpHost value="smtp.126.com" />
      <bufferSize value="512" />
      <!--  超长是否丢弃 -->
      <lossy value="true" />
      <!-- 下面的定义， 就是 日志级别 大于 ERROR 的， 才发邮件.  -->
      <filter type="log4net.Filter.LevelRangeFilter">
        <levelMin value="ERROR" />
        <levelMax value="FATAL" />
      </filter>
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%newline %date  %newline [%thread] %newline %-5level %logger  %newline - %message" />
      </layout>
    </appender>
    <appender name="ElasticSearchAppender" type="WeihanLi.Common.Logging.Log4Net.ElasticSearchAppender, WeihanLi.Common.Logging.Log4Net">
      <bufferSize value="2" />
      <ElasticSearchUrl value="http://localhost:9200" />
    </appender>
    <root>
      <level value="ALL" />
      <appender-ref ref="TraceAppender" />
      <appender-ref ref="ConsoleAppender" />
      <appender-ref ref="RollingLogFileAppender" />
      <!--
      <appender-ref ref="ColoredConsoleAppender" />
      <appender-ref ref="AdoNetAppender" />
      <appender-ref ref="SmtpAppender" />
      <appender-ref ref="ElasticSearchAppender" />
      -->
    </root>
  </log4net>
</configuration>
```

## use Log4NetLoggerProvider

``` csharp
ILoggerFactory loggerFactory = new LoggerFactory();
loggerFactory.AddLog4Net(); // loggerFactory.AddLog4Net(log4netConfigFilePath);
```

you can config in asp.net core use code like below in your `Startup`:

``` csharp
// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();

            // ...
        }
```

## use Log4NetLogHelperProvider

``` csharp
LogHelper.AddLogProvider(new ILogHelperProvider[] {
                new WeihanLi.Common.Logging.Log4Net.Log4NetLogHelperProvider()
            });
```