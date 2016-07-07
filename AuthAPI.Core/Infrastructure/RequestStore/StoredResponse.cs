using AuthAPI.Core.Infrastructure.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Core.Infrastructure.RequestStore
{
    public class StoredResponse
    {
        public ResponsePayload Response { get; set; }
        public DateTime Expiration { get; set; }
    }
}
