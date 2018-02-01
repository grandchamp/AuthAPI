using AuthAPI.Core.Infrastructure.Headers;
using System;
using System.Text;

namespace AuthAPI.Core
{
    public static class RequestPayloadExtensions
    {
        public static string ToAuthorizationHeader(this RequestPayload request)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Concat(request.ClientId, "|",
                                                                               request.UserName, "|",
                                                                               request.Identifier, "|",
                                                                               request.RequestCount)));
        }

        public static ResponsePayload ToResponsePayload(this RequestPayload request) => new ResponsePayload { Identifier = request.Identifier, RequestCount = request.RequestCount };
    }
}
