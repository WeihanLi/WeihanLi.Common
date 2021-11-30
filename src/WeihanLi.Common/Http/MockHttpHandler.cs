using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WeihanLi.Common.Http
{
    public sealed class MockHttpHandler : HttpMessageHandler
    {
        private Func<HttpRequestMessage, Task<HttpResponseMessage>> _responseFactory;

        public MockHttpHandler() : this(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)))
        {
        }

        public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            Guard.NotNull(responseFactory);
            _responseFactory = req => Task.FromResult(responseFactory(req));
        }

        public MockHttpHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> responseFactory)
        {
            Guard.NotNull(responseFactory);
            _responseFactory = responseFactory;
        }

        public void SetResponseFactory(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            Guard.NotNull(responseFactory);
            _responseFactory = req => Task.FromResult(responseFactory(req));
        }

        public void SetResponseFactory(Func<HttpRequestMessage, Task<HttpResponseMessage>> responseFactory)
        {
            Guard.NotNull(responseFactory);
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _responseFactory(request);
        }
    }
}
