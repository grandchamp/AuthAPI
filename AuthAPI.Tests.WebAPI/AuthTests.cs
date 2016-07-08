using AuthAPI.Core.Infrastructure.Headers;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AuthAPI.Core;
using AuthAPI.Samples.WebAPI.Models;
using Newtonsoft.Json;
using AuthAPI.Core.Handlers;

namespace AuthAPI.Tests.WebAPI
{
    [TestClass]
    public class AuthTests
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
        public async Task ShouldReceive401OnAuthorizeController()
        {
            var client = new HttpClient();
            var url = string.Concat(_address, "/api/values/5");
            var result = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task ShouldReceiveWWWAuthenticateHeaderOn404()
        {
            var client = new HttpClient();
            var url = string.Concat(_address, "/api/values/5");
            var result = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            StringAssert.Equals(result.Headers.WwwAuthenticate.First().Scheme, "AuthAPI");
        }

        [TestMethod]
        public async Task ShouldReceive200AndValueContentWithAuthorization()
        {
            var client = new HttpClient();

            var url = string.Concat(_address, "/api/values/5");
            var result = await client.GetAsync(url);
            var wwwAuthenticate = result.Headers.WwwAuthenticate.First();

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            StringAssert.Equals(wwwAuthenticate.Scheme, "AuthAPI");

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
                RequestURI = "/api/values/5",
                UserName = "nicolas"
            };

            var authHeader = new AuthHeader
            {
                Data = dataPayload,
                Request = requestPayload
            };


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AuthAPI", authHeader.ToAuthorizationHeader("QiU6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ="));

            result = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(await result.Content.ReadAsAsync<string>(), "value");
        }

        [TestMethod]
        public async Task ShouldReceive401OnDuplicateAuthorizeRequest()
        {
            var client = new HttpClient();

            var url = string.Concat(_address, "/api/values/5");
            var result = await client.GetAsync(url);
            var wwwAuthenticate = result.Headers.WwwAuthenticate.First();

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            StringAssert.Equals(wwwAuthenticate.Scheme, "AuthAPI");

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
                RequestURI = "/api/values/5",
                UserName = "nicolas"
            };

            var authHeader = new AuthHeader
            {
                Data = dataPayload,
                Request = requestPayload
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AuthAPI", authHeader.ToAuthorizationHeader("QiU6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ="));

            result = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(await result.Content.ReadAsAsync<string>(), "value");

            result = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public async Task ShouldReceive200AndValueOnIncrementRequestCount()
        {
            var client = new HttpClient();

            var url = string.Concat(_address, "/api/values/5");
            var result = await client.GetAsync(url);
            var wwwAuthenticate = result.Headers.WwwAuthenticate.First();

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            StringAssert.Equals(wwwAuthenticate.Scheme, "AuthAPI");

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
                RequestURI = "/api/values/5",
                UserName = "nicolas"
            };

            var authHeader = new AuthHeader
            {
                Data = dataPayload,
                Request = requestPayload
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AuthAPI", authHeader.ToAuthorizationHeader("QiU6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ="));

            result = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(await result.Content.ReadAsAsync<string>(), "value");

            authHeader.Request.RequestCount = string.Format("{0:D8}", int.Parse(authHeader.Request.RequestCount) + 1);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AuthAPI", authHeader.ToAuthorizationHeader("QiU6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ="));
            result = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public async Task ShouldReceive200OnPost()
        {
            var client = new HttpClient();

            var valuesModel = new ValuesModel
            {
                Id = 1,
                Name = "oi"
            };
            var url = string.Concat(_address, "/api/values");
            var result = await client.PostAsJsonAsync<ValuesModel>(url, valuesModel);

            var wwwAuthenticate = result.Headers.WwwAuthenticate.First();

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            StringAssert.Equals(wwwAuthenticate.Scheme, "AuthAPI");

            var responsePayload = new ResponsePayload(wwwAuthenticate.Parameter);

            var requestPayload = new RequestPayload
            {
                ClientId = "TestAPI",
                Identifier = responsePayload.Identifier,
                RequestCount = string.Format("{0:D8}", int.Parse(responsePayload.RequestCount) + 1),
                UserName = "nicolas"
            };

            var json = JsonConvert.SerializeObject(valuesModel);

            var dataPayload = new DataPayload
            {
                ClientId = "TestAPI",
                Method = "POST",
                Password = "123456",
                RequestBodyBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(valuesModel))),
                RequestURI = "/api/values",
                UserName = "nicolas"
            };

            var authHeader = new AuthHeader
            {
                Data = dataPayload,
                Request = requestPayload
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AuthAPI", authHeader.ToAuthorizationHeader("QiU6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ="));

            result = result = await client.PostAsJsonAsync<ValuesModel>(url, valuesModel);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        //This is called Z because it should be the last test to run, because it changes the TokenExpirationMiliseconds
        public async Task ZhouldReceive401OnTokenExpiration()
        {
            AuthAPIConfiguration.Instance.TokenExpirationMiliseconds = 1;

            var client = new HttpClient();

            var url = string.Concat(_address, "/api/values/5");
            var result = await client.GetAsync(url);
            var wwwAuthenticate = result.Headers.WwwAuthenticate.First();

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            StringAssert.Equals(wwwAuthenticate.Scheme, "AuthAPI");

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
                RequestURI = "/api/values/5",
                UserName = "nicolas"
            };

            var authHeader = new AuthHeader
            {
                Data = dataPayload,
                Request = requestPayload
            };


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AuthAPI", authHeader.ToAuthorizationHeader("QiU6bSt3anE2OURfX3IsKlVZen05K1tBLW5AQ1x1d0xIXVZwaGE7Zj83QTc0ZXthVy9aWV9UZ0tUcnRUVEQ6d2JxTEhGOi9fMitBfiNZOS5NXHlyJzNnNSl1VzxNQExkQXtHJEQ+fWElMkMhWUJhLT8kbUFeQERWa310J2N+NkQ="));

            result = await client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
