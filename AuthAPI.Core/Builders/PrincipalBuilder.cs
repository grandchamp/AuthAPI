using AuthAPI.Core.Infrastructure;
using AuthAPI.Core.Infrastructure.Headers;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;

namespace AuthAPI.Core.Builders
{
    public class PrincipalBuilder : IPrincipalBuilder
    {
        private readonly IAuthStore _authStore;
        private readonly IOptions<AuthAPIConfiguration> _authApiConfiguration;
        public PrincipalBuilder(IAuthStore authStore, IOptions<AuthAPIConfiguration> authApiConfiguration)
        {
            _authStore = authStore;
            _authApiConfiguration = authApiConfiguration;
        }

        public ClaimsPrincipal BuildPrincipal(AuthHeader authHeader)
        {
            if (_authApiConfiguration.Value.UseIdentity)
                return _authStore.BuildClaimsPrincipalForIdentity(authHeader.Request.UserName);
            else
                return new GenericPrincipal(new ClaimsIdentity(new GenericIdentity(authHeader.Request.UserName)), new string[] { });
        }
    }
}
