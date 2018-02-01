using AuthAPI.Core.Infrastructure.Headers;
using System;

namespace AuthAPI.Core.Infrastructure.RequestStore
{
    public class StoredResponse
    {
        public ResponsePayload Response { get; set; }
        public DateTime Expiration { get; set; }
    }
}
