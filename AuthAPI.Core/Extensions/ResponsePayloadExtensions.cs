using AuthAPI.Core.Infrastructure.Headers;
using AuthAPI.Core.Infrastructure.RequestStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Core
{
    public static class ResponsePayloadExtensions
    {
        public static string ToBase64(this ResponsePayload request)
        {
            var sb = new StringBuilder()
                            .AppendLine(request.Identifier)
                            .AppendLine(request.RequestCount);

            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sb.ToString()));
        }

        public static StoredResponse ToStoredResponse(this ResponsePayload response, DateTime expirationDate)
        {
            return new StoredResponse { Response = response, Expiration = expirationDate };
        }
    }
}
