using AuthAPI.Core.Infrastructure.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Core
{
    public static class RequestPayloadExtensions
    {
        public static string ToAuthorizationHeader(this RequestPayload request)
        {
            return Convert.ToBase64String(
                                System.Text.Encoding.UTF8.GetBytes(
                                    string.Concat(request.ClientId, "|",
                                                  request.UserName, "|",
                                                  request.Identifier, "|",
                                                  request.RequestCount)
                                                                   )
                                          );
        }

        public static ResponsePayload ToResponsePayload(this RequestPayload request)
        {
            return new ResponsePayload { Identifier = request.Identifier, RequestCount = request.RequestCount };
        }
    }
}
