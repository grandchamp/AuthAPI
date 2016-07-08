using AuthAPI.Core;
using AuthAPI.Core.Handlers;
using AuthAPI.Samples.WebAPI.Models;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Tests.WebAPI
{
    [TestClass]
    public class AuthAPIHttpClientHandlerTests
    {
        private IDisposable _webApi;
        private string _address = "http://localhost:9000";

        [TestInitialize]
        public void Initialize()
        {
            _webApi = WebApp.Start<Startup>(url: _address);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _webApi.Dispose();
        }

        [TestMethod]
        public async Task ShouldReceiveHttp200OnSingleRequest()
        {
            var client = new HttpClient(new AuthAPIHttpClientHandler("nicolas", "123456"));

            var url = string.Concat(_address, "/api/values/5");
            var result = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(await result.Content.ReadAsAsync<string>(), "value");
        }

        [TestMethod]
        public async Task ShouldReceiveHttp200OnConsecutivesGetRequests()
        {
            var client = new HttpClient(new AuthAPIHttpClientHandler("nicolas", "123456"));

            var url = string.Concat(_address, "/api/values/5");
            var result1 = await client.GetAsync(url);
            var result2 = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.OK, result1.StatusCode);
            Assert.AreEqual(await result1.Content.ReadAsAsync<string>(), "value");

            Assert.AreEqual(HttpStatusCode.OK, result2.StatusCode);
            Assert.AreEqual(await result2.Content.ReadAsAsync<string>(), "value");
        }

        [TestMethod]
        public async Task ShouldReceiveHttp200OnConsecutivesRequests()
        {
            var client = new HttpClient(new AuthAPIHttpClientHandler("nicolas", "123456"));
            var valuesModel = new ValuesModel
            {
                Id = 1,
                Name = "oi"
            };

            var url1 = string.Concat(_address, "/api/values");
            var result1 = await client.PostAsJsonAsync<ValuesModel>(url1, valuesModel);

            var url2 = string.Concat(_address, "/api/values/5");
            var result2 = await client.GetAsync(url2);

            Assert.AreEqual(HttpStatusCode.OK, result1.StatusCode);

            Assert.AreEqual(HttpStatusCode.OK, result2.StatusCode);
            Assert.AreEqual(await result2.Content.ReadAsAsync<string>(), "value");
        }

        [TestMethod]
        public async Task ShouldReceiveHttp200OnMultiplesRequests()
        {
            var requests = new Random().Next(10, 30);
            var client = new HttpClient(new AuthAPIHttpClientHandler("nicolas", "123456"));
            var valuesModel = new ValuesModel
            {
                Id = 1,
                Name = "oi"
            };

            var responseCollection = new List<HttpStatusCode>(requests);
            var expectedResponseCollection = new List<HttpStatusCode>(requests);
            for (int i = 0; i < requests; i++)
            {
                expectedResponseCollection.Add(HttpStatusCode.OK);

                var url = string.Empty;
                HttpResponseMessage result = null;
                if (i % 2 == 0)
                {
                    url = string.Concat(_address, "/api/values");
                    result = await client.PostAsJsonAsync<ValuesModel>(url, valuesModel);

                }
                else
                {
                    url = string.Concat(_address, "/api/values/5");
                    result = await client.GetAsync(url);
                }

                responseCollection.Add(result.StatusCode);
            }

            CollectionAssert.AreEqual(expectedResponseCollection, responseCollection);
        }

        [TestMethod]
        public async Task ShouldReceiveHttp401OnInvalidClientSecret()
        {
            AuthAPIConfiguration.Instance.ClientSecret = "QkM6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ=";

            var client = new HttpClient(new AuthAPIHttpClientHandler("nicolas", "123456"));

            var url = string.Concat(_address, "/api/values/5");
            var result = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
