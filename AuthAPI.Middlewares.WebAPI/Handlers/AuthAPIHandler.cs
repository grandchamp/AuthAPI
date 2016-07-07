using AuthAPI.Core.Infrastructure.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuthAPI.Core;
using AuthAPI.Core.Infrastructure;
using AuthAPI.Core.Infrastructure.RequestStore.Contracts;
using System.Security.Claims;
using System.Security.Principal;

namespace AuthAPI.Middlewares.WebAPI.Handlers
{
    public class AuthAPIHandler : DelegatingHandler
    {
        private IAuthStore _authStore = AuthAPIConfiguration.Instance.AuthStore;
        private IResponseStore _responseStore = AuthAPIConfiguration.Instance.ResponseStore;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_responseStore == null) _responseStore = AuthAPIConfiguration.Instance.ResponseStore;
            if (_authStore == null) _authStore = AuthAPIConfiguration.Instance.AuthStore;

            var authHeader = new AuthHeader(request.Headers.Authorization == null ? string.Empty : request.Headers.Authorization.Parameter);

            if (authHeader.IsValid)
            {
                var clientSecret = await _authStore.GetClientSecretById(authHeader.Request.ClientId);
                var userPassword = await _authStore.GetPasswordByUserName(authHeader.Request.UserName);

                var cachedResponseIdentifier = await _responseStore.GetResponse(authHeader.Request.Identifier);
                if (cachedResponseIdentifier != null)
                {
                    var expectedRequestPayload = authHeader.Request.Copy<RequestPayload>();
                    expectedRequestPayload.RequestCount = string.Format("{0:D8}", int.Parse(cachedResponseIdentifier.Response.RequestCount) +1);

                    var content = await request.Content.ReadAsByteArrayAsync();
                    var contentString = System.Text.Encoding.UTF8.GetString(content);

                    var expectedAuthHeader = new AuthHeader
                                   {
                                       Request = expectedRequestPayload,
                                       Data = new DataPayload
                                       {
                                           ClientId = authHeader.Request.ClientId,
                                           Method = request.Method.Method,
                                           Password = userPassword,
                                           RequestBodyBase64 = Convert.ToBase64String(content),
                                           RequestURI = request.RequestUri.PathAndQuery,
                                           UserName = authHeader.Request.UserName
                                       }
                                   };

                    if (expectedAuthHeader.ToHMAC(clientSecret).Equals(request.Headers.Authorization.Parameter.Split(':')[1]))
                    {
                        var newResponse = authHeader.Request.ToResponsePayload();

                        await _responseStore.UpdateResponse(newResponse.Identifier, newResponse);

                        request.GetRequestContext().Principal = new GenericPrincipal(new GenericIdentity(authHeader.Request.UserName),
                                                                                     new string[] { });
                    }
                }
            }

            return await base.SendAsync(request, cancellationToken)
                             .ContinueWith(task =>
                             {
                                 var response = task.Result;
                                 if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                                 {
                                     var responsePayload = new ResponsePayload
                                     {
                                         Identifier = Guid.NewGuid().ToString(),
                                         RequestCount = string.Format("{0:D8}", 0)
                                     };

                                     response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("AuthAPI",
                                                                                                        responsePayload.ToBase64()));

                                     _responseStore.StoreResponse(responsePayload, DateTime.Now.AddMilliseconds(AuthAPIConfiguration.Instance.TokenExpirationMiliseconds));
                                 }

                                 return response;
                             });
        }
    }
}
