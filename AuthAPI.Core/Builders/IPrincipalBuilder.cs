using AuthAPI.Core.Infrastructure.Headers;
using System.Security.Claims;

namespace AuthAPI.Core.Builders
{
    public interface IPrincipalBuilder
    {
        ClaimsPrincipal BuildPrincipal(AuthHeader authHeader);
    }
}
