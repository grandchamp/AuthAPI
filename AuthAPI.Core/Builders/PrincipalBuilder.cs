using AuthAPI.Core.Infrastructure;
using AuthAPI.Core.Infrastructure.Headers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

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

        public async Task<ClaimsPrincipal> BuildPrincipal(AuthHeader authHeader)
        {
            if (_authApiConfiguration.Value.UseIdentity)
                return await _authStore.BuildClaimsPrincipalForIdentity(authHeader.Request.UserName);
            else
                return new ClaimsPrincipal(new List<ClaimsIdentity> { new ClaimsIdentity("AuthAPI") });
        }
    }
}
