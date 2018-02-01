using AuthAPI.Core.Infrastructure.Headers;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AuthAPI.Core.Handlers
{
    public class AuthAPIHttpClientHandler : DelegatingHandler
    {
        private AuthHeader _tempAuthHeader;
        private AuthHeader _authHeader;
        private Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _wrappedMessageHandlerAction;

        private readonly string _userName;
        private readonly string _password;
        private readonly IOptions<AuthAPIConfiguration> _authApiConfiguration;
        private readonly HttpMessageHandler _wrappedMessageHandler;
        public AuthAPIHttpClientHandler(string userName, string password, IOptions<AuthAPIConfiguration> authApiConfiguration)
        {
            InnerHandler = new HttpClientHandler();
            _userName = userName;
            _password = password;
            _authApiConfiguration = authApiConfiguration;
        }

        public AuthAPIHttpClientHandler(string userName, string password, IOptions<AuthAPIConfiguration> authApiConfiguration, HttpMessageHandler messageHandler)
            : this(userName, password, authApiConfiguration)
        {
            _wrappedMessageHandler = messageHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                Task<HttpResponseMessage> sendAsyncTask = null;

                if (_wrappedMessageHandler != null)
                {
                    var method = typeof(HttpMessageHandler).GetMethod("SendAsync", BindingFlags.Instance | BindingFlags.NonPublic);

                    _wrappedMessageHandlerAction = async (hrm, ct) =>
                    {
                        var invoked = method.Invoke(_wrappedMessageHandler, new object[] { request, cancellationToken });
                        sendAsyncTask = (Task<HttpResponseMessage>)invoked;

                        return await sendAsyncTask;
                    };

                    var result = method.Invoke(_wrappedMessageHandler, new object[] { request, cancellationToken });
                    sendAsyncTask = (Task<HttpResponseMessage>)result;
                }

                if (_authHeader != null)
                {
                    _authHeader.Request.RequestCount = string.Format("{0:D8}", int.Parse(_authHeader.Request.RequestCount) + 1);

                    _authHeader.Data = new DataPayload
                    {
                        ClientId = _authApiConfiguration.Value.ClientId,
                        Method = request.Method.Method,
                        Password = _password,
                        RequestBodyBase64 = request.Content != null ? Convert.ToBase64String(await request.Content.ReadAsByteArrayAsync()) : string.Empty,
                        RequestURI = request.RequestUri.PathAndQuery,
                        UserName = _userName
                    };

                    request.Headers
                           .Authorization = new AuthenticationHeaderValue(_authApiConfiguration.Value.ClientId,
                                                                          _authHeader.ToAuthorizationHeader(_authApiConfiguration.Value.ClientSecret));

                }

                if (sendAsyncTask == null)
                    sendAsyncTask = base.SendAsync(request, cancellationToken);

                return await sendAsyncTask
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
                                                 ClientId = _authApiConfiguration.Value.ClientId,
                                                 Identifier = responsePayload.Identifier,
                                                 RequestCount = string.Format("{0:D8}", int.Parse(responsePayload.RequestCount) + 1),
                                                 UserName = _userName
                                             };

                                             var dataPayload = new DataPayload
                                             {
                                                 ClientId = _authApiConfiguration.Value.ClientId,
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

                                             request.Headers.Authorization = new AuthenticationHeaderValue("AuthAPI", _tempAuthHeader.ToAuthorizationHeader(_authApiConfiguration.Value.ClientSecret));

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

        private Task<HttpResponseMessage> ReplayRequest(HttpRequestMessage request, CancellationToken cancellationToken) => _wrappedMessageHandler != null
                                                                                                                                ? _wrappedMessageHandlerAction(request, cancellationToken)
                                                                                                                                : base.SendAsync(request, cancellationToken);
    }
}
