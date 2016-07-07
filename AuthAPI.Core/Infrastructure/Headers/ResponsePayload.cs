using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Core.Infrastructure.Headers
{
    public class ResponsePayload
    {
        public string Identifier { get; set; }
        public string RequestCount { get; set; }

        public ResponsePayload() { }

        public ResponsePayload(string header)
        {
            if (!string.IsNullOrEmpty(header))
            {
                var response = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(header)).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Identifier = response[0];
                RequestCount = response[1];
            }
        }
    }
}
