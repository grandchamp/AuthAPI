using AuthAPI.Core.Infrastructure.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuthAPI.Core.Handlers
{
    public class AuthAPIHttpClientHandler : DelegatingHandler
    {
        private AuthHeader _tempAuthHeader;
        private AuthHeader _authHeader;

        private string _userName, _password;

        public AuthAPIHttpClientHandler(string userName, string password)
        {
            InnerHandler = new HttpClientHandler();
            _userName = userName;
            _password = password;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                if (_authHeader != null)
                {
                    _authHeader.Request.RequestCount = string.Format("{0:D8}", int.Parse(_authHeader.Request.RequestCount) + 1);

                    _authHeader.Data = new DataPayload
                    {
                        ClientId = AuthAPIConfiguration.Instance.ClientId,
                        Method = request.Method.Method,
                        Password = _password,
                        RequestBodyBase64 = request.Content != null ? Convert.ToBase64String(await request.Content.ReadAsByteArrayAsync()) : string.Empty,
                        RequestURI = request.RequestUri.PathAndQuery,
                        UserName = _userName
                    };

                    request.Headers
                           .Authorization = new AuthenticationHeaderValue(AuthAPIConfiguration.Instance.ClientId,
                                                                          _authHeader.ToAuthorizationHeader(AuthAPIConfiguration.Instance.ClientSecret));

                }

                return await base.SendAsync(request, cancellationToken)
                                 .ContinueWith(async task =>
                                 {
                                     var response = task.Result;
                                     if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                                     {
                                         var wwwAuthenticate = response.Headers.WwwAuthenticate.FirstOrDefault();

                                         if (wwwAuthenticate != null && wwwAuthenticate.Scheme.Equals("AuthAPI"))
                                         {
                                             var responsePayload = new ResponsePayload(wwwAuthenticate.Parameter);

                                             var requestPayload = new RequestPayload
                                             {
                                                 ClientId = AuthAPIConfiguration.Instance.ClientId,
                                                 Identifier = responsePayload.Identifier,
                                                 RequestCount = string.Format("{0:D8}", int.Parse(responsePayload.RequestCount) + 1),
                                                 UserName = _userName
                                             };

                                             var dataPayload = new DataPayload
                                             {
                                                 ClientId = AuthAPIConfiguration.Instance.ClientId,
                                                 Method = request.Method.Method,
                                                 Password = _password,
                                                 RequestBodyBase64 = request.Content != null ? Convert.ToBase64String(await request.Content.ReadAsByteArrayAsync()) : string.Empty,
                                                 RequestURI = request.RequestUri.PathAndQuery,
                                                 UserName = _userName
                                             };

                                             var authHeader = new AuthHeader
                                             {
                                                 Data = dataPayload,
                                                 Request = requestPayload
                                             };

                                             _tempAuthHeader = authHeader;

                                             request.Headers.Authorization = new AuthenticationHeaderValue("AuthAPI", _tempAuthHeader.ToAuthorizationHeader(AuthAPIConfiguration.Instance.ClientSecret));

                                             response = await ReplayRequest(request, cancellationToken);

                                             if (response.IsSuccessStatusCode)
                                                 _authHeader = _tempAuthHeader;
                                         }
                                     }
                                     else if (response.IsSuccessStatusCode)
                                     {
                                         if (_tempAuthHeader != null)
                                             _authHeader = _tempAuthHeader;
                                     }

                                     return response;
                                 }).Unwrap();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Task<HttpResponseMessage> ReplayRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}
