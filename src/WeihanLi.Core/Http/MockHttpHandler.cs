using System.Net;

namespace WeihanLi.Common.Http;

/// <summary>
/// HTTP message handler that returns responses from a configurable factory.
/// </summary>
public sealed class MockHttpHandler : HttpMessageHandler
{
    private Func<HttpRequestMessage, Task<HttpResponseMessage>> _responseFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockHttpHandler"/> class that returns 200 OK responses.
    /// </summary>
    public MockHttpHandler() : this(HttpStatusCode.OK)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockHttpHandler"/> class that returns the specified status code.
    /// </summary>
    /// <param name="httpStatusCode">The status code to return.</param>
    public MockHttpHandler(HttpStatusCode httpStatusCode) : this(_ => Task.FromResult(new HttpResponseMessage(httpStatusCode)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockHttpHandler"/> class with a synchronous response factory.
    /// </summary>
    /// <param name="responseFactory">The response factory.</param>
    public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        Guard.NotNull(responseFactory);
        _responseFactory = req => Task.FromResult(responseFactory(req));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockHttpHandler"/> class with an asynchronous response factory.
    /// </summary>
    /// <param name="responseFactory">The response factory.</param>
    public MockHttpHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> responseFactory)
    {
        Guard.NotNull(responseFactory);
        _responseFactory = responseFactory;
    }

    /// <summary>
    /// Sets the synchronous response factory.
    /// </summary>
    /// <param name="responseFactory">The response factory.</param>
    public void SetResponseFactory(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        Guard.NotNull(responseFactory);
        _responseFactory = req => Task.FromResult(responseFactory(req));
    }

    /// <summary>
    /// Sets the asynchronous response factory.
    /// </summary>
    /// <param name="responseFactory">The response factory.</param>
    public void SetResponseFactory(Func<HttpRequestMessage, Task<HttpResponseMessage>> responseFactory)
    {
        Guard.NotNull(responseFactory);
        _responseFactory = responseFactory;
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _responseFactory(request);
    }

    /// <summary>
    /// Creates an <see cref="HttpClient"/> backed by this handler.
    /// </summary>
    /// <returns>An <see cref="HttpClient"/> with a localhost base address.</returns>
    public HttpClient GetHttpClient() => new(this)
    {
        BaseAddress = new Uri("http://localhost/")
    };
}
