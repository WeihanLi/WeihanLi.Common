// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the MIT license.

using System.Net;
using WeihanLi.Common.Http;
using Xunit;

namespace WeihanLi.Common.Test.HttpTest;

public class MockHttpHandlerTest
{
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task HttpStatusTest(HttpStatusCode httpStatusCode)
    {
        using var httpHandler = new MockHttpHandler(_ => new HttpResponseMessage(httpStatusCode));
        using var httpClient = new HttpClient(httpHandler);
        using var response = await httpClient.GetAsync("http://localhost:32123/api/values", TestContext.Current.CancellationToken);
        Assert.Equal(httpStatusCode, response.StatusCode);
    }

    [Fact]
    public async Task SetResponseFactoryTest()
    {
        using var httpHandler = new MockHttpHandler();
        using var httpClient = new HttpClient(httpHandler);
        using var response = await httpClient.GetAsync("http://localhost:32123/api/values", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        httpHandler.SetResponseFactory(_ => new HttpResponseMessage(HttpStatusCode.BadRequest));
        using var response1 = await httpClient.GetAsync("http://localhost:32123/api/values", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);
    }

    [Fact]
    public async Task DynamicResponseTest()
    {
        using var httpHandler = new MockHttpHandler(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(req.Method.Method)
        });
        using var httpClient = new HttpClient(httpHandler);
        var response = await httpClient.GetStringAsync("http://localhost:32123/api/values", TestContext.Current.CancellationToken);
        Assert.Equal(HttpMethod.Get.Method, response);

        using var httpResponse = await httpClient.PostAsync("http://localhost:32123/api/values", new StringContent(""), TestContext.Current.CancellationToken);
        response = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal(HttpMethod.Post.Method, response);
    }
}
