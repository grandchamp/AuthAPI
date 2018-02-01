using AuthAPI.Core;
using AuthAPI.Core.Builders;
using AuthAPI.Core.Infrastructure;
using AuthAPI.Core.Infrastructure.Headers;
using AuthAPI.Core.Infrastructure.RequestStore.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AuthAPI.Middlewares.Mvc
{
    public class AuthAPIAuthenticationHandler : AuthenticationHandler<AuthAPIAuthenticationOptions>
    {
        private readonly IAuthStore _authStore;
        private readonly IResponseStore _responseStore;
        private readonly IOptions<AuthAPIConfiguration> _authApiConfiguration;
        private readonly IPrincipalBuilder _principalBuilder;
        public AuthAPIAuthenticationHandler(IAuthStore authStore, IResponseStore responseStore, IOptions<AuthAPIConfiguration> authApiConfiguration,
                                            IOptionsMonitor<AuthAPIAuthenticationOptions> options, IPrincipalBuilder principalBuilder, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _authStore = authStore;
            _responseStore = responseStore;
            _authApiConfiguration = authApiConfiguration;
            _principalBuilder = principalBuilder;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = new AuthHeader(Context.Request.Headers.TryGetValue("Authorization", out StringValues value)
                                               ? value.FirstOrDefault()
                                               : string.Empty,
                                            _responseStore);

            if (authHeader.IsValid)
            {
                var clientSecret = await _authStore.GetClientSecretById(authHeader.Request.ClientId);
                var userPassword = await _authStore.GetPasswordByUserName(authHeader.Request.UserName);

                var cachedResponseIdentifier = await _responseStore.GetResponse(authHeader.Request.Identifier);
                if (cachedResponseIdentifier != null)
                {
                    var expectedRequestPayload = authHeader.Request.Copy();
                    expectedRequestPayload.RequestCount = string.Format("{0:D8}", int.Parse(cachedResponseIdentifier.Response.RequestCount) + 1);

                    var content = await Context.Request.ReadAsByteArrayAsync();
                    var contentString = Encoding.UTF8.GetString(content);

                    var expectedAuthHeader = new AuthHeader
                    {
                        Request = expectedRequestPayload,
                        Data = new DataPayload
                        {
                            ClientId = authHeader.Request.ClientId,
                            Method = Context.Request.Method,
                            Password = userPassword,
                            RequestBodyBase64 = Convert.ToBase64String(content),
                            RequestURI = Context.Request.PathAndQuery(),
                            UserName = authHeader.Request.UserName
                        }
                    };

                    if (expectedAuthHeader.ToHMAC(clientSecret).Equals(Context.Request.Headers["Authorization"].FirstOrDefault().Split(':')[1]))
                    {
                        var newResponse = authHeader.Request.ToResponsePayload();

                        await _responseStore.UpdateResponse(newResponse.Identifier, newResponse);

                        return AuthenticateResult.Success(new AuthenticationTicket(_principalBuilder.BuildPrincipal(authHeader),
                                                                                   new AuthenticationProperties(),
                                                                                   Scheme.Name));
                    }
                }
            }

            Context.Response.OnStarting(async () =>
            {
                var responsePayload = new ResponsePayload
                {
                    Identifier = Guid.NewGuid().ToString(),
                    RequestCount = string.Format("{0:D8}", 0)
                };

                Context.Response.Headers.Add("WWW-Authenticate", $"AuthAPI {responsePayload.ToBase64()}");

                await _responseStore.StoreResponse(responsePayload, DateTime.Now.AddMilliseconds(_authApiConfiguration.Value.TokenExpirationMiliseconds));
            });

            return AuthenticateResult.Fail("Invalid Authorization.");
        }
    }
}
