using AuthAPI.Core.Infrastructure.RequestStore.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Core.Infrastructure.Headers
{
    public class AuthHeader
    {
        public DataPayload Data { get; set; }
        public RequestPayload Request { get; set; }

        public bool IsValid { get { return _isValid; } }

        private bool _isValid = false;
        private IResponseStore _requestStore = AuthAPIConfiguration.Instance.ResponseStore;

        public AuthHeader() { }

        public AuthHeader(string header)
        {
            try
            {
                if (!string.IsNullOrEmpty(header))
                {
                    var requestString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(header.Split(':')[0]));
                    var requestParts = requestString.Split('|');

                    Request = new RequestPayload
                    {
                        ClientId = requestParts[0],
                        UserName = requestParts[1],
                        Identifier = requestParts[2],
                        RequestCount = requestParts[3]
                    };

                    var storedResponse = _requestStore.GetResponse(Request.Identifier)
                                                      .GetAwaiter()
                                                      .GetResult();

                    if (storedResponse == null)
                        _isValid = false;
                    else
                        if (int.Parse(storedResponse.Response.RequestCount) + 1 != int.Parse(Request.RequestCount))
                            _isValid = false;

                    _isValid = true;

                    return;
                }

                _isValid = false;
            }
            catch (Exception ex)
            {
                _isValid = false;
            }
        }
    }
}
