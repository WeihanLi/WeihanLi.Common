namespace WeihanLi.Common.Http;

/// <summary>
/// HttpHeaderNames
/// get latest from
/// https://github.com/dotnet/aspnetcore/blob/main/src/Http/Headers/src/HeaderNames.cs
/// </summary>
public static class HttpHeaderNames
{
    /// <summary>Gets the <c>Accept</c> HTTP header name.</summary>
    public const string Accept = "Accept";

    /// <summary>Gets the <c>Accept-Charset</c> HTTP header name.</summary>
    public const string AcceptCharset = "Accept-Charset";

    /// <summary>Gets the <c>Accept-Encoding</c> HTTP header name.</summary>
    public const string AcceptEncoding = "Accept-Encoding";

    /// <summary>Gets the <c>Accept-Language</c> HTTP header name.</summary>
    public const string AcceptLanguage = "Accept-Language";

    /// <summary>Gets the <c>Accept-Ranges</c> HTTP header name.</summary>
    public const string AcceptRanges = "Accept-Ranges";

    /// <summary>Gets the <c>Access-Control-Allow-Credentials</c> HTTP header name.</summary>
    public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";

    /// <summary>Gets the <c>Access-Control-Allow-Headers</c> HTTP header name.</summary>
    public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

    /// <summary>Gets the <c>Access-Control-Allow-Methods</c> HTTP header name.</summary>
    public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";

    /// <summary>Gets the <c>Access-Control-Allow-Origin</c> HTTP header name.</summary>
    public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";

    /// <summary>Gets the <c>Access-Control-Expose-Headers</c> HTTP header name.</summary>
    public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";

    /// <summary>Gets the <c>Access-Control-Max-Age</c> HTTP header name.</summary>
    public const string AccessControlMaxAge = "Access-Control-Max-Age";

    /// <summary>Gets the <c>Access-Control-Request-Headers</c> HTTP header name.</summary>
    public const string AccessControlRequestHeaders = "Access-Control-Request-Headers";

    /// <summary>Gets the <c>Access-Control-Request-Method</c> HTTP header name.</summary>
    public const string AccessControlRequestMethod = "Access-Control-Request-Method";

    /// <summary>Gets the <c>Age</c> HTTP header name.</summary>
    public const string Age = "Age";

    /// <summary>Gets the <c>Allow</c> HTTP header name.</summary>
    public const string Allow = "Allow";

    /// <summary>Gets the <c>Alt-Svc</c> HTTP header name.</summary>
    public const string AltSvc = "Alt-Svc";

    /// <summary>Gets the <c>:authority</c> HTTP header name.</summary>
    public const string Authority = ":authority";

    /// <summary>Gets the <c>Authorization</c> HTTP header name.</summary>
    public const string Authorization = "Authorization";

    /// <summary>Gets the <c>baggage</c> HTTP header name.</summary>
    public const string Baggage = "baggage";

    /// <summary>Gets the <c>Cache-Control</c> HTTP header name.</summary>
    public const string CacheControl = "Cache-Control";

    /// <summary>Gets the <c>Connection</c> HTTP header name.</summary>
    public const string Connection = "Connection";

    /// <summary>Gets the <c>Content-Disposition</c> HTTP header name.</summary>
    public const string ContentDisposition = "Content-Disposition";

    /// <summary>Gets the <c>Content-Encoding</c> HTTP header name.</summary>
    public const string ContentEncoding = "Content-Encoding";

    /// <summary>Gets the <c>Content-Language</c> HTTP header name.</summary>
    public const string ContentLanguage = "Content-Language";

    /// <summary>Gets the <c>Content-Length</c> HTTP header name.</summary>
    public const string ContentLength = "Content-Length";

    /// <summary>Gets the <c>Content-Location</c> HTTP header name.</summary>
    public const string ContentLocation = "Content-Location";

    /// <summary>Gets the <c>Content-MD5</c> HTTP header name.</summary>
    public const string ContentMD5 = "Content-MD5";

    /// <summary>Gets the <c>Content-Range</c> HTTP header name.</summary>
    public const string ContentRange = "Content-Range";

    /// <summary>Gets the <c>Content-Security-Policy</c> HTTP header name.</summary>
    public const string ContentSecurityPolicy = "Content-Security-Policy";

    /// <summary>Gets the <c>Content-Security-Policy-Report-Only</c> HTTP header name.</summary>
    public const string ContentSecurityPolicyReportOnly = "Content-Security-Policy-Report-Only";

    /// <summary>Gets the <c>Content-Type</c> HTTP header name.</summary>
    public const string ContentType = "Content-Type";

    /// <summary>Gets the <c>Correlation-Context</c> HTTP header name.</summary>
    public const string CorrelationContext = "Correlation-Context";

    /// <summary>Gets the <c>Cookie</c> HTTP header name.</summary>
    public const string Cookie = "Cookie";

    /// <summary>Gets the <c>Date</c> HTTP header name.</summary>
    public const string Date = "Date";

    /// <summary>Gets the <c>DNT</c> HTTP header name.</summary>
    public const string DNT = "DNT";

    /// <summary>Gets the <c>ETag</c> HTTP header name.</summary>
    public const string ETag = "ETag";

    /// <summary>Gets the <c>Expires</c> HTTP header name.</summary>
    public const string Expires = "Expires";

    /// <summary>Gets the <c>Expect</c> HTTP header name.</summary>
    public const string Expect = "Expect";

    /// <summary>Gets the <c>From</c> HTTP header name.</summary>
    public const string From = "From";

    /// <summary>Gets the <c>Grpc-Accept-Encoding</c> HTTP header name.</summary>
    public const string GrpcAcceptEncoding = "Grpc-Accept-Encoding";

    /// <summary>Gets the <c>Grpc-Encoding</c> HTTP header name.</summary>
    public const string GrpcEncoding = "Grpc-Encoding";

    /// <summary>Gets the <c>Grpc-Message</c> HTTP header name.</summary>
    public const string GrpcMessage = "Grpc-Message";

    /// <summary>Gets the <c>Grpc-Status</c> HTTP header name.</summary>
    public const string GrpcStatus = "Grpc-Status";

    /// <summary>Gets the <c>Grpc-Timeout</c> HTTP header name.</summary>
    public const string GrpcTimeout = "Grpc-Timeout";

    /// <summary>Gets the <c>Host</c> HTTP header name.</summary>
    public const string Host = "Host";

    /// <summary>Gets the <c>Keep-Alive</c> HTTP header name.</summary>
    public const string KeepAlive = "Keep-Alive";

    /// <summary>Gets the <c>If-Match</c> HTTP header name.</summary>
    public const string IfMatch = "If-Match";

    /// <summary>Gets the <c>If-Modified-Since</c> HTTP header name.</summary>
    public const string IfModifiedSince = "If-Modified-Since";

    /// <summary>Gets the <c>If-None-Match</c> HTTP header name.</summary>
    public const string IfNoneMatch = "If-None-Match";

    /// <summary>Gets the <c>If-Range</c> HTTP header name.</summary>
    public const string IfRange = "If-Range";

    /// <summary>Gets the <c>If-Unmodified-Since</c> HTTP header name.</summary>
    public const string IfUnmodifiedSince = "If-Unmodified-Since";

    /// <summary>Gets the <c>Last-Modified</c> HTTP header name.</summary>
    public const string LastModified = "Last-Modified";

    /// <summary>Gets the <c>Link</c> HTTP header name.</summary>
    public const string Link = "Link";

    /// <summary>Gets the <c>Location</c> HTTP header name.</summary>
    public const string Location = "Location";

    /// <summary>Gets the <c>Max-Forwards</c> HTTP header name.</summary>
    public const string MaxForwards = "Max-Forwards";

    /// <summary>Gets the <c>:method</c> HTTP header name.</summary>
    public const string Method = ":method";

    /// <summary>Gets the <c>Origin</c> HTTP header name.</summary>
    public const string Origin = "Origin";

    /// <summary>Gets the <c>:path</c> HTTP header name.</summary>
    public const string Path = ":path";

    /// <summary>Gets the <c>Pragma</c> HTTP header name.</summary>
    public const string Pragma = "Pragma";

    /// <summary>Gets the <c>Proxy-Authenticate</c> HTTP header name.</summary>
    public const string ProxyAuthenticate = "Proxy-Authenticate";

    /// <summary>Gets the <c>Proxy-Authorization</c> HTTP header name.</summary>
    public const string ProxyAuthorization = "Proxy-Authorization";

    /// <summary>Gets the <c>Proxy-Connection</c> HTTP header name.</summary>
    public const string ProxyConnection = "Proxy-Connection";

    /// <summary>Gets the <c>Range</c> HTTP header name.</summary>
    public const string Range = "Range";

    /// <summary>Gets the <c>Referer</c> HTTP header name.</summary>
    public const string Referer = "Referer";

    /// <summary>Gets the <c>Retry-After</c> HTTP header name.</summary>
    public const string RetryAfter = "Retry-After";

    /// <summary>Gets the <c>Request-Id</c> HTTP header name.</summary>
    public const string RequestId = "Request-Id";

    /// <summary>Gets the <c>:scheme</c> HTTP header name.</summary>
    public const string Scheme = ":scheme";

    /// <summary>Gets the <c>Sec-WebSocket-Accept</c> HTTP header name.</summary>
    public const string SecWebSocketAccept = "Sec-WebSocket-Accept";

    /// <summary>Gets the <c>Sec-WebSocket-Key</c> HTTP header name.</summary>
    public const string SecWebSocketKey = "Sec-WebSocket-Key";

    /// <summary>Gets the <c>Sec-WebSocket-Protocol</c> HTTP header name.</summary>
    public const string SecWebSocketProtocol = "Sec-WebSocket-Protocol";

    /// <summary>Gets the <c>Sec-WebSocket-Version</c> HTTP header name.</summary>
    public const string SecWebSocketVersion = "Sec-WebSocket-Version";

    /// <summary>Gets the <c>Sec-WebSocket-Extensions</c> HTTP header name.</summary>
    public const string SecWebSocketExtensions = "Sec-WebSocket-Extensions";

    /// <summary>Gets the <c>Server</c> HTTP header name.</summary>
    public const string Server = "Server";

    /// <summary>Gets the <c>Set-Cookie</c> HTTP header name.</summary>
    public const string SetCookie = "Set-Cookie";

    /// <summary>Gets the <c>Strict-Transport-Security</c> HTTP header name.</summary>
    public const string StrictTransportSecurity = "Strict-Transport-Security";

    /// <summary>Gets the <c>TE</c> HTTP header name.</summary>
    public const string TE = "TE";

    /// <summary>Gets the <c>Trailer</c> HTTP header name.</summary>
    public const string Trailer = "Trailer";

    /// <summary>Gets the <c>Transfer-Encoding</c> HTTP header name.</summary>
    public const string TransferEncoding = "Transfer-Encoding";

    /// <summary>Gets the <c>Translate</c> HTTP header name.</summary>
    public const string Translate = "Translate";

    /// <summary>Gets the <c>traceparent</c> HTTP header name.</summary>
    public const string TraceParent = "traceparent";

    /// <summary>Gets the <c>tracestate</c> HTTP header name.</summary>
    public const string TraceState = "tracestate";

    /// <summary>Gets the <c>Upgrade</c> HTTP header name.</summary>
    public const string Upgrade = "Upgrade";

    /// <summary>Gets the <c>Upgrade-Insecure-Requests</c> HTTP header name.</summary>
    public const string UpgradeInsecureRequests = "Upgrade-Insecure-Requests";

    /// <summary>Gets the <c>User-Agent</c> HTTP header name.</summary>
    public const string UserAgent = "User-Agent";

    /// <summary>Gets the <c>Vary</c> HTTP header name.</summary>
    public const string Vary = "Vary";

    /// <summary>Gets the <c>Via</c> HTTP header name.</summary>
    public const string Via = "Via";

    /// <summary>Gets the <c>Warning</c> HTTP header name.</summary>
    public const string Warning = "Warning";

    /// <summary>Gets the <c>Sec-WebSocket-Protocol</c> HTTP header name.</summary>
    public const string WebSocketSubProtocols = "Sec-WebSocket-Protocol";

    /// <summary>Gets the <c>WWW-Authenticate</c> HTTP header name.</summary>
    public const string WWWAuthenticate = "WWW-Authenticate";

    /// <summary>Gets the <c>X-Content-Type-Options</c> HTTP header name.</summary>
    public const string XContentTypeOptions = "X-Content-Type-Options";

    /// <summary>Gets the <c>X-Frame-Options</c> HTTP header name.</summary>
    public const string XFrameOptions = "X-Frame-Options";

    /// <summary>Gets the <c>X-Powered-By</c> HTTP header name.</summary>
    public const string XPoweredBy = "X-Powered-By";

    /// <summary>Gets the <c>X-Requested-With</c> HTTP header name.</summary>
    public const string XRequestedWith = "X-Requested-With";

    /// <summary>Gets the <c>X-UA-Compatible</c> HTTP header name.</summary>
    public const string XUACompatible = "X-UA-Compatible";

    /// <summary>Gets the <c>X-XSS-Protection</c> HTTP header name.</summary>
    public const string XXSSProtection = "X-XSS-Protection";
}
