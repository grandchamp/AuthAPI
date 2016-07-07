using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Core.Infrastructure.Headers
{
    public class RequestPayload
    {
        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string Identifier { get; set; }
        public string RequestCount { get; set; }
    }
}
