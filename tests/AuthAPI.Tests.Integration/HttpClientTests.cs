using AuthAPI.Core;
using AuthAPI.Core.Handlers;
using AuthAPI.Core.Infrastructure.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace AuthAPI.Tests.Integration
{
    public class HttpClientTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public HttpClientTests()
        {
            _server = new TestServer(new WebHostBuilder()
                                            .UseStartup<TestStartup>());

            _client = new HttpClient(new AuthAPIHttpClientHandler("test", "123456", _server.Host.Services.GetService<IOptions<AuthAPIConfiguration>>(), _server.CreateHandler()));
            _client.BaseAddress = _server.BaseAddress;
        }

        [Fact]
        public async Task NoCredentialsProvidedReturnsUnauthorizedAndWWWAuthenticateHeader()
        {
            var response = await _client.GetAsync("/test");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotNull(response.Headers.WwwAuthenticate);
            Assert.Contains("AuthAPI", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task ShouldReceive200AndValueContentWithAuthorization()
        {
            var response = await _client.GetAsync("/test");
            var wwwAuthenticate = response.Headers.WwwAuthenticate.First();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("AuthAPI", wwwAuthenticate.Scheme);

            var responsePayload = new ResponsePayload(wwwAuthenticate.Parameter);

            var requestPayload = new RequestPayload
            {
                ClientId = "TestAPI",
                Identifier = responsePayload.Identifier,
                RequestCount = string.Format("{0:D8}", int.Parse(responsePayload.RequestCount) + 1),
                UserName = "nicolas"
            };

            var dataPayload = new DataPayload
            {
                ClientId = "TestAPI",
                Method = "GET",
                Password = "123456",
                RequestBodyBase64 = string.Empty,
                RequestURI = "/test",
                UserName = "nicolas"
            };

            var authHeader = new AuthHeader
            {
                Data = dataPayload,
                Request = requestPayload
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AuthAPI", authHeader.ToAuthorizationHeader("QiU6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ="));

            response = await _client.GetAsync("/test");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
