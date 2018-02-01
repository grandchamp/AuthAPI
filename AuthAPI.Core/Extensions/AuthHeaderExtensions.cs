using AuthAPI.Core.Infrastructure.Headers;
using System;
using System.Security.Cryptography;
using System.Text;

namespace AuthAPI.Core
{
    public static class AuthHeaderExtensions
    {
        public static string ToHMAC(this AuthHeader auth, string clientSecret)
        {
            var sb = new StringBuilder()
                            .AppendLine(auth.Request.ClientId)
                            .AppendLine(auth.Request.Identifier)
                            .AppendLine(auth.Request.RequestCount)
                            .AppendLine(auth.Request.UserName)
                            .AppendLine(auth.Data.ClientId)
                            .AppendLine(auth.Data.Method)
                            .AppendLine(auth.Data.Password)
                            .AppendLine(auth.Data.RequestBodyBase64)
                            .AppendLine(auth.Data.RequestURI)
                            .AppendLine(auth.Data.UserName);

            using (var hmac = new HMACSHA256(Convert.FromBase64String(clientSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                return Convert.ToBase64String(hash);
            }
        }

        public static string ToAuthorizationHeader(this AuthHeader auth, string clientSecret)
        {
            var requestBase64 = auth.Request.ToAuthorizationHeader();
            var dataHash = auth.ToHMAC(clientSecret);

            return string.Concat(requestBase64, ":", dataHash);
        }
    }
}
