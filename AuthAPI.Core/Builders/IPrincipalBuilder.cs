using AuthAPI.Core.Infrastructure.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthAPI.Core.Builders
{
    public interface IPrincipalBuilder
    {
        Task<ClaimsPrincipal> BuildPrincipal(AuthHeader authHeader);
    }
}
