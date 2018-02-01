using AuthAPI.Core.Infrastructure.Headers;
using AuthAPI.Core.Infrastructure.RequestStore;
using System;
using System.Text;

namespace AuthAPI.Core
{
    public static class ResponsePayloadExtensions
    {
        public static string ToBase64(this ResponsePayload request)
        {
            var sb = new StringBuilder()
                            .AppendLine(request.Identifier)
                            .AppendLine(request.RequestCount);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        public static StoredResponse ToStoredResponse(this ResponsePayload response, DateTime expirationDate) => new StoredResponse { Response = response, Expiration = expirationDate };
    }
}
