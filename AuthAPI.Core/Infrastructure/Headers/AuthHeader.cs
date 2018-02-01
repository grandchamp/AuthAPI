using AuthAPI.Core.Infrastructure.RequestStore.Contracts;
using System;
using System.Net.Http.Headers;

namespace AuthAPI.Core.Infrastructure.Headers
{
    public class AuthHeader
    {
        public DataPayload Data { get; set; }
        public RequestPayload Request { get; set; }
        public bool IsValid { get; private set; }

        public AuthHeader() { }
        public AuthHeader(string header, IResponseStore responseStore)
        {
            try
            {
                if (!string.IsNullOrEmpty(header))
                {
                    var parsedHeader = AuthenticationHeaderValue.Parse(header);

                    var requestString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(parsedHeader.Parameter.Split(':')[0]));
                    var requestParts = requestString.Split('|');

                    Request = new RequestPayload
                    {
                        ClientId = requestParts[0],
                        UserName = requestParts[1],
                        Identifier = requestParts[2],
                        RequestCount = requestParts[3]
                    };

                    var storedResponse = responseStore.GetResponse(Request.Identifier)
                                                      .GetAwaiter()
                                                      .GetResult();

                    if (storedResponse == null)
                    {
                        IsValid = false;
                    }
                    else
                    {
                        if (int.Parse(storedResponse.Response.RequestCount) + 1 != int.Parse(Request.RequestCount))
                            IsValid = false;
                        else
                            IsValid = true;
                    }

                    return;
                }

                IsValid = false;
            }
            catch (Exception)
            {
                IsValid = false;
            }
        }
    }
}
